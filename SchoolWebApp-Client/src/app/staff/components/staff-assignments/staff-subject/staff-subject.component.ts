import {StaffDetails} from '@/staff/models/staff-details';
import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import {StaffSubjectFormComponent} from './staff-subject-form/staff-subject-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StaffSubject} from '@/staff/models/staff-subject';
import {ToastrService} from 'ngx-toastr';
import {ActivatedRoute} from '@angular/router';
import {StaffSubjectsService} from '@/staff/services/staff-subjects.service';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Subject} from '@/academics/models/subject';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SchoolClass} from '@/class/models/school-class';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

@Component({
    selector: 'app-staff-subject',
    templateUrl: './staff-subject.component.html',
    styleUrl: './staff-subject.component.scss'
})
export class StaffSubjectComponent implements OnInit, AfterViewInit {
    @Input() statuses;
    @Input() staff: StaffDetails;
    @Input() subjects: Subject[];
    @ViewChild(StaffSubjectFormComponent)
    staffSubjectFormComponent: StaffSubjectFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    isDLoading = false;
    firstLoad: boolean = true;
    staffId: number = 0;
    staffSubject: StaffSubject;
    staffSubjects: StaffSubject[];
    academicYears: AcademicYear[];
    schoolClasses: SchoolClass[];

    constructor(
        private toastr: ToastrService,
        private staffSubjectsSvc: StaffSubjectsService,
        private route: ActivatedRoute,
        private academicYearsSvc: AcademicYearsService,
        private schoolClassesSvc: SchoolClassesService
    ) {}
    ngAfterViewInit(): void {
        this.loadStaffSubjects();
    }

    ngOnInit(): void {
        
    }

    academicYearChanged = (yearId: number) => {
        this.staffSubjects = [];
    };

    yearChanged = (yearId: number) => {
        this.schoolClasses = [];
        if (!yearId || yearId <= 0) {
            this.toastr.error('Select year first!');
            return;
        }
        this.schoolClassesSvc.getByAcademicYearId(yearId).subscribe({
            next: (schoolClasses) => {
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => a.rank - b.rank
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    searchForSubjects = (cyf: SchoolSoftFilter) => {
        if (!cyf.academicYearId) {
            this.toastr.error('Select academic year before clicking search!');
            return;
        }
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            this.staffSubjectsSvc
                .getByStaffYearId(this.staffId, cyf.academicYearId)
                .subscribe({
                    next: (staffSubjects) => {
                        this.staffSubjects = staffSubjects;
                        if (this.staffSubjects.length <= 0 && !this.firstLoad) {
                            this.toastr.info(
                                'No staff subject record/s found for the search parameters!'
                            );
                        }
                        if (this.firstLoad) {
                            this.ssFilterFormComponent.setFormControls(cyf);
                        }
                        this.firstLoad = false;
                        this.isDLoading = true;
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        });
    };

    loadStaffSubjects = () => {
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');

        forkJoin([academicYearsReq]).subscribe(
            ([academicYears]) => {
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                const year = this.academicYears[0];

                let dmy = new SchoolSoftFilter();
                dmy.academicYearId = parseInt(year.id);

                this.searchForSubjects(dmy);
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    editItem(id: number, action = 'edit') {
        this.staffSubjectsSvc.getById(id, '/staffSubjects').subscribe(
            (res) => {
                let staffSubjectId = res.id;
                this.staffSubject = new StaffSubject(res);
                this.staffSubject.id = staffSubjectId;
                this.staffSubjectFormComponent.setFormControls(
                    this.staffSubject
                );
                this.staffSubjectFormComponent.action = action;
                this.staffSubjectFormComponent.staffSubject = this.staffSubject;
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
                this.staffSubjectsSvc.delete('/staffSubjects', id).subscribe(
                    (res) => {
                        this.loadStaffSubjects();
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

    AddStaffSubject = (staffSubject: StaffSubject) => {
        Swal.fire({
            title: `${this.staffSubjectFormComponent.action == 'edit' ? 'Update' : 'Add'} Staff subject record?`,
            text: `Confirm if you want to ${
                this.staffSubjectFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } staff subject.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.staffSubjectFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StaffSubject(staffSubject);
                if (this.staffSubjectFormComponent.action == 'edit')
                    app.id = staffSubject.id;
                let reqToProcess =
                    this.staffSubjectFormComponent.action == 'edit'
                        ? this.staffSubjectsSvc.update('/staffSubjects', app)
                        : this.staffSubjectsSvc.create('/staffSubjects', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.staffSubjectFormComponent.action = 'add';
                        this.toastr.success('Staff subject saved successfully');
                        this.staffSubjectFormComponent.closeButton.nativeElement.click();
                        this.loadStaffSubjects();
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
