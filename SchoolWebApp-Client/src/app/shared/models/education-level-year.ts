export class EducationLevelYear {
    public educationLevelId?: number;
    public academicYearId?: number;   

    constructor(educationLevelYear?: EducationLevelYear) {
        if (educationLevelYear) {
            Object.assign(this, educationLevelYear);
        }
    }
}
