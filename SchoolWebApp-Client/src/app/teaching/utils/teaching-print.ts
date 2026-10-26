import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {
    LessonPlanWeek,
    RecordOfWork,
    SchemeOfWork,
    STATUS_LABELS,
    TAUGHT_LABELS
} from '../models/teaching-models';

// Printed teaching documents, laid out like the schools' own Word templates:
// the scheme as one landscape table, lesson plans one lesson to a page, the
// record of work as a landscape table with the HOD's weekly checks.

export interface PrintSchool {
    name: string;
    address?: string | null;
    logo?: string | null;
}

// Escaped, with line breaks kept.
const text = (s: string | null | undefined): string => escapeHtml(s || '').replace(/\r?\n/g, '<br>');

// A calendar date (lesson date, date taught) - the date part as stored.
const day = (iso: string | null | undefined): string => {
    if (!iso) return '';
    let [y, m, d] = iso.substring(0, 10).split('-').map(Number);
    return new Date(y, m - 1, d).toLocaleDateString('en-GB', {weekday: 'short', day: 'numeric', month: 'short', year: 'numeric'});
};

// A moment (when a week was checked) - shown in local time.
const stamp = (iso: string | null | undefined): string =>
    iso ? new Date(iso).toLocaleDateString('en-GB', {day: 'numeric', month: 'short', year: 'numeric'}) : '';

const letterhead = (school: PrintSchool, title: string, sub: string): string => `
    <div class="head">
        ${school.logo ? `<img src="${school.logo}" alt="">` : ''}
        <div>
            <div class="school">${escapeHtml(school.name)}</div>
            ${school.address ? `<div class="addr">${escapeHtml(school.address)}</div>` : ''}
            <div class="title">${escapeHtml(title)}</div>
            <div class="sub">${sub}</div>
        </div>
    </div>`;

const baseCss = (orientation: 'portrait' | 'landscape') => `
    @page { size: A4 ${orientation}; margin: 10mm; }
    body { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #111; margin: 0; }
    .head { display: flex; align-items: center; gap: 10px; border-bottom: 2px solid #333; padding-bottom: 4px; margin-bottom: 6px; }
    .head img { height: 52px; }
    .school { font-size: 15px; font-weight: bold; text-transform: uppercase; }
    .addr { font-size: 10px; color: #444; }
    .title { font-size: 13px; font-weight: bold; margin-top: 2px; text-transform: uppercase; }
    .sub { font-size: 10px; margin-top: 2px; }
    table { border-collapse: collapse; width: 100%; }
    th, td { border: 1px solid #555; padding: 3px 4px; vertical-align: top; text-align: left; }
    th { background: #e9e9e9; }
    .c { text-align: center; }
    .muted { color: #666; }
    tr { page-break-inside: avoid; }
    .sign { margin-top: 18px; display: flex; gap: 40px; }
    .sign div { flex: 1; border-top: 1px dotted #333; padding-top: 2px; }`;

export function printScheme(s: SchemeOfWork, school: PrintSchool): void {
    let lessons = s.lessons.filter((l) => l.strandName || l.subStrandName || l.learningOutcomes || l.learningExperiences);
    let rows = '';
    let weeks = Array.from(new Set(lessons.map((l) => l.weekNo))).sort((a, b) => a - b);
    weeks.forEach((w) => {
        let inWeek = lessons.filter((l) => l.weekNo === w).sort((a, b) => a.lessonNo - b.lessonNo);
        inWeek.forEach((l, i) => {
            rows += `<tr>
                ${i === 0 ? `<td class="c" rowspan="${inWeek.length}"><b>${w}</b></td>` : ''}
                <td class="c">${l.lessonNo}</td>
                <td>${text(l.strandName)}</td>
                <td>${text(l.subStrandName)}</td>
                <td>${text(l.learningOutcomes)}</td>
                <td>${text(l.learningExperiences)}</td>
                <td>${text(l.keyInquiryQuestions)}</td>
                <td>${text(l.learningResources)}</td>
                <td>${text(l.assessmentMethods)}</td>
            </tr>`;
        });
    });
    let title = `${s.academicYearName} ${s.learningLevelName} ${s.subjectName} Schemes of Work ${s.sessionName}`;
    let sub = `Teacher: <b>${escapeHtml(s.staffName)}</b> &nbsp; Class: <b>${escapeHtml(s.className)}</b>`
        + (s.references ? ` &nbsp; References: ${escapeHtml(s.references)}` : '')
        + ` &nbsp; Status: ${STATUS_LABELS[s.status]}`;
    let body = letterhead(school, title, sub) + `
        <table>
            <thead><tr>
                <th style="width:3%">Wk</th><th style="width:3%">Lsn</th><th style="width:9%">Strand</th>
                <th style="width:10%">Sub-strand</th><th style="width:17%">Lesson Learning Outcomes</th>
                <th style="width:17%">Learning Experiences</th><th style="width:11%">Key Inquiry Question(s)</th>
                <th style="width:14%">Learning Resources</th><th style="width:16%">Assessment Methods</th>
            </tr></thead>
            <tbody>${rows || '<tr><td colspan="9" class="c muted">No lessons filled in.</td></tr>'}</tbody>
        </table>
        <div class="sign"><div>Teacher's signature &amp; date</div><div>HOD's signature &amp; date</div><div>Head Teacher's signature &amp; date</div></div>`;
    printInFrame(title, baseCss('landscape'), body, 'landscape');
}

