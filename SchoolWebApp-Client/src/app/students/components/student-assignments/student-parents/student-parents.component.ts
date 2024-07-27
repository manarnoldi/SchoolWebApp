import {Nationality} from '@/settings/models/nationality';
import {Occupation} from '@/settings/models/occupation';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {StudentParentsNewFormComponent} from './student-parents-new-form/student-parents-new-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentParent} from '@/students/models/student-parent';
import {ToastrService} from 'ngx-toastr';
import {StudentParentsService} from '@/students/services/student-parents.service';
import {ActivatedRoute} from '@angular/router';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import { Parent } from '@/students/models/parent';

@Component({
    selector: 'app-student-parents',
    templateUrl: './student-parents.component.html',
    styleUrl: './student-parents.component.scss'
})
export class StudentParentsComponent implements OnInit {
    @Input() statuses;
    @Input() parent: Parent;

    @Input() occupations: Occupation[];
    @Input() nationalities: Nationality[];

    @ViewChild(StudentParentsNewFormComponent)
    studentParentsNewFormComponent: StudentParentsNewFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    studentId: number = 0;
    studentParent: StudentParent;
    studentParents: StudentParent[] = [];
    constructor(
        private toastr: ToastrService,
        private studentParentsSvc: StudentParentsService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStudentParents();
    }

    loadStudentParents = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            let parentsByStudentIdReq = this.studentParentsSvc.get(
                '/students/studentParents/' + this.studentId.toString()
            );

            forkJoin([parentsByStudentIdReq]).subscribe(
                ([studentParents]) => {
                    this.studentParents = studentParents;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    editItem(studentId: number, parentId: number, action = 'edit') {
        this.studentParentsSvc
            .getByIds([studentId, parentId], '/students/studentParents/')
            .subscribe(
                (res) => {
                    let studentId = res.studentId;
                    let parentId = res.parentId;
                    this.studentParent = new StudentParent(res);
                    this.studentParent.parentId = parentId;
                    this.studentParent.studentId = studentId;
                    // this.studentParentsNewFormComponent.setFormControls(
                    //     this.studentParent
                    // );
                    this.studentParentsNewFormComponent.action = action;
                    // this.studentParentsNewFormComponent.studentParent =
                    //     this.studentParent;
                    this.tableButton.onClick();
                },
                (err) => {
                    this.toastr.error(err);
                }
            );
    }

    deleteItem(studentId: number, parentId: number) {
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
                this.studentParentsSvc
                    .deleteByIds('/students/studentParent/', [
                        parentId,
                        studentId
                    ])
                    .subscribe(
                        (res) => {
                            this.loadStudentParents();
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

    AddStudentParent = (studentParent: StudentParent) => {
        Swal.fire({
            title: `${this.studentParentsNewFormComponent.action == 'edit' ? 'Update' : 'Add'} Staff attendance record?`,
            text: `Confirm if you want to ${
                this.studentParentsNewFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } staff attendance.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentParentsNewFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentParent(studentParent);
                if (this.studentParentsNewFormComponent.action == 'edit')
                    app.id = studentParent.id;
                let reqToProcess =
                    this.studentParentsNewFormComponent.action == 'edit'
                        ? this.studentParentsSvc.update(
                              '/students/studentParents/',
                              app
                          )
                        : this.studentParentsSvc.create(
                              '/students/studentParents/',
                              app
                          );

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.studentParentsNewFormComponent.action = 'add';
                        this.toastr.success(
                            'Student parent saved successfully'
                        );
                        this.studentParentsNewFormComponent.closeButton.nativeElement.click();
                        this.loadStudentParents();
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
