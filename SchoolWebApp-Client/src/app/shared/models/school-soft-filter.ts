import { Status } from "@/core/enums/status";

export class SchoolSoftFilter {
    public curriculumId?: number;
    public educationLevelId?: number;
    public academicYearId?: number;
    public staffCategoryId?: number;
    public employmentTypeId?: number;
    public learningModeId?: number;

    public dateFrom?: Date;
    public dateTo?: Date;
    public month?: number;
    public year?: number;

    public studentClassId?: number;
    public staffDetailsId?: number;
    public status?: Status;

    constructor(model?: SchoolSoftFilter) {
        Object.assign(this, model);
    }
}