export function printLessonPlans(week: LessonPlanWeek, school: PrintSchool): void {
    let s = week.scheme;
    let pages = week.plans.map((p) => {
        let steps = p.steps.map((st) => `<div><b>Step ${st.stepNo}:</b> ${text(st.activity)}</div>`).join('');
        let time = [p.startTime, p.endTime].filter((t) => !!t).join(' - ');
        let mins = (m: number | null) => (m ? `${m} minutes` : '');
        return `<div class="page">
            ${letterhead(school, `Lesson Plan - Week ${week.weekNo}: Lesson ${p.lessonNo}`, '')}
            <table>
                <tr><th>School</th><th>Level</th><th>Learning Area</th><th>Date</th><th>Time</th><th>Roll</th></tr>
                <tr><td>${escapeHtml(school.name)}</td><td>${escapeHtml(s.className)}</td><td>${escapeHtml(s.subjectName)}</td>
                    <td>${day(p.lessonDate)}</td><td>${escapeHtml(time)}</td><td>${p.roll ?? ''}</td></tr>
            </table>
            <div class="block"><b>Strand:</b> ${text(p.strandName)}</div>
            <div class="block"><b>Sub strand:</b> ${text(p.subStrandName)}</div>
            <div class="block"><b>Specific learning outcomes:</b><br>${text(p.learningOutcomes)}</div>
            <div class="block"><b>Key inquiry question(s):</b><br>${text(p.keyInquiryQuestions)}</div>
            <div class="block"><b>Learning resources:</b><br>${text(p.learningResources)}</div>
            <div class="block"><b>Organisation of learning:</b><br>${text(p.organisationOfLearning)}</div>
            <table class="flow">
                <tr><th style="width:14%">Time</th><th>Content</th></tr>
                <tr><td>${mins(p.introductionMinutes)}</td><td><b>Introduction</b><br>${text(p.introduction)}</td></tr>
                <tr><td>${mins(p.developmentMinutes)}</td><td><b>Lesson development</b>${steps}</td></tr>
                <tr><td></td><td><b>Extended activities</b><br>${text(p.extendedActivities)}</td></tr>
                <tr><td>${mins(p.conclusionMinutes)}</td><td><b>Conclusion</b><br>${text(p.conclusion)}</td></tr>
            </table>
            <div class="block"><b>Summary:</b> ${text(p.workCovered) || '.....................................................................'}</div>
            <div class="block"><b>Reflection on the lesson / self-remarks:</b> ${text(p.remarks) || '.....................................................................'}</div>
            <div class="sign"><div>Teacher: ${escapeHtml(s.staffName)}</div><div>HOD's signature &amp; date</div></div>
        </div>`;
    }).join('');
    let title = `${s.className} ${s.subjectName} lesson plans - ${s.sessionName} ${s.academicYearName} week ${week.weekNo}`;
    let css = baseCss('portrait') + `
        body { font-size: 11px; }
        .page { page-break-after: always; }
        .page:last-child { page-break-after: auto; }
        .block { margin-top: 6px; }
        table.flow { margin-top: 8px; }`;
    printInFrame(title, css, pages || '<p>No lesson plans.</p>');
}

export function printRecordOfWork(r: RecordOfWork, school: PrintSchool): void {
    let s = r.scheme;
    let rows = '';
    let weeks = Array.from(new Set(r.rows.map((x) => x.weekNo))).sort((a, b) => a - b);
    weeks.forEach((w) => {
        let inWeek = r.rows.filter((x) => x.weekNo === w && (x.subStrandName || x.status));
        let check = r.checks.find((c) => c.weekNo === w);
        inWeek.forEach((x, i) => {
            rows += `<tr>
                ${i === 0 ? `<td class="c" rowspan="${inWeek.length}"><b>${w}</b></td>` : ''}
                <td class="c">${x.lessonNo}</td>
                <td>${day(x.taughtOn)}</td>
                <td>${text(x.strandName)}${x.subStrandName ? ' - ' + text(x.subStrandName) : ''}</td>
                <td>${text(x.workCovered)}</td>
                <td>${x.status ? TAUGHT_LABELS[x.status] : ''}</td>
                <td>${text(x.remarks)}</td>
                ${i === 0 ? `<td rowspan="${inWeek.length}">${check
                    ? `${escapeHtml(check.checkedByName)}<br>${stamp(check.checkedAt)}${check.comment ? '<br><i>' + text(check.comment) + '</i>' : ''}`
                    : ''}</td>` : ''}
            </tr>`;
        });
    });
    let title = `${s.academicYearName} ${s.learningLevelName} ${s.subjectName} Record of Work ${s.sessionName}`;
    let sub = `Teacher: <b>${escapeHtml(s.staffName)}</b> &nbsp; Class: <b>${escapeHtml(s.className)}</b>`
        + ` &nbsp; Lessons taught: ${r.taught} fully, ${r.partlyTaught} partly, of ${r.planned} planned`;
    let body = letterhead(school, title, sub) + `
        <table>
            <thead><tr>
                <th style="width:4%">Wk</th><th style="width:4%">Lsn</th><th style="width:10%">Date</th>
                <th style="width:20%">Strand / Sub-strand</th><th style="width:24%">Work covered</th>
                <th style="width:8%">Status</th><th style="width:18%">Remarks</th><th style="width:12%">Checked by (HOD)</th>
            </tr></thead>
            <tbody>${rows || '<tr><td colspan="8" class="c muted">No lessons in the scheme.</td></tr>'}</tbody>
        </table>`;
    printInFrame(title, baseCss('landscape'), body, 'landscape');
}
