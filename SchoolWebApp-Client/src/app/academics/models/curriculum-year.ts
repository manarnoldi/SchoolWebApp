export class CurriculumYear {
    public curriculumId?: number;
    public academicYearId?: number;

    constructor(curriculumId?: number, academicYearId?: number) {
        this.curriculumId = curriculumId;
        this.academicYearId = academicYearId;
    }
}