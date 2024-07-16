import {StudentDetails} from '@/students/models/student-details';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {StudentFormerSchoolFormComponent} from './student-former-school-form/student-former-school-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentFormerSchool} from '@/students/models/student-former-school';
import {ToastrService} from 'ngx-toastr';
import {StudentFormerSchoolsService} from '@/students/services/student-former-schools.service';
import {ActivatedRoute} from '@angular/router';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {EducationLevel} from '@/school/models/educationLevel';
import {Curriculum} from '@/academics/models/curriculum';

@Component({
    selector: 'app-student-former-school',
    templateUrl: './student-former-school.component.html',
    styleUrl: './student-former-school.component.scss'
})
export class StudentFormerSchoolComponent implements OnInit {
    @Input() statuses;
    @Input() student: StudentDetails;
    @Input() curricula: Curriculum[];
    @Input() educationLevels: EducationLevel[];
    @ViewChild(StudentFormerSchoolFormComponent)
    studentFormerSchoolFormComponent: StudentFormerSchoolFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    studentId: number = 0;
    studentFormerSchool: StudentFormerSchool;
    studentFormerSchools: StudentFormerSchool[] = [];
    constructor(
        private toastr: ToastrService,
        private studentFormerSchoolsSvc: StudentFormerSchoolsService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStudentFormerSchools();
    }

    loadStudentFormerSchools = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            let formerSchoolByStudentIdReq = this.studentFormerSchoolsSvc.get(
                '/formerSchools/byStudentId/' + this.studentId.toString()
            );

            forkJoin([formerSchoolByStudentIdReq]).subscribe(
                ([studentFormerSchools]) => {
                    this.studentFormerSchools = studentFormerSchools;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    editItem(id: number, action = 'edit') {
        this.studentFormerSchoolsSvc.getById(id, '/formerSchools').subscribe(
            (res) => {
                let studentFormerSchoolId = res.id;
                this.studentFormerSchool = new StudentFormerSchool(res);
                this.studentFormerSchool.id = studentFormerSchoolId;
                this.studentFormerSchoolFormComponent.setFormControls(
                    this.studentFormerSchool
                );
                this.studentFormerSchoolFormComponent.action = action;
                this.studentFormerSchoolFormComponent.studentFormerSchool =
                    this.studentFormerSchool;
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
                this.studentFormerSchoolsSvc
                    .delete('/formerSchools', id)
                    .subscribe(
                        (res) => {
                            this.loadStudentFormerSchools();
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

    AddStudentFormerSchool = (studentFormerSchool: StudentFormerSchool) => {
        Swal.fire({
            title: `${this.studentFormerSchoolFormComponent.action == 'edit' ? 'Update' : 'Add'} Staff attendance record?`,
            text: `Confirm if you want to ${
                this.studentFormerSchoolFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } staff attendance.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentFormerSchoolFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentFormerSchool(studentFormerSchool);
                if (this.studentFormerSchoolFormComponent.action == 'edit')
                    app.id = studentFormerSchool.id;
                let reqToProcess =
                    this.studentFormerSchoolFormComponent.action == 'edit'
                        ? this.studentFormerSchoolsSvc.update(
                              '/formerSchools',
                              app
                          )
                        : this.studentFormerSchoolsSvc.create(
                              '/formerSchools',
                              app
                          );

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.studentFormerSchoolFormComponent.action = 'add';
                        this.toastr.success(
                            'Student former school saved successfully'
                        );
                        this.studentFormerSchoolFormComponent.closeButton.nativeElement.click();
                        this.loadStudentFormerSchools();
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
