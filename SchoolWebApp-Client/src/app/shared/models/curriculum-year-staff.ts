export class CurriculumYearStaff {
    public curriculumId?: number;
    public academicYearId?: number;
    public staffCategoryId?: number;
    public employmentTypeId?: number;
    public learningModeId?: number;

    constructor(
        curriculumId?: number,
        academicYearId?: number,
        staffCategoryId?: number,
        employmentTypeId?: number,
        learningModeId?: number
    ) {
        this.curriculumId = curriculumId;
        this.academicYearId = academicYearId;
        this.staffCategoryId = staffCategoryId;
        this.employmentTypeId = employmentTypeId;
        this.learningModeId = learningModeId;
    }
}
