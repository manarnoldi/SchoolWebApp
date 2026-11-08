import {BellSlotKind, TimetableCard, TimetableDetail, TimetableSchedule, TimetableSlot, WEEK_DAYS} from '../models/timetable-models';

// Turns a timetable's cards into grids - one class's week, one teacher's week,
// or the master timetable (every class of a bell schedule, days across). The
// screens and the printouts both draw from these, so they always agree.

export interface GridEntry {
    card: TimetableCard;
    subject: string;
    abbr: string;
    teacher: string;
    initials: string;
    className: string;
    // The second lesson of a double.
    continued: boolean;
    // When the card's own time differs from the row's (a teacher on two bell schedules).
    time: string | null;
}

export interface GridCell {
    day: number;
    entries: GridEntry[];
    // A blocked slot's activity, e.g. "Games".
    fixed: string | null;
}

export interface GridRow {
    slot: TimetableSlot;
    lessonNo: number | null;
    cells: GridCell[];
}

export interface WeekGrid {
    days: number[];
    rows: GridRow[];
}

export interface MasterGrid {
    schedule: TimetableSchedule;
    days: number[];
    lessons: TimetableSlot[];
    rows: {classId: number; className: string; cells: GridCell[][]}[];   // [day][lesson]
}

export const dayShort = (d: number) => WEEK_DAYS.find((x) => x.value === d)?.short ?? '';
export const dayName = (d: number) => WEEK_DAYS.find((x) => x.value === d)?.name ?? '';
const minutes = (t: string) => {
    let [h, m] = t.split(':').map(Number);
    return h * 60 + m;
};

export class TimetableIndex {
    private slotById = new Map<number, {slot: TimetableSlot; schedule: TimetableSchedule}>();
    private scheduleByClass = new Map<number, TimetableSchedule>();

    constructor(public t: TimetableDetail) {
        t.schedules.forEach((s) => s.slots.forEach((slot) => this.slotById.set(slot.id, {slot, schedule: s})));
        t.classes.forEach((c) => {
            let s = t.schedules.find((x) => x.id === c.scheduleId);
            if (s) this.scheduleByClass.set(c.id, s);
        });
    }

    scheduleOf(classId: number): TimetableSchedule | undefined {
        return this.scheduleByClass.get(classId);
    }

    lessons(s: TimetableSchedule): TimetableSlot[] {
        return s.slots.filter((x) => x.kind === BellSlotKind.Lesson);
    }

    /** The lesson slots a placed card takes: its own, and the next for a double. */
    covers(card: TimetableCard): TimetableSlot[] {
        if (!card.day || !card.bellSlotId) return [];
        let at = this.slotById.get(card.bellSlotId);
        if (!at) return [];
        let lessons = this.lessons(at.schedule);
        let i = lessons.findIndex((x) => x.id === card.bellSlotId);
        return lessons.slice(i, i + card.length);
    }

    /** "08:00–09:20" for a card. */
    timeOf(card: TimetableCard): string {
        let c = this.covers(card);
        return c.length ? `${c[0].startTime}–${c[c.length - 1].endTime}` : '';
    }

    entry(card: TimetableCard, continued: boolean): GridEntry {
        let s = this.t.subjects.find((x) => x.id === card.subjectId);
        let p = this.t.teachers.find((x) => x.id === card.staffDetailsId);
        return {
            card,
            subject: s?.name ?? '',
            abbr: s?.abbr ?? '',
            teacher: p?.name ?? '',
            initials: p?.initials ?? (card.staffDetailsId ? '' : '—'),
            className: this.t.classes.find((x) => x.id === card.schoolClassId)?.name ?? '',
            continued,
            time: null
        };
    }

    private fixedFor(s: TimetableSchedule, classId: number | null, day: number, slotId: number): string | null {
        let level = classId ? this.t.classes.find((c) => c.id === classId)?.learningLevelId : null;
        let f = s.fixed.find((x) => x.day === day && x.bellSlotId === slotId && (!x.learningLevelId || x.learningLevelId === level));
        return f ? f.label : null;
    }

