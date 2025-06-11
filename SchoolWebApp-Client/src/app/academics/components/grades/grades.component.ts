import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {GradesAddFormComponent} from './grades-add-form/grades-add-form.component';
import {forkJoin, Subscription} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Grade} from '@/academics/models/grade';
import {Curriculum} from '@/academics/models/curriculum';
import {ToastrService} from 'ngx-toastr';
import {GradesService} from '@/academics/services/grades.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import Swal from 'sweetalert2';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

@Component({
    selector: 'app-grades',
    templateUrl: './grades.component.html',
    styleUrl: './grades.component.scss'
})
export class GradesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(GradesAddFormComponent)
    gradeForm: GradesAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    tblShowViewButton: true;
    isAuthLoading: boolean;

    firstLoad: boolean = true;
    page = 1;
    pageSize = 10;

    tableModel: string = 'grade';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/academics/grades'], title: 'Academics: Grades'}
    ];
    dashboardTitle = 'Academics: Grades';
    tableTitle: string = ' Grades list';
    tableHeaders: string[] = [
        'Name',
        'Abbreviation',
        'Rank',
        'Minimum Score',
        'Maximum Score',
        'Points',
        'Remarks (Swahili)',
        'Remarks (English)',
        'Curriculum',
        'Action'
    ];

    grade: Grade;
    grades: Grade[] = [];
    curricula: Curriculum[] = [];

    constructor(
        private toastr: ToastrService,
        private gradesSvc: GradesService,
        private curriculumSvc: CurriculumService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/grades/byCurriculumId?curriculumId=${cys.curriculumId ?? ''}`;
        this.gradesSvc.get(searchStr).subscribe({
            next: (grades) => {
                this.grades = grades.sort((a, b) => a.rank - b.rank);

                if (this.grades.length <= 0 && !this.firstLoad) {
                    this.toastr.info(
                        'No record found for the selected curriculum!'
                    );
                }
                this.isAuthLoading = false;
                this.firstLoad = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    curriculumChanged = (id: number) => {
        this.grades = [];
    };

    refreshItems() {
        let curriculaReq = this.curriculumSvc.get('/curricula');

        forkJoin([curriculaReq]).subscribe(
            ([curricula]) => {
                this.isAuthLoading = false;
                this.gradeForm.editMode = false;

                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                const topCurriculum = this.curricula[0];

                let cysPass = new SchoolSoftFilter();
                cysPass.curriculumId = parseInt(topCurriculum.id);

                this.ssFilterFormComponent.setFormControls(cysPass);
                this.ssFilterFormComponent.onSubmit();

                this.isAuthLoading = false;
                this.gradeForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.gradesSvc.getById(id, '/grades').subscribe(
            (res) => {
                let gradeId = res.id;
                this.grade = new Grade(res);
                this.grade.id = gradeId;
                this.gradeForm.setFormControls(this.grade);
                this.gradeForm.editMode = true;
                this.gradeForm.grade = this.grade;
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
                this.gradesSvc.delete('/grades', id).subscribe(
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

    resetForm = () => {
        this.gradeForm.editMode = false;
        this.gradeForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    addGrade = (grade: Grade) => {
        Swal.fire({
            title: `${this.gradeForm.editMode ? 'Update' : 'Add'} grade?`,
            text: `Confirm if you want to ${
                this.gradeForm.editMode ? 'update' : 'add'
            } grade.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.gradeForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new Grade(grade);
                if (this.gradeForm.editMode) app.id = grade.id;
                let reqToProcess = this.gradeForm.editMode
                    ? this.gradesSvc.update('/grades', app)
                    : this.gradesSvc.create('/grades', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.gradeForm.editMode = false;
                        this.gradeForm.refreshItems();
                        this.toastr.success('Grade saved successfully');
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
