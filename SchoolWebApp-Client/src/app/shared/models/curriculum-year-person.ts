import { Status } from "@/core/enums/status";

export class CurriculumYearPerson {
    public curriculumId?: number;
    public educationLevelId?: number;
    public academicYearId?: number;
    public staffCategoryId?: number;
    public employmentTypeId?: number;
    public learningModeId?: number;
    public status?: Status;

    constructor(model?: CurriculumYearPerson) {
        Object.assign(this, model);
    }
}
