export class StaffAttendanceSearch {
    public staffCategoryId?: number;
    public employmentTypeId?: number;
    public attendanceDate?: Date;

    constructor(saSeach?: StaffAttendanceSearch) {
        if (saSeach) {
            Object.assign(this, saSeach);
        }
    }
}
