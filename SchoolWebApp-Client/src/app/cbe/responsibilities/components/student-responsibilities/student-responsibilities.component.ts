import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';
import {StudentResponsibility} from '../../models/student-responsibility';
import {StudentResponsibilityService} from '../../services/student-responsibility.service';
import {ResponsibilityService} from '../../services/responsibility.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-student-responsibilities',
    templateUrl: './student-responsibilities.component.html',
    styleUrl: './student-responsibilities.component.scss'
})
export class StudentResponsibilitiesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/responsibilities/student-assignments'], title: 'CBE Responsibilities: Student Assignments'}
    ];
    dashboardTitle = 'CBE Responsibilities: Student Assignments';

    curricula: any[] = [];
    academicYears: any[] = [];
    schoolClasses: any[] = [];
    responsibilities: any[] = [];
    socialSkills: any[] = [];
    learningLevels: any[] = [];
    allItems: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSchoolClassId: any = null;

    studentRows: {
        studentId: number;
        studentName: string;
        selectedItemId: any;
        assignments: {
            itemId: number;
            itemName: string;
            itemType: string;
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
    onSchoolClassPicked = () => { this.syncUrl(); };

    // Persists current filter selections to the URL so refresh / bookmark
    // restores the same view.
    private syncUrl() {
        pushQueryParams(this.router, this.route, {
            curriculumId: this.filterCurriculumId,
            academicYearId: this.filterAcademicYearId,
            schoolClassId: this.filterSchoolClassId,
            q: this.searchText,
            page: this.tablePage > 1 ? this.tablePage : null,
            pageSize: this.tablePageSize !== 30 ? this.tablePageSize : null,
            loaded: this.studentsLoaded ? 1 : null
        });
    }

    constructor(
        private toastr: ToastrService,
        private studentResponsibilitySvc: StudentResponsibilityService,
        private responsibilitySvc: ResponsibilityService,
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
            this.responsibilitySvc.get('/responsibilities')
        ]).subscribe({
            next: ([curricula, academicYears, allResponsibilities]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.allItems = allResponsibilities.sort((a, b) => a.rank - b.rank);
                this.responsibilities = this.allItems.filter((r) => r.category === 'Responsibility');
                this.socialSkills = this.allItems.filter((r) => r.category === 'Social Skill');
                this.restoreFromUrl();
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    // Reads filter state from query params + replays the cascade so the grid
    // populates the same way it did before refresh.
    private restoreFromUrl() {
        const curriculumId = readQueryParam(this.route, 'curriculumId');
        const academicYearId = readQueryParam(this.route, 'academicYearId');
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
                this.schoolClassesSvc
                    .get(`/schoolClasses/byAcademicYearId/${academicYearId}`)
                    .subscribe({
                        next: (schoolClasses) => {
                            const currLLIds = this.learningLevels.map((ll) => +ll.id);
                            this.schoolClasses = schoolClasses.filter((sc) =>
                                currLLIds.includes(+sc.learningLevelId)
                            ).sort((a, b) => (a.rank || 0) - (b.rank || 0));
                            if (schoolClassId) this.filterSchoolClassId = matchOptionId(this.schoolClasses, schoolClassId);
                            if (schoolClassId && loaded) this.loadStudents();
                        }
                    });
            }
        });
    }

    onCurriculumChange = () => {
        this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSchoolClassId = null;
        this.studentsLoaded = false;
        this.syncUrl();
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.schoolClasses = [];
        this.filterSchoolClassId = null;
        this.studentsLoaded = false;
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

    loadStudents = () => {
        if (!this.filterSchoolClassId || !this.filterAcademicYearId) {
            this.toastr.info('Please select Year and Class.');
            return;
        }

        this.studentRows = [];
        this.studentsLoaded = false;

        forkJoin([
            this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active),
            this.studentResponsibilitySvc.get(`/studentResponsibilities/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([studentClasses, allAssignments]) => {
                let classStudents = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));

                if (classStudents.length === 0) {
                    this.toastr.info('No students found in this class.');
                    return;
                }

                this.studentRows = classStudents.map((student) => {
                    let studentAssignments = allAssignments.filter((a) => a.studentId == +student.id);
                    let assignments = studentAssignments.map((a) => {
                        let item = a.responsibilitySocialSkill;
                        return {
                            itemId: a.responsibilitySocialSkillId,
                            itemName: item ? item.name : 'Unknown',
                            itemType: item ? (item.category || '') : '',
                            description: a.description || '',
                            existingId: a.id
                        };
                    });
                    return {
                        studentId: +student.id,
                        studentName: `${student.upi || ''}-${student.fullName || ''}`,
                        selectedItemId: null,
                        assignments: assignments
                    };
                });
                this.studentsLoaded = true;
                this.syncUrl();
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    getAvailableItems = (row: any): any[] => {
        let assignedIds = row.assignments.map((a) => +a.itemId);
        return this.allItems.filter((item) => !assignedIds.includes(+item.id));
    };

    getAvailableResponsibilities = (row: any): any[] => {
        let assignedIds = row.assignments.map((a) => +a.itemId);
        return this.responsibilities.filter((r) => !assignedIds.includes(+r.id));
    };

    getAvailableSocialSkills = (row: any): any[] => {
        let assignedIds = row.assignments.map((a) => +a.itemId);
        return this.socialSkills.filter((s) => !assignedIds.includes(+s.id));
    };

    addItem = (row: any) => {
        if (!row.selectedItemId) return;
        let item = this.allItems.find((i) => +i.id == +row.selectedItemId);
        if (!item) return;
        if (row.assignments.find((a) => +a.itemId == +item.id)) return;
        row.assignments.push({
            itemId: +item.id,
            itemName: item.name,
            itemType: item.category || '',
            description: '',
            existingId: null
        });
        row.selectedItemId = null;
    };

    removeItem = (row: any, assignment: any) => {
        if (assignment.existingId) {
            Swal.fire({
                title: 'Remove?',
                text: `Remove "${assignment.itemName}" from ${row.studentName}?`,
                width: 400, position: 'top', padding: '1em', icon: 'warning',
                showCancelButton: true, confirmButtonText: 'Remove', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
            }).then((result) => {
                if (result.value) {
                    this.studentResponsibilitySvc.delete('/studentResponsibilities', +assignment.existingId).subscribe({
                        next: () => {
                            row.assignments = row.assignments.filter((a) => a !== assignment);
                            this.toastr.success('Removed.');
                        },
                        error: (err) => this.toastr.error(err.error?.message || 'Error removing.')
                    });
                }
            });
        } else {
            row.assignments = row.assignments.filter((a) => a !== assignment);
        }
    };

    saveAll = () => {
        let newItems: any[] = [];
        let updateItems: any[] = [];
        this.studentRows.forEach((row) => {
            row.assignments.forEach((a) => {
                if (!a.existingId) {
                    newItems.push({studentId: row.studentId, itemId: a.itemId, description: a.description});
                } else {
                    updateItems.push({studentId: row.studentId, itemId: a.itemId, description: a.description, existingId: a.existingId});
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
                    let sr = new StudentResponsibility({
                        academicYearId: this.filterAcademicYearId,
                        studentId: item.studentId,
                        responsibilitySocialSkillId: item.itemId,
                        description: item.description
                    });
                    requests.push(this.studentResponsibilitySvc.create('/studentResponsibilities', sr));
                }
                for (let item of updateItems) {
                    let sr = new StudentResponsibility({
                        academicYearId: this.filterAcademicYearId,
                        studentId: item.studentId,
                        responsibilitySocialSkillId: item.itemId,
                        description: item.description
                    });
                    sr.id = item.existingId;
                    requests.push(this.studentResponsibilitySvc.update('/studentResponsibilities', sr));
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
