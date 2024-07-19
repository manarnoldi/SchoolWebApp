import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {StudentDetails} from '@/students/models/student-details';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {StudentClassFormComponent} from './student-class-form/student-class-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentClass} from '@/students/models/student-class';
import {StudentClassService} from '@/students/services/student-class.service';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-student-class',
    templateUrl: './student-class.component.html',
    styleUrl: './student-class.component.scss'
})
export class StudentClassComponent implements OnInit {
    @Input() statuses;
    @Input() student: StudentDetails;
    @Input() academicYears: AcademicYear[];
    @Input() schoolStreams: SchoolStream[];
    @Input() learningLevels: LearningLevel[];
    @ViewChild(StudentClassFormComponent)
    studentClassFormComponent: StudentClassFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    studentId: number = 0;
    studentClass: StudentClass;
    studentClasses: StudentClass[];
    constructor(
        private toastr: ToastrService,
        private studentClassSvc: StudentClassService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStudentClasses();
    }

    loadStudentClasses = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            let sClassesByStudIdReq = this.studentClassSvc.get(
                '/studentClasses/byStudentId/' + this.studentId.toString()
            );
            forkJoin([sClassesByStudIdReq]).subscribe(
                ([studentClasses]) => {
                    this.studentClasses = studentClasses;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    editItem(id: number, action = 'edit') {
        this.studentClassSvc.getById(id, '/studentClasses').subscribe(
            (res) => {
                let studentClassId = res.id;
                this.studentClass = new StudentClass(res);
                this.studentClass.id = studentClassId;
                this.studentClassFormComponent.setFormControls(
                    this.studentClass
                );
                this.studentClassFormComponent.action = action;
                this.studentClassFormComponent.studentClass = this.studentClass;
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
                this.studentClassSvc.delete('/studentClasses', id).subscribe(
                    (res) => {
                        this.loadStudentClasses();
                        this.toastr.success('Record deleted successfully!');
                    },
                    (err) => {
                        this.toastr.error(err?.error);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    AddStudentClass = (studentClass: StudentClass) => {
        Swal.fire({
            title: `${this.studentClassFormComponent.action == 'edit' ? 'Update' : 'Add'} Student class record?`,
            text: `Confirm if you want to ${
                this.studentClassFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } student class.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentClassFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentClass(studentClass);
                if (this.studentClassFormComponent.action == 'edit')
                    app.id = studentClass.id;
                let reqToProcess =
                    this.studentClassFormComponent.action == 'edit'
                        ? this.studentClassSvc.update('/studentClasses', app)
                        : this.studentClassSvc.create('/studentClasses', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.toastr.success('Student class saved successfully');
                        this.studentClassFormComponent.closeButton.nativeElement.click();
                        this.loadStudentClasses();
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
