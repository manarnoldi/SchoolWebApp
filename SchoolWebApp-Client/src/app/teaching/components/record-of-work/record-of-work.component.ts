import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, ParamMap, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {
    dateOnly,
    LessonTaughtStatus,
    RecordOfWork,
    RecordOfWorkCheck,
    RecordOfWorkRow,
    STATUS_BADGES,
    STATUS_LABELS,
    TAUGHT_LABELS,
    TeachingContext
} from '@/teaching/models/teaching-models';
import {RecordOfWorkService, TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {AssignmentOpened} from '../teaching-assignments/teaching-assignments.component';
import {PrintSchool, printRecordOfWork} from '@/teaching/utils/teaching-print';
import {backLabel, fromParam, withFrom} from '@/teaching/utils/back-link';

interface WeekRows {
    weekNo: number;
    rows: RecordOfWorkRow[];
    check: RecordOfWorkCheck | undefined;
}

/**
 * Record of work: the scheme's lessons with what was actually taught. Filled
 * here or from the lesson plans' after-lesson notes; the head of department
 * checks it week by week, and a checked week is locked until the check is
 * removed.
 */
@Component({
    selector: 'app-record-of-work',
    templateUrl: './record-of-work.component.html',
    styleUrl: '../teaching.scss'
})
export class RecordOfWorkComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/teaching/record-of-work'], title: 'Record of Work'}
    ];
    dashboardTitle = 'Teaching Management: Record of Work';

    context: TeachingContext | null = null;
    record: RecordOfWork | null = null;
    weeks: WeekRows[] = [];
    school: PrintSchool = {name: ''};
    dirty = new Set<number>();
    // The one week shown open; the rest are folded.
    openWeek: number | null = null;

    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;
    taughtLabels = TAUGHT_LABELS;
    taughtOptions = [LessonTaughtStatus.NotRecorded, LessonTaughtStatus.Taught, LessonTaughtStatus.PartlyTaught, LessonTaughtStatus.NotTaught];

    // Where this page was opened from (another teaching page), for the Back button.
    from: string | null = null;
    get backText(): string { return backLabel(this.from, this.router); }

    constructor(private route: ActivatedRoute, private router: Router, private toastr: ToastrService,
                private teachingSvc: TeachingDocumentsService, private svc: RecordOfWorkService,
                private schoolSvc: SchoolDetailsService) {}

    ngOnInit(): void {
        this.teachingSvc.context().subscribe({next: (c) => this.context = c});
        this.schoolSvc.get('/schoolDetails').subscribe({next: (r: any) => {
            let s = (r || [])[0] || r;
            this.school = {name: s?.name || '', address: s?.address, logo: s?.logoAsBase64};
        }});
        this.route.queryParamMap.subscribe((q) => this.followUrl(q));
    }

    // The URL says which scheme's record is open, or the list; following it lets
    // the browser's Back button return to the list it was opened from.
    private followUrl(q: ParamMap) {
        let id = +(q.get('schemeId') || 0);
        if (this.record && this.dirty.size && this.record.scheme.id !== id) {
            if (!window.confirm('Leave without saving your changes?')) {
                this.router.navigate([], {relativeTo: this.route, queryParams: withFrom(this.from, {schemeId: this.record.scheme.id})});
                return;
            }
            this.dirty.clear();
        }
        this.from = fromParam(this.route);
        if (id) { if (this.record?.scheme.id !== id) this.open(id); }
        else if (this.record) { this.record = null; this.weeks = []; }
    }

    get editable(): boolean {
        return !!this.record?.scheme.canEdit;
    }

    onOpened(e: AssignmentOpened) {
        // A history entry, so Back returns to the list as it was.
        if (e.assignment.schemeOfWorkId)
            this.router.navigate([], {relativeTo: this.route, queryParams: {schemeId: e.assignment.schemeOfWorkId, from: this.router.url}});
    }

    open(schemeId: number) {
        this.svc.get(schemeId).subscribe({
            next: (r) => {
                this.set(r);
                this.router.navigate([], {relativeTo: this.route, queryParams: withFrom(this.from, {schemeId}), replaceUrl: true});
            },
            error: (err) => this.toastr.error(err.status === 403
                ? 'You do not have access to this record of work.'
                : (err.error?.message || 'Error loading the record of work.'))
        });
    }

    private set(r: RecordOfWork) {
        r.rows.forEach((x) => x.taughtOn = dateOnly(x.taughtOn));
        this.record = r;
        this.dirty.clear();
        let weekNos = Array.from(new Set(r.rows.map((x) => x.weekNo))).sort((a, b) => a - b);
        this.weeks = weekNos.map((w) => ({
            weekNo: w,
            rows: r.rows.filter((x) => x.weekNo === w),
            check: r.checks.find((c) => c.weekNo === w)
        }));
        // Open the first week with a lesson still to record.
        this.openWeek = this.weeks.find((w) => this.recorded(w) < w.rows.length)?.weekNo ?? null;
    }

    toggleWeek(w: WeekRows) {
        this.openWeek = this.openWeek === w.weekNo ? null : w.weekNo;
    }

    recorded(w: WeekRows): number {
        return w.rows.filter((x) => +x.status !== LessonTaughtStatus.NotRecorded).length;
    }

    hasUnsaved(w: WeekRows): boolean {
        return w.rows.some((x) => this.dirty.has(x.schemeOfWorkLessonId));
    }

    back() {
        if (this.dirty.size && !window.confirm('Leave without saving your changes?')) return;
        this.dirty.clear();
        if (this.from) this.router.navigateByUrl(this.from);
        else this.router.navigate([], {relativeTo: this.route, queryParams: {}});
    }

    get coverage(): number {
        let r = this.record;
        if (!r || !r.planned) return 0;
        return Math.round(100 * (r.taught + r.partlyTaught / 2) / r.planned);
    }

    touch(row: RecordOfWorkRow) {
        // Marking a lesson taught defaults the date to today.
        if ((row.status === LessonTaughtStatus.Taught || row.status === LessonTaughtStatus.PartlyTaught) && !row.taughtOn) {
            row.taughtOn = new Date().toLocaleDateString('en-CA');
        }
        this.dirty.add(row.schemeOfWorkLessonId);
    }

    save() {
        let r = this.record!;
        let entries = r.rows.filter((x) => this.dirty.has(x.schemeOfWorkLessonId)).map((x) => ({
            schemeOfWorkLessonId: x.schemeOfWorkLessonId, status: +x.status, taughtOn: x.taughtOn,
            workCovered: x.workCovered, remarks: x.remarks
        }));
        if (!entries.length) return;
        this.svc.saveEntries(r.scheme.id, entries).subscribe({
            next: (res) => { this.set(res); this.toastr.success('Record of work saved.'); },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving the record of work.')
        });
    }

    check(w: WeekRows) {
        Swal.fire({
            title: `Mark week ${w.weekNo} as checked?`,
            input: 'textarea', inputPlaceholder: 'Comment for the teacher (optional)',
            width: 420, position: 'top', icon: 'question', showCancelButton: true, confirmButtonText: 'Checked'
        }).then((res) => {
            if (!res.isConfirmed) return;
            this.svc.check(this.record!.scheme.id, w.weekNo, res.value || null).subscribe({
                next: (r) => { this.set(r); this.toastr.success(`Week ${w.weekNo} checked.`); },
                error: (err) => this.toastr.error(err.error?.message || 'Error checking the week.')
            });
        });
    }

    canRemoveCheck(c: RecordOfWorkCheck): boolean {
        return !!this.context && (c.checkedByUserId === this.context.userId || this.context.isAdmin);
    }

    removeCheck(c: RecordOfWorkCheck) {
        Swal.fire({title: `Remove the check on week ${c.weekNo}?`, text: 'The teacher can then change that week again.',
            width: 400, position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Remove'})
            .then((res) => {
                if (!res.value) return;
                this.svc.removeCheck(c.id).subscribe({
                    next: (r) => this.set(r),
                    error: (err) => this.toastr.error(err.error?.message || 'Error removing the check.')
                });
            });
    }

    print() {
        if (this.dirty.size) this.toastr.info('Unsaved changes are not in the printout - save first.');
        printRecordOfWork(this.record!, this.school);
    }
}
