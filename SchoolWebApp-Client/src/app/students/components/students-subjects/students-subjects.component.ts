import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {EducationLevelSubject} from '@/academics/models/education-level-subject';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {StudentClass} from '@/students/models/student-class';
import {StudentSubject} from '@/students/models/student-subject';
import {StudentSubjectSearch} from '@/students/models/student-subject-search';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {StudentsSubjectsStateService} from '@/students/services/students-subjects-state.service';
import {StudentsSubjectsSearchFormComponent} from './students-subjects-search-form/students-subjects-search-form.component';
import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {catchError} from 'rxjs/operators';
import Swal from 'sweetalert2';

interface ChipSubject {
    studentSubjectId: number;
    subjectId: number;
    name: string;
    code?: string;
}

interface StudentRow {
    studentClassId: number;
    upi: string;
    fullName: string;
    chips: ChipSubject[];
}

@Component({
    selector: 'app-students-subjects',
    templateUrl: './students-subjects.component.html',
    styleUrl: './students-subjects.component.scss'
})
export class StudentsSubjectsComponent implements OnInit, AfterViewInit {
    @ViewChild(StudentsSubjectsSearchFormComponent)
    searchForm: StudentsSubjectsSearchFormComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/students/subjects'], title: 'Students: Subjects'}
    ];
    dashboardTitle = 'Students: Subjects';

    curricula: Curriculum[] = [];
    educationLevels: EducationLevel[] = [];
    academicYears: AcademicYear[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[] = [];

    rows: StudentRow[] = [];
    selectedRowIds = new Set<number>();
    allSelected = false;

    availableSubjects: {id: number; name: string; code?: string}[] = [];

    currentEducationLevelId: number | null = null;
    currentAcademicYearId: number | null = null;
    currentSchoolClassId: number | null = null;

    showAddModal = false;
    modalStudentIds = new Set<number>();
    modalSubjectIds = new Set<number>();
    modalSaving = false;

    studentsChanged = () => {};
    doneLoading = false;

    constructor(
        private toastr: ToastrService,
        private http: HttpClient,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private educationLevelSvc: EducationLevelService,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private schoolClassSvc: SchoolClassesService,
        private studentClassesSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService,
        private stateSvc: StudentsSubjectsStateService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    ngAfterViewInit(): void {
        setTimeout(() => this.restoreStateIfAny(), 0);
    }

    loadInitials = () => {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');

        forkJoin([curriculaReq, academicYearsReq]).subscribe({
            next: ([curricula, academicYears]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.doneLoading = true;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    curriculumChanged = (curriculumId: number | null) => {
        this.resetClassScopedState();
        // Upstream control change invalidates downstream selection — buttons
        // re-disable until the user picks year + level + class again.
        this.currentEducationLevelId = null;
        this.currentAcademicYearId = null;
        this.currentSchoolClassId = null;
        this.availableSubjects = [];
        this.schoolClasses = [];
        this.educationLevels = [];
        this.stateSvc.set({
            curriculumId: curriculumId ?? null,
            educationLevelId: null,
            schoolClassId: null,
            selectedStudentClassId: null
        });
        if (curriculumId == null) return;
        this.educationLevelSvc
            .educationLevelsByCurriculum(curriculumId)
            .subscribe({
                next: (educationLevels) => {
                    this.educationLevels = educationLevels.sort(
                        (a, b) => a.rank - b.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    educationLevelYearChanged = (ely: EducationLevelYear | null) => {
        this.resetClassScopedState();
        // Class belongs to the previous level/year — clear so the Add /
        // Delete buttons disable until the user picks a class again.
        this.currentSchoolClassId = null;
        this.availableSubjects = [];
        this.schoolClasses = [];
        if (ely == null) {
            this.currentEducationLevelId = null;
            this.currentAcademicYearId = null;
            this.stateSvc.set({
                academicYearId: null,
                educationLevelId: null,
                schoolClassId: null,
                selectedStudentClassId: null
            });
            return;
        }
        this.currentEducationLevelId = ely.educationLevelId;
        this.currentAcademicYearId = ely.academicYearId;
        this.stateSvc.set({
            academicYearId: ely.academicYearId,
            educationLevelId: ely.educationLevelId,
            schoolClassId: null,
            selectedStudentClassId: null
        });
        this.schoolClassSvc
            .getByEducationLevelandYear(
                ely.educationLevelId,
                ely.academicYearId
            )
            .subscribe({
                next: (schoolClasses) => {
                    this.schoolClasses = schoolClasses.sort(
                        (a, b) => a.rank - b.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });

        // Subjects offered for this education level + year — populates the
        // Add Allocations modal's subject picker.
        this.educationLevelSubjectSvc
            .getByEducationLevelAndAcademicYear(
                ely.educationLevelId,
                ely.academicYearId
            )
            .subscribe({
                next: (els) => {
                    this.availableSubjects = (els || []).map(
                        (e: EducationLevelSubject) => ({
                            id: e.subjectId,
                            name: e.subject?.name ?? '',
                            code: e.subject?.code
                        })
                    );
                },
                error: () => {
                    this.availableSubjects = [];
                }
            });
    };

    schoolClassChanged = (schoolClassId: number | null) => {
        this.resetClassScopedState();
        this.currentSchoolClassId = schoolClassId;
        this.stateSvc.set({
            schoolClassId: schoolClassId ?? null,
            selectedStudentClassId: null
        });
        if (schoolClassId == null) return;
        this.loadRowsForClass(schoolClassId);
    };

    private loadRowsForClass(schoolClassId: number): void {
        this.studentClassesSvc
            .getBySchoolClassId(schoolClassId, Status.Active)
            .subscribe({
                next: (studentClasses) => {
                    this.studentClasses = studentClasses.sort((a, b) =>
                        a.student.upi.localeCompare(b.student.upi)
                    );
                    if (!studentClasses.length) {
                        this.rows = [];
                        return;
                    }
                    // Fan-out: pull each student's allocated subjects in
                    // parallel. The class-roster page only has a handful of
                    // students (well under the dashboard's class-cascade
                    // request count that previously tripped the perimeter
                    // rate limiter), so this is fine.
                    const reqs = studentClasses.map((sc) =>
                        this.studentSubjectsSvc
                            .get('/studentSubjects/byStudentClassId/' + sc.id)
                            .pipe(catchError(() => of([])))
                    );
                    forkJoin(reqs).subscribe({
                        next: (results) => {
                            this.rows = studentClasses.map((sc, idx) => {
                                const subjects = (results[idx] || []) as StudentSubject[];
                                return {
                                    studentClassId: parseInt(sc.id),
                                    upi: sc.student?.upi ?? '',
                                    fullName: sc.student?.fullName ?? '',
                                    chips: subjects.map((s) => ({
                                        studentSubjectId: parseInt(s.id),
                                        subjectId: s.subjectId,
                                        name: s.subject?.name ?? '',
                                        code: s.subject?.code
                                    }))
                                };
                            });
                        },
                        error: () => {
                            this.rows = [];
                        }
                    });
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    }

    private resetClassScopedState(): void {
        this.rows = [];
        this.studentClasses = [];
        this.selectedRowIds.clear();
        this.allSelected = false;
    }

    private reloadCurrentClass(): void {
        if (this.currentSchoolClassId != null) {
            this.loadRowsForClass(this.currentSchoolClassId);
        }
    }

    dataSubmitted = (_ssSearch: StudentSubjectSearch) => {
        // The new layout searches on Class change; this submit hook is
        // retained for compatibility with the existing search form but is
        // effectively a no-op now.
        if (this.currentSchoolClassId == null) {
            this.toastr.info('Select class to view its student subjects.');
        }
    };

    // Per-chip delete: removes one student/subject allocation.
    removeChip(row: StudentRow, chip: ChipSubject): void {
        Swal.fire({
            title: 'Remove subject?',
            text: `Remove "${chip.name}" from ${row.fullName}?`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Remove',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (!result.value) return;
            this.studentSubjectsSvc
                .delete('/studentSubjects', chip.studentSubjectId)
                .subscribe({
                    next: () => {
                        row.chips = row.chips.filter(
                            (c) => c.studentSubjectId !== chip.studentSubjectId
                        );
                        this.toastr.success('Subject allocation removed.');
                    },
                    error: (err) => {
                        this.toastr.error(err?.error ?? 'Delete failed.');
                    }
                });
        });
    }

    // Row selection helpers
    toggleRow(studentClassId: number, checked: boolean): void {
        if (checked) this.selectedRowIds.add(studentClassId);
        else this.selectedRowIds.delete(studentClassId);
        this.allSelected =
            this.rows.length > 0 &&
            this.selectedRowIds.size === this.rows.length;
    }

    toggleSelectAll(checked: boolean): void {
        this.allSelected = checked;
        this.selectedRowIds.clear();
        if (checked) {
            this.rows.forEach((r) => this.selectedRowIds.add(r.studentClassId));
        }
    }

    isRowSelected(studentClassId: number): boolean {
        return this.selectedRowIds.has(studentClassId);
    }

    // Bulk delete: removes every allocation belonging to every selected row.
    deleteSelected(): void {
        if (this.selectedRowIds.size === 0) return;
        const targetRows = this.rows.filter((r) =>
            this.selectedRowIds.has(r.studentClassId)
        );
        const ids = targetRows.flatMap((r) =>
            r.chips.map((c) => c.studentSubjectId)
        );
        if (ids.length === 0) {
            this.toastr.info(
                'The selected students have no subject allocations.'
            );
            return;
        }
        Swal.fire({
            title: 'Delete selected allocations?',
            text: `This will remove ${ids.length} allocation(s) across ${targetRows.length} student(s).`,
            width: 420,
            position: 'top',
            padding: '1em',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (!result.value) return;
            const reqs = ids.map((id) =>
                this.studentSubjectsSvc
                    .delete('/studentSubjects', id)
                    .pipe(catchError(() => of(null)))
            );
            forkJoin(reqs).subscribe({
                next: () => {
                    this.toastr.success(
                        `Removed ${ids.length} subject allocation(s).`
                    );
                    this.selectedRowIds.clear();
                    this.allSelected = false;
                    this.reloadCurrentClass();
                },
                error: (err) => {
                    this.toastr.error(err?.error ?? 'Bulk delete failed.');
                    this.reloadCurrentClass();
                }
            });
        });
    }

    // Add Allocations modal --------------------------------------------------
    openAddModal(): void {
        if (this.currentSchoolClassId == null) {
            this.toastr.info('Select a class first.');
            return;
        }
        if (this.rows.length === 0) {
            this.toastr.info('No students are enrolled in this class.');
            return;
        }
        if (this.availableSubjects.length === 0) {
            this.toastr.info(
                'No subjects are configured for this education level.'
            );
            return;
        }
        this.modalStudentIds.clear();
        this.modalSubjectIds.clear();
        this.modalSaving = false;
        this.showAddModal = true;
    }

    closeAddModal(): void {
        if (this.modalSaving) return;
        this.showAddModal = false;
    }

    toggleModalStudent(id: number, checked: boolean): void {
        if (checked) this.modalStudentIds.add(id);
        else this.modalStudentIds.delete(id);
    }

    toggleModalSubject(id: number, checked: boolean): void {
        if (checked) this.modalSubjectIds.add(id);
        else this.modalSubjectIds.delete(id);
    }

    toggleAllModalStudents(checked: boolean): void {
        this.modalStudentIds.clear();
        if (checked)
            this.rows.forEach((r) =>
                this.modalStudentIds.add(r.studentClassId)
            );
    }

    toggleAllModalSubjects(checked: boolean): void {
        this.modalSubjectIds.clear();
        if (checked)
            this.availableSubjects.forEach((s) =>
                this.modalSubjectIds.add(s.id)
            );
    }

    get allModalStudentsChecked(): boolean {
        return (
            this.rows.length > 0 &&
            this.modalStudentIds.size === this.rows.length
        );
    }

    get allModalSubjectsChecked(): boolean {
        return (
            this.availableSubjects.length > 0 &&
            this.modalSubjectIds.size === this.availableSubjects.length
        );
    }

    saveModal(): void {
        if (this.modalStudentIds.size === 0) {
            this.toastr.info('Select at least one student.');
            return;
        }
        if (this.modalSubjectIds.size === 0) {
            this.toastr.info('Select at least one subject.');
            return;
        }
        // Build the cross-product, then drop pairs that are already
        // allocated so the server doesn't have to no-op upsert them.
        const existing = new Set<string>();
        this.rows.forEach((r) =>
            r.chips.forEach((c) =>
                existing.add(`${r.studentClassId}:${c.subjectId}`)
            )
        );
        const payload: {studentClassId: number; subjectId: number}[] = [];
        let skipped = 0;
        this.modalStudentIds.forEach((scId) => {
            this.modalSubjectIds.forEach((subjId) => {
                if (existing.has(`${scId}:${subjId}`)) {
                    skipped++;
                } else {
                    payload.push({
                        studentClassId: scId,
                        subjectId: subjId
                    });
                }
            });
        });
        if (payload.length === 0) {
            this.toastr.info(
                'Every selected student already has every selected subject.'
            );
            return;
        }
        this.modalSaving = true;
        this.http
            .post('/studentSubjects/batch', payload, {responseType: 'text'})
            .subscribe({
                next: () => {
                    this.modalSaving = false;
                    this.showAddModal = false;
                    let msg = `Added ${payload.length} subject allocation(s).`;
                    if (skipped > 0) msg += ` Skipped ${skipped} already present.`;
                    this.toastr.success(msg);
                    this.reloadCurrentClass();
                },
                error: (err) => {
                    this.modalSaving = false;
                    this.toastr.error(err?.error ?? 'Failed to save allocations.');
                }
            });
    }

    // Restores previous filter state when re-entering the page.
    private restoreStateIfAny(): void {
        const state = this.stateSvc.get();
        if (!state || !state.curriculumId) return;

        const waitForInitials = () => {
            if (!this.doneLoading) {
                setTimeout(waitForInitials, 50);
                return;
            }
            this.searchForm?.setFormValues(state);

            this.educationLevelSvc
                .educationLevelsByCurriculum(state.curriculumId!)
                .subscribe({
                    next: (els) => {
                        this.educationLevels = els.sort(
                            (a, b) => a.rank - b.rank
                        );
                        if (!state.educationLevelId || !state.academicYearId)
                            return;
                        this.currentEducationLevelId = state.educationLevelId;
                        this.currentAcademicYearId = state.academicYearId;

                        this.educationLevelSubjectSvc
                            .getByEducationLevelAndAcademicYear(
                                state.educationLevelId,
                                state.academicYearId
                            )
                            .subscribe({
                                next: (subjs) => {
                                    this.availableSubjects = (subjs || []).map(
                                        (e) => ({
                                            id: e.subjectId,
                                            name: e.subject?.name ?? '',
                                            code: e.subject?.code
                                        })
                                    );
                                }
                            });

                        this.schoolClassSvc
                            .getByEducationLevelandYear(
                                state.educationLevelId,
                                state.academicYearId
                            )
                            .subscribe({
                                next: (scs) => {
                                    this.schoolClasses = scs.sort(
                                        (a, b) => a.rank - b.rank
                                    );
                                    if (!state.schoolClassId) return;
                                    this.currentSchoolClassId =
                                        state.schoolClassId;
                                    this.loadRowsForClass(state.schoolClassId);
                                }
                            });
                    }
                });
        };
        waitForInitials();
    }
}
