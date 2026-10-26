import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ApprovalService} from '@/approvals/services/approval.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {
    CurriculumStrand,
    CurriculumSubStrand,
    SchemeCopySource,
    SchemeLesson,
    SchemeOfWork,
    STATUS_BADGES,
    STATUS_LABELS,
    TeachingContext
} from '@/teaching/models/teaching-models';
import {SchemesOfWorkService, TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {AssignmentOpened} from '../teaching-assignments/teaching-assignments.component';
import {PrintSchool, printScheme} from '@/teaching/utils/teaching-print';
import {blobError, safeFileName, saveBlob} from '@/teaching/utils/download';

interface WeekGroup {
    weekNo: number;
    startDate: string | null;
    endDate: string | null;
    lessons: SchemeLesson[];
}

/**
 * Schemes of work. A teacher opens one of their subject allocations for a
 * term, picks strands and sub-strands from the curriculum (which fills in the
 * outcomes, experiences and key questions for them to edit) and submits it for
 * approval. Approvers and heads of department open the same screen read-only.
 */
@Component({
    selector: 'app-schemes-of-work',
    templateUrl: './schemes-of-work.component.html',
    styleUrl: '../teaching.scss'
})
export class SchemesOfWorkComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/teaching/schemes'], title: 'Schemes of Work'}
    ];
    dashboardTitle = 'Teaching Documents: Schemes of Work';

    context: TeachingContext | null = null;
    scheme: SchemeOfWork | null = null;
    groups: WeekGroup[] = [];
    strands: CurriculumStrand[] = [];
    preferredApprovers: number[] = [];
    copySources: SchemeCopySource[] | null = null;
    school: PrintSchool = {name: ''};
    dirty = false;
    saving = false;
    historyVersion = 0;

    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;

    constructor(private route: ActivatedRoute, private router: Router, private toastr: ToastrService,
                private teachingSvc: TeachingDocumentsService, private svc: SchemesOfWorkService,
                private approvalSvc: ApprovalService, private schoolSvc: SchoolDetailsService) {}

    ngOnInit(): void {
        this.teachingSvc.context().subscribe({next: (c) => this.context = c});
        this.schoolSvc.get('/schoolDetails').subscribe({next: (r: any) => {
            let s = (r || [])[0] || r;
            this.school = {name: s?.name || '', address: s?.address, logo: s?.logoAsBase64};
        }});
        // ?actId= from the approvals bell, ?schemeId= from the tracker.
        let id = +(this.route.snapshot.queryParamMap.get('actId') || this.route.snapshot.queryParamMap.get('schemeId') || 0);
        if (id) this.openScheme(id);
    }

    get editable(): boolean {
        return !!this.scheme?.canEdit;
    }

    onOpened(e: AssignmentOpened) {
        let a = e.assignment;
        if (a.schemeOfWorkId) {
            this.openScheme(a.schemeOfWorkId);
            return;
        }
        Swal.fire({
            title: 'Start the scheme of work?',
            html: `${a.subjectName}, ${a.className} - ${e.session.name} ${e.session.academicYearName}.<br>
                <small>It is laid out as ${e.session.weeks} weeks; you can change that after.</small>`,
            width: 420, position: 'top', icon: 'question', showCancelButton: true, confirmButtonText: 'Start'
        }).then((r) => {
            if (!r.value) return;
            this.svc.create(a.subjectId, a.schoolClassId, e.session.id,
                a.staffDetailsId !== e.context.staffDetailsId ? a.staffDetailsId : undefined).subscribe({
                next: (res) => this.openScheme(res.id),
                error: (err) => this.toastr.error(err.error?.message || 'Error starting the scheme.')
            });
        });
    }

    openScheme(id: number) {
        this.svc.get(id).subscribe({
            next: (s) => {
                this.setScheme(s);
                this.router.navigate([], {relativeTo: this.route, queryParams: {schemeId: s.id}, replaceUrl: true});
                this.teachingSvc.curriculum(s.subjectId, s.learningLevelId, s.sessionId).subscribe({next: (c) => this.strands = c});
                this.teachingSvc.preferredApprovers(s.subjectId).subscribe({next: (ids) => this.preferredApprovers = ids});
            },
            error: (err) => this.toastr.error(err.status === 403
                ? 'You do not have access to this scheme of work.'
                : (err.error?.message || 'Error loading the scheme of work.'))
        });
    }

    private setScheme(s: SchemeOfWork) {
        this.scheme = s;
        this.dirty = false;
        this.regroup();
    }

    regroup() {
        if (!this.scheme) { this.groups = []; return; }
        let s = this.scheme;
        let weekNos = new Set<number>();
        for (let w = 1; w <= s.numberOfWeeks; w++) weekNos.add(w);
        s.lessons.forEach((l) => weekNos.add(l.weekNo));
        this.groups = Array.from(weekNos).sort((a, b) => a - b).map((w) => {
            let week = s.weeks.find((x) => x.weekNo === w);
            return {
                weekNo: w,
                startDate: week?.startDate || null,
                endDate: week?.endDate || null,
                lessons: s.lessons.filter((l) => l.weekNo === w).sort((a, b) => a.lessonNo - b.lessonNo)
            };
        });
    }

    back() {
        let leave = () => {
            this.scheme = null;
            this.groups = [];
            this.router.navigate([], {relativeTo: this.route, queryParams: {}, replaceUrl: true});
        };
        if (!this.dirty) { leave(); return; }
        Swal.fire({title: 'Leave without saving?', text: 'Your changes to this scheme will be lost.', width: 400,
            position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Leave'})
            .then((r) => { if (r.value) leave(); });
    }

    // ---------- Curriculum pickers ----------

    subStrandsOf(strandId: number | null): CurriculumSubStrand[] {
        return this.strands.find((s) => s.id === strandId)?.subStrands || [];
    }

    private findSubStrand(id: number | null): CurriculumSubStrand | undefined {
        for (let s of this.strands) {
            let ss = s.subStrands.find((x) => x.id === id);
            if (ss) return ss;
        }
        return undefined;
    }

    onStrand(l: SchemeLesson) {
        let strand = this.strands.find((s) => s.id === l.strandId);
        if (strand) l.strandName = strand.name;
        if (!this.subStrandsOf(l.strandId).some((ss) => ss.id === l.subStrandId)) l.subStrandId = null;
        this.dirty = true;
    }

    // Picking a sub-strand fills the boxes still empty; "refill" overwrites them.
    onSubStrand(l: SchemeLesson, overwrite = false) {
        let ss = this.findSubStrand(l.subStrandId);
        if (!ss) return;
        l.subStrandName = ss.name;
        let fill = (current: string | null, value: string) => (overwrite || !current?.trim()) ? value : current;
        l.learningOutcomes = fill(l.learningOutcomes, lines(ss.specificOutcomes, 'By the end of the lesson, the learner should be able to:'));
        l.learningExperiences = fill(l.learningExperiences, lines(ss.learningExperiences, 'The learner is guided to:'));
        l.keyInquiryQuestions = fill(l.keyInquiryQuestions, ss.keyQuestions.join('\n'));
        if (!l.learningResources?.trim() && this.scheme?.references) l.learningResources = this.scheme.references;
        this.dirty = true;
    }

    refill(l: SchemeLesson) {
        Swal.fire({title: 'Replace this lesson\'s text?', text: 'Outcomes, experiences and key questions are refilled from the curriculum.',
            width: 400, position: 'top', icon: 'question', showCancelButton: true, confirmButtonText: 'Replace'})
            .then((r) => { if (r.value) this.onSubStrand(l, true); });
    }

    // The sub-strand from the lesson before, for "same as above".
    copyAbove(g: WeekGroup, index: number) {
        let l = g.lessons[index];
        let prev = index > 0 ? g.lessons[index - 1]
            : this.groups.filter((x) => x.weekNo < g.weekNo && x.lessons.length).slice(-1)[0]?.lessons.slice(-1)[0];
        if (!prev) return;
        Object.assign(l, {
            strandId: prev.strandId, subStrandId: prev.subStrandId, strandName: prev.strandName, subStrandName: prev.subStrandName,
            learningOutcomes: prev.learningOutcomes, learningExperiences: prev.learningExperiences,
            keyInquiryQuestions: prev.keyInquiryQuestions, learningResources: prev.learningResources,
            assessmentMethods: prev.assessmentMethods
        });
        this.dirty = true;
    }

    // ---------- Rows ----------

    addLesson(g: WeekGroup) {
        let next = g.lessons.reduce((m, l) => Math.max(m, l.lessonNo), 0) + 1;
        this.scheme!.lessons.push(blankLesson(g.weekNo, next));
        this.dirty = true;
        this.regroup();
    }

    removeLesson(l: SchemeLesson) {
        let remove = () => {
            this.scheme!.lessons = this.scheme!.lessons.filter((x) => x !== l);
            // Close the gap so the week reads 1, 2, 3...
            this.scheme!.lessons.filter((x) => x.weekNo === l.weekNo && x.lessonNo > l.lessonNo).forEach((x) => x.lessonNo--);
            this.dirty = true;
            this.regroup();
        };
        if (!hasContent(l)) { remove(); return; }
        Swal.fire({title: `Remove week ${l.weekNo} lesson ${l.lessonNo}?`, text: 'Anything recorded against it in the record of work goes too.',
            width: 400, position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Remove'})
            .then((r) => { if (r.value) remove(); });
    }

    /** Fits the rows to the weeks and lessons per week: adds blank rows, drops empty ones outside. */
    applyGrid() {
        let s = this.scheme!;
        s.numberOfWeeks = Math.min(20, Math.max(1, Math.floor(+s.numberOfWeeks || 1)));
        s.lessonsPerWeek = Math.min(20, Math.max(1, Math.floor(+s.lessonsPerWeek || 1)));
        let outside = s.lessons.filter((l) => l.weekNo > s.numberOfWeeks && hasContent(l));
        if (outside.length) {
            this.toastr.warning(`Week ${outside[0].weekNo} has lessons filled in - clear or remove them before reducing the weeks.`);
            return;
        }
        s.lessons = s.lessons.filter((l) => l.weekNo <= s.numberOfWeeks
            && (l.lessonNo <= s.lessonsPerWeek || hasContent(l)));
        for (let w = 1; w <= s.numberOfWeeks; w++) {
            for (let n = 1; n <= s.lessonsPerWeek; n++) {
                if (!s.lessons.some((l) => l.weekNo === w && l.lessonNo === n)) s.lessons.push(blankLesson(w, n));
            }
        }
        this.dirty = true;
        this.regroup();
    }

    get weeksChanged(): boolean {
        let s = this.scheme;
        if (!s) return false;
        let expected = s.numberOfWeeks * s.lessonsPerWeek;
        let inGrid = s.lessons.filter((l) => l.weekNo <= s.numberOfWeeks && l.lessonNo <= s.lessonsPerWeek).length;
        return inGrid !== expected || s.lessons.some((l) => l.weekNo > s.numberOfWeeks);
    }

    // ---------- Actions ----------

    save(after?: () => void) {
        if (!this.scheme) return;
        if (this.weeksChanged) this.applyGrid();
        this.saving = true;
        this.svc.save(this.scheme).subscribe({
            next: (s) => {
                this.saving = false;
                this.setScheme(s);
                this.toastr.success('Scheme of work saved.');
                after?.();
            },
            error: (err) => { this.saving = false; this.toastr.error(err.error?.message || 'Error saving the scheme.'); }
        });
    }

    generate() {
        let s = this.scheme!;
        let run = (replace: boolean) => this.svc.generate(s.id, replace).subscribe({
            next: (res) => { this.setScheme(res); this.toastr.success('Scheme laid out from the curriculum - check and edit each lesson.'); },
            error: (err) => this.toastr.error(err.error?.message || 'Error generating the scheme.')
        });
        let filled = s.lessons.some(hasContent);
        Swal.fire({
            title: 'Fill the scheme from the curriculum?',
            html: `Sub-strands are laid out in curriculum order across the ${s.numberOfWeeks} weeks, ${s.lessonsPerWeek} lessons a week,
                each taking the number of lessons the curriculum gives it. Sub-strands already in an earlier term's scheme are skipped.`
                + (filled ? '<br><br><b>This replaces the lessons already filled in.</b>' : '')
                + (this.dirty ? '<br><br>Unsaved changes will be lost.' : ''),
            width: 460, position: 'top', icon: filled ? 'warning' : 'question', showCancelButton: true,
            confirmButtonText: filled ? 'Replace' : 'Fill in'
        }).then((r) => { if (r.value) run(filled); });
    }

    openCopy() {
        this.svc.copySources(this.scheme!.id).subscribe({
            next: (rows) => {
                this.copySources = rows;
                if (!rows.length) this.toastr.info('There is no other scheme for this subject and grade to copy from yet.');
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading schemes to copy from.')
        });
    }

    copyFrom(src: SchemeCopySource) {
        Swal.fire({
            title: 'Copy this scheme?',
            html: `${src.staffName} - ${src.className}, ${src.sessionName} ${src.academicYearName}.<br><b>This replaces every lesson in your scheme.</b>`,
            width: 420, position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Copy'
        }).then((r) => {
            if (!r.value) return;
            this.svc.copyFrom(this.scheme!.id, src.id).subscribe({
                next: (s) => { this.copySources = null; this.setScheme(s); this.toastr.success('Scheme copied - edit it to suit your class.'); },
                error: (err) => this.toastr.error(err.error?.message || 'Error copying the scheme.')
            });
        });
    }

    delete() {
        Swal.fire({title: 'Delete this scheme of work?', text: 'Its lesson plans and record of work are deleted with it.',
            width: 400, position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'})
            .then((r) => {
                if (!r.value) return;
                this.svc.delete(this.scheme!.id).subscribe({
                    next: () => { this.dirty = false; this.toastr.success('Scheme deleted.'); this.back(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting the scheme.')
                });
            });
    }

    // ---------- Excel ----------

    showImport = false;

    // The teacher, the head of the subject's department and administrators, while it is not locked.
    get canExcel(): boolean {
        let s = this.scheme;
        if (!s || s.isLocked || !this.context) return false;
        return s.canEdit || this.context.hodDepartmentIds.includes(s.departmentId);
    }

    downloadExcel() {
        let s = this.scheme!;
        let go = () => this.svc.schemeWorkbook(s.id).subscribe({
            next: (blob) => saveBlob(blob, safeFileName(`Scheme of work - ${s.subjectName} ${s.className} - ${s.sessionName} ${s.academicYearName}`) + '.xlsx'),
            error: async (err) => this.toastr.error(await blobError(err, 'Error preparing the workbook.'))
        });
        // The workbook holds what is saved.
        if (this.dirty) this.save(go); else go();
    }

    onImportClosed(saved: boolean) {
        this.showImport = false;
        if (saved) this.openScheme(this.scheme!.id);
    }

    print() {
        if (this.dirty) { this.save(() => printScheme(this.scheme!, this.school)); return; }
        printScheme(this.scheme!, this.school);
    }

    onApprovalChanged() {
        this.approvalSvc.notifyRefresh();
        this.historyVersion++;
        this.openScheme(this.scheme!.id);
    }
}

function lines(items: string[], heading: string): string {
    let list = items.filter((i) => !!i?.trim());
    return list.length ? heading + '\n' + list.map((i) => '- ' + i.trim()).join('\n') : '';
}

function blankLesson(weekNo: number, lessonNo: number): SchemeLesson {
    return {
        id: 0, weekNo, lessonNo, strandId: null, subStrandId: null, strandName: null, subStrandName: null,
        learningOutcomes: null, learningExperiences: null, keyInquiryQuestions: null, learningResources: null,
        assessmentMethods: null, reflection: null
    };
}

function hasContent(l: SchemeLesson): boolean {
    return [l.strandName, l.subStrandName, l.learningOutcomes, l.learningExperiences, l.keyInquiryQuestions]
        .some((v) => !!v?.trim());
}
