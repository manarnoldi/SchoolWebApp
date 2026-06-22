import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StudentCoCurriculumActivity} from '../../models/student-co-curriculum-activity';
import {StudentCoCurriculumActivityService} from '../../services/student-co-curriculum-activity.service';
import {CoCurriculumActivityService} from '../../services/co-curriculum-activity.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-student-assignments',
    templateUrl: './student-assignments.component.html',
    styleUrl: './student-assignments.component.scss'
})
export class StudentCoCurriculumAssignmentsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/cocurriculum/student-assignments'], title: 'CBE Co-curricular: Student Assignments'}
    ];
    dashboardTitle = 'CBE Co-curricular: Student Assignments';

    curricula: any[] = [];
    academicYears: any[] = [];
    schoolClasses: any[] = [];
    activities: any[] = [];
    learningLevels: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSchoolClassId: any = null;
    filterActivityId: any = null;

    // Each row = one student, with assigned checkbox and description
    assignmentRows: {
        studentId: number;
        studentName: string;
        assigned: boolean;
        description: string;
        existingId: string | null;
    }[] = [];

    studentsLoaded: boolean = false;
    isSaving: boolean = false;

    constructor(
        private toastr: ToastrService,
        private studentActivitySvc: StudentCoCurriculumActivityService,
        private activitySvc: CoCurriculumActivityService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private learningLevelSvc: LearningLevelsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.activitySvc.get('/coCurriculumActivities')
        ]).subscribe({
            next: ([curricula, academicYears, activities]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => a.rank - b.rank);
                this.activities = activities.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onCurriculumChange = () => {
        this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSchoolClassId = this.filterActivityId = null;
        this.studentsLoaded = false;
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.schoolClasses = [];
        this.filterSchoolClassId = this.filterActivityId = null;
        this.studentsLoaded = false;
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`).subscribe({
            next: (schoolClasses) => {
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId)).sort((a, b) => (a.rank || 0) - (b.rank || 0));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadStudents = () => {
        if (!this.filterSchoolClassId || !this.filterActivityId) {
            this.toastr.info('Please select Class and Activity.');
            return;
        }

        this.assignmentRows = [];
        this.studentsLoaded = false;

        this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active).subscribe({
            next: (studentClasses) => {
                let classStudents = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));

                if (classStudents.length === 0) {
                    this.toastr.info('No students found in this class.');
                    return;
                }

                // For each student, check if they are already assigned to this activity
                let activityRequests = classStudents.map((student) =>
                    this.studentActivitySvc.get(`/studentCoCurriculumActivities/byStudentId/${student.id}`)
                );

                forkJoin(activityRequests).subscribe({
                    next: (allStudentActivities) => {
                        this.assignmentRows = classStudents.map((student, idx) => {
                            let existing = allStudentActivities[idx].find(
                                (sa) => sa.coCurriculumActivityId == this.filterActivityId
                            );
                            return {
                                studentId: +student.id,
                                studentName: `${student.upi || ''}-${student.fullName || ''}`,
                                assigned: !!existing,
                                description: existing ? (existing.description || '') : '',
                                existingId: existing ? existing.id : null
                            };
                        });
                        this.studentsLoaded = true;
                    },
                    error: (err) => this.toastr.error(err.error)
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    selectAll = () => {
        let allAssigned = this.assignmentRows.every((r) => r.assigned);
        this.assignmentRows.forEach((r) => r.assigned = !allAssigned);
    };

    saveAll = () => {
        let toCreate = this.assignmentRows.filter((r) => r.assigned && !r.existingId);
        let toDelete = this.assignmentRows.filter((r) => !r.assigned && r.existingId);
        let toUpdate = this.assignmentRows.filter((r) => r.assigned && r.existingId);

        if (toCreate.length === 0 && toDelete.length === 0 && toUpdate.length === 0) {
            this.toastr.info('No changes to save.');
            return;
        }

        Swal.fire({
            title: 'Save assignments?',
            text: `${toCreate.length} new, ${toUpdate.length} updated, ${toDelete.length} removed.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = [];

                for (let row of toCreate) {
                    let item = new StudentCoCurriculumActivity({
                        studentId: row.studentId,
                        coCurriculumActivityId: this.filterActivityId,
                        description: row.description
                    });
                    requests.push(this.studentActivitySvc.create('/studentCoCurriculumActivities', item));
                }
                for (let row of toUpdate) {
                    let item = new StudentCoCurriculumActivity({
                        studentId: row.studentId,
                        coCurriculumActivityId: this.filterActivityId,
                        description: row.description
                    });
                    item.id = row.existingId;
                    requests.push(this.studentActivitySvc.update('/studentCoCurriculumActivities', item));
                }
                for (let row of toDelete) {
                    requests.push(this.studentActivitySvc.delete('/studentCoCurriculumActivities', +row.existingId));
                }

                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success('Assignments saved!');
                        this.loadStudents();
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error saving.');
                    }
                );
            }
        });
    };

    getAssignedCount = (): number => {
        return this.assignmentRows.filter((r) => r.assigned).length;
    };

    getSelectedActivityName = (): string => {
        let act = this.activities.find((a) => a.id == this.filterActivityId);
        return act?.name || '';
    };

    allSelected = (): boolean => {
        return this.assignmentRows.length > 0 && this.assignmentRows.every((r) => r.assigned);
    };
}
