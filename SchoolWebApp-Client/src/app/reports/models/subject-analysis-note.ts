// The teacher-authored action plan persisted for a Subject Performance Trend
// report (one per year + class + subject). Everything else on the report is
// recomputed on load; only this note is saved.
export interface SubjectAnalysisNote {
    id?: number;
    academicYearId: number;
    schoolClassId: number;
    subjectId: number;
    // The school exam (and therefore term) the plan is written for.
    schoolExamId: number;
    actionPlan?: string | null;
}
