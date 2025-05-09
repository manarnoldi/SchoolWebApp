
export class StudentAttendanceSearch {
    public curriculumId?: number;
    public educationLevelId?: number;
    public academicYearId?: number;
    public schoolClassId?: number;
    public attendanceDate?: Date;

    constructor(saSeach?: StudentAttendanceSearch) {
        if (saSeach) {
            Object.assign(this, saSeach);
        }
    }
}
