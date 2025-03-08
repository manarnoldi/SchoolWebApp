import {StudentDetails} from '@/students/models/student-details';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {StudentAttendanceFormComponent} from './student-attendance-form/student-attendance-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentAttendance} from '@/students/models/student-attendance';
import {ToastrService} from 'ngx-toastr';
import {StudentAttendancesService} from '@/students/services/student-attendances.service';
import {ActivatedRoute} from '@angular/router';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SchoolClass} from '@/class/models/school-class';

@Component({
    selector: 'app-student-attendance',
    templateUrl: './student-attendance.component.html',
    styleUrl: './student-attendance.component.scss'
})
export class StudentAttendanceComponent implements OnInit {
    @Input() statuses;
    @Input() student: StudentDetails;
    @ViewChild(StudentAttendanceFormComponent)
    studentAttendanceFormComponent: StudentAttendanceFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    studentId: number = 0;
    studentAttendance: StudentAttendance;
    studentAttendances: StudentAttendance[];
    schoolClasses: SchoolClass[];
    constructor(
        private toastr: ToastrService,
        private studentAttendancesSvc: StudentAttendancesService,
        private schoolClassesSvc: SchoolClassesService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStudentAttendances();
    }

    loadStudentAttendances = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            let attendanceByStudentIdReq = this.studentAttendancesSvc.get(
                '/studentAttendances/byStudentId/' + this.studentId.toString()
            );
            let schoolClassesReq = this.schoolClassesSvc.get('/schoolClasses');

            forkJoin([attendanceByStudentIdReq, schoolClassesReq]).subscribe(
                ([studentAttendances, schoolClasses]) => {
                    this.studentAttendances = studentAttendances;
                    this.schoolClasses = schoolClasses;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    editItem(id: number) {
        this.studentAttendancesSvc.getById(id, '/studentAttendances').subscribe(
            (res) => {
                let studentAttendanceId = res.id;
                this.studentAttendance = new StudentAttendance(res);
                this.studentAttendance.id = studentAttendanceId;
                this.studentAttendanceFormComponent.setFormControls(
                    this.studentAttendance
                );
                this.studentAttendanceFormComponent.editMode = true;
                this.studentAttendanceFormComponent.studentAttendance =
                    this.studentAttendance;
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
                this.studentAttendancesSvc
                    .delete('/studentAttendances', id)
                    .subscribe(
                        (res) => {
                            this.loadStudentAttendances();
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

    AddStudentAttendance = (studentAttendance: StudentAttendance) => {
        Swal.fire({
            title: `${this.studentAttendanceFormComponent.editMode ? 'Update' : 'Add'} Student attendance record?`,
            text: `Confirm if you want to ${
                this.studentAttendanceFormComponent.editMode ? 'update' : 'add'
            } student attendance.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentAttendanceFormComponent.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentAttendance(studentAttendance);
                if (this.studentAttendanceFormComponent.editMode)
                    app.id = studentAttendance.id;
                let reqToProcess = this.studentAttendanceFormComponent.editMode
                    ? this.studentAttendancesSvc.update(
                          '/studentAttendances',
                          app
                      )
                    : this.studentAttendancesSvc.create(
                          '/studentAttendances',
                          app
                      );

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.studentAttendanceFormComponent.editMode = false;
                        this.toastr.success(
                            'Student attendance saved successfully'
                        );
                        this.studentAttendanceFormComponent.closeButton.nativeElement.click();
                        this.loadStudentAttendances();
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
