import { ResourceModel } from "@/core/models/ResourceModel";
import { StudentDetails } from "./student-details";
import { SchoolClass } from "@/class/models/school-class";

export class StudentClass extends ResourceModel<StudentClass> {
    public description?: string;

    public studentId?: number;
    public student?: StudentDetails;

    public schoolClassId?: number;
    public schoolClass?: SchoolClass;

    constructor(model?: Partial<StudentClass>) {
        super(model);
    }
}