import {Component, OnInit, ViewChild} from '@angular/core';
import {EducationLevelSubjectsFormComponent} from './education-level-subjects-form/education-level-subjects-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {EducationLevelSubject} from '@/academics/models/education-level-subject';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYear} from '@/school/models/academic-year';
import {ToastrService} from 'ngx-toastr';
import {EducationLevelService} from '@/school/services/education-level.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {forkJoin} from 'rxjs';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import Swal from 'sweetalert2';
import {Subject} from '@/academics/models/subject';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SubjectsService} from '@/academics/services/subjects.service';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

@Component({
    selector: 'app-education-level-subjects',
    templateUrl: './education-level-subjects.component.html',
    styleUrl: './education-level-subjects.component.scss'
})
export class EducationLevelSubjectsComponent implements OnInit {
    @ViewChild(EducationLevelSubjectsFormComponent)
    educationLevelSubjectsFormComponent: EducationLevelSubjectsFormComponent;

    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFC: SchoolSoftFilterFormComponent;

    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    educationLevelSubject: EducationLevelSubject;
    educationLevels: EducationLevel[] = [];
    academicYears: AcademicYear[] = [];
    subjects: Subject[] = [];

    isLoading: boolean = false;

    educationLevelSubjects: EducationLevelSubject[] = [];

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/academics/educationLevelSubjects'],
            title: 'Academics: Edu-Level Subjects'
        }
    ];
    dashboardTitle = 'Academics: Edu-Level Subjects';

    constructor(
        private toastr: ToastrService,
        private educationLevelsSvc: EducationLevelService,
        private academicYearsSvc: AcademicYearsService,
        private subjectsSvc: SubjectsService,
        private educationLevelSubjectSvc: EducationLevelSubjectService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    academicYearChanged = ($academicYearId: number) => {
        this.educationLevelSubjects = [];
    };

    educationLevelChanged = (educationLevelId: number) => {
        this.educationLevelSubjects = [];
    };

    checkInputAll(check: boolean = false) {
        if (
            this.educationLevelSubjectsFormComponent &&
            this.educationLevelSubjectsFormComponent.subjectsMinTableComponent
        )
            this.educationLevelSubjectsFormComponent.subjectsMinTableComponent.checkAll.nativeElement.checked =
                check;
    }

    loadInitials = () => {
        let educationLevelReq = this.educationLevelsSvc.get('/educationLevels');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');
        let subjectsReq = this.subjectsSvc.get('/subjects');

        forkJoin([educationLevelReq, academicYearsReq, subjectsReq]).subscribe({
            next: ([educationLevels, academicYears, subjects]) => {
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.subjects = subjects.sort((a, b) => a.rank - b.rank);

                const topEduLevel = this.educationLevels[0];
                const topAcadYear = this.academicYears[0];

                let cysPass = new SchoolSoftFilter();
                cysPass.educationLevelId = parseInt(topEduLevel.id);
                cysPass.academicYearId = parseInt(topAcadYear.id);

                this.ssFFC.setFormControls(cysPass);
                this.ssFFC.onSubmit();

                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    loadByEducationLevelYear = (ssf: SchoolSoftFilter) => {
        if (!ssf.academicYearId) {
            this.toastr.error(
                'Select academic year first before clicking search'
            );
            return;
        }
        if (!ssf.educationLevelId) {
            this.toastr.error(
                'Select education level first before clicking search'
            );
            return;
        }

        let educationLevelSubjectsReq = this.educationLevelSubjectSvc.get(
            `/educationLevelSubjects/byEducationLevelYearId/${ssf.educationLevelId}/${ssf.academicYearId}`
        );

        forkJoin([educationLevelSubjectsReq]).subscribe(
            ([educationLevelSubjects]) => {
                this.educationLevelSubjects = educationLevelSubjects;
                if (this.educationLevelSubjects.length <= 0) {
                    this.toastr.info('No record found for the selection!');
                }
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    academicYearEduLevelChanged = () => {
        this.educationLevelSubjects = [];
        this.subjects.forEach((s) => (s.isSelected = false));
    };

    academicYearEduLevelChangedInForm = (ly: SchoolSoftFilter) => {
        this.subjects.forEach((s) => (s.isSelected = false));
        this.checkInputAll(false);
        if (!ly.academicYearId) {
            this.toastr.error(
                'Select academic year first before clicking search'
            );
            return;
        }
        if (!ly.educationLevelId) {
            this.toastr.error(
                'Select education level first before clicking search'
            );
            return;
        }

        let educationLevelSubjectsReq = this.educationLevelSubjectSvc.get(
            `/educationLevelSubjects/byEducationLevelYearId/${ly.educationLevelId}/${ly.academicYearId}`
        );

        forkJoin([educationLevelSubjectsReq]).subscribe(
            ([educationLevelSubjects]) => {
                educationLevelSubjects.forEach((els) => {
                    if (
                        this.subjects.some(
                            (s) => s.id == els.subjectId.toString()
                        )
                    ) {
                        this.subjects.find(
                            (s) => s.id == els.subjectId.toString()
                        ).isSelected = true;
                    }
                });
                if (this.subjects.some((s) => s.isSelected == false)) {
                    this.checkInputAll(false);
                } else {
                    this.checkInputAll(true);
                }
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    editItem(id: number) {
        this.educationLevelSubjectSvc
            .getById(id, '/educationLevelSubjects')
            .subscribe(
                (res) => {
                    let educationLevelSubjectId = res.id;
                    this.educationLevelSubject = new EducationLevelSubject(res);
                    this.educationLevelSubject.id = educationLevelSubjectId;
                    this.educationLevelSubjectsFormComponent.setFormControls(
                        this.educationLevelSubject
                    );
                    this.educationLevelSubjectsFormComponent.editMode = true;
                    this.educationLevelSubjectsFormComponent.educationLevelSubject =
                        this.educationLevelSubject;
                    let ely = new SchoolSoftFilter();
                    ely.academicYearId =
                        this.educationLevelSubject.academicYearId;
                    ely.educationLevelId =
                        this.educationLevelSubject.educationLevelId;
                    this.academicYearEduLevelChangedInForm(ely);
                    this.tableButton.onClick();
                },
                (err) => {
                    this.toastr.error(err);
                }
            );
    }

    deleteItem(id: number) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let eduLvlYear = new SchoolSoftFilter();
                eduLvlYear.educationLevelId = this.educationLevelSubjects.find(
                    (s) => s.id == id.toString()
                ).educationLevelId;
                eduLvlYear.academicYearId = this.educationLevelSubjects.find(
                    (s) => s.id == id.toString()
                ).academicYearId;
                this.educationLevelSubjectSvc
                    .delete('/educationLevelSubjects', id)
                    .subscribe(
                        (res) => {
                            this.loadInitials();
                            this.loadByEducationLevelYear(eduLvlYear);
                            this.toastr.success('Record deleted successfully!');
                        },
                        (err) => {
                            this.toastr.error(err);
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    deleteItems(educationLevelSubjects: EducationLevelSubject[]) {
        let deleteSubjectIdsReq = [];
        let checkForExistenceReq = [];

        this.subjects.forEach((s) => {
            if (!s.isSelected) {
                checkForExistenceReq.push(
                    this.educationLevelSubjectSvc.getObjectBySearch(
                        `/educationLevelSubjects/byEducationLevelYearSubjectId/${educationLevelSubjects[0].educationLevelId}/${educationLevelSubjects[0].academicYearId}/${s.id}`
                    )
                );
            }
        });

        if (checkForExistenceReq.length <= 0) {
            this.recordsUpdated(educationLevelSubjects);
        }

        forkJoin(checkForExistenceReq).subscribe(
            (educationLevelSubjs) => {
                educationLevelSubjs.forEach((ls) => {
                    if (ls.id) {
                        deleteSubjectIdsReq.push(
                            this.educationLevelSubjectSvc.delete(
                                '/educationLevelSubjects',
                                parseInt(ls.id)
                            )
                        );
                    }
                });
                if (deleteSubjectIdsReq.length <= 0) {
                    this.recordsUpdated(educationLevelSubjects);
                }

                forkJoin(deleteSubjectIdsReq).subscribe(
                    (res) => {
                        this.recordsUpdated(educationLevelSubjects);
                    },
                    (err) => {
                        this.toastr.error(err.error);
                    }
                );
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    AddEducationLevelSubject = (
        educationLevelSubjects: EducationLevelSubject[]
    ) => {
        Swal.fire({
            title: `Update education level subjects record?`,
            text: `Confirm if you want to update education level subjects.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Update`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.educationLevelSubjectSvc
                    .createBatch(
                        '/educationLevelSubjects/batch',
                        educationLevelSubjects
                    )
                    .subscribe(
                        (res) => {
                            this.deleteItems(educationLevelSubjects);
                        },
                        (err) => {
                            this.toastr.error(err.error?.message);
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };

    private recordsUpdated(educationLevelSubjects: EducationLevelSubject[]) {
        this.toastr.success('Education level subjects updated successfully');
        this.educationLevelSubjectsFormComponent.editMode = false;
        this.educationLevelSubjectsFormComponent.closeButton.nativeElement.click();

        let ey = new SchoolSoftFilter();
        ey.educationLevelId = educationLevelSubjects[0].educationLevelId;
        ey.academicYearId = educationLevelSubjects[0].academicYearId;
        this.ssFFC.setFormControls(ey);
        this.educationLevelSubjectsFormComponent.subjects.forEach(
            (s) => (s.isSelected = false)
        );
        this.checkInputAll(false);
        this.loadByEducationLevelYear(ey);
    }
}
