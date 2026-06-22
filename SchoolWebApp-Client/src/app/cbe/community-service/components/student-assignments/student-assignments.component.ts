import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {catchError} from 'rxjs/operators';
import Swal from 'sweetalert2';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';
import {StudentCommunityServiceActivity} from '../../models/student-community-service-activity';
import {StudentCommunityServiceActivityService} from '../../services/student-community-service-activity.service';
import {CommunityServiceActivityService} from '../../services/community-service-activity.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-community-service-student-assignments',
    templateUrl: './student-assignments.component.html',
    styleUrl: './student-assignments.component.scss'
})
export class CommunityServiceStudentAssignmentsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/community-service/student-assignments'], title: 'CBE Community Service: Student Assignments'}
    ];
    dashboardTitle = 'CBE Community Service: Student Assignments';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    activities: any[] = [];
    learningLevels: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;

    studentRows: {
        studentId: number;
        studentName: string;
        selectedActivityId: any;
        assignments: {
            activityId: number;
            activityName: string;
            description: string;
            existingId: string | null;
        }[];
    }[] = [];

    studentsLoaded: boolean = false;
    isSaving: boolean = false;

    // Client-side search + paging for the student-row table.
    searchText: string = '';
    tablePage: number = 1;
    tablePageSize: number = 30;

    get filteredStudentRows() {
        const q = (this.searchText || '').trim().toLowerCase();
        if (!q) return this.studentRows;
        return this.studentRows.filter((r) => (r.studentName || '').toLowerCase().includes(q));
    }
    onSearchChanged = () => { this.tablePage = 1; this.syncUrl(); };
    onTablePageChanged = (p: number) => { this.tablePage = p; this.syncUrl(); };
    onTablePageSizeChanged = (s: number) => { this.tablePageSize = s; this.syncUrl(); };
    onSessionPicked = () => { this.syncUrl(); };
    onSchoolClassPicked = () => { this.syncUrl(); };

    private syncUrl() {
        pushQueryParams(this.router, this.route, {
            curriculumId: this.filterCurriculumId,
            academicYearId: this.filterAcademicYearId,
            sessionId: this.filterSessionId,
            schoolClassId: this.filterSchoolClassId,
            q: this.searchText,
            page: this.tablePage > 1 ? this.tablePage : null,
            pageSize: this.tablePageSize !== 30 ? this.tablePageSize : null,
            loaded: this.studentsLoaded ? 1 : null
        });
    }

    constructor(
        private toastr: ToastrService,
        private studentActivitySvc: StudentCommunityServiceActivityService,
        private activitySvc: CommunityServiceActivityService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private learningLevelSvc: LearningLevelsService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.activitySvc.get('/communityServiceActivities')
        ]).subscribe({
            next: ([curricula, academicYears, activities]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.activities = activities.sort((a, b) => a.rank - b.rank);
                this.restoreFromUrl();
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    private restoreFromUrl() {
        const curriculumId = readQueryParam(this.route, 'curriculumId');
        const academicYearId = readQueryParam(this.route, 'academicYearId');
        const sessionId = readQueryParam(this.route, 'sessionId');
        const schoolClassId = readQueryParam(this.route, 'schoolClassId');
        const q = readQueryParam(this.route, 'q');
        const page = readQueryParam(this.route, 'page');
        const pageSize = readQueryParam(this.route, 'pageSize');
        const loaded = readQueryParam(this.route, 'loaded');

        if (q) this.searchText = q;
        if (page) this.tablePage = +page;
        if (pageSize) this.tablePageSize = +pageSize;
        if (!curriculumId) return;

        this.filterCurriculumId = matchOptionId(this.curricula, curriculumId);
        this.learningLevelSvc.getLearningLevelsByCurriculum(+curriculumId).subscribe({
            next: (levels) => {
                this.learningLevels = levels.sort((a, b) => a.rank - b.rank);
                if (!academicYearId) return;
                this.filterAcademicYearId = matchOptionId(this.academicYears, academicYearId);
                forkJoin([
                    this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${curriculumId}&academicYearId=${academicYearId}`),
                    this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${academicYearId}`)
                ]).subscribe({
                    next: ([sessions, schoolClasses]) => {
                        this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                        const currLLIds = this.learningLevels.map((ll) => +ll.id);
                        this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId)).sort((a, b) => (a.rank || 0) - (b.rank || 0));
                        if (sessionId) this.filterSessionId = matchOptionId(this.sessions, sessionId);
                        if (schoolClassId) this.filterSchoolClassId = matchOptionId(this.schoolClasses, schoolClassId);
                        if (sessionId && schoolClassId && loaded) this.loadStudents();
                    }
                });
            }
        });
    }

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.studentsLoaded = false;
        this.syncUrl();
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
        this.syncUrl();
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId)).sort((a, b) => (a.rank || 0) - (b.rank || 0));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadStudents = () => {
        if (!this.filterSchoolClassId || !this.filterSessionId || !this.filterAcademicYearId) {
            this.toastr.info('Please select Year, Session, and Class.');
            return;
        }

        this.studentRows = [];
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

                let assignmentRequests = classStudents.map((student) =>
                    this.studentActivitySvc.get(`/studentCommunityServiceActivities/byStudentId/${student.id}`)
                        .pipe(catchError(() => of([])))
                );

                forkJoin(assignmentRequests).subscribe({
                    next: (allStudentAssignments: any[][]) => {
                        this.studentRows = classStudents.map((student, idx) => {
                            let allForStudent = allStudentAssignments[idx] || [];
                            let relevant = allForStudent.filter((a: any) =>
                                a.sessionId == this.filterSessionId &&
                                a.academicYearId == this.filterAcademicYearId
                            );
                            let assignments = relevant.map((a: any) => {
                                let act = this.activities.find((x) => +x.id == +a.communityServiceActivityId);
                                return {
                                    activityId: a.communityServiceActivityId,
                                    activityName: act ? act.name : 'Unknown',
                                    description: a.description || '',
                                    existingId: a.id
                                };
                            });
                            return {
                                studentId: +student.id,
                                studentName: `${student.upi || ''}-${student.fullName || ''}`,
                                selectedActivityId: null,
                                assignments: assignments
                            };
                        });
                        this.studentsLoaded = true;
                        this.syncUrl();
                    },
                    error: (err) => this.toastr.error(err.error)
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    getAvailableActivities = (row: any): any[] => {
        let assignedIds = row.assignments.map((a: any) => +a.activityId);
        return this.activities.filter((act) => !assignedIds.includes(+act.id));
    };

    addItem = (row: any) => {
        if (!row.selectedActivityId) return;
        let act = this.activities.find((a) => +a.id == +row.selectedActivityId);
        if (!act) return;
        if (row.assignments.find((a: any) => +a.activityId == +act.id)) return;
        row.assignments.push({
            activityId: +act.id,
            activityName: act.name,
            description: '',
            existingId: null
        });
        row.selectedActivityId = null;
    };

    removeItem = (row: any, assignment: any) => {
        if (assignment.existingId) {
            Swal.fire({
                title: 'Remove?',
                text: `Remove "${assignment.activityName}" from ${row.studentName}?`,
                width: 400, position: 'top', padding: '1em', icon: 'warning',
                showCancelButton: true, confirmButtonText: 'Remove', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
            }).then((result) => {
                if (result.value) {
                    this.studentActivitySvc.delete('/studentCommunityServiceActivities', +assignment.existingId).subscribe({
                        next: () => {
                            row.assignments = row.assignments.filter((a: any) => a !== assignment);
                            this.toastr.success('Removed.');
                        },
                        error: (err) => this.toastr.error(err.error?.message || 'Error removing.')
                    });
                }
            });
        } else {
            row.assignments = row.assignments.filter((a: any) => a !== assignment);
        }
    };

    saveAll = () => {
        let newItems: any[] = [];
        let updateItems: any[] = [];
        this.studentRows.forEach((row) => {
            row.assignments.forEach((a) => {
                if (!a.existingId) {
                    newItems.push({studentId: row.studentId, activityId: a.activityId, description: a.description});
                } else {
                    updateItems.push({studentId: row.studentId, activityId: a.activityId, description: a.description, existingId: a.existingId});
                }
            });
        });

        if (newItems.length === 0 && updateItems.length === 0) {
            this.toastr.info('No changes to save.');
            return;
        }

        Swal.fire({
            title: 'Save all?',
            text: `${newItems.length} new, ${updateItems.length} updated.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = [];
                for (let item of newItems) {
                    let scsa = new StudentCommunityServiceActivity({
                        studentId: item.studentId,
                        communityServiceActivityId: item.activityId,
                        sessionId: this.filterSessionId,
                        academicYearId: this.filterAcademicYearId,
                        description: item.description
                    });
                    requests.push(this.studentActivitySvc.create('/studentCommunityServiceActivities', scsa));
                }
                for (let item of updateItems) {
                    let scsa = new StudentCommunityServiceActivity({
                        studentId: item.studentId,
                        communityServiceActivityId: item.activityId,
                        sessionId: this.filterSessionId,
                        academicYearId: this.filterAcademicYearId,
                        description: item.description
                    });
                    scsa.id = item.existingId;
                    requests.push(this.studentActivitySvc.update('/studentCommunityServiceActivities', scsa));
                }
                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success('All assignments saved!');
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

    getTotalAssignments = (): number => {
        return this.studentRows.reduce((sum, r) => sum + r.assignments.length, 0);
    };
}
