// Teaching documents: schemes of work, weekly lesson plans, record of work.

export type DocStatus = 'NotStarted' | 'Draft' | 'Submitted' | 'Returned' | 'Rejected' | 'Approved' | 'NotApplicable';

export const STATUS_LABELS: Record<DocStatus, string> = {
    NotStarted: 'Not started',
    Draft: 'Draft',
    Submitted: 'Submitted',
    Returned: 'Returned',
    Rejected: 'Rejected',
    Approved: 'Approved',
    NotApplicable: '-'
};

// Tracker colours: grey not started, amber draft, blue awaiting approval,
// red returned / rejected, green approved. Overdue adds a red outline.
export const STATUS_CLASSES: Record<DocStatus, string> = {
    NotStarted: 'td-notstarted',
    Draft: 'td-draft',
    Submitted: 'td-submitted',
    Returned: 'td-returned',
    Rejected: 'td-returned',
    Approved: 'td-approved',
    NotApplicable: 'td-na'
};

export const STATUS_BADGES: Record<DocStatus, string> = {
    NotStarted: 'bg-secondary',
    Draft: 'bg-warning text-dark',
    Submitted: 'bg-primary',
    Returned: 'bg-danger',
    Rejected: 'bg-danger',
    Approved: 'bg-success',
    NotApplicable: 'bg-light text-dark'
};

export enum LessonTaughtStatus {
    NotRecorded = 0,
    Taught = 1,
    PartlyTaught = 2,
    NotTaught = 3
}

export const TAUGHT_LABELS: Record<LessonTaughtStatus, string> = {
    [LessonTaughtStatus.NotRecorded]: 'Not recorded',
    [LessonTaughtStatus.Taught]: 'Taught',
    [LessonTaughtStatus.PartlyTaught]: 'Partly taught',
    [LessonTaughtStatus.NotTaught]: 'Not taught'
};

export interface TeachingSession {
    id: number;
    name: string;
    academicYearId: number;
    academicYearName: string;
    startDate: string;
    endDate: string;
    weeks: number;
    isCurrent: boolean;
}

export interface TeachingContext {
    userId: number;
    staffDetailsId: number | null;
    staffName: string | null;
    isAdmin: boolean;
    wholeSchool: boolean;
    hodDepartmentIds: number[];
    sessions: TeachingSession[];
    currentSessionId: number | null;
}

export interface TeachingAssignment {
    staffDetailsId: number;
    staffName: string;
    subjectId: number;
    subjectName: string;
    subjectRank: number;
    schoolClassId: number;
    className: string;
    learningLevelId: number;
    learningLevelName: string;
    learningLevelRank: number;
    departmentId: number;
    departmentName: string | null;
    schemeOfWorkId: number | null;
    schemeStatus: DocStatus;
    schemeLessons: number;
}

export interface Week {
    weekNo: number;
    startDate: string;
    endDate: string;
    dueAt: string;
}

export interface CurriculumSubStrand {
    id: number;
    name: string;
    code: string | null;
    lessons: number;
    specificOutcomes: string[];
    learningExperiences: string[];
    keyQuestions: string[];
    scheduledIn: string[];
}

export interface CurriculumStrand {
    id: number;
    name: string;
    code: string | null;
    subStrands: CurriculumSubStrand[];
}

export interface SchemeLesson {
    id: number;
    weekNo: number;
    lessonNo: number;
    strandId: number | null;
    subStrandId: number | null;
    strandName: string | null;
    subStrandName: string | null;
    learningOutcomes: string | null;
    learningExperiences: string | null;
    keyInquiryQuestions: string | null;
    learningResources: string | null;
    assessmentMethods: string | null;
}

export interface SchemeOfWork {
    id: number;
    staffDetailsId: number;
    staffName: string;
    subjectId: number;
    subjectName: string;
    schoolClassId: number;
    className: string;
    learningLevelId: number;
    learningLevelName: string;
    sessionId: number;
    sessionName: string;
    academicYearName: string;
    departmentId: number;
    references: string | null;
    numberOfWeeks: number;
    lessonsPerWeek: number;
    status: DocStatus;
    isLocked: boolean;
    canEdit: boolean;
    dueAt: string;
    weeks: Week[];
    lessons: SchemeLesson[];
    modified: string | null;
    modifiedBy: string | null;
}

