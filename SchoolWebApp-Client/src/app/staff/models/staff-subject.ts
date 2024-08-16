import { ResourceModel } from "@/core/models/ResourceModel";
import { StaffDetails } from "./staff-details";
import { Subject } from "@/academics/models/subject";
import { SchoolClass } from "@/class/models/school-class";

export class StaffSubject extends ResourceModel<StaffSubject> {
    public staffDetailsId?: number;
    public staffDetails?: StaffDetails;
    
    public description?: string;

    public subjectId?: number;
    public subject?: Subject;
    public schoolClassId?: number;
    public schoolClass?: SchoolClass;

    constructor(model?: Partial<StaffSubject>) {
        super(model);
    }
}