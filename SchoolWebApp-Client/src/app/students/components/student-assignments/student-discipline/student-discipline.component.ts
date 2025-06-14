import {OccurenceType} from '@/settings/models/occurence-type';
import {Outcome} from '@/settings/models/outcome';
import {StudentDetails} from '@/students/models/student-details';
import {
    AfterViewInit,
    Component,
    Input,
    OnInit,
    ViewChild
} from '@angular/core';
import {StudentDisciplineFormComponent} from './student-discipline-form/student-discipline-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentDiscipline} from '@/students/models/student-discipline';
import {ToastrService} from 'ngx-toastr';
import {StudentDisciplinesService} from '@/students/services/student-disciplines.service';
import {ActivatedRoute} from '@angular/router';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {DatePipe} from '@angular/common';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import { SchoolSoftFilter } from '@/shared/models/school-soft-filter';

@Component({
    selector: 'app-student-discipline',
    templateUrl: './student-discipline.component.html',
    styleUrl: './student-discipline.component.scss'
})
export class StudentDisciplineComponent implements OnInit, AfterViewInit {
    @Input() statuses;
    @Input() student: StudentDetails;
    @Input() outcomes: Outcome[];
    @Input() occurenceTypes: OccurenceType[];
    @ViewChild(StudentDisciplineFormComponent)
    studentDisciplineFormComponent: StudentDisciplineFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    firstLoad: boolean = true;

    studentId: number = 0;
    studentDiscipline: StudentDiscipline;
    studentDisciplines: StudentDiscipline[] = [];

    constructor(
        private toastr: ToastrService,
        private studentDisciplinesSvc: StudentDisciplinesService,
        private route: ActivatedRoute,
        private datePipe: DatePipe
    ) {}
    ngOnInit(): void {}

    ngAfterViewInit(): void {
        this.loadStudentDisciplines();
    }

    dateFromChanged = (id: number) => {
        this.studentDisciplines = [];
    };

    dateToChanged = (id: number) => {
        this.studentDisciplines = [];
    };

    loadStudentDisciplines = () => {
        const today = new Date();
        const dateFrom = new Date(today.getFullYear(), 0, 1);
        const dateTo = new Date(today.getFullYear(), 11, 31);

        let dmy = new SchoolSoftFilter();
        dmy.dateFrom = dateFrom;
        dmy.dateTo = dateTo;

        this.ssFilterFormComponent.setFormControls(dmy);

        this.searchByDate(dmy);
    };

    searchByDate = (dmy: SchoolSoftFilter) => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            this.studentDisciplinesSvc
                .getByDateFromDateToStudentId(
                    this.studentId,
                    this.datePipe.transform(dmy.dateFrom, 'yyyy-MM-dd'),
                    this.datePipe.transform(dmy.dateTo, 'yyyy-MM-dd')
                )
                .subscribe({
                    next: (studentDisciplines) => {
                        this.studentDisciplines = studentDisciplines.sort(
                            (a, b) =>
                                new Date(
                                    a?.occurenceStartDate ?? ''
                                ).getTime() -
                                new Date(b?.occurenceStartDate ?? '').getTime()
                        );
                        if (
                            this.studentDisciplines.length <= 0 &&
                            !this.firstLoad
                        ) {
                            this.toastr.info(
                                'No student discipline record/s found for the search parameters!'
                            );
                        }
                        this.firstLoad = false;
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        });
    };

    editItem(id: number, action = 'edit') {
        this.studentDisciplinesSvc.getById(id, '/studentDisciplines').subscribe(
            (res) => {
                let studentDisciplineId = res.id;
                this.studentDiscipline = new StudentDiscipline(res);
                this.studentDiscipline.id = studentDisciplineId;
                this.studentDisciplineFormComponent.setFormControls(
                    this.studentDiscipline
                );
                this.studentDisciplineFormComponent.action = action;
                this.studentDisciplineFormComponent.studentDiscipline =
                    this.studentDiscipline;
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
                this.studentDisciplinesSvc
                    .delete('/studentDisciplines', id)
                    .subscribe(
                        (res) => {
                            this.loadStudentDisciplines();
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

    AddStudentDiscipline = (studentDiscipline: StudentDiscipline) => {
        Swal.fire({
            title: `${this.studentDisciplineFormComponent.action == 'edit' ? 'Update' : 'Add'} Student discipline record?`,
            text: `Confirm if you want to ${
                this.studentDisciplineFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } student discipline.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentDisciplineFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentDiscipline(studentDiscipline);
                if (this.studentDisciplineFormComponent.action == 'edit')
                    app.id = studentDiscipline.id;
                let reqToProcess =
                    this.studentDisciplineFormComponent.action == 'edit'
                        ? this.studentDisciplinesSvc.update(
                              '/studentDisciplines',
                              app
                          )
                        : this.studentDisciplinesSvc.create(
                              '/studentDisciplines',
                              app
                          );

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.studentDisciplineFormComponent.action = 'add';
                        this.toastr.success(
                            'Student discipline saved successfully'
                        );
                        this.studentDisciplineFormComponent.closeButton.nativeElement.click();
                        this.loadStudentDisciplines();
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
