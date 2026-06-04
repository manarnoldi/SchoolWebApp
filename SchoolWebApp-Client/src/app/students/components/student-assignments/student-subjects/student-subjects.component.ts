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
import {ExamResultsService} from '@/academics/services/exam-results.service';
import {StudentClass} from '@/students/models/student-class';
import {StudentSubjectsLoadFormComponent} from './student-subjects-load-form/student-subjects-load-form.component';
import { EducationLevel } from '@/school/models/educationLevel';
import { EducationLevelSubjectService } from '@/academics/services/education-level-subject.service';

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
        private subjectsSvc: SubjectsService,
        private educationLevelSubjectsSvc: EducationLevelSubjectService,
        private examResultsSvc: ExamResultsService
    ) {}

    // Builds the "these results will also be deleted" block for a deallocation
    // dialog. Empty string when there are none.
    examResultsWarningHtml = (results: any[]): string => {
        if (!results || results.length === 0) return '';
        const fmtDate = (d: any) =>
            d
                ? new Date(d).toLocaleDateString('en-US', {
                      year: 'numeric',
                      month: 'short',
                      day: 'numeric'
                  })
                : '';
        const items = results
            .map(
                (r) =>
                    `<li>${r.exam?.examType?.name ?? 'Exam'} (${fmtDate(r.exam?.examStartDate)}) – ${r.exam?.subject?.name ?? ''}: <b>${r.score}</b></li>`
            )
            .join('');
        return (
            `<div class="text-danger mt-2"><strong>${results.length} exam result(s)</strong> are attached and will be permanently deleted too:</div>` +
            `<ul style="text-align:left; max-height:160px; overflow:auto;">${items}</ul>`
        );
    };

    ngOnInit(): void {
        this.loadStudentSubjects();
    }

    loadStudentSubjectsBySchoolClassId = (schoolClassId: number) => {
        if (schoolClassId == null || schoolClassId == undefined) {
            this.studentSubjects = [];
            return;
        }
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

    studentClassChanged = (studentClassId: number) => {
        if (!studentClassId) {
            this.studentSubjects = [];
            this.studentSubjectsFormComponent.showSubjectsTbl = false;
            this.currentStudentClass = null;
        } else {
            this.currentStudentClass = this.studentClasses.find(
                (sc) => sc.id == studentClassId.toString()
            );
            this.loadStudentSubjectsBySchoolClassId(
                this.currentStudentClass?.schoolClassId
            );
        }
        this.studentSubjectsFormComponent.studentSubjectsForm
            .get('studentClassId')
            ?.setValue(studentClassId);
        
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
        // Fetch any exam results attached to this allocation first so the user
        // sees what will be cascade-deleted. On error, fall back to a plain
        // confirm rather than blocking the delete.
        this.examResultsSvc.getByAllocationId(id).subscribe({
            next: (results) => this.confirmDeleteItem(id, results),
            error: () => this.confirmDeleteItem(id, [])
        });
    }

    private confirmDeleteItem(id: number, results: any[]) {
        Swal.fire({
            title: `Delete record?`,
            html: `Confirm if you want to delete this subject allocation.` + this.examResultsWarningHtml(results),
            width: 420,
            position: 'top',
            padding: '1em',
            icon: results && results.length > 0 ? 'warning' : 'question',
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
                            this.studentSubjectsLoadFormComponent.setFormControls(
                                parseInt(this.currentStudentClass?.id)
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
            title: `${this.studentSubjectsFormComponent.action == 'edit' ? 'Update' : 'Add'} Student subject record?`,
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
                
                this.studentSubjectsSvc
                    .createBatch(
                        '/studentSubjects/batch',
                        studentSubjects
                    )
                    .subscribe(
                        (res) => {
                            this.studentSubjectsFormComponent.action = 'add';
                            this.toastr.success(
                                'Student subjects saved successfully'
                            );
                            this.studentSubjectsFormComponent.closeButton.nativeElement.click();
                            this.loadStudentSubjects();
                            this.studentClassChanged(
                                studentSubjects[0]?.studentClassId
                            );
                            this.studentSubjectsLoadFormComponent.setFormControls(
                                studentSubjects[0]?.studentClassId
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
