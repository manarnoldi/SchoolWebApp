// One row per student: the subject codes they are missing marks for,
// across the registered exams for the selection.
export interface MissingMarksStudent {
    className: string;
    classRank: number;
    upi: string;
    studentName: string;
    subjectCodes: string[];
}
