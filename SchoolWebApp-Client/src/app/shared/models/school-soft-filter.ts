import {Status} from '@/core/enums/status';

export class SchoolSoftFilter {
    public curriculumId?: number;
    public educationLevelId?: number;
    public academicYearId?: number;
    public sessionId?: number;
    public staffCategoryId?: number;
    public employmentTypeId?: number;
    public learningModeId?: number;

    public dateFrom?: Date;
    public dateTo?: Date;
    public month?: number;
    public year?: number;

    public studentClassId?: number;
    public schoolClassId?: number;
    public staffDetailsId?: number;
    public status?: Status;

    public subjectId?: number;
    public examTypeId?: number;
    public examId?: number;

    constructor(model?: SchoolSoftFilter) {
        Object.assign(this, model);
    }
}
