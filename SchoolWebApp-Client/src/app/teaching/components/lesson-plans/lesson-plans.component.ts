import {Component, HostListener, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ApprovalService} from '@/approvals/services/approval.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {
    CurriculumStrand,
    CurriculumSubStrand,
    dateOnly,
    LessonPlan,
    LessonPlanWeek,
    LessonPlanWeekSummary,
    LessonTaughtStatus,
    SchemeOfWork,
    STATUS_BADGES,
    STATUS_CLASSES,
    STATUS_LABELS,
    TAUGHT_LABELS,
    TeachingContext
} from '@/teaching/models/teaching-models';
import {LessonPlansService, SchemesOfWorkService, TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {AssignmentOpened} from '../teaching-assignments/teaching-assignments.component';
import {PrintSchool, printLessonPlans} from '@/teaching/utils/teaching-print';

/**
 * Lesson plans, a week at a time. A week is prepared from the scheme's lessons
 * for it (strand, sub-strand, outcomes, key question, resources, and the
 * learning experiences as steps), completed by the teacher and submitted as one.
 * After each lesson the teacher records whether it was taught, a summary and a
 * reflection - these feed the record of work and stay open after approval.
 */
@Component({
    selector: 'app-lesson-plans',
    templateUrl: './lesson-plans.component.html',
    styleUrl: '../teaching.scss'
})
export class LessonPlansComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/teaching/lesson-plans'], title: 'Lesson Plans'}
    ];
    dashboardTitle = 'Teaching Documents: Lesson Plans';

    context: TeachingContext | null = null;
    scheme: SchemeOfWork | null = null;
    weeks: LessonPlanWeekSummary[] = [];
    week: LessonPlanWeek | null = null;
    strands: CurriculumStrand[] = [];
    preferredApprovers: number[] = [];
    school: PrintSchool = {name: ''};
    expanded = new Set<number>();
    dirtyPlans = new Set<number>();
    historyVersion = 0;

    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;
    statusClasses = STATUS_CLASSES;
    taughtLabels = TAUGHT_LABELS;
    taughtOptions = [LessonTaughtStatus.NotRecorded, LessonTaughtStatus.Taught, LessonTaughtStatus.PartlyTaught, LessonTaughtStatus.NotTaught];

    constructor(private route: ActivatedRoute, private router: Router, private toastr: ToastrService,
                private teachingSvc: TeachingDocumentsService, private schemeSvc: SchemesOfWorkService,
                private svc: LessonPlansService, private approvalSvc: ApprovalService,
                private schoolSvc: SchoolDetailsService) {}

    ngOnInit(): void {
        this.teachingSvc.context().subscribe({next: (c) => this.context = c});
        this.schoolSvc.get('/schoolDetails').subscribe({next: (r: any) => {
            let s = (r || [])[0] || r;
            this.school = {name: s?.name || '', address: s?.address, logo: s?.logoAsBase64};
        }});
        // ?actId= (a lesson plan week) from the approvals bell; ?weekId= / ?schemeId= from the tracker.
        let q = this.route.snapshot.queryParamMap;
        let weekId = +(q.get('actId') || q.get('weekId') || 0);
        let schemeId = +(q.get('schemeId') || 0);
        if (weekId) this.openWeek(weekId, true);
        else if (schemeId) this.openScheme(schemeId);
    }

    get editable(): boolean {
        return !!this.week && this.week.canEdit && !this.week.isLocked;
    }

    onOpened(e: AssignmentOpened) {
        if (e.assignment.schemeOfWorkId) this.openScheme(e.assignment.schemeOfWorkId);
    }

    openScheme(id: number, thenWeekNo?: number) {
        this.schemeSvc.get(id).subscribe({
            next: (s) => {
                this.scheme = s;
                this.teachingSvc.curriculum(s.subjectId, s.learningLevelId, s.sessionId).subscribe({next: (c) => this.strands = c});
                this.teachingSvc.preferredApprovers(s.subjectId).subscribe({next: (ids) => this.preferredApprovers = ids});
                this.loadWeeks(thenWeekNo);
                if (!thenWeekNo) this.setUrl({schemeId: s.id});
            },
            error: (err) => this.toastr.error(err.status === 403
                ? 'You do not have access to these lesson plans.'
                : (err.error?.message || 'Error loading the scheme of work.'))
        });
    }

    loadWeeks(selectWeekNo?: number) {
        this.svc.weeks(this.scheme!.id).subscribe({
            next: (w) => {
                this.weeks = w;
                // Open the week asked for, or the current week of the term.
                let today = new Date().toISOString().substring(0, 10);
                let pick = selectWeekNo ?? (this.week ? undefined
                    : w.find((x) => dateOnly(x.startDate)! <= today && today <= dateOnly(x.endDate)!)?.weekNo);
                let target = w.find((x) => x.weekNo === pick);
                if (target?.lessonPlanWeekId && !this.week) this.openWeek(target.lessonPlanWeekId);
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the weeks.')
        });
    }

    selectWeek(w: LessonPlanWeekSummary) {
        if (!this.confirmLeave()) return;
        if (w.lessonPlanWeekId) { this.openWeek(w.lessonPlanWeekId); return; }
        this.week = null;
        this.pendingWeekNo = w.weekNo;
    }

    // A week not prepared yet, selected in the strip.
    pendingWeekNo: number | null = null;

    get canPrepare(): boolean {
        return !!this.scheme && (this.scheme.staffDetailsId === this.context?.staffDetailsId || !!this.context?.isAdmin);
    }

    prepareWeek(weekNo: number) {
        this.svc.createWeek(this.scheme!.id, weekNo).subscribe({
            next: (res) => { this.pendingWeekNo = null; this.openWeek(res.id); this.loadWeeks(weekNo); },
            error: (err) => this.toastr.error(err.error?.message || 'Error preparing the week.')
        });
    }

    openWeek(id: number, loadScheme = false) {
        this.svc.week(id).subscribe({
            next: (w) => {
                this.setWeek(w);
                this.pendingWeekNo = null;
                if (loadScheme || !this.scheme || this.scheme.id !== w.scheme.id) this.openScheme(w.scheme.id, w.weekNo);
                this.setUrl({weekId: w.id});
            },
            error: (err) => this.toastr.error(err.status === 403
                ? 'You do not have access to these lesson plans.'
                : (err.error?.message || 'Error loading the lesson plans.'))
        });
    }

    private setWeek(w: LessonPlanWeek) {
        w.plans.forEach(normalise);
        let first = !this.week || this.week.id !== w.id;
        this.week = w;
        this.dirtyPlans.clear();
        if (first) {
            this.expanded.clear();
            if (w.plans.length) this.expanded.add(w.plans[0].id);
        }
    }

    private setUrl(params: any) {
        this.router.navigate([], {relativeTo: this.route, queryParams: params, replaceUrl: true});
    }

    private confirmLeave(): boolean {
        return this.dirtyPlans.size === 0 || window.confirm('You have unsaved lesson plans. Leave without saving them?');
    }

    back() {
        if (!this.confirmLeave()) return;
        this.scheme = null;
        this.week = null;
        this.weeks = [];
        this.pendingWeekNo = null;
        this.setUrl({});
    }

    toggle(p: LessonPlan) {
        this.expanded.has(p.id) ? this.expanded.delete(p.id) : this.expanded.add(p.id);
    }

    touch(p: LessonPlan) {
        this.dirtyPlans.add(p.id);
    }

    // ---------- Curriculum pickers ----------

    subStrandsOf(strandId: number | null): CurriculumSubStrand[] {
        return this.strands.find((s) => s.id === strandId)?.subStrands || [];
    }

    onStrand(p: LessonPlan) {
        let strand = this.strands.find((s) => s.id === p.strandId);
        if (strand) p.strandName = strand.name;
        if (!this.subStrandsOf(p.strandId).some((ss) => ss.id === p.subStrandId)) p.subStrandId = null;
        this.touch(p);
    }

    onSubStrand(p: LessonPlan) {
        let ss = this.subStrandsOf(p.strandId).find((x) => x.id === p.subStrandId);
        if (ss) {
            p.subStrandName = ss.name;
            if (!p.learningOutcomes?.trim() && ss.specificOutcomes.length) {
                p.learningOutcomes = 'By the end of the lesson, the learner should be able to:\n'
                    + ss.specificOutcomes.map((o) => '- ' + o).join('\n');
            }
            if (!p.keyInquiryQuestions?.trim()) p.keyInquiryQuestions = ss.keyQuestions.join('\n');
        }
        this.touch(p);
    }

    // ---------- Steps ----------

    addStep(p: LessonPlan) {
        p.steps.push({id: 0, stepNo: p.steps.length + 1, activity: ''});
        this.touch(p);
    }

    removeStep(p: LessonPlan, i: number) {
        p.steps.splice(i, 1);
        p.steps.forEach((s, n) => s.stepNo = n + 1);
        this.touch(p);
    }

    moveStep(p: LessonPlan, i: number, by: number) {
        let j = i + by;
        if (j < 0 || j >= p.steps.length) return;
        [p.steps[i], p.steps[j]] = [p.steps[j], p.steps[i]];
        p.steps.forEach((s, n) => s.stepNo = n + 1);
        this.touch(p);
    }

    totalMinutes(p: LessonPlan): number {
        return (+p.introductionMinutes || 0) + (+p.developmentMinutes || 0) + (+p.conclusionMinutes || 0);
    }

    // ---------- Saving ----------

    savePlan(p: LessonPlan, after?: () => void, failed?: () => void) {
        this.svc.savePlan(p).subscribe({
            next: (w) => {
                // Keep the other lessons' unsaved edits: take only this plan back.
                let saved = w.plans.find((x) => x.id === p.id);
                if (saved) { normalise(saved); Object.assign(p, saved); }
                this.dirtyPlans.delete(p.id);
                this.toastr.success(`Lesson ${p.lessonNo} saved.`);
                after?.();
            },
            error: (err) => {
                this.toastr.error(err.error?.message || 'Error saving the lesson plan.');
                failed?.();
            }
        });
    }

    // Saves every lesson with unsaved edits, one after another; true when all saved.
    saveAll(): Promise<boolean> {
        let plans = this.week!.plans.filter((p) => this.dirtyPlans.has(p.id));
        return new Promise((resolve) => {
            let next = () => {
                let p = plans.shift();
                if (p) this.savePlan(p, next, () => resolve(false));
                else resolve(true);
            };
            next();
        });
    }

    // Submitting sends the week as saved, so unsaved lessons are saved first;
    // a failed save stops the submit rather than losing the edits.
    saveBeforeSubmit = (): Promise<boolean> => this.dirtyPlans.size ? this.saveAll() : Promise.resolve(true);

    // Closing or reloading the tab with unsaved lessons asks first.
    @HostListener('window:beforeunload', ['$event'])
    warnUnsaved(e: BeforeUnloadEvent) {
        if (this.dirtyPlans.size) e.preventDefault();
    }

    saveAfterLesson(p: LessonPlan) {
        if ((p.taughtStatus === LessonTaughtStatus.Taught || p.taughtStatus === LessonTaughtStatus.PartlyTaught) && !p.taughtOn && !p.lessonDate) {
            this.toastr.info('Enter the date the lesson was taught.');
            return;
        }
        this.svc.afterLesson(p.id, {summary: p.summary, reflection: p.reflection, taughtStatus: +p.taughtStatus, taughtOn: p.taughtOn || p.lessonDate})
            .subscribe({
                next: (w) => {
                    let saved = w.plans.find((x) => x.id === p.id);
                    if (saved) {
                        normalise(saved);
                        Object.assign(p, {summary: saved.summary, reflection: saved.reflection, taughtStatus: saved.taughtStatus, taughtOn: saved.taughtOn});
                    }
                    this.toastr.success('After-lesson notes saved' + (p.schemeOfWorkLessonId ? ' to the record of work.' : '.'));
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error saving the notes.')
            });
    }

    addPlan() {
        this.svc.addPlan(this.week!.id).subscribe({
            next: (w) => {
                let known = new Set(this.week!.plans.map((p) => p.id));
                let added = w.plans.find((p) => !known.has(p.id));
                normalise(added!);
                this.week!.plans.push(added!);
                this.expanded.add(added!.id);
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error adding a lesson.')
        });
    }

    // Pulls the scheme's current content into the week, e.g. resources added to
    // the scheme after the week was prepared. Unsaved edits are saved first.
    refreshFromScheme() {
        Swal.fire({
            title: 'Refresh from the scheme?',
            html: '<div class="text-start small">Takes the strand, sub-strand, learning outcomes, key inquiry questions, ' +
                'learning resources and lesson development steps from the scheme of work, and adds plans for any scheme ' +
                'lessons this week is missing. Dates, times, the rest of the lesson flow and after-lesson notes stay as they are.' +
                '<br><br><b>Fill blanks</b> keeps what you have typed and only fills empty fields.<br>' +
                '<b>Replace</b> overwrites those fields with the scheme\'s current wording.</div>',
            width: 520, position: 'top', icon: 'question',
            showCancelButton: true, showDenyButton: true,
            confirmButtonText: 'Fill blanks', denyButtonText: 'Replace', denyButtonColor: '#d33'
        }).then(async (r) => {
            if (!r.isConfirmed && !r.isDenied) return;
            if (this.dirtyPlans.size && !(await this.saveAll())) return;
            let before = new Set(this.week!.plans.map((p) => p.id));
            this.svc.refreshFromScheme(this.week!.id, r.isDenied).subscribe({
                next: (w) => {
                    let added = w.plans.filter((p) => !before.has(p.id)).length;
                    this.setWeek(w);
                    this.toastr.success('Refreshed from the scheme' + (added ? ` - ${added} lesson(s) added.` : '.'));
                    this.loadWeeks();
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error refreshing from the scheme.')
            });
        });
    }

    deletePlan(p: LessonPlan) {
        Swal.fire({title: `Remove lesson ${p.lessonNo}'s plan?`, width: 400, position: 'top', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Remove', confirmButtonColor: '#d33'}).then((r) => {
            if (!r.value) return;
            this.svc.deletePlan(p.id).subscribe({
                next: () => {
                    this.week!.plans = this.week!.plans.filter((x) => x.id !== p.id);
                    this.dirtyPlans.delete(p.id);
                    this.loadWeeks();
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error removing the lesson plan.')
            });
        });
    }

    deleteWeek() {
        Swal.fire({title: `Delete week ${this.week!.weekNo}'s lesson plans?`, width: 400, position: 'top', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (!r.value) return;
            this.svc.deleteWeek(this.week!.id).subscribe({
                next: () => { this.week = null; this.dirtyPlans.clear(); this.loadWeeks(); this.setUrl({schemeId: this.scheme!.id}); },
                error: (err) => this.toastr.error(err.error?.message || 'Error deleting the week.')
            });
        });
    }

    print() {
        if (this.dirtyPlans.size) this.toastr.info('Unsaved changes are not in the printout - save first.');
        printLessonPlans(this.week!, this.school);
    }

    onApprovalChanged() {
        this.approvalSvc.notifyRefresh();
        this.historyVersion++;
        this.openWeek(this.week!.id);
        this.loadWeeks();
    }

    weekSummary(): LessonPlanWeekSummary | undefined {
        return this.weeks.find((w) => w.weekNo === (this.week?.weekNo ?? this.pendingWeekNo));
    }
}

// Dates as yyyy-MM-dd for the date inputs.
function normalise(p: LessonPlan) {
    p.lessonDate = dateOnly(p.lessonDate);
    p.taughtOn = dateOnly(p.taughtOn);
}
