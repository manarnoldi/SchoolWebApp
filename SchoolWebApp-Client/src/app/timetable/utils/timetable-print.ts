import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {TimetableDetail} from '../models/timetable-models';
import {dayName, dayShort, GridCell, TimetableIndex, WeekGrid} from './timetable-grid';

// Printed timetables: a class or a teacher one to an A4 landscape page, and
// the master timetable on A3 landscape, one table per bell schedule.

export interface PrintSchool {
    name: string;
    address?: string | null;
    logo?: string | null;
}

const head = (school: PrintSchool, title: string, sub: string) => `
    <div class="head">
        ${school.logo ? `<img src="${school.logo}" alt="">` : ''}
        <div>
            <div class="school">${escapeHtml(school.name)}</div>
            <div class="title">${escapeHtml(title)}</div>
            <div class="sub">${escapeHtml(sub)}</div>
        </div>
    </div>`;

const css = (size: 'A4' | 'A3', font: number) => `
    @page { size: ${size} landscape; margin: 8mm; }
    body { font-family: Arial, Helvetica, sans-serif; font-size: ${font}px; color: #111; margin: 0; }
    .page { page-break-after: always; }
    .page:last-child { page-break-after: auto; }
    .head { display: flex; align-items: center; gap: 10px; border-bottom: 2px solid #333; padding-bottom: 4px; margin-bottom: 6px; }
    .head img { height: 46px; }
    .school { font-size: 15px; font-weight: bold; text-transform: uppercase; }
    .title { font-size: 13px; font-weight: bold; margin-top: 2px; text-transform: uppercase; }
    .sub { font-size: 10px; margin-top: 2px; color: #333; }
    table { border-collapse: collapse; width: 100%; table-layout: fixed; }
    th, td { border: 1px solid #555; padding: 2px 3px; text-align: center; vertical-align: middle; }
    th { background: #e9e9e9; }
    th.time { width: 92px; text-align: left; white-space: nowrap; font-size: .9em; }
    td.brk { background: #f2f2f2; font-style: italic; color: #555; }
    td.fixed { background: #fff6d8; font-style: italic; }
    .subj { font-weight: bold; }
    .who { font-size: .85em; color: #333; }
    .cont { color: #777; }
    .legend { margin-top: 6px; font-size: 9px; columns: 4; }
    .foot { margin-top: 4px; font-size: 9px; color: #555; }`;

function weekTable(t: TimetableIndex, g: WeekGrid, mode: 'class' | 'teacher'): string {
    let rows = g.rows.map((r) => {
        if (!r.lessonNo) {
            return `<tr><th class="time">${r.slot.startTime}–${r.slot.endTime}</th>
                <td class="brk" colspan="${g.days.length}">${escapeHtml(r.slot.label || '')}</td></tr>`;
        }
        let cells = r.cells.map((c) => cellHtml(c, mode)).join('');
        return `<tr><th class="time">${r.lessonNo}. ${r.slot.startTime}–${r.slot.endTime}</th>${cells}</tr>`;
    }).join('');
    return `<table><thead><tr><th class="time">Time</th>${g.days.map((d) => `<th>${dayName(d)}</th>`).join('')}</tr></thead>
        <tbody>${rows}</tbody></table>`;
}

function cellHtml(c: GridCell, mode: 'class' | 'teacher' | 'master'): string {
    if (!c.entries.length) return c.fixed ? `<td class="fixed">${escapeHtml(c.fixed)}</td>` : '<td></td>';
    let parts = c.entries.map((e) => {
        let subj = `<span class="subj${e.continued ? ' cont' : ''}">${escapeHtml(mode === 'master' ? e.abbr : e.subject)}</span>`;
        let who = mode === 'class' ? e.teacher : mode === 'teacher' ? e.className : e.initials;
        return `${subj}${who ? `<br><span class="who">${escapeHtml(who)}${e.time ? ` (${e.time})` : ''}</span>` : ''}`;
    });
    return `<td>${parts.join('<hr style="margin:1px 0;border:0;border-top:1px dotted #999">')}</td>`;
}

