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

@Component({
    selector: 'app-student-promotion',
    templateUrl: './student-promotion.component.html',
    styleUrl: './student-promotion.component.scss'
})
export class StudentPromotionComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/students/promotion'], title: 'Student Promotion'}
    ];
    dashboardTitle = 'Student Promotion';

    academicYears: any[] = [];
    fromClasses: any[] = [];
    toClasses: any[] = [];

    fromYearId: any = null;
    toYearId: any = null;
    fromClassId: any = null;
    toClassId: any = null;

    students: {
        studentId: number;
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

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService
    ) {}

    ngOnInit(): void {
        this.academicYearSvc.get('/academicYears').subscribe({
            next: (years) => {
                this.academicYears = years.sort((a, b) => (a.rank || 0) - (b.rank || 0));
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onFromYearChange() {
        this.fromClasses = [];
        this.fromClassId = null;
        this.students = [];
        this.hasLoaded = false;
        if (!this.fromYearId) return;

        this.schoolClassesSvc.getByAcademicYearId(parseInt(this.fromYearId)).subscribe({
            next: (classes) => {
                this.fromClasses = classes.sort((a, b) => {
                    let nameA = (a.learningLevel?.name || '') + (a.schoolStream?.name || '');
                    let nameB = (b.learningLevel?.name || '') + (b.schoolStream?.name || '');
                    return nameA.localeCompare(nameB);
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onToYearChange() {
        this.toClasses = [];
        this.toClassId = null;
        if (!this.toYearId) return;

        this.schoolClassesSvc.getByAcademicYearId(parseInt(this.toYearId)).subscribe({
            next: (classes) => {
                this.toClasses = classes.sort((a, b) => {
                    let nameA = (a.learningLevel?.name || '') + (a.schoolStream?.name || '');
                    let nameB = (b.learningLevel?.name || '') + (b.schoolStream?.name || '');
                    return nameA.localeCompare(nameB);
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onFromClassChange() {
        this.students = [];
        this.hasLoaded = false;
        this.selectAll = false;
        this.selectAllRemoval = false;
        this.showPromoted = false;
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
                        fullName: sc.student?.fullName || '',
                        upi: sc.student?.upi || '',
                        selected: false,
                        alreadyPromoted: targetMap.has(sc.studentId),
                        targetRecordId: targetMap.get(sc.studentId) || null,
                        selectedForRemoval: false
                    }))
                    .sort((a, b) => a.fullName.localeCompare(b.fullName));

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
            this.toastr.info('Please select at least one student to promote.');
            return;
        }

        Swal.fire({
            title: 'Promote students?',
            html: `<strong>${toPromote.length}</strong> student(s) will be promoted from <strong>${this.getFromClassName()}</strong> to <strong>${this.getToClassName()}</strong>.`,
            width: 450,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Promote',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;

                let requests = toPromote.map((student) => {
                    let payload = new StudentClass({
                        studentId: student.studentId,
                        schoolClassId: parseInt(this.toClassId),
                        description: 'Promoted from ' + this.getFromClassName()
                    });
                    return this.studentClassSvc.create('/studentClasses', payload)
                        .pipe(catchError((err) => {
                            this.toastr.error(`Error promoting ${student.fullName}: ${err.error?.message || 'Unknown error'}`);
                            return of(null);
                        }));
                });

                forkJoin(requests).subscribe({
                    next: (results) => {
                        let successCount = results.filter((r) => r !== null).length;
                        this.isSaving = false;
                        if (successCount > 0) {
                            this.toastr.success(`${successCount} student(s) promoted successfully!`);
                            this.loadStudents();
                        }
                    },
                    error: () => {
                        this.isSaving = false;
                        this.toastr.error('Error during promotion.');
                    }
                });
            }
        });
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