export interface SchemeCopySource {
    id: number;
    staffName: string;
    className: string;
    sessionName: string;
    academicYearName: string;
    lessons: number;
    status: DocStatus;
}

export interface LessonPlanWeekSummary {
    weekNo: number;
    startDate: string;
    endDate: string;
    dueAt: string;
    lessonPlanWeekId: number | null;
    plans: number;
    schemeLessons: number;
    status: DocStatus;
    overdue: boolean;
}

export interface LessonPlanStep {
    id: number;
    stepNo: number;
    activity: string | null;
}

export interface LessonPlan {
    id: number;
    lessonPlanWeekId: number;
    schemeOfWorkLessonId: number | null;
    lessonNo: number;
    lessonDate: string | null;
    startTime: string | null;
    endTime: string | null;
    roll: number | null;
    strandId: number | null;
    subStrandId: number | null;
    strandName: string | null;
    subStrandName: string | null;
    learningOutcomes: string | null;
    keyInquiryQuestions: string | null;
    learningResources: string | null;
    organisationOfLearning: string | null;
    introduction: string | null;
    introductionMinutes: number | null;
    developmentMinutes: number | null;
    conclusion: string | null;
    conclusionMinutes: number | null;
    extendedActivities: string | null;
    summary: string | null;
    reflection: string | null;
    steps: LessonPlanStep[];
    taughtStatus: LessonTaughtStatus;
    taughtOn: string | null;
}

export interface LessonPlanWeek {
    id: number;
    weekNo: number;
    startDate: string;
    endDate: string;
    dueAt: string;
    status: DocStatus;
    isLocked: boolean;
    canEdit: boolean;
    scheme: SchemeOfWork;
    plans: LessonPlan[];
}

export interface RecordOfWorkRow {
    schemeOfWorkLessonId: number;
    weekNo: number;
    lessonNo: number;
    strandName: string | null;
    subStrandName: string | null;
    learningOutcomes: string | null;
    status: LessonTaughtStatus;
    taughtOn: string | null;
    workCovered: string | null;
    remarks: string | null;
    modified: string | null;
}

export interface RecordOfWorkCheck {
    id: number;
    weekNo: number;
    checkedByName: string;
    checkedByUserId: number;
    checkedAt: string;
    comment: string | null;
}

export interface RecordOfWork {
    scheme: SchemeOfWork;
    canCheck: boolean;
    rows: RecordOfWorkRow[];
    checks: RecordOfWorkCheck[];
    planned: number;
    taught: number;
    partlyTaught: number;
    notTaught: number;
}

export interface TrackerCell {
    id: number | null;
    status: DocStatus;
    overdue: boolean;
    awaitingName: string | null;
}

export interface TrackerRow {
    staffDetailsId: number;
    staffName: string;
    subjectId: number;
    subjectName: string;
    subjectRank: number;
    schoolClassId: number;
    className: string;
    learningLevelId: number;
    learningLevelName: string;
    learningLevelRank: number;
    departmentId: number;
    departmentName: string | null;
    scheme: TrackerCell;
    weeks: TrackerCell[];
    plannedLessons: number;
    taughtLessons: number;
    checkedWeeks: number;
}

export interface Tracker {
    session: TeachingSession;
    weeks: Week[];
    schemeDueAt: string;
    scope: 'school' | 'department' | 'own';
    rows: TrackerRow[];
}

/** "2026-08-24T00:00:00.000Z" -> "2026-08-24", for date inputs and the API. */
export function dateOnly(value: string | null | undefined): string | null {
    return value ? value.substring(0, 10) : null;
}

// ---------- Scheme workbook upload ----------

export interface ImportIssue {
    row: number | null;
    message: string;
}

export interface SchemeImportSheet {
    sheetName: string;
    staffName: string | null;
    subjectName: string | null;
    className: string | null;
    schemeOfWorkId: number | null;
    action: 'Create' | 'Update' | 'Skip' | 'Empty';
    rows: number;
    lessonsFilled: number;
    curriculumMatched: number;
    errors: ImportIssue[];
    warnings: ImportIssue[];
    saved: boolean;
}

export interface SchemeImportResult {
    committed: boolean;
    sheets: SchemeImportSheet[];
    fileErrors: string[];
}
