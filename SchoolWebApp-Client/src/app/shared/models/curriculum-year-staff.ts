export class CurriculumYearStaff {
    public curriculumId?: number;
    public academicYearId?: number;
    public staffCategoryId?: number;
    public employmentTypeId?: number;
    public learningModeId?: number;

    constructor(model?: CurriculumYearStaff) {
        Object.assign(this, model);
    }
}
