import { ResourceModel } from "@/core/models/ResourceModel";
import { StudentDetails } from "@/students/models/student-details";

export class StudentAttendancesReport extends ResourceModel<StudentAttendancesReport> {
    public studentId?: number;
    public student?: StudentDetails;
    public StudentClassId?: number;
    public month?: number;
    public year?: number;
    public day1?: string;
    public day2?: string;
    public day3?: string;
    public day4?: string;
    public day5?: string;
    public day6?: string;
    public day7?: string;
    public day8?: string;
    public day9?: string;
    public day10?: string;
    public day11?: string;
    public day12?: string;
    public day13?: string;
    public day14?: string;
    public day15?: string;
    public day16?: string;
    public day17?: string;
    public day18?: string;
    public day19?: string;
    public day20?: string;
    public day21?: string;
    public day22?: string;
    public day23?: string;
    public day24?: string;
    public day25?: string;
    public day26?: string;
    public day27?: string;
    public day28?: string;
    public day29?: string;
    public day30?: string;
    public day31?: string;

    constructor(model?: Partial<StudentAttendancesReport>) {
        super(model);
    }
}