    private rowsFor(s: TimetableSchedule): GridRow[] {
        let n = 0;
        return s.slots.map((slot) => ({
            slot,
            lessonNo: slot.kind === BellSlotKind.Lesson ? ++n : null,
            cells: s.days.map((day) => ({day, entries: [], fixed: null}))
        }));
    }

    classWeek(classId: number): WeekGrid | null {
        let s = this.scheduleOf(classId);
        if (!s) return null;
        let rows = this.rowsFor(s);
        rows.forEach((r) => r.cells.forEach((c) => (c.fixed = r.lessonNo ? this.fixedFor(s!, classId, c.day, r.slot.id) : null)));
        for (let card of this.t.cards.filter((c) => c.schoolClassId === classId && c.day)) {
            this.covers(card).forEach((slot, k) => {
                let cell = rows.find((r) => r.slot.id === slot.id)?.cells.find((c) => c.day === card.day);
                cell?.entries.push(this.entry(card, k > 0));
            });
        }
        return {days: s.days, rows};
    }

    /**
     * A teacher's week, on the bell schedule they teach most on. Lessons on
     * another schedule go in the row they overlap, with their own time shown.
     */
    teacherWeek(staffId: number): WeekGrid | null {
        let cards = this.t.cards.filter((c) => c.staffDetailsId === staffId && c.day);
        if (!cards.length) return null;
        let counts = new Map<TimetableSchedule, number>();
        cards.forEach((c) => {
            let s = this.scheduleOf(c.schoolClassId);
            if (s) counts.set(s, (counts.get(s) ?? 0) + c.length);
        });
        let main = [...counts.entries()].sort((a, b) => b[1] - a[1])[0][0];
        let days = Array.from(new Set([...counts.keys()].flatMap((s) => s.days))).sort();
        let rows = this.rowsFor({...main, days});
        for (let card of cards) {
            let own = this.scheduleOf(card.schoolClassId) === main;
            this.covers(card).forEach((slot, k) => {
                let row = own
                    ? rows.find((r) => r.slot.id === slot.id)
                    : rows.find((r) => r.lessonNo && minutes(r.slot.startTime) < minutes(slot.endTime) && minutes(slot.startTime) < minutes(r.slot.endTime));
                let cell = row?.cells.find((c) => c.day === card.day);
                if (!cell) return;
                let e = this.entry(card, k > 0);
                if (!own) e.time = `${slot.startTime}–${slot.endTime}`;
                cell.entries.push(e);
            });
        }
        return {days, rows};
    }

    master(s: TimetableSchedule): MasterGrid {
        let lessons = this.lessons(s);
        let classes = this.t.classes.filter((c) => c.scheduleId === s.id);
        return {
            schedule: s,
            days: s.days,
            lessons,
            rows: classes.map((c) => ({
                classId: c.id,
                className: c.name,
                cells: s.days.map((day) => lessons.map((slot) => ({day, entries: [], fixed: this.fixedFor(s, c.id, day, slot.id)} as GridCell)))
            })).map((row) => {
                for (let card of this.t.cards.filter((x) => x.schoolClassId === row.classId && x.day)) {
                    let d = s.days.indexOf(card.day!);
                    this.covers(card).forEach((slot, k) => {
                        let i = lessons.findIndex((x) => x.id === slot.id);
                        if (d >= 0 && i >= 0) row.cells[d][i].entries.push(this.entry(card, k > 0));
                    });
                }
                return row;
            })
        };
    }

    /** One meeting of an option group shows as one entry per subject; these pair them up. */
    static label(entries: GridEntry[], withTeacher: 'initials' | 'name' | 'none' = 'initials', withClass = false): string {
        return entries
            .map((e) => [withClass ? e.className : '', e.abbr, withTeacher === 'none' ? '' : withTeacher === 'name' ? e.teacher : e.initials]
                .filter((x) => x).join(' '))
            .join(' / ');
    }
}
