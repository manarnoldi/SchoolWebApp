import {ResourceModel} from '@/core/models/ResourceModel';

export class ExamSearch {
    public examTypeId?: number;
    public schoolClassId?: number;
    public sessionId?: number;
    public subjectId?: number;
    public academicYearId?: number;
    public curriculumId?: number;
    public educationLevelId?: number;

    constructor(examSeach?: ExamSearch) {
        if (examSeach) {
            Object.assign(this, examSeach);
        }
    }
}
