import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {catchError} from 'rxjs/operators';
import Swal from 'sweetalert2';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '../../services/student-class.service';
import {StudentClass} from '../../models/student-class';
import {Status} from '@/core/enums/status';
import {ActivatedRoute, Router} from '@angular/router';
import {currentYear, firstOf} from '@/core/utils/current-defaults';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';

@Component({
    selector: 'app-student-promotion',
    templateUrl: './student-promotion.component.html',
    styleUrl: './student-promotion.component.scss'
})
export class StudentPromotionComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/students/promotion'], title: 'Class Assignment'}
    ];
    dashboardTitle = 'Class Assignment';

    academicYears: any[] = [];
    fromClasses: any[] = [];
    toClasses: any[] = [];

    fromYearId: any = null;
    toYearId: any = null;
    fromClassId: any = null;
    toClassId: any = null;

    students: {
        studentId: number;
        sourceRecordId: string;        // StudentClass ID in the FROM class (for same-year removal)
        fullName: string;
        upi: string;
        selected: boolean;
        alreadyPromoted: boolean;
        targetRecordId: string | null; // StudentClass ID in target class (for removal)
        selectedForRemoval: boolean;
    }[] = [];

    isLoading: boolean = false;
    isSaving: boolean = false;
    hasLoaded: boolean = false;
    selectAll: boolean = false;
    selectAllRemoval: boolean = false;
    showPromoted: boolean = false;
    // When ticked, the assignment also removes the student from the source
    // class. Defaults true for mid-year corrections (same year source+target)
    // like moving 5E -> 5W; defaults false for cross-year promotion so the
    // history of the previous class is preserved.
    removeFromSource: boolean = false;

    // Client-side paging for the two student lists; default 30 items per page.
    pendingPage: number = 1;
    promotedPage: number = 1;
    listPageSize: number = 30;

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.academicYearSvc.get('/academicYears').subscribe({
            next: (years) => {
                this.academicYears = years.sort((a, b) => (a.rank || 0) - (b.rank || 0));
                this.restoreSelections();
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    // Selections come from the URL (refresh / shared link), else defaults: the
    // current year's first class, into the next year if there is one (the
    // promotion), else into the same year. The learners load either way; no
    // assignment is made until asked.
    private restoreSelections() {
        let year = currentYear(this.academicYears);
        let urlFromYear = readQueryParam(this.route, 'fromYearId');
        this.fromYearId = urlFromYear ? matchOptionId(this.academicYears, urlFromYear) : year?.id ?? null;
        let urlToYear = readQueryParam(this.route, 'toYearId');
        this.toYearId = urlToYear ? matchOptionId(this.academicYears, urlToYear) : (this.nextYear(year) ?? year)?.id ?? null;
        if (!this.fromYearId || !this.toYearId) return;

        this.onFromYearChange(() => {
            this.fromClassId = matchOptionId(this.fromClasses, readQueryParam(this.route, 'fromClassId'))
                ?? firstOf(this.fromClasses)?.id ?? null;
            this.onToYearChange(() => {
                this.toClassId = matchOptionId(this.toClasses, readQueryParam(this.route, 'toClassId'))
                    ?? this.defaultToClass()?.id ?? null;
                this.syncUrl();
                if (this.fromClassId && this.toClassId) this.loadStudents();
            });
        });
    }

    // The year starting after the given one, if set up yet.
    private nextYear(year: any): any {
        if (!year?.startDate) return null;
        let start = String(year.startDate);
        return this.academicYears
            .filter((y) => y.startDate && String(y.startDate) > start)
            .sort((a, b) => String(a.startDate).localeCompare(String(b.startDate)))[0] ?? null;
    }

    // Across years: the next level's class in the same stream. Within a year:
    // another stream of the same level. Never the source class itself.
    private defaultToClass(): any {
        let from = this.fromClasses.find((c) => c.id == this.fromClassId);
        let others = this.toClasses.filter((c) => c.id != this.fromClassId);
        if (!from) return firstOf(others);
        let rank = from.learningLevel?.rank ?? 0;
        let match = +this.fromYearId === +this.toYearId
            ? others.find((c) => c.learningLevelId == from.learningLevelId)
            : others.find((c) => (c.learningLevel?.rank ?? 0) === rank + 1 && c.schoolStreamId == from.schoolStreamId)
              ?? others.find((c) => (c.learningLevel?.rank ?? 0) === rank + 1);
        return match ?? firstOf(others);
    }

    private syncUrl() {
        pushQueryParams(this.router, this.route, {
            fromYearId: this.fromYearId,
            fromClassId: this.fromClassId,
            toYearId: this.toYearId,
            toClassId: this.toClassId
        });
    }

    // `then` runs once the classes have loaded (used by the defaults).
    onFromYearChange(then?: () => void) {
        this.fromClasses = [];
        this.fromClassId = null;
        this.students = [];
        this.hasLoaded = false;
        if (!then) this.syncUrl();
        if (!this.fromYearId) return;

        this.schoolClassesSvc.getByAcademicYearId(parseInt(this.fromYearId)).subscribe({
            next: (classes) => {
                this.fromClasses = classes.sort((a, b) => {
                    let nameA = (a.learningLevel?.name || '') + (a.schoolStream?.name || '');
                    let nameB = (b.learningLevel?.name || '') + (b.schoolStream?.name || '');
                    return nameA.localeCompare(nameB);
                });
                then?.();
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onToYearChange(then?: () => void) {
        this.toClasses = [];
        this.toClassId = null;
        // Same-year switch is almost always a mid-year correction (e.g. 5E -> 5W)
        // where the source class membership should be cleared.
        this.removeFromSource = this.fromYearId && this.toYearId
            && +this.fromYearId === +this.toYearId;
        if (!then) this.onFromClassChange();
        if (!this.toYearId) return;

        this.schoolClassesSvc.getByAcademicYearId(parseInt(this.toYearId)).subscribe({
            next: (classes) => {
                this.toClasses = classes.sort((a, b) => {
                    let nameA = (a.learningLevel?.name || '') + (a.schoolStream?.name || '');
                    let nameB = (b.learningLevel?.name || '') + (b.schoolStream?.name || '');
                    return nameA.localeCompare(nameB);
                });
                then?.();
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    // Either class changing clears the lists until Load is pressed again.
    onFromClassChange() {
        this.students = [];
        this.hasLoaded = false;
        this.selectAll = false;
        this.selectAllRemoval = false;
        this.showPromoted = false;
        this.syncUrl();
    }

    loadStudents() {
        if (!this.fromClassId || !this.toYearId || !this.toClassId) {
            this.toastr.info('Please select all fields: From class, To year, and To class.');
            return;
        }

        this.isLoading = true;
        this.students = [];
        this.selectAll = false;
        this.selectAllRemoval = false;

        forkJoin([
            this.studentClassSvc.getBySchoolClassId(parseInt(this.fromClassId), Status.Active),
            this.studentClassSvc.get('/studentClasses/bySchoolClassId/' + this.toClassId + '/' + Status.Active)
                .pipe(catchError(() => of([])))
        ]).subscribe({
            next: ([fromStudents, toStudents]) => {
                // Build a map of studentId -> target record ID
                let targetMap = new Map<number, string>();
                (toStudents || []).forEach((sc) => {
                    targetMap.set(sc.studentId, sc.id);
                });

                this.students = fromStudents
                    .filter((sc) => sc.student)
                    .map((sc) => ({
                        studentId: sc.studentId,
                        sourceRecordId: sc.id,
                        fullName: sc.student?.fullName || '',
                        upi: sc.student?.upi || '',
                        selected: false,
                        alreadyPromoted: targetMap.has(sc.studentId),
                        targetRecordId: targetMap.get(sc.studentId) || null,
                        selectedForRemoval: false
                    }))
                    .sort((a, b) => a.fullName.localeCompare(b.fullName));
                this.pendingPage = this.promotedPage = 1;

                this.isLoading = false;
                this.hasLoaded = true;

                let promotedCount = this.students.filter((s) => s.alreadyPromoted).length;
                if (promotedCount > 0) {
                    this.toastr.info(`${promotedCount} student(s) already assigned to the target class.`);
                }
            },
            error: (err) => {
                this.isLoading = false;
                this.toastr.error(err.error);
            }
        });
    }

    // --- Promote selection ---

    getSelectableStudents() {
        return this.students.filter((s) => !s.alreadyPromoted);
    }

    getSelectedCount(): number {
        return this.students.filter((s) => s.selected && !s.alreadyPromoted).length;
    }

    toggleSelectAll() {
        this.selectAll = !this.selectAll;
        this.students.forEach((s) => {
            if (!s.alreadyPromoted) {
                s.selected = this.selectAll;
            }
        });
    }

    onStudentToggle() {
        let selectable = this.getSelectableStudents();
        this.selectAll = selectable.length > 0 && selectable.every((s) => s.selected);
    }

    // --- Removal selection ---

    getPromotedStudents() {
        return this.students.filter((s) => s.alreadyPromoted);
    }

    getRemovalCount(): number {
        return this.students.filter((s) => s.selectedForRemoval && s.alreadyPromoted).length;
    }

    toggleSelectAllRemoval() {
        this.selectAllRemoval = !this.selectAllRemoval;
        this.students.forEach((s) => {
            if (s.alreadyPromoted) {
                s.selectedForRemoval = this.selectAllRemoval;
            }
        });
    }

    onRemovalToggle() {
        let promoted = this.getPromotedStudents();
        this.selectAllRemoval = promoted.length > 0 && promoted.every((s) => s.selectedForRemoval);
    }

    // --- Helpers ---

    getFromClassName(): string {
        let cls = this.fromClasses.find((c) => c.id == this.fromClassId);
        if (!cls) return '';
        return (cls.learningLevel?.name || '') + (cls.schoolStream?.name ? ' - ' + cls.schoolStream.name : '');
    }

    getToClassName(): string {
        let cls = this.toClasses.find((c) => c.id == this.toClassId);
        if (!cls) return '';
        return (cls.learningLevel?.name || '') + (cls.schoolStream?.name ? ' - ' + cls.schoolStream.name : '');
    }

    // --- Actions ---

    promoteStudents() {
        let toPromote = this.students.filter((s) => s.selected && !s.alreadyPromoted);
        if (toPromote.length === 0) {
            this.toastr.info('Please select at least one student to assign.');
            return;
        }

        const verb = this.removeFromSource ? 'moved' : 'assigned';
        const noun = this.removeFromSource ? 'Move' : 'Assign';
        const removeBlurb = this.removeFromSource
            ? ` They will also be <strong>removed</strong> from ${this.getFromClassName()}.`
            : '';

        Swal.fire({
            title: `${noun} students?`,
            html: `<strong>${toPromote.length}</strong> student(s) will be ${verb} from <strong>${this.getFromClassName()}</strong> to <strong>${this.getToClassName()}</strong>.${removeBlurb}`,
            width: 460,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: noun,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;

                // Two distinct save modes:
                //  - Same-year correction: UPDATE the existing StudentClass row in
                //    place, just changing its schoolClassId. Preserves the row Id
                //    and every dependent record (StudentSubject, StudentAttendance,
                //    exam results, etc.) that already references it.
                //  - Cross-year promotion: CREATE a new StudentClass row in the
                //    target class; keep the old one for historical reference.
                const requests = toPromote.map((student) => {
                    if (this.removeFromSource) {
                        const updatePayload = new StudentClass({
                            id: student.sourceRecordId,
                            studentId: student.studentId,
                            schoolClassId: parseInt(this.toClassId),
                            description: `Re-assigned from ${this.getFromClassName()} to ${this.getToClassName()}`
                        });
                        return this.studentClassSvc.update('/studentClasses', updatePayload)
                            .pipe(catchError((err) => {
                                this.toastr.error(`Error moving ${student.fullName}: ${err.error?.message || 'Unknown error'}`);
                                return of(null);
                            }));
                    }
                    const createPayload = new StudentClass({
                        studentId: student.studentId,
                        schoolClassId: parseInt(this.toClassId),
                        description: 'Promoted from ' + this.getFromClassName()
                    });
                    return this.studentClassSvc.create('/studentClasses', createPayload)
                        .pipe(catchError((err) => {
                            this.toastr.error(`Error promoting ${student.fullName}: ${err.error?.message || 'Unknown error'}`);
                            return of(null);
                        }));
                });

                forkJoin(requests).subscribe({
                    next: (results) => {
                        const successCount = results.filter((r) => r !== null).length;
                        // For same-year moves, every success is both an add (target)
                        // AND a remove (source) because it's the same row.
                        const removedCount = this.removeFromSource ? successCount : 0;
                        this.finishAssignment(successCount, removedCount);
                    },
                    error: () => {
                        this.isSaving = false;
                        this.toastr.error('Error during assignment.');
                    }
                });
            }
        });
    }

    private finishAssignment(addedCount: number, removedCount: number) {
        this.isSaving = false;
        if (addedCount > 0) {
            const msg = removedCount > 0
                ? `${addedCount} student(s) moved (added to target, removed from source).`
                : `${addedCount} student(s) assigned successfully!`;
            this.toastr.success(msg);
            this.loadStudents();
        }
    }

    removePromotions() {
        let toRemove = this.students.filter((s) => s.selectedForRemoval && s.alreadyPromoted && s.targetRecordId);
        if (toRemove.length === 0) {
            this.toastr.info('Please select at least one student to remove.');
            return;
        }

        Swal.fire({
            title: 'Reverse promotions?',
            html: `<strong>${toRemove.length}</strong> student(s) will be removed from <strong>${this.getToClassName()}</strong>. This will undo their promotion.`,
            width: 450,
            position: 'top',
            padding: '1em',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Remove',
            confirmButtonColor: '#dc3545',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;

                let requests = toRemove.map((student) =>
                    this.studentClassSvc.delete('/studentClasses', parseInt(student.targetRecordId))
                        .pipe(catchError((err) => {
                            this.toastr.error(`Error removing ${student.fullName}: ${err.error?.message || 'Unknown error'}`);
                            return of(null);
                        }))
                );

                forkJoin(requests).subscribe({
                    next: (results) => {
                        let successCount = results.filter((r) => r !== null).length;
                        this.isSaving = false;
                        if (successCount > 0) {
                            this.toastr.success(`${successCount} promotion(s) reversed successfully!`);
                            this.loadStudents();
                        }
                    },
                    error: () => {
                        this.isSaving = false;
                        this.toastr.error('Error removing promotions.');
                    }
                });
            }
        });
    }
}
