import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StudentAssessment} from '../../models/student-assessment';
import {StudentAssessmentService} from '../../services/student-assessment.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {AssessmentTypeService} from '../../services/assessment-type.service';
import {SpecificOutcomeService} from '../../services/specific-outcome.service';
import {SubStrandService} from '../../services/sub-strand.service';
import {StrandService} from '../../services/strand.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {ThemeService} from '../../services/theme.service';
import {Status} from '@/core/enums/status';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-student-assessments',
    templateUrl: './student-assessments.component.html',
    styleUrl: './student-assessments.component.scss'
})
export class StudentAssessmentsComponent implements OnInit {
    isAuthLoading: boolean;
    querySource: string = '';

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/assessments/assessments'], title: 'Student Assessments'}
    ];
    dashboardTitle = 'Student Assessments';

    // Dropdown data
    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    subjects: any[] = [];
    themes: any[] = [];
    strands: any[] = [];
    subStrands: any[] = [];
    specificOutcomes: any[] = [];
    assessmentTypes: any[] = [];
    grades: any[] = [];
    teachers: any[] = [];
    learningLevels: any[] = [];
    gradingCategory: string = '4-Point';

    // Filter selections
    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;
    filterSubjectId: any = null;
    filterThemeId: any = null;
    filterStrandId: any = null;
    filterSubStrandId: any = null;
    filterSpecificOutcomeId: any = null;
    filterAssessmentTypeId: any = null;
    filterStaffDetailsId: any = null;
    filterAssessmentDate: string = new Date().toISOString().split('T')[0];

    // Batch grading rows
    gradingRows: {
        studentId: number;
        upi: string;
        fullName: string;
        gradeId: any;
        description: string;
        existingId: string | null;
    }[] = [];

    // Sub-strand assessment rows (auto-computed grades)
    subStrandRows: {
        studentId: number;
        upi: string;
        fullName: string;
        gradeId: any;
        gradeName: string;
        description: string;
        existingId: string | null;
    }[] = [];

    // Strand assessment rows (auto-computed grades)
    strandRows: {
        studentId: number;
        upi: string;
        fullName: string;
        gradeId: any;
        gradeName: string;
        description: string;
        existingId: string | null;
    }[] = [];

    // Bulk assessment grid (rows=students, columns=specific outcomes)
    bulkRows: {
        studentId: number;
        upi: string;
        fullName: string;
        outcomes: { [outcomeId: number]: { gradeId: any; existingId: string | null } };
    }[] = [];
    bulkOutcomes: any[] = []; // specific outcomes used as columns
    bulkLoaded: boolean = false;
    isSavingBulk: boolean = false;
    isDeletingBulk: boolean = false;

    subStrandLoaded: boolean = false;
    strandLoaded: boolean = false;
    studentsLoaded: boolean = false;
    isSaving: boolean = false;
    isSavingSubStrand: boolean = false;
    isSavingStrand: boolean = false;

    constructor(
        private toastr: ToastrService,
        private studentAssessmentSvc: StudentAssessmentService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private assessmentTypeSvc: AssessmentTypeService,
        private specificOutcomeSvc: SpecificOutcomeService,
        private subStrandSvc: SubStrandService,
        private strandSvc: StrandService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService,
        private staffDetailsSvc: StaffDetailsService,
        private studentClassSvc: StudentClassService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private learningLevelSvc: LearningLevelsService,
        private subjectsSvc: SubjectsService,
        private themeSvc: ThemeService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.querySource = this.route.snapshot.queryParamMap.get('source') || '';
        this.refreshItems();
    }

    refreshItems() {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.assessmentTypeSvc.get('/assessmentTypes'),
            this.gradesSvc.get('/grades'),
            this.staffDetailsSvc.get('/staffDetails/byStaffCategoryId/2'),
            this.globalSettingSvc.getByKey('Grading', 'StudentAssessment')
        ]).subscribe({
            next: ([curricula, academicYears, assessmentTypes, allGrades, teachers, gradingSetting]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.assessmentTypes = assessmentTypes;
                let settingResponse = gradingSetting as any;
                this.gradingCategory = settingResponse?.settingValue || '4-Point';
                this.grades = allGrades.filter(g => g.category === this.gradingCategory).sort((a, b) => a.rank - b.rank);
                this.teachers = teachers;
                this.isAuthLoading = false;
            },
            error: (err) => {
                this.toastr.error('An error occurred while loading data.');
                this.isAuthLoading = false;
            }
        });
    }

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.subjects = this.strands = this.subStrands = this.specificOutcomes = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = this.filterSubjectId = null;
        this.filterStrandId = this.filterSubStrandId = this.filterSpecificOutcomeId = null;
        this.studentsLoaded = false;
        if (!this.filterCurriculumId) return;

        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => {
                this.learningLevels = levels.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = this.subjects = this.strands = this.subStrands = this.specificOutcomes = [];
        this.filterSessionId = this.filterSchoolClassId = this.filterSubjectId = this.filterStrandId = null;
        this.filterSubStrandId = this.filterSpecificOutcomeId = null;
        this.studentsLoaded = false;
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;

        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                // Filter classes whose learning level belongs to the selected curriculum
                let currLearningLevelIds = this.learningLevels.map((ll) => parseInt(ll.id));
                this.schoolClasses = schoolClasses.filter(
                    (sc) => currLearningLevelIds.includes(parseInt(sc.learningLevelId))
                );
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onSessionChange = () => {
        this.studentsLoaded = false;
    };

    onClassChange = () => {
        this.subjects = this.strands = this.subStrands = this.specificOutcomes = [];
        this.filterSubjectId = this.filterStrandId = this.filterSubStrandId = this.filterSpecificOutcomeId = null;
        this.studentsLoaded = false;
        this.bulkLoaded = false;
        if (!this.filterSchoolClassId) return;

        // Load only subjects allocated to students in this class
        this.subjectsSvc.get(
            `/studentSubjects/subjectsBySchoolClassId/${this.filterSchoolClassId}`
        ).subscribe({
            next: (subjects) => {
                this.subjects = subjects.sort((a, b) => (a.rank || 0) - (b.rank || 0));
                if (this.subjects.length === 0) {
                    this.toastr.info('No subjects allocated to students in this class.');
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onSubjectChange = () => {
        this.themes = this.strands = this.subStrands = this.specificOutcomes = [];
        this.filterThemeId = this.filterStrandId = this.filterSubStrandId = this.filterSpecificOutcomeId = null;
        this.studentsLoaded = false;
        this.bulkLoaded = false;
        if (!this.filterSubjectId || !this.filterSchoolClassId) return;

        let selectedClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
        if (!selectedClass || !selectedClass.learningLevelId) return;

        // Load both themes and strands for the subject+grade
        forkJoin([
            this.themeSvc.get(`/themes/bySubjectId?subjectId=${this.filterSubjectId}&learningLvlId=${selectedClass.learningLevelId}`),
            this.strandSvc.get(`/strands/bySubjectId?subjectId=${this.filterSubjectId}&learningLvlId=${selectedClass.learningLevelId}&academicYearId=${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([themes, strands]) => {
                this.themes = themes.sort((a, b) => a.rank - b.rank);
                this.strands = strands.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onThemeChange = () => {
        this.subStrands = this.specificOutcomes = [];
        this.filterStrandId = this.filterSubStrandId = this.filterSpecificOutcomeId = null;
        this.studentsLoaded = false;
        this.bulkLoaded = false;
        if (!this.filterThemeId) return;

        // Filter strands by theme (if theme selected, show only strands under that theme)
        let selectedClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
        if (!selectedClass || !selectedClass.learningLevelId) return;

        this.strandSvc.get(
            `/strands/bySubjectId?subjectId=${this.filterSubjectId}&learningLvlId=${selectedClass.learningLevelId}&academicYearId=${this.filterAcademicYearId}`
        ).subscribe({
            next: (strands) => {
                // Filter strands that belong to the selected theme (or have no theme)
                this.strands = strands
                    .filter((s) => s.themeId == this.filterThemeId || !s.themeId)
                    .sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onStrandChange = () => {
        this.subStrands = this.specificOutcomes = [];
        this.filterSubStrandId = this.filterSpecificOutcomeId = null;
        this.studentsLoaded = false;
        if (!this.filterStrandId) return;

        this.subStrandSvc.get(`/subStrands/byStrandId/${this.filterStrandId}`).subscribe({
            next: (subStrands) => {
                this.subStrands = subStrands.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onSubStrandChange = () => {
        this.specificOutcomes = [];
        this.filterSpecificOutcomeId = null;
        this.studentsLoaded = false;
        if (!this.filterSubStrandId) return;

        this.specificOutcomeSvc.get(`/specificOutcomes/bySubStrandId/${this.filterSubStrandId}`).subscribe({
            next: (outcomes) => {
                this.specificOutcomes = outcomes.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadStudents = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId ||
            !this.filterAssessmentTypeId || !this.filterSpecificOutcomeId ||
            !this.filterStaffDetailsId) {
            this.toastr.info('Please select Session, Class, Assessment Type, Specific Outcome and Teacher.');
            return;
        }

        this.gradingRows = [];
        this.studentsLoaded = false;

        let studentsReq = this.studentClassSvc.getBySchoolClassId(
            this.filterSchoolClassId, Status.Active
        );
        let existingReq = this.studentAssessmentSvc.get(
            `/studentAssessments/bySessionIdAndParams/${this.filterSessionId}?schoolClassId=${this.filterSchoolClassId}&assessmentTypeId=${this.filterAssessmentTypeId}&specificOutcomeId=${this.filterSpecificOutcomeId}`
        );

        forkJoin([studentsReq, existingReq]).subscribe({
            next: ([studentClasses, existingAssessments]) => {
                let students = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));

                this.gradingRows = students.map((student) => {
                    let existing = existingAssessments.find(
                        (ea) => ea.studentId == parseInt(student.id)
                    );
                    return {
                        studentId: parseInt(student.id),
                        upi: student.upi || '',
                        fullName: student.fullName || '',
                        gradeId: existing ? existing.gradeId : null,
                        description: existing ? (existing.description || '') : '',
                        existingId: existing ? existing.id : null
                    };
                });

                this.studentsLoaded = true;

                if (this.gradingRows.length === 0) {
                    this.toastr.info('No students found in the selected class.');
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    saveAll = () => {
        let rowsToSave = this.gradingRows.filter((r) => r.gradeId != null);
        if (rowsToSave.length === 0) {
            this.toastr.info('Please assign at least one grade before saving.');
            return;
        }

        Swal.fire({
            title: 'Save all assessments?',
            text: `${rowsToSave.length} student assessment(s) will be saved.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Save',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = rowsToSave.map((row) => {
                    let sa = new StudentAssessment({
                        studentId: row.studentId,
                        schoolClassId: this.filterSchoolClassId,
                        specificOutcomeId: this.filterSpecificOutcomeId,
                        gradeId: row.gradeId,
                        sessionId: this.filterSessionId,
                        assessmentTypeId: this.filterAssessmentTypeId,
                        assessmentDate: this.filterAssessmentDate || null,
                        staffDetailsId: this.filterStaffDetailsId,
                        description: row.description
                    });

                    if (row.existingId) {
                        sa.id = row.existingId;
                        return this.studentAssessmentSvc.update('/studentAssessments', sa);
                    } else {
                        return this.studentAssessmentSvc.create('/studentAssessments', sa);
                    }
                });

                forkJoin(requests).subscribe(
                    (res) => {
                        this.isSaving = false;
                        this.toastr.success(`${rowsToSave.length} assessment(s) saved successfully!`);
                        this.loadStudents();
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error saving assessments.');
                    }
                );
            }
        });
    };

    getGradedCount = (): number => {
        return this.gradingRows.filter((r) => r.gradeId != null).length;
    };

    getAveragePoints = (): number => {
        let gradedRows = this.gradingRows.filter((r) => r.gradeId != null);
        if (gradedRows.length === 0) return 0;
        let totalPoints = gradedRows.reduce((sum, row) => {
            let grade = this.grades.find((g) => g.id == row.gradeId);
            return sum + (grade ? grade.points : 0);
        }, 0);
        return Math.round(totalPoints / gradedRows.length);
    };

    getAverageGrade = (): any => {
        let avgPoints = this.getAveragePoints();
        // Find the grade whose points match the rounded average
        let match = this.grades.find((g) => g.points == avgPoints);
        if (match) return match;
        // If no exact match, find the closest grade
        return this.grades.reduce((closest, g) =>
            Math.abs(g.points - avgPoints) < Math.abs(closest.points - avgPoints) ? g : closest
        , this.grades[0]);
    };

    // Compute the closest grade for a given average points value
    findGradeByPoints = (avgPoints: number): any => {
        if (this.grades.length === 0) return null;
        let match = this.grades.find((g) => g.points == avgPoints);
        if (match) return match;
        return this.grades.reduce((closest, g) =>
            Math.abs(g.points - avgPoints) < Math.abs(closest.points - avgPoints) ? g : closest
        , this.grades[0]);
    };

    // Load Sub-Strand assessment: compute average of all specific outcomes under the selected sub-strand per student
    loadSubStrandAssessment = () => {
        if (!this.filterSubStrandId || !this.filterSessionId || !this.filterSchoolClassId ||
            !this.filterAssessmentTypeId) return;

        // Get all specific outcomes under this sub-strand
        forkJoin([
            this.specificOutcomeSvc.get(`/specificOutcomes/bySubStrandId/${this.filterSubStrandId}`),
            this.studentAssessmentSvc.get(
                `/studentAssessments/bySessionIdAndParams/${this.filterSessionId}?schoolClassId=${this.filterSchoolClassId}&assessmentTypeId=${this.filterAssessmentTypeId}`
            ),
            this.studentAssessmentSvc.get(
                `/studentAssessments/bySessionIdAndParams/${this.filterSessionId}?schoolClassId=${this.filterSchoolClassId}&assessmentTypeId=${this.filterAssessmentTypeId}&subStrandId=${this.filterSubStrandId}`
            )
        ]).subscribe({
            next: ([specificOutcomes, allAssessments, existingSubStrandAssessments]) => {
                let soIds = specificOutcomes.map((so) => parseInt(so.id));
                // Filter assessments to only those for specific outcomes in this sub-strand
                let relevantAssessments = allAssessments.filter(
                    (a) => a.specificOutcomeId && soIds.includes(a.specificOutcomeId)
                );

                this.subStrandRows = this.gradingRows.map((student) => {
                    let studentAssessments = relevantAssessments.filter(
                        (a) => a.studentId == student.studentId
                    );
                    let avgPoints = 0;
                    let computedGrade: any = null;
                    let gradeName = '';
                    let gradeId: any = null;

                    if (studentAssessments.length > 0) {
                        let totalPoints = studentAssessments.reduce((sum, a) => {
                            let grade = this.grades.find((g) => g.id == a.gradeId);
                            return sum + (grade ? grade.points : 0);
                        }, 0);
                        avgPoints = Math.round(totalPoints / studentAssessments.length);
                        computedGrade = this.findGradeByPoints(avgPoints);
                        if (computedGrade) {
                            gradeId = computedGrade.id;
                            gradeName = `${computedGrade.name} - ${computedGrade.abbr} (${computedGrade.points})`;
                        }
                    }

                    // Check for existing sub-strand assessment
                    let existing = existingSubStrandAssessments.find(
                        (ea) => ea.studentId == student.studentId && ea.subStrandId == this.filterSubStrandId
                    );

                    return {
                        studentId: student.studentId,
                        upi: student.upi,
                        fullName: student.fullName,
                        gradeId: gradeId,
                        gradeName: gradeName,
                        description: existing ? (existing.description || '') : '',
                        existingId: existing ? existing.id : null
                    };
                });
                this.subStrandLoaded = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    saveSubStrandAssessments = () => {
        let rowsToSave = this.subStrandRows.filter((r) => r.gradeId != null);
        if (rowsToSave.length === 0) {
            this.toastr.info('No computed grades to save.');
            return;
        }
        this.isSavingSubStrand = true;
        let requests = rowsToSave.map((row) => {
            let sa = new StudentAssessment({
                studentId: row.studentId,
                schoolClassId: this.filterSchoolClassId,
                subStrandId: this.filterSubStrandId,
                gradeId: row.gradeId,
                sessionId: this.filterSessionId,
                assessmentTypeId: this.filterAssessmentTypeId,
                assessmentDate: this.filterAssessmentDate || null,
                staffDetailsId: this.filterStaffDetailsId,
                description: row.description
            });
            if (row.existingId) {
                sa.id = row.existingId;
                return this.studentAssessmentSvc.update('/studentAssessments', sa);
            } else {
                return this.studentAssessmentSvc.create('/studentAssessments', sa);
            }
        });
        forkJoin(requests).subscribe(
            () => {
                this.isSavingSubStrand = false;
                this.toastr.success('Sub-strand assessments saved!');
                this.loadSubStrandAssessment();
            },
            (err) => {
                this.isSavingSubStrand = false;
                this.toastr.error(err.error?.message || 'Error saving sub-strand assessments.');
            }
        );
    };

    // Load Strand assessment: compute average of all specific outcomes under all sub-strands of the selected strand
    loadStrandAssessment = () => {
        if (!this.filterStrandId || !this.filterSessionId || !this.filterSchoolClassId ||
            !this.filterAssessmentTypeId) return;

        forkJoin([
            this.subStrandSvc.get(`/subStrands/byStrandId/${this.filterStrandId}`),
            this.studentAssessmentSvc.get(
                `/studentAssessments/bySessionIdAndParams/${this.filterSessionId}?schoolClassId=${this.filterSchoolClassId}&assessmentTypeId=${this.filterAssessmentTypeId}`
            ),
            this.studentAssessmentSvc.get(
                `/studentAssessments/bySessionIdAndParams/${this.filterSessionId}?schoolClassId=${this.filterSchoolClassId}&assessmentTypeId=${this.filterAssessmentTypeId}&strandId=${this.filterStrandId}`
            )
        ]).subscribe({
            next: ([subStrands, allAssessments, existingStrandAssessments]) => {
                let ssIds = subStrands.map((ss) => parseInt(ss.id));
                // Get all specific outcome assessments where the specific outcome belongs to sub-strands of this strand
                let relevantAssessments = allAssessments.filter((a) => {
                    if (!a.specificOutcomeId || !a.specificOutcome) return false;
                    return ssIds.includes(a.specificOutcome.subStrandId);
                });

                // Also include sub-strand level assessments for sub-strands under this strand
                let subStrandAssessments = allAssessments.filter(
                    (a) => a.subStrandId && ssIds.includes(a.subStrandId)
                );

                this.strandRows = this.gradingRows.map((student) => {
                    // Prefer specific outcome assessments for computing strand average
                    let studentAssessments = relevantAssessments.filter(
                        (a) => a.studentId == student.studentId
                    );

                    // If no specific outcome assessments, fall back to sub-strand assessments
                    if (studentAssessments.length === 0) {
                        studentAssessments = subStrandAssessments.filter(
                            (a) => a.studentId == student.studentId
                        );
                    }

                    let avgPoints = 0;
                    let computedGrade: any = null;
                    let gradeName = '';
                    let gradeId: any = null;

                    if (studentAssessments.length > 0) {
                        let totalPoints = studentAssessments.reduce((sum, a) => {
                            let grade = this.grades.find((g) => g.id == a.gradeId);
                            return sum + (grade ? grade.points : 0);
                        }, 0);
                        avgPoints = Math.round(totalPoints / studentAssessments.length);
                        computedGrade = this.findGradeByPoints(avgPoints);
                        if (computedGrade) {
                            gradeId = computedGrade.id;
                            gradeName = `${computedGrade.name} - ${computedGrade.abbr} (${computedGrade.points})`;
                        }
                    }

                    let existing = existingStrandAssessments.find(
                        (ea) => ea.studentId == student.studentId && ea.strandId == this.filterStrandId
                    );

                    return {
                        studentId: student.studentId,
                        upi: student.upi,
                        fullName: student.fullName,
                        gradeId: gradeId,
                        gradeName: gradeName,
                        description: existing ? (existing.description || '') : '',
                        existingId: existing ? existing.id : null
                    };
                });
                this.strandLoaded = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    saveStrandAssessments = () => {
        let rowsToSave = this.strandRows.filter((r) => r.gradeId != null);
        if (rowsToSave.length === 0) {
            this.toastr.info('No computed grades to save.');
            return;
        }
        this.isSavingStrand = true;
        let requests = rowsToSave.map((row) => {
            let sa = new StudentAssessment({
                studentId: row.studentId,
                schoolClassId: this.filterSchoolClassId,
                strandId: this.filterStrandId,
                gradeId: row.gradeId,
                sessionId: this.filterSessionId,
                assessmentTypeId: this.filterAssessmentTypeId,
                assessmentDate: this.filterAssessmentDate || null,
                staffDetailsId: this.filterStaffDetailsId,
                description: row.description
            });
            if (row.existingId) {
                sa.id = row.existingId;
                return this.studentAssessmentSvc.update('/studentAssessments', sa);
            } else {
                return this.studentAssessmentSvc.create('/studentAssessments', sa);
            }
        });
        forkJoin(requests).subscribe(
            () => {
                this.isSavingStrand = false;
                this.toastr.success('Strand assessments saved!');
                this.loadStrandAssessment();
            },
            (err) => {
                this.isSavingStrand = false;
                this.toastr.error(err.error?.message || 'Error saving strand assessments.');
            }
        );
    };

    deleteAssessment = (row: any) => {
        Swal.fire({
            title: 'Delete assessment?',
            text: `Remove assessment for ${row.fullName}?`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel',
            confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.studentAssessmentSvc.delete('/studentAssessments', parseInt(row.existingId)).subscribe({
                    next: () => {
                        this.toastr.success(`Assessment for ${row.fullName} deleted.`);
                        row.existingId = null;
                        row.gradeId = null;
                        row.description = '';
                    },
                    error: (err) => {
                        this.toastr.error(err.error?.message || 'Error deleting assessment.');
                    }
                });
            }
        });
    };

    // ============================================
    // Bulk Assessment Grid
    // ============================================

    loadBulkAssessment = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId ||
            !this.filterAssessmentTypeId || !this.filterStaffDetailsId ||
            !this.filterSubStrandId) {
            this.toastr.info('Please select Session, Class, Assessment Type, Teacher and Sub-Strand.');
            return;
        }

        this.bulkLoaded = false;
        this.bulkRows = [];
        this.bulkOutcomes = [];

        forkJoin([
            this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active),
            this.specificOutcomeSvc.get(`/specificOutcomes/bySubStrandId/${this.filterSubStrandId}`),
            this.studentAssessmentSvc.get(
                `/studentAssessments/bySessionIdAndParams/${this.filterSessionId}?schoolClassId=${this.filterSchoolClassId}&assessmentTypeId=${this.filterAssessmentTypeId}`
            )
        ]).subscribe({
            next: ([studentClasses, outcomes, allAssessments]) => {
                this.bulkOutcomes = outcomes.sort((a, b) => a.rank - b.rank);

                if (this.bulkOutcomes.length === 0) {
                    this.toastr.info('No specific outcomes found for the selected sub-strand.');
                    return;
                }

                let students = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));

                let outcomeIds = this.bulkOutcomes.map((o) => parseInt(o.id));
                let relevantAssessments = allAssessments.filter(
                    (a) => a.specificOutcomeId && outcomeIds.includes(a.specificOutcomeId)
                );

                this.bulkRows = students.map((student) => {
                    let outcomes: { [outcomeId: number]: { gradeId: any; existingId: string | null } } = {};
                    for (let outcome of this.bulkOutcomes) {
                        let existing = relevantAssessments.find(
                            (a) => a.studentId == parseInt(student.id) && a.specificOutcomeId == parseInt(outcome.id)
                        );
                        outcomes[outcome.id] = {
                            gradeId: existing ? existing.gradeId : null,
                            existingId: existing ? existing.id : null
                        };
                    }
                    return {
                        studentId: parseInt(student.id),
                        upi: student.upi || '',
                        fullName: student.fullName || '',
                        outcomes: outcomes
                    };
                });

                this.bulkLoaded = true;

                if (this.bulkRows.length === 0) {
                    this.toastr.info('No students found in the selected class.');
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    saveBulkAssessments = () => {
        let requests: any[] = [];
        let count = 0;

        for (let row of this.bulkRows) {
            for (let outcome of this.bulkOutcomes) {
                let cell = row.outcomes[outcome.id];
                if (cell.gradeId == null) continue;

                let sa = new StudentAssessment({
                    studentId: row.studentId,
                    schoolClassId: this.filterSchoolClassId,
                    specificOutcomeId: parseInt(outcome.id),
                    gradeId: cell.gradeId,
                    sessionId: this.filterSessionId,
                    assessmentTypeId: this.filterAssessmentTypeId,
                    assessmentDate: this.filterAssessmentDate || null,
                    staffDetailsId: this.filterStaffDetailsId,
                    description: ''
                });

                if (cell.existingId) {
                    sa.id = cell.existingId;
                    requests.push(this.studentAssessmentSvc.update('/studentAssessments', sa));
                } else {
                    requests.push(this.studentAssessmentSvc.create('/studentAssessments', sa));
                }
                count++;
            }
        }

        if (requests.length === 0) {
            this.toastr.info('Please assign at least one grade before saving.');
            return;
        }

        Swal.fire({
            title: 'Save bulk assessments?',
            text: `${count} assessment(s) will be saved.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Save',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSavingBulk = true;
                forkJoin(requests).subscribe(
                    () => {
                        this.isSavingBulk = false;
                        this.toastr.success(`${count} assessment(s) saved successfully!`);
                        this.loadBulkAssessment();
                    },
                    (err) => {
                        this.isSavingBulk = false;
                        this.toastr.error(err.error?.message || 'Error saving bulk assessments.');
                    }
                );
            }
        });
    };

    getBulkGradedCount = (): number => {
        let count = 0;
        for (let row of this.bulkRows) {
            for (let outcome of this.bulkOutcomes) {
                if (row.outcomes[outcome.id]?.gradeId != null) count++;
            }
        }
        return count;
    };

    getBulkSavedCount = (): number => {
        let count = 0;
        for (let row of this.bulkRows) {
            for (let outcome of this.bulkOutcomes) {
                if (row.outcomes[outcome.id]?.existingId != null) count++;
            }
        }
        return count;
    };

    getOutcomeAvg = (outcomeId: number): string => {
        let graded = this.bulkRows.filter((r) => r.outcomes[outcomeId]?.gradeId != null);
        if (graded.length === 0) return '-';
        let totalPoints = graded.reduce((sum, r) => {
            let grade = this.grades.find((g) => g.id == r.outcomes[outcomeId].gradeId);
            return sum + (grade ? grade.points : 0);
        }, 0);
        let avgPoints = Math.round(totalPoints / graded.length);
        let grade = this.findGradeByPoints(avgPoints);
        return grade ? grade.abbr : '-';
    };

    applyGradeToAll = (outcomeId: number, gradeId: any) => {
        for (let row of this.bulkRows) {
            if (row.outcomes[outcomeId].gradeId == null) {
                row.outcomes[outcomeId].gradeId = gradeId;
            }
        }
    };

    deleteBulkCell = (row: any, outcomeId: number) => {
        let cell = row.outcomes[outcomeId];
        if (!cell?.existingId) return;
        this.studentAssessmentSvc.delete('/studentAssessments', parseInt(cell.existingId)).subscribe({
            next: () => {
                cell.existingId = null;
                cell.gradeId = null;
                this.toastr.success(`Assessment deleted for ${row.fullName}.`);
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error deleting assessment.')
        });
    };

    deleteAllSavedAssessments = () => {
        let savedCells: { row: any; outcomeId: number; existingId: string }[] = [];
        for (let row of this.bulkRows) {
            for (let oc of this.bulkOutcomes) {
                let cell = row.outcomes[oc.id];
                if (cell?.existingId) {
                    savedCells.push({ row, outcomeId: oc.id, existingId: cell.existingId });
                }
            }
        }
        if (savedCells.length === 0) return;

        Swal.fire({
            title: 'Delete all saved assessments?',
            text: `${savedCells.length} assessment(s) will be permanently deleted.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete All',
            cancelButtonText: 'Cancel',
            confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.isDeletingBulk = true;
                let requests = savedCells.map((c) =>
                    this.studentAssessmentSvc.delete('/studentAssessments', parseInt(c.existingId))
                );
                forkJoin(requests).subscribe(
                    () => {
                        this.isDeletingBulk = false;
                        this.toastr.success(`${savedCells.length} assessment(s) deleted.`);
                        this.loadBulkAssessment();
                    },
                    (err) => {
                        this.isDeletingBulk = false;
                        this.toastr.error(err.error?.message || 'Error deleting assessments.');
                    }
                );
            }
        });
    };
}
