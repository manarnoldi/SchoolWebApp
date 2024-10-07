import {StudentSubject} from '@/students/models/student-subject';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StudentSubjectsFormComponent} from './student-subjects-form/student-subjects-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentDetails} from '@/students/models/student-details';
import {SchoolClass} from '@/class/models/school-class';
import {StudentClassService} from '@/students/services/student-class.service';
import {Subject} from '@/academics/models/subject';
import {SubjectsService} from '@/academics/services/subjects.service';
import {StudentClass} from '@/students/models/student-class';
import {StudentSubjectsLoadFormComponent} from './student-subjects-load-form/student-subjects-load-form.component';

@Component({
    selector: 'app-student-subjects',
    templateUrl: './student-subjects.component.html',
    styleUrl: './student-subjects.component.scss'
})
export class StudentSubjectsComponent implements OnInit {
    @Input() statuses;
    @Input() student: StudentDetails;

    @ViewChild(StudentSubjectsFormComponent)
    studentSubjectsFormComponent: StudentSubjectsFormComponent;
    @ViewChild(StudentSubjectsLoadFormComponent)
    studentSubjectsLoadFormComponent: StudentSubjectsLoadFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    studentId: number = 0;
    studentSubject: StudentSubject;
    studentSubjects: StudentSubject[] = [];
    subjects: Subject[] = [];

    studentClasses: StudentClass[] = [];
    currentStudentClass: StudentClass;

    constructor(
        private toastr: ToastrService,
        private studentSubjectsSvc: StudentSubjectsService,
        private route: ActivatedRoute,
        private studentClassesSvc: StudentClassService,
        private subjectsSvc: SubjectsService
    ) {}

    ngOnInit(): void {
        this.loadStudentSubjects();
    }

    loadStudentSubjectsBySchoolClassId = (schoolClassId: number) => {
        let studentSubjectByStudentIdReq = this.studentSubjectsSvc.get(
            '/studentSubjects/bySchoolClassId/' +
                schoolClassId +
                '/' +
                this.student?.id
        );
        forkJoin([studentSubjectByStudentIdReq]).subscribe(
            ([studentSubjects]) => {
                this.studentSubjects = studentSubjects;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    loadClicked = (studentClass: StudentClass) => {
        this.currentStudentClass = studentClass;
        this.studentSubjects = [];
        if (!studentClass) this.studentSubjects = [];
        else
            this.loadStudentSubjectsBySchoolClassId(
                studentClass?.schoolClassId
            );
    };

    loadStudentSubjects = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            let studentClassesReq = this.studentClassesSvc.get(
                '/studentClasses/byStudentId/' + this.studentId
            );
            let subjectsReq = this.subjectsSvc.get('/subjects');
            forkJoin([studentClassesReq, subjectsReq]).subscribe(
                ([studentClasses, subjects]) => {
                    this.studentClasses = studentClasses;
                    this.subjects = subjects;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

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
                this.studentSubjectsSvc
                    .delete('/studentSubjects', id)
                    .subscribe(
                        (res) => {
                            this.loadStudentSubjectsBySchoolClassId(
                                this.currentStudentClass?.schoolClassId
                            );
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

    AddStudentSubject = (studentSubjects: StudentSubject[]) => {
        Swal.fire({
            title: `${this.studentSubjectsFormComponent.action == 'edit' ? 'Update' : 'Add'} Staff subject record?`,
            text: `Confirm if you want to ${
                this.studentSubjectsFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } staff subject.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentSubjectsFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.currentStudentClass = studentSubjects[0].studentClass;
                studentSubjects.forEach((e) => {
                    delete e.studentClass;
                });
                this.studentSubjectsSvc
                    .createBatch('/studentSubjects/batch', studentSubjects)
                    .subscribe(
                        (res) => {
                            this.studentSubjectsFormComponent.action = 'add';
                            this.toastr.success(
                                'Student subjects saved successfully'
                            );
                            this.studentSubjectsFormComponent.closeButton.nativeElement.click();
                            this.loadStudentSubjects();
                            this.loadClicked(this.currentStudentClass);
                            this.studentSubjectsLoadFormComponent.setFormControls(
                                this.currentStudentClass
                            );
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
