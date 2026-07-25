import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import Swal from 'sweetalert2';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';
import {DirtyTracker} from '@/core/utils/dirty-tracker';
import {StudentCoCurriculumActivity} from '../../models/student-co-curriculum-activity';
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

    // Each row = one student in the class, with a score column per score type.
    // studentActivityId is null when the student hasn't been enrolled in the
    // activity yet; saveAll() auto-enrols them on demand before posting scores.
    batchRows: {
        studentId: number;
        studentName: string;
        studentActivityId: number | null;
        isEnrolled: boolean;
        // Activity-level remark (the enrolment record's description), shown on
        // the report form. originalRemark lets us save only changed remarks.
        remark: string;
        originalRemark: string;
        scoresByType: { [scoreTypeId: number]: { scoreId: any; description: string; existingId: string | null } };
    }[] = [];

    batchLoaded: boolean = false;
    isSaving: boolean = false;

    // Snapshots each saved score at load so saveAll() submits only new/changed
    // score cells (and only auto-enrols students who actually gained a new
    // score). Keyed by the score's DB id.
    private dirty = new DirtyTracker();

    // Client-side search + paging for the student-row table.
    searchText: string = '';
    tablePage: number = 1;
    tablePageSize: number = 30;

    // Search matches studentName (upi-fullName) case-insensitive substring.
    get filteredBatchRows() {
        const q = (this.searchText || '').trim().toLowerCase();
        if (!q) return this.batchRows;
        return this.batchRows.filter((r) => (r.studentName || '').toLowerCase().includes(q));
    }
    onSearchChanged = () => { this.tablePage = 1; this.syncUrl(); };
    onTablePageChanged = (p: number) => { this.tablePage = p; this.syncUrl(); };
    onTablePageSizeChanged = (s: number) => { this.tablePageSize = s; this.syncUrl(); };
    onSchoolClassPicked = () => { this.syncUrl(); };
    onActivityPicked = () => { this.syncUrl(); };

    // Persists current filter selections to the URL so a refresh / bookmark
    // restores the same view. Called from every filter handler.
    private syncUrl() {
        pushQueryParams(this.router, this.route, {
            curriculumId: this.filterCurriculumId,
            academicYearId: this.filterAcademicYearId,
            schoolClassId: this.filterSchoolClassId,
            activityId: this.filterActivityId,
            q: this.searchText,
            page: this.tablePage > 1 ? this.tablePage : null,
            pageSize: this.tablePageSize !== 30 ? this.tablePageSize : null,
            loaded: this.batchLoaded ? 1 : null
        });
    }

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
        private learningLevelSvc: LearningLevelsService,
        private route: ActivatedRoute,
        private router: Router
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
                // Restore previously-bookmarked / refreshed selections.
                this.restoreFromUrl();
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    // Reads filter state from query params and replays the cascade so the
    // grid populates the same way it did before refresh.
    private restoreFromUrl() {
        const curriculumId = readQueryParam(this.route, 'curriculumId');
        const academicYearId = readQueryParam(this.route, 'academicYearId');
        const schoolClassId = readQueryParam(this.route, 'schoolClassId');
        const activityId = readQueryParam(this.route, 'activityId');
        const q = readQueryParam(this.route, 'q');
        const page = readQueryParam(this.route, 'page');
        const pageSize = readQueryParam(this.route, 'pageSize');
        const loaded = readQueryParam(this.route, 'loaded');

        if (q) this.searchText = q;
        if (page) this.tablePage = +page;
        if (pageSize) this.tablePageSize = +pageSize;
        if (!curriculumId) return;

        // matchOptionId aligns the URL string id with the canonical option id
        // (sometimes number-typed) so the dropdown actually highlights.
        this.filterCurriculumId = matchOptionId(this.curricula, curriculumId);
        this.learningLevelSvc.getLearningLevelsByCurriculum(+curriculumId).subscribe({
            next: (levels) => {
                this.learningLevels = levels.sort((a, b) => a.rank - b.rank);
                if (!academicYearId) return;

                this.filterAcademicYearId = matchOptionId(this.academicYears, academicYearId);
                this.schoolClassesSvc
                    .get(`/schoolClasses/byAcademicYearId/${academicYearId}`)
                    .subscribe({
                        next: (schoolClasses) => {
                            const currLLIds = this.learningLevels.map((ll) => +ll.id);
                            this.schoolClasses = schoolClasses.filter((sc) =>
                                currLLIds.includes(+sc.learningLevelId)
                            ).sort((a, b) => (a.rank || 0) - (b.rank || 0));
                            if (schoolClassId) this.filterSchoolClassId = matchOptionId(this.schoolClasses, schoolClassId);
                            if (activityId) this.filterActivityId = matchOptionId(this.activities, activityId);

                            if (schoolClassId && activityId && loaded) {
                                this.loadBatchScores();
                            }
                        }
                    });
            }
        });
    }

    getScoresForType = (scoreTypeId: number): any[] => {
        return this.allScores.filter((s) => s.coCurriculumScoreTypeId == scoreTypeId);
    };

    onCurriculumChange = () => {
        this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSchoolClassId = this.filterActivityId = null;
        this.batchLoaded = false;
        this.syncUrl();
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
        this.syncUrl();
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`).subscribe({
            next: (schoolClasses) => {
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId)).sort((a, b) => (a.rank || 0) - (b.rank || 0));
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
                        // Build one row per student in the class, regardless of
                        // whether they've been enrolled in this activity yet.
                        // Rows without a studentActivityId render with empty
                        // score cells; if the user picks any score, saveAll()
                        // auto-enrols them before saving.
                        const studentRows = classStudents.map((student, idx) => {
                            const match = allStudentActivities[idx].find(
                                (sa) => sa.coCurriculumActivityId == this.filterActivityId
                            );
                            return {
                                student,
                                studentActivityId: match ? +match.id : null,
                                isEnrolled: !!match,
                                activityDescription: match?.description || ''
                            };
                        });

                        // Fetch existing scores only for already-enrolled students.
                        const enrolled = studentRows.filter((r) => r.studentActivityId != null);
                        const scoreRequests = enrolled.length
                            ? forkJoin(
                                  enrolled.map((r) =>
                                      this.studentScoreSvc.get(
                                          `/studentCoCurriculumScores/byStudentCoCurriculumActivityId/${r.studentActivityId}`
                                      )
                                  )
                              )
                            : of([] as any[][]);

                        scoreRequests.subscribe({
                            next: (allScores) => {
                                // Index existing scores by studentActivityId so non-enrolled
                                // students fall through to empty cells.
                                const scoresByActivityId = new Map<number, any[]>();
                                enrolled.forEach((r, i) => {
                                    if (r.studentActivityId != null) {
                                        scoresByActivityId.set(r.studentActivityId, allScores[i] || []);
                                    }
                                });

                                this.dirty.reset();
                                this.batchRows = studentRows.map((r) => {
                                    const existingScores =
                                        r.studentActivityId != null
                                            ? scoresByActivityId.get(r.studentActivityId) || []
                                            : [];

                                    const scoresByType: any = {};
                                    this.scoreTypes.forEach((st) => {
                                        const existing = existingScores.find((es: any) => {
                                            const matchedScore = this.allScores.find(
                                                (s) => +s.id == es.coCurriculumScoreId
                                            );
                                            return matchedScore && matchedScore.coCurriculumScoreTypeId == st.id;
                                        });
                                        scoresByType[st.id] = {
                                            scoreId: existing ? existing.coCurriculumScoreId : null,
                                            description: existing ? (existing.description || '') : '',
                                            existingId: existing ? existing.id : null
                                        };
                                        if (existing) {
                                            this.dirty.snapshot(existing.id, this.cellKey(existing.coCurriculumScoreId, existing.description));
                                        }
                                    });

                                    return {
                                        studentId: +r.student.id,
                                        studentName: `${r.student.upi || ''}-${r.student.fullName || ''}`,
                                        studentActivityId: r.studentActivityId,
                                        isEnrolled: r.isEnrolled,
                                        remark: r.activityDescription,
                                        originalRemark: r.activityDescription,
                                        scoresByType
                                    };
                                });
                                this.batchLoaded = true;
                                this.syncUrl();
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

    // Comparable value for change-detection. Field order must match the
    // snapshot taken at load. Ids coerced to string to avoid type drift.
    private cellKey = (scoreId: any, description: any) => ({
        scoreId: scoreId == null ? null : String(scoreId),
        description: description || ''
    });

    saveAll = () => {
        // Collect only score cells that are new (no existingId) or changed since
        // load. Items keep a reference to their parent row so we can patch in a
        // newly-created studentActivityId for auto-enrolment before posting.
        const itemsToSave: {
            row: any;
            scoreId: any;
            description: string;
            existingId: string | null;
        }[] = [];
        this.batchRows.forEach((row) => {
            this.scoreTypes.forEach((st) => {
                const entry = row.scoresByType[st.id];
                if (entry && entry.scoreId != null &&
                    this.dirty.hasChanged(entry.existingId, this.cellKey(entry.scoreId, entry.description))) {
                    itemsToSave.push({
                        row,
                        scoreId: entry.scoreId,
                        description: entry.description,
                        existingId: entry.existingId
                    });
                }
            });
        });

        // Rows whose activity-level remark changed since load.
        const remarkRows = this.batchRows.filter((r) => (r.remark || '') !== (r.originalRemark || ''));

        if (itemsToSave.length === 0 && remarkRows.length === 0) {
            this.toastr.info('No new or changed scores or remarks to save.');
            return;
        }

        // Distinct rows lacking a studentActivityId need to be enrolled first
        // (a score OR a remark implies participation in the activity).
        const rowsNeedingEnrolment = Array.from(new Set([
            ...itemsToSave.filter((i) => i.row.studentActivityId == null).map((i) => i.row),
            ...remarkRows.filter((r) => r.studentActivityId == null)
        ]));

        let parts: string[] = [];
        if (itemsToSave.length) parts.push(`${itemsToSave.length} score(s)`);
        if (remarkRows.length) parts.push(`${remarkRows.length} remark(s)`);
        const confirmText =
            `${parts.join(' and ')} will be saved.` +
            (rowsNeedingEnrolment.length > 0 ? ` ${rowsNeedingEnrolment.length} student(s) will be auto-enrolled in this activity.` : '');

        Swal.fire({
            title: 'Save co-curricular?',
            text: confirmText,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (!result.value) return;
            this.isSaving = true;

            // Phase 1: enrol students that don't yet have a
            // studentCoCurriculumActivity record, carrying their remark.
            const enrolReqs = rowsNeedingEnrolment.map((row) => {
                const enrol = new StudentCoCurriculumActivity({
                    studentId: row.studentId,
                    coCurriculumActivityId: this.filterActivityId,
                    description: row.remark || ''
                });
                return this.studentActivitySvc.create('/studentCoCurriculumActivities', enrol);
            });

            const enrolStream = enrolReqs.length > 0 ? forkJoin(enrolReqs) : of([] as any[]);

            enrolStream.subscribe({
                next: (enrolledResponses: any[]) => {
                    // Patch the new ids back so all items have a studentActivityId.
                    rowsNeedingEnrolment.forEach((row, idx) => {
                        row.studentActivityId = +enrolledResponses[idx].id;
                        row.isEnrolled = true;
                    });

                    // Phase 2a: save all new/changed scores in a single batch.
                    const scoreBatch = itemsToSave.map((item) => {
                        const score = new StudentCoCurriculumScore({
                            studentCoCurriculumActivityId: item.row.studentActivityId,
                            coCurriculumScoreId: item.scoreId,
                            description: item.description
                        });
                        if (item.existingId) score.id = item.existingId;
                        return score;
                    });

                    // Phase 2b: update the enrolment remark for rows that were
                    // ALREADY enrolled (just-enrolled rows already carry it via
                    // the create above).
                    const justEnrolled = new Set(rowsNeedingEnrolment);
                    const remarkUpdateReqs = remarkRows
                        .filter((r) => !justEnrolled.has(r) && r.studentActivityId != null)
                        .map((r) => {
                            const act = new StudentCoCurriculumActivity({
                                studentId: r.studentId,
                                coCurriculumActivityId: this.filterActivityId,
                                description: r.remark || ''
                            });
                            act.id = r.studentActivityId;
                            return this.studentActivitySvc.update('/studentCoCurriculumActivities', act);
                        });

                    const saveReqs: any[] = [];
                    if (scoreBatch.length) saveReqs.push(this.studentScoreSvc.createBatch('/studentCoCurriculumScores/batch', scoreBatch));
                    saveReqs.push(...remarkUpdateReqs);

                    (saveReqs.length ? forkJoin(saveReqs) : of([])).subscribe({
                        next: () => {
                            this.isSaving = false;
                            this.toastr.success('Co-curricular saved!');
                            this.loadBatchScores();
                        },
                        error: (err) => {
                            this.isSaving = false;
                            this.toastr.error(err.error?.message || err.error || 'Error saving co-curricular.');
                        }
                    });
                },
                error: (err) => {
                    this.isSaving = false;
                    this.toastr.error(err.error?.message || 'Error enrolling students.');
                }
            });
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
