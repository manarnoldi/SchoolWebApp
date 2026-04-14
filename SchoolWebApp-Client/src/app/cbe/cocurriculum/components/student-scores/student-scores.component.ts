import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StudentCoCurriculumScore} from '../../models/student-co-curriculum-score';
import {StudentCoCurriculumScoreService} from '../../services/student-co-curriculum-score.service';
import {StudentCoCurriculumActivityService} from '../../services/student-co-curriculum-activity.service';
import {CoCurriculumScoreService} from '../../services/co-curriculum-score.service';
import {CoCurriculumScoreTypeService} from '../../services/co-curriculum-score-type.service';
import {CoCurriculumActivityService} from '../../services/co-curriculum-activity.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-student-scores',
    templateUrl: './student-scores.component.html',
    styleUrl: './student-scores.component.scss'
})
export class StudentCoCurriculumScoresComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/cocurriculum/student-scores'], title: 'CBE Co-curricular: Student Scores'}
    ];
    dashboardTitle = 'CBE Co-curricular: Student Scores';

    curricula: any[] = [];
    academicYears: any[] = [];
    schoolClasses: any[] = [];
    scoreTypes: any[] = [];
    allScores: any[] = [];
    activities: any[] = [];
    learningLevels: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSchoolClassId: any = null;
    filterActivityId: any = null;

    // Each row = one student, with a score column per score type
    batchRows: {
        studentId: number;
        studentName: string;
        studentActivityId: number;
        // scores keyed by scoreTypeId: { scoreId, description, existingId }
        scoresByType: { [scoreTypeId: number]: { scoreId: any; description: string; existingId: string | null } };
    }[] = [];

    batchLoaded: boolean = false;
    isSaving: boolean = false;

    constructor(
        private toastr: ToastrService,
        private studentScoreSvc: StudentCoCurriculumScoreService,
        private studentActivitySvc: StudentCoCurriculumActivityService,
        private scoreSvc: CoCurriculumScoreService,
        private scoreTypeSvc: CoCurriculumScoreTypeService,
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
            this.scoreTypeSvc.get('/coCurriculumScoreTypes'),
            this.scoreSvc.get('/coCurriculumScores'),
            this.activitySvc.get('/coCurriculumActivities')
        ]).subscribe({
            next: ([curricula, academicYears, scoreTypes, scores, activities]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.scoreTypes = scoreTypes.sort((a, b) => a.rank - b.rank);
                this.allScores = scores.sort((a, b) => a.rank - b.rank);
                this.activities = activities.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    getScoresForType = (scoreTypeId: number): any[] => {
        return this.allScores.filter((s) => s.coCurriculumScoreTypeId == scoreTypeId);
    };

    onCurriculumChange = () => {
        this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSchoolClassId = this.filterActivityId = null;
        this.batchLoaded = false;
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.schoolClasses = [];
        this.filterSchoolClassId = this.filterActivityId = null;
        this.batchLoaded = false;
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`).subscribe({
            next: (schoolClasses) => {
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadBatchScores = () => {
        if (!this.filterSchoolClassId || !this.filterActivityId) {
            this.toastr.info('Please select Class and Activity.');
            return;
        }

        this.batchRows = [];
        this.batchLoaded = false;

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

                // For each student, get their activity assignments
                let activityRequests = classStudents.map((student) =>
                    this.studentActivitySvc.get(`/studentCoCurriculumActivities/byStudentId/${student.id}`)
                );

                forkJoin(activityRequests).subscribe({
                    next: (allStudentActivities) => {
                        let studentsWithActivity: any[] = [];

                        classStudents.forEach((student, idx) => {
                            let match = allStudentActivities[idx].find(
                                (sa) => sa.coCurriculumActivityId == this.filterActivityId
                            );
                            if (match) {
                                studentsWithActivity.push({student, studentActivityId: +match.id});
                            }
                        });

                        if (studentsWithActivity.length === 0) {
                            this.toastr.info('No students in this class are assigned to this activity.');
                            return;
                        }

                        // Load all existing scores for each student's activity
                        let scoreRequests = studentsWithActivity.map((item) =>
                            this.studentScoreSvc.get(
                                `/studentCoCurriculumScores/byStudentCoCurriculumActivityId/${item.studentActivityId}`
                            )
                        );

                        forkJoin(scoreRequests).subscribe({
                            next: (allScores) => {
                                this.batchRows = studentsWithActivity.map((item, idx) => {
                                    let existingScores = allScores[idx];

                                    // Build scoresByType: for each score type, find the existing score
                                    let scoresByType: any = {};
                                    this.scoreTypes.forEach((st) => {
                                        let existing = existingScores.find((es) => {
                                            let matchedScore = this.allScores.find((s) => +s.id == es.coCurriculumScoreId);
                                            return matchedScore && matchedScore.coCurriculumScoreTypeId == st.id;
                                        });
                                        scoresByType[st.id] = {
                                            scoreId: existing ? existing.coCurriculumScoreId : null,
                                            description: existing ? (existing.description || '') : '',
                                            existingId: existing ? existing.id : null
                                        };
                                    });

                                    return {
                                        studentId: +item.student.id,
                                        studentName: `${item.student.upi || ''}-${item.student.fullName || ''}`,
                                        studentActivityId: item.studentActivityId,
                                        scoresByType: scoresByType
                                    };
                                });
                                this.batchLoaded = true;
                            },
                            error: (err) => this.toastr.error(err.error)
                        });
                    },
                    error: (err) => this.toastr.error(err.error)
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    saveAll = () => {
        // Collect all score entries across all rows and all score types
        let itemsToSave: any[] = [];
        this.batchRows.forEach((row) => {
            this.scoreTypes.forEach((st) => {
                let entry = row.scoresByType[st.id];
                if (entry && entry.scoreId != null) {
                    itemsToSave.push({
                        studentActivityId: row.studentActivityId,
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
            title: 'Save all scores?',
            text: `${itemsToSave.length} score(s) will be saved.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = itemsToSave.map((item) => {
                    let score = new StudentCoCurriculumScore({
                        studentCoCurriculumActivityId: item.studentActivityId,
                        coCurriculumScoreId: item.scoreId,
                        description: item.description
                    });
                    if (item.existingId) {
                        score.id = item.existingId;
                        return this.studentScoreSvc.update('/studentCoCurriculumScores', score);
                    } else {
                        return this.studentScoreSvc.create('/studentCoCurriculumScores', score);
                    }
                });

                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success('All scores saved!');
                        this.loadBatchScores();
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error saving.');
                    }
                );
            }
        });
    };

    hasAnyExisting = (row: any): boolean => {
        return this.scoreTypes.some((st) => row.scoresByType[st.id]?.existingId != null);
    };

    getSelectedActivityName = (): string => {
        let act = this.activities.find((a) => a.id == this.filterActivityId);
        return act?.name || '';
    };

    getExistingCount = (): number => {
        let count = 0;
        this.batchRows.forEach((row) => {
            this.scoreTypes.forEach((st) => {
                if (row.scoresByType[st.id]?.existingId != null) count++;
            });
        });
        return count;
    };

    deleteStudentScores = (row: any) => {
        let existingIds: string[] = [];
        this.scoreTypes.forEach((st) => {
            if (row.scoresByType[st.id]?.existingId != null) {
                existingIds.push(row.scoresByType[st.id].existingId);
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
                    this.studentScoreSvc.delete('/studentCoCurriculumScores', +id)
                );
                forkJoin(requests).subscribe(
                    () => {
                        this.toastr.success(`Scores for ${row.studentName} deleted.`);
                        this.scoreTypes.forEach((st) => {
                            row.scoresByType[st.id] = {scoreId: null, description: '', existingId: null};
                        });
                    },
                    (err) => this.toastr.error(err.error?.message || 'Error deleting.')
                );
            }
        });
    };

    deleteAll = () => {
        let allExistingIds: string[] = [];
        this.batchRows.forEach((row) => {
            this.scoreTypes.forEach((st) => {
                if (row.scoresByType[st.id]?.existingId != null) {
                    allExistingIds.push(row.scoresByType[st.id].existingId);
                }
            });
        });
        if (allExistingIds.length === 0) return;

        Swal.fire({
            title: 'Delete all scores?',
            text: `${allExistingIds.length} score(s) will be permanently deleted.`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete All', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = allExistingIds.map((id) =>
                    this.studentScoreSvc.delete('/studentCoCurriculumScores', +id)
                );
                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success(`${allExistingIds.length} score(s) deleted.`);
                        this.loadBatchScores();
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
