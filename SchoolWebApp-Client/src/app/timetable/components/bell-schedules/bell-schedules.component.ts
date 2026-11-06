import {Component, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {
    BellSchedule,
    BellSlotKind,
    DayPart,
    FixedSlot,
    SLOT_KINDS,
    TimetableLookups,
    WEEK_DAYS
} from '@/timetable/models/timetable-models';
import {TimetableSetupService} from '@/timetable/services/timetable-setup.service';

/** A slot as edited: its length, with times running on from the slot before. */
interface EditSlot {
    id: number;
    minutes: number;
    kind: BellSlotKind;
    label: string | null;
}

interface EditSchedule {
    id: number;
    name: string;
    description: string | null;
    days: number[];
    dayStart: string;
    slots: EditSlot[];
    learningLevelIds: number[];
    fixedSlots: FixedSlot[];
}

const toMinutes = (t: string) => {
    let [h, m] = (t || '0:0').split(':').map((x) => +x || 0);
    return h * 60 + m;
};
const toTime = (min: number) => `${String(Math.floor(min / 60) % 24).padStart(2, '0')}:${String(min % 60).padStart(2, '0')}`;

/**
 * Bell schedules: the school day for a group of grades. Slots run one after
 * another from the day's start, so changing a lesson's length moves everything
 * after it - as a real bell does. Slots after lunch are the afternoon.
 */
@Component({
    selector: 'app-tt-bell-schedules',
    templateUrl: './bell-schedules.component.html',
    styleUrl: '../timetable.scss'
})
export class BellSchedulesComponent implements OnInit {
    @Input() lookups!: TimetableLookups;

    schedules: BellSchedule[] = [];
    edit: EditSchedule | null = null;
    saving = false;
    kinds = SLOT_KINDS;
    Kind = BellSlotKind;
    days = WEEK_DAYS;

    // "Build the day" quick fill.
    build = {start: '08:00', lessonMinutes: 40, lessons: 8, shortAfter: 2, shortMinutes: 20, teaAfter: 4, teaMinutes: 30,
        lunchAfter: 6, lunchMinutes: 60};

    // The week grid cell being edited: day and slot position.
    cell: {day: number; slotIndex: number} | null = null;

    constructor(private svc: TimetableSetupService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.load();
    }

    load(selectId?: number) {
        this.svc.bellSchedules().subscribe({
            next: (s) => {
                this.schedules = s;
                let pick = s.find((x) => x.id === (selectId ?? this.edit?.id)) ?? s[0];
                if (pick) this.select(pick);
                else this.edit = null;
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading bell schedules.')
        });
    }

    select(s: BellSchedule) {
        this.cell = null;
        this.edit = {
            id: s.id,
            name: s.name,
            description: s.description,
            days: [...s.days],
            dayStart: s.slots[0]?.startTime ?? '08:00',
            slots: s.slots.map((x, i) => ({
                id: x.id,
                // A gap before the next slot is kept as part of this slot's length.
                minutes: (i + 1 < s.slots.length ? toMinutes(s.slots[i + 1].startTime) : toMinutes(x.endTime)) - toMinutes(x.startTime),
                kind: x.kind,
                label: x.label
            })),
            learningLevelIds: [...s.learningLevelIds],
            fixedSlots: s.fixedSlots.map((f) => ({...f}))
        };
    }

    newSchedule() {
        this.cell = null;
        this.edit = {id: 0, name: this.schedules.length ? '' : 'Whole School', description: null, days: [1, 2, 3, 4, 5],
            dayStart: '08:00', slots: [], learningLevelIds: [], fixedSlots: []};
        this.buildDay();
    }

    // ------------------------------------------------------------ slots

    buildDay() {
        if (!this.edit) return;
        let b = this.build;
        let slots: EditSlot[] = [];
        for (let n = 1; n <= b.lessons; n++) {
            slots.push({id: 0, minutes: b.lessonMinutes, kind: BellSlotKind.Lesson, label: null});
            if (n === b.shortAfter && n < b.lessons) slots.push({id: 0, minutes: b.shortMinutes, kind: BellSlotKind.Break, label: 'Short break'});
            if (n === b.teaAfter && n < b.lessons) slots.push({id: 0, minutes: b.teaMinutes, kind: BellSlotKind.Break, label: 'Tea break'});
            if (n === b.lunchAfter && n < b.lessons) slots.push({id: 0, minutes: b.lunchMinutes, kind: BellSlotKind.Lunch, label: 'Lunch'});
        }
        // Keep existing slot ids where the position still exists, so timetables keep their lessons.
        this.edit.slots.forEach((s, i) => {
            if (slots[i]) slots[i].id = s.id;
        });
        this.edit.dayStart = b.start;
        this.edit.slots = slots;
        this.edit.fixedSlots = this.edit.fixedSlots.filter((f) => f.slotIndex < slots.length);
    }

    start(i: number): string {
        let m = toMinutes(this.edit!.dayStart);
        for (let k = 0; k < i; k++) m += +this.edit!.slots[k].minutes || 0;
        return toTime(m);
    }

    end(i: number): string {
        return toTime(toMinutes(this.start(i)) + (+this.edit!.slots[i].minutes || 0));
    }

    part(i: number): DayPart {
        let slots = this.edit!.slots;
        let lunch = slots.findIndex((s) => s.kind === BellSlotKind.Lunch);
        if (lunch >= 0) return i > lunch ? DayPart.Afternoon : DayPart.Morning;
        return toMinutes(this.start(i)) >= 12 * 60 + 30 ? DayPart.Afternoon : DayPart.Morning;
    }

    lessonNo(i: number): number {
        return this.edit!.slots.slice(0, i + 1).filter((s) => s.kind === BellSlotKind.Lesson).length;
    }

    get lessonsPerDay(): number {
        return this.edit?.slots.filter((s) => s.kind === BellSlotKind.Lesson).length ?? 0;
    }

    insertAfter(i: number, kind: BellSlotKind) {
        let s = this.edit!;
        let minutes = kind === BellSlotKind.Lesson ? this.build.lessonMinutes : kind === BellSlotKind.Lunch ? 60 : 20;
        let label = kind === BellSlotKind.Break ? 'Break' : kind === BellSlotKind.Lunch ? 'Lunch' : kind === BellSlotKind.Activity ? 'Activity' : null;
        s.slots.splice(i + 1, 0, {id: 0, minutes, kind, label});
        s.fixedSlots.forEach((f) => {
            if (f.slotIndex > i) f.slotIndex++;
        });
    }

    remove(i: number) {
        let s = this.edit!;
        s.slots.splice(i, 1);
        s.fixedSlots = s.fixedSlots.filter((f) => f.slotIndex !== i);
        s.fixedSlots.forEach((f) => {
            if (f.slotIndex > i) f.slotIndex--;
        });
        this.cell = null;
    }

    // ------------------------------------------------------------ days and grades

    toggleDay(d: number) {
        let s = this.edit!;
        s.days = s.days.includes(d) ? s.days.filter((x) => x !== d) : [...s.days, d].sort();
        s.fixedSlots = s.fixedSlots.filter((f) => s.days.includes(f.day));
    }

    get levelGroups(): {name: string; levels: TimetableLookups['levels']}[] {
        let groups: {name: string; levels: TimetableLookups['levels']}[] = [];
        for (let l of this.lookups.levels) {
            let g = groups.find((x) => x.name === l.educationLevel);
            if (!g) groups.push((g = {name: l.educationLevel, levels: []}));
            g.levels.push(l);
        }
        return groups;
    }

    /** The other schedule a grade already follows, if any. */
    takenBy(levelId: number): string | null {
        let other = this.schedules.find((s) => s.id !== this.edit?.id && s.learningLevelIds.includes(levelId));
        return other ? other.name : null;
    }

    toggleLevel(levelId: number) {
        if (this.takenBy(levelId)) return;
        let s = this.edit!;
        s.learningLevelIds = s.learningLevelIds.includes(levelId)
            ? s.learningLevelIds.filter((x) => x !== levelId)
            : [...s.learningLevelIds, levelId];
    }

    levelName(id: number | null): string {
        return id == null ? 'All grades' : this.lookups.levels.find((l) => l.id === id)?.name ?? '';
    }

    subjectAbbr(id: number | null): string {
        return this.lookups.subjects.find((s) => s.id === id)?.abbr ?? '';
    }

    // ------------------------------------------------------------ fixed slots (the week grid)

    get scheduleDays() {
        return this.days.filter((d) => this.edit!.days.includes(d.value));
    }

    fixedAt(day: number, slotIndex: number): FixedSlot[] {
        return this.edit!.fixedSlots.filter((f) => f.day === day && f.slotIndex === slotIndex);
    }

    cellText(day: number, slotIndex: number): string {
        return this.fixedAt(day, slotIndex)
            .map((f) => (f.subjectId ? this.subjectAbbr(f.subjectId) : f.label) + (f.learningLevelId ? ` (${this.levelName(f.learningLevelId)})` : ''))
            .join(', ');
    }

    cellClass(day: number, slotIndex: number): string {
        let f = this.fixedAt(day, slotIndex);
        if (!f.length) return '';
        return f.some((x) => x.subjectId) ? 'tt-fixed-subject' : 'tt-fixed-activity';
    }

    pick(day: number, slotIndex: number) {
        this.cell = this.cell?.day === day && this.cell.slotIndex === slotIndex ? null : {day, slotIndex};
    }

    get cellFixed(): FixedSlot[] {
        return this.cell ? this.fixedAt(this.cell.day, this.cell.slotIndex) : [];
    }

    addFixed(kind: 'activity' | 'subject') {
        if (!this.cell) return;
        this.edit!.fixedSlots.push({id: 0, day: this.cell.day, slotIndex: this.cell.slotIndex, learningLevelId: null,
            subjectId: kind === 'subject' ? this.lookups.subjects[0]?.id ?? null : null, label: kind === 'activity' ? 'Games' : null});
    }

    removeFixed(f: FixedSlot) {
        this.edit!.fixedSlots = this.edit!.fixedSlots.filter((x) => x !== f);
    }

    get scheduleLevels() {
        return this.lookups.levels.filter((l) => this.edit!.learningLevelIds.includes(l.id));
    }

    dayName(d: number): string {
        return this.days.find((x) => x.value === d)?.name ?? '';
    }

    // ------------------------------------------------------------ save / delete

    save() {
        let s = this.edit!;
        if (!s.name.trim()) {
            this.toastr.warning('Give the schedule a name.');
            return;
        }
        if (s.slots.some((x) => !(+x.minutes > 0))) {
            this.toastr.warning('Every slot needs a length in minutes.');
            return;
        }
        let dto: BellSchedule = {
            id: s.id,
            name: s.name.trim(),
            description: s.description,
            days: s.days,
            slots: s.slots.map((x, i) => ({id: x.id, startTime: this.start(i), endTime: this.end(i), kind: x.kind,
                part: this.part(i), label: x.label})),
            learningLevelIds: s.learningLevelIds,
            fixedSlots: s.fixedSlots
        };
        this.saving = true;
        this.svc.saveBellSchedule(dto).subscribe({
            next: (r) => {
                this.saving = false;
                this.toastr.success('Bell schedule saved.');
                this.load(r.id);
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(err.error?.message || 'Error saving the bell schedule.');
            }
        });
    }

    delete() {
        let s = this.edit!;
        if (!s.id) {
            this.edit = null;
            this.load();
            return;
        }
        Swal.fire({title: `Delete the ${s.name} schedule?`, width: 420, position: 'top', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (!r.value) return;
            this.svc.deleteBellSchedule(s.id).subscribe({
                next: () => {
                    this.toastr.success('Bell schedule deleted.');
                    this.edit = null;
                    this.load();
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error deleting the bell schedule.')
            });
        });
    }
}
