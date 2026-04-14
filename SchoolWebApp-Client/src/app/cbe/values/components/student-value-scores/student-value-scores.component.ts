import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StudentValueScore} from '../../models/student-value-score';
import {StudentValueScoreService} from '../../services/student-value-score.service';
import {ValueService} from '../../services/value.service';
import {ValueScoreService} from '../../services/value-score.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-student-value-scores',
    templateUrl: './student-value-scores.component.html',
    styleUrl: './student-value-scores.component.scss'
})
export class StudentValueScoresComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/values/student-assignments'], title: 'CBE Values: Student Assignments'}
    ];
    dashboardTitle = 'CBE Values: Student Assignments';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    values: any[] = [];
    valueScores: any[] = [];
    learningLevels: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;

    // Each row = one student, with a score per value
    studentRows: {
        studentId: number;
        studentName: string;
        scoresByValue: { [valueId: number]: { scoreId: any; description: string; existingId: string | null } };
    }[] = [];

    studentsLoaded: boolean = false;
    isSaving: boolean = false;

    constructor(
        private toastr: ToastrService,
        private studentValueScoreSvc: StudentValueScoreService,
        private valueSvc: ValueService,
        private valueScoreSvc: ValueScoreService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private learningLevelSvc: LearningLevelsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.valueSvc.get('/values'),
            this.valueScoreSvc.get('/valueScores')
        ]).subscribe({
            next: ([curricula, academicYears, values, valueScores]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.values = values.sort((a, b) => a.rank - b.rank);
                this.valueScores = valueScores.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.studentsLoaded = false;
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterSessionId = this.filterSchoolClassId = null;
        this.studentsLoaded = false;
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadStudents = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId) {
            this.toastr.info('Please select Session and Class.');
            return;
        }

        this.studentRows = [];
        this.studentsLoaded = false;

        forkJoin([
            this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active),
            this.studentValueScoreSvc.get(`/studentValueScores/bySessionId/${this.filterSessionId}`)
        ]).subscribe({
            next: ([studentClasses, allScores]) => {
                let classStudents = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));

                if (classStudents.length === 0) {
                    this.toastr.info('No students found in this class.');
                    return;
                }

                this.studentRows = classStudents.map((student) => {
                    let studentScores = allScores.filter((s) => s.studentId == +student.id);
                    let scoresByValue: any = {};
                    this.values.forEach((v) => {
                        let existing = studentScores.find((s) => s.valueId == +v.id);
                        scoresByValue[v.id] = {
                            scoreId: existing ? existing.valueScoreId : null,
                            description: existing ? (existing.description || '') : '',
                            existingId: existing ? existing.id : null
                        };
                    });
                    return {
                        studentId: +student.id,
                        studentName: `${student.upi || ''}-${student.fullName || ''}`,
                        scoresByValue: scoresByValue
                    };
                });
                this.studentsLoaded = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    hasAnyExisting = (row: any): boolean => {
        return this.values.some((v) => row.scoresByValue[v.id]?.existingId != null);
    };

    saveAll = () => {
        let itemsToSave: any[] = [];
        this.studentRows.forEach((row) => {
            this.values.forEach((v) => {
                let entry = row.scoresByValue[v.id];
                if (entry && entry.scoreId != null) {
                    itemsToSave.push({
                        studentId: row.studentId,
                        valueId: +v.id,
                        scoreId: entry.scoreId,
                        description: entry.description,
                        existingId: entry.existingId
                    });
                }
            });
        });

        if (itemsToSave.length === 0) {
            this.toastr.info('Please select at least one score.');
            return;
        }

        Swal.fire({
            title: 'Save all value scores?',
            text: `${itemsToSave.length} score(s) will be saved.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = itemsToSave.map((item) => {
                    let sv = new StudentValueScore({
                        studentId: item.studentId,
                        valueId: item.valueId,
                        sessionId: this.filterSessionId,
                        valueScoreId: item.scoreId,
                        description: item.description
                    });
                    if (item.existingId) {
                        sv.id = item.existingId;
                        return this.studentValueScoreSvc.update('/studentValueScores', sv);
                    } else {
                        return this.studentValueScoreSvc.create('/studentValueScores', sv);
                    }
                });

                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success('All value scores saved!');
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

    deleteStudentScores = (row: any) => {
        let existingIds: string[] = [];
        this.values.forEach((v) => {
            if (row.scoresByValue[v.id]?.existingId != null) {
                existingIds.push(row.scoresByValue[v.id].existingId);
            }
        });
        if (existingIds.length === 0) return;

        Swal.fire({
            title: 'Delete scores?',
            text: `Remove ${existingIds.length} score(s) for ${row.studentName}?`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                let requests = existingIds.map((id) =>
                    this.studentValueScoreSvc.delete('/studentValueScores', +id)
                );
                forkJoin(requests).subscribe(
                    () => {
                        this.toastr.success(`Scores for ${row.studentName} deleted.`);
                        this.values.forEach((v) => {
                            row.scoresByValue[v.id] = {scoreId: null, description: '', existingId: null};
                        });
                    },
                    (err) => this.toastr.error(err.error?.message || 'Error deleting.')
                );
            }
        });
    };

    getExistingCount = (): number => {
        let count = 0;
        this.studentRows.forEach((row) => {
            this.values.forEach((v) => {
                if (row.scoresByValue[v.id]?.existingId != null) count++;
            });
        });
        return count;
    };

    deleteAll = () => {
        let allExistingIds: string[] = [];
        this.studentRows.forEach((row) => {
            this.values.forEach((v) => {
                if (row.scoresByValue[v.id]?.existingId != null) {
                    allExistingIds.push(row.scoresByValue[v.id].existingId);
                }
            });
        });
        if (allExistingIds.length === 0) return;

        Swal.fire({
            title: 'Delete all value scores?',
            text: `${allExistingIds.length} score(s) will be permanently deleted.`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete All', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = allExistingIds.map((id) =>
                    this.studentValueScoreSvc.delete('/studentValueScores', +id)
                );
                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success(`${allExistingIds.length} score(s) deleted.`);
                        this.loadStudents();
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error deleting.');
                    }
                );
            }
        });
    };
}
