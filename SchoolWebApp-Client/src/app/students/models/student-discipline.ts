import { ResourceModel } from "@/core/models/ResourceModel";
import { Outcome } from "@/settings/models/outcome";
import { OccurenceType } from "@/settings/models/occurence-type";
import { StudentDetails } from "./student-details";

export class StudentDiscipline extends ResourceModel<StudentDiscipline> {
    public studentId?: number;
    public student?: StudentDetails;
    
    public outcomeDetails?: string;
    public occurenceDetails?: string;
    public occurenceStartDate?: Date;
    public occurenceEndDate?: Date;

    public outcomeId?: number;
    public outcome?: Outcome;
    public occurenceTypeId?: number;
    public occurenceType?: OccurenceType;

    constructor(model?: Partial<StudentDiscipline>) {
        super(model);
    }
}
