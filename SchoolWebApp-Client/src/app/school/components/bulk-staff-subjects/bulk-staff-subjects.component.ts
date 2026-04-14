import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {catchError} from 'rxjs/operators';
import Swal from 'sweetalert2';
import {AcademicYearsService} from '../../services/academic-years.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {StaffSubjectsService} from '@/staff/services/staff-subjects.service';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {StaffSubject} from '@/staff/models/staff-subject';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-bulk-staff-subjects',
    templateUrl: './bulk-staff-subjects.component.html',
    styleUrl: './bulk-staff-subjects.component.scss'
})
export class BulkStaffSubjectsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/school/bulk-staff-subjects'], title: 'Bulk Staff-Subject Allocation'}
    ];
    dashboardTitle = 'Bulk Staff-Subject Allocation';

    academicYears: any[] = [];
    schoolClasses: any[] = [];
    teachingStaff: any[] = [];

    selectedAcademicYearId: any = null;
    selectedSchoolClassId: any = null;

    allocationRows: {
        subjectId: number;
        subjectName: string;
        staffDetailsId: number | null;
        existingId: string | null;
        existingStaffId: number | null;
        changed: boolean;
    }[] = [];

    isLoading: boolean = false;
    isSaving: boolean = false;
    hasLoaded: boolean = false;

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private schoolClassesSvc: SchoolClassesService,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private staffDetailsSvc: StaffDetailsService,
        private staffSubjectsSvc: StaffSubjectsService,
        private staffCategoriesSvc: StaffCategoriesService
    ) {}

    ngOnInit(): void {
        this.loadInitialData();
    }

    loadInitialData() {
        forkJoin([
            this.academicYearSvc.get('/academicYears'),
            this.staffCategoriesSvc.get('/staffCategories')
        ]).subscribe({
            next: ([academicYears, staffCategories]) => {
                this.academicYears = academicYears.sort((a, b) => a.rank - b.rank);
                let activeYear = this.academicYears.find((y) => y.status === true);
                if (activeYear) {
                    this.selectedAcademicYearId = activeYear.id;
                    this.onAcademicYearChange();
                }

                // Load teaching staff
                let teachingCategory = staffCategories.find((c) => c.forTeaching === true);
                if (teachingCategory) {
                    this.staffDetailsSvc.get('/staffDetails/byStaffCategoryId/' + teachingCategory.id).subscribe({
                        next: (staff) => {
                            this.teachingStaff = staff
                                .filter((s) => s.status == Status.Active)
                                .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));
                        },
                        error: () => {}
                    });
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onAcademicYearChange() {
        this.schoolClasses = [];
        this.selectedSchoolClassId = null;
        this.allocationRows = [];
        this.hasLoaded = false;
        if (!this.selectedAcademicYearId) return;

        this.schoolClassesSvc.getByAcademicYearId(parseInt(this.selectedAcademicYearId)).subscribe({
            next: (classes) => {
                this.schoolClasses = classes.sort((a, b) => {
                    let nameA = (a.learningLevel?.name || '') + (a.schoolStream?.name || '');
                    let nameB = (b.learningLevel?.name || '') + (b.schoolStream?.name || '');
                    return nameA.localeCompare(nameB);
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onSchoolClassChange() {
        this.allocationRows = [];
        this.hasLoaded = false;
        if (!this.selectedSchoolClassId) return;
        this.loadSubjectsAndAllocations();
    }

    loadSubjectsAndAllocations() {
        this.isLoading = true;
        let sc = this.schoolClasses.find((c) => c.id == this.selectedSchoolClassId);
        if (!sc || !sc.learningLevel?.educationLevelId) {
            this.isLoading = false;
            return;
        }

        forkJoin([
            this.educationLevelSubjectSvc.getByEducationLevelAndAcademicYear(
                sc.learningLevel.educationLevelId,
                parseInt(this.selectedAcademicYearId)
            ),
            this.staffSubjectsSvc.get('/staffSubjects/bySchoolClassId/' + this.selectedSchoolClassId)
        ]).subscribe({
            next: ([elSubjects, existingAllocations]) => {
                let subjects = elSubjects
                    .map((es) => es.subject)
                    .filter(Boolean)
                    .sort((a, b) => (a.rank || 0) - (b.rank || 0));

                this.allocationRows = subjects.map((subj) => {
                    let existing = existingAllocations.find((a) => a.subjectId == subj.id);
                    return {
                        subjectId: parseInt(subj.id),
                        subjectName: subj.name || '',
                        staffDetailsId: existing ? existing.staffDetailsId : null,
                        existingId: existing ? existing.id : null,
                        existingStaffId: existing ? existing.staffDetailsId : null,
                        changed: false
                    };
                });

                this.isLoading = false;
                this.hasLoaded = true;
            },
            error: (err) => {
                this.isLoading = false;
                this.toastr.error(err.error);
            }
        });
    }

    onStaffChange(row: any) {
        row.changed = (row.staffDetailsId != row.existingStaffId);
    }

    hasChanges(): boolean {
        return this.allocationRows.some((r) => r.changed);
    }

    getChangedCount(): number {
        return this.allocationRows.filter((r) => r.changed).length;
    }

    getStaffName(staffId: number): string {
        let staff = this.teachingStaff.find((s) => s.id == staffId);
        return staff ? staff.fullName : '';
    }

    saveAllocations() {
        let changedRows = this.allocationRows.filter((r) => r.changed);
        if (changedRows.length === 0) {
            this.toastr.info('No changes to save.');
            return;
        }

        Swal.fire({
            title: 'Save allocations?',
            text: `${changedRows.length} subject allocation(s) will be updated.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Save',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = changedRows.map((row) => {
                    let payload = new StaffSubject({
                        staffDetailsId: row.staffDetailsId,
                        subjectId: row.subjectId,
                        schoolClassId: parseInt(this.selectedSchoolClassId),
                        description: ''
                    });

                    if (row.existingId && row.staffDetailsId) {
                        // Update existing
                        payload.id = row.existingId;
                        return this.staffSubjectsSvc.update('/staffSubjects', payload)
                            .pipe(catchError((err) => {
                                this.toastr.error(`Error updating ${row.subjectName}: ${err.error?.message || 'Unknown error'}`);
                                return of(null);
                            }));
                    } else if (row.existingId && !row.staffDetailsId) {
                        // Remove allocation (staff cleared)
                        return this.staffSubjectsSvc.delete('/staffSubjects', parseInt(row.existingId))
                            .pipe(catchError((err) => {
                                this.toastr.error(`Error removing ${row.subjectName}: ${err.error?.message || 'Unknown error'}`);
                                return of(null);
                            }));
                    } else if (!row.existingId && row.staffDetailsId) {
                        // Create new
                        return this.staffSubjectsSvc.create('/staffSubjects', payload)
                            .pipe(catchError((err) => {
                                this.toastr.error(`Error allocating ${row.subjectName}: ${err.error?.message || 'Unknown error'}`);
                                return of(null);
                            }));
                    }
                    return of(null);
                });

                forkJoin(requests).subscribe({
                    next: () => {
                        this.isSaving = false;
                        this.toastr.success('Allocations saved successfully!');
                        this.loadSubjectsAndAllocations();
                    },
                    error: () => {
                        this.isSaving = false;
                        this.toastr.error('Error saving allocations.');
                    }
                });
            }
        });
    }
}