function sub(t: TimetableDetail): string {
    let from = t.effectiveFrom ? ` · from ${new Date(t.effectiveFrom).toLocaleDateString('en-GB', {day: 'numeric', month: 'short', year: 'numeric'})}` : '';
    return `${t.sessionName}${from}${t.status === 0 ? ' · DRAFT' : ''}`;
}

export function printClasses(t: TimetableDetail, classIds: number[], school: PrintSchool): void {
    let idx = new TimetableIndex(t);
    let pages = classIds.map((id) => {
        let g = idx.classWeek(id);
        let name = t.classes.find((c) => c.id === id)?.name ?? '';
        return g ? `<div class="page">${head(school, `Class timetable: ${name}`, sub(t))}${weekTable(idx, g, 'class')}</div>` : '';
    }).join('');
    printInFrame(classIds.length === 1 ? `Timetable ${t.classes.find((c) => c.id === classIds[0])?.name}` : 'Class timetables',
        css('A4', 10), pages || '<p>No timetable.</p>', 'landscape');
}

export function printTeachers(t: TimetableDetail, staffIds: number[], school: PrintSchool): void {
    let idx = new TimetableIndex(t);
    let pages = staffIds.map((id) => {
        let g = idx.teacherWeek(id);
        let name = t.teachers.find((p) => p.id === id)?.name ?? '';
        let count = t.cards.filter((c) => c.staffDetailsId === id && c.day).reduce((n, c) => n + c.length, 0);
        return g ? `<div class="page">${head(school, `Teacher timetable: ${name}`, `${sub(t)} · ${count} lessons a week`)}${weekTable(idx, g, 'teacher')}</div>` : '';
    }).join('');
    printInFrame(staffIds.length === 1 ? `Timetable ${t.teachers.find((p) => p.id === staffIds[0])?.name}` : 'Teacher timetables',
        css('A4', 10), pages || '<p>No timetable.</p>', 'landscape');
}

/** The master timetable: each bell schedule's classes down, days and lessons across. */
export function printMaster(t: TimetableDetail, school: PrintSchool): void {
    let idx = new TimetableIndex(t);
    let pages = t.schedules.filter((s) => t.classes.some((c) => c.scheduleId === s.id)).map((s) => {
        let m = idx.master(s);
        let headRow1 = m.days.map((d) => `<th colspan="${m.lessons.length}">${dayShort(d)}</th>`).join('');
        let headRow2 = m.days.map(() => m.lessons.map((_, i) => `<th>${i + 1}</th>`).join('')).join('');
        let body = m.rows.map((r) => `<tr><th class="time">${escapeHtml(r.className)}</th>${
            r.cells.map((day) => day.map((c) => cellHtml(c, 'master')).join('')).join('')}</tr>`).join('');
        let used = new Set(t.cards.filter((c) => t.classes.find((k) => k.id === c.schoolClassId)?.scheduleId === s.id));
        let subjects = t.subjects.filter((x) => [...used].some((c) => c.subjectId === x.id));
        let teachers = t.teachers.filter((x) => [...used].some((c) => c.staffDetailsId === x.id));
        let legend = `<div class="legend">${subjects.map((x) => `<div><b>${escapeHtml(x.abbr)}</b> ${escapeHtml(x.name)}</div>`).join('')}</div>
            <div class="legend">${teachers.map((x) => `<div><b>${escapeHtml(x.initials)}</b> ${escapeHtml(x.name)}</div>`).join('')}</div>`;
        let times = m.lessons.map((l, i) => `${i + 1}: ${l.startTime}–${l.endTime}`).join(' · ');
        return `<div class="page">${head(school, `Master timetable: ${s.name}`, sub(t))}
            <table><thead><tr><th class="time" rowspan="2">Class</th>${headRow1}</tr><tr>${headRow2}</tr></thead><tbody>${body}</tbody></table>
            <div class="foot">Lessons ${times}</div>${legend}</div>`;
    }).join('');
    printInFrame('Master timetable', css('A3', 9), pages || '<p>No timetable.</p>', 'landscape');
}
