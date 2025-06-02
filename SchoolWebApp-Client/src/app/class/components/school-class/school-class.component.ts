import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {
    Component,
    EventEmitter,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {SchoolClassAddFormComponent} from './school-class-add-form/school-class-add-form.component';
import {forkJoin} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
import {AcademicYear} from '@/school/models/academic-year';
import {ToastrService} from 'ngx-toastr';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import Swal from 'sweetalert2';
import {LearningLevel} from '@/class/models/learning-level';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {ClassLeadershipsService} from '@/class/services/class-leaderships.service';
import {CurriculumYearFilterFormComponent} from '@/shared/components/curriculum-year-filter-form/curriculum-year-filter-form.component';
import {CurriculumYearPerson} from '@/shared/models/curriculum-year-person';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {Curriculum} from '@/academics/models/curriculum';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevel} from '@/school/models/educationLevel';

@Component({
    selector: 'app-school-class',
    templateUrl: './school-class.component.html',
    styleUrl: './school-class.component.scss'
})
export class SchoolClassComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SchoolClassAddFormComponent)
    schoolClassForm: SchoolClassAddFormComponent;
    @Output() btnAddClickEvent = new EventEmitter<void>();
    @ViewChild(CurriculumYearFilterFormComponent)
    cyfFormComponent: CurriculumYearFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'schoolClass';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/classes'], title: 'School: Classes'}
    ];
    dashboardTitle = 'School: Classes';
    tableTitle: string = ' Classes list';
    tableHeaders: string[] = [
        'Academic year',
        'Class',
        'Stream',
        'Class name',
        'Rank',
        'Class Leaders',
        'Description',
        'Action',
        'Manage Leadership'
    ];

    schoolClass: SchoolClass;
    schoolClasses: SchoolClass[] = [];
    learningLevels: LearningLevel[] = [];
    schoolStreams: SchoolStream[] = [];
    academicYears: AcademicYear[] = [];
    curricula: Curriculum[] = [];
    educationLevels: EducationLevel[] = [];

    constructor(
        private toastr: ToastrService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelsSvc: LearningLevelsService,
        private schoolStreamsSvc: SchoolStreamsService,
        private academicYearsSvc: AcademicYearsService,
        private classLeadershipsService: ClassLeadershipsService,
        private educationLevelSvc: EducationLevelService,
        private curriculumSvc: CurriculumService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    academicYearChanged = (acadYearId: number) => {
        this.schoolClasses = [];
    };

    educLevelChanged = (edulvId: number) => {
        this.schoolClasses = [];
    };

    curriculumChanged = (curId: number) => {
        this.cyfFormComponent.curriculumYearStaffFilterForm
            .get('educationLevelId')
            .reset();
        this.schoolClasses = [];
        this.educationLevels = [];
        this.educationLevelSvc.educationLevelsByCurriculum(curId).subscribe({
            next: (educLevels) => {
                this.educationLevels = educLevels;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    refreshItems() {
        let learningLevelsReq = this.learningLevelsSvc.get('/learningLevels');
        let schoolStreamsReq = this.schoolStreamsSvc.get('/schoolStreams');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');
        let curriculaReq = this.curriculumSvc.get('/curricula');

        forkJoin([
            learningLevelsReq,
            schoolStreamsReq,
            academicYearsReq,
            curriculaReq
        ]).subscribe(
            ([learningLevels, schoolStreams, academicYears, curricula]) => {
                this.educationLevelSvc
                    .educationLevelsByCurriculum(
                        parseInt(
                            curricula.sort((a, b) => a.rank - b.rank)[0].id
                        )
                    )
                    .subscribe({
                        next: (educationLevels) => {
                            this.learningLevels = learningLevels;
                            this.schoolStreams = schoolStreams;
                            this.academicYears = academicYears.sort(
                                (a, b) => b.rank - a.rank
                            );
                            this.curricula = curricula.sort(
                                (a, b) => a.rank - b.rank
                            );
                            const topCurriculum = this.curricula[0];
                            const topYear = this.academicYears[0];
                            let cysPass = new CurriculumYearPerson();
                            cysPass.academicYearId = parseInt(topYear.id);
                            cysPass.curriculumId = parseInt(topCurriculum.id);
                            this.cyfFormComponent.setFormControls(cysPass);
                            this.educationLevels = educationLevels.sort(
                                (a, b) => a.rank - b.rank
                            );
                            this.loadSchoolClasses(cysPass);
                            this.isAuthLoading = false;
                            this.schoolClassForm.editMode = false;
                        },
                        error: (err) => {
                            this.toastr.error(err.error);
                        }
                    });
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    loadSchoolClasses = (cys: CurriculumYearPerson) => {
        this.schoolClassesSvc
            .getByEducationLevelandYear(
                cys.educationLevelId,
                cys.academicYearId
            )
            .subscribe({
                next: (schoolClasses) => {
                    let schoolClassLeadersReq = [];
                    schoolClasses.forEach((sc) =>
                        schoolClassLeadersReq.push(
                            this.classLeadershipsService.get(
                                '/schoolClassLeaders/bySchoolClassId/' + sc.id
                            )
                        )
                    );
                    schoolClassLeadersReq.push();
                    forkJoin([...schoolClassLeadersReq]).subscribe(
                        (resp) => {
                            for (let i = 0; i < schoolClasses.length - 1; i++) {
                                schoolClasses[i].schoolClassLeaders = resp[i];
                            }
                            this.schoolClasses = schoolClasses.sort(
                                (a, b) => a.rank - b.rank
                            );
                        },
                        (er) => {
                            this.toastr.error(er.error);
                        }
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    editItem(id: number) {
        this.schoolClassesSvc.getById(id, '/schoolClasses').subscribe(
            (res) => {
                let schoolClassId = res.id;
                this.schoolClass = new SchoolClass(res);
                this.schoolClass.id = schoolClassId;
                this.schoolClassForm.editMode = true;
                this.schoolClassForm.schoolClass = this.schoolClass;
                this.schoolClassForm.setFormControls(this.schoolClass);
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
                this.schoolClassesSvc.delete('/schoolClasses', id).subscribe(
                    (res) => {
                        this.refreshItems();
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

    // addButtonClicked = () => {
    //     this.schoolClassForm.editMode = false;
    //     this.schoolClassForm.resetFormControls();
    // };

    resetForm = () => {
        this.schoolClassForm.editMode = false;
        this.schoolClassForm.resetFormControls();
    };

    errorSchoolClass = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addSchoolClass = (schoolClass: SchoolClass) => {
        Swal.fire({
            title: `${this.schoolClassForm.editMode ? 'Update' : 'Add'} school class?`,
            text: `Confirm if you want to ${
                this.schoolClassForm.editMode ? 'update' : 'add'
            } school class.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.schoolClassForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new SchoolClass(schoolClass);
                if (this.schoolClassForm.editMode) app.id = schoolClass.id;
                let reqToProcess = this.schoolClassForm.editMode
                    ? this.schoolClassesSvc.update('/schoolClasses', app)
                    : this.schoolClassesSvc.create('/schoolClasses', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.schoolClassForm.editMode = false;
                        this.schoolClassForm.resetFormControls();
                        this.toastr.success('School class saved successfully');
                        this.refreshItems();
                        this.closeButton.nativeElement.click();
                    },
                    (err) => {
                        this.toastr.error(err.error);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
