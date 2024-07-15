import { ResourceModel } from "@/core/models/ResourceModel";
import { StaffDetails } from "./staff-details";
import { Outcome } from "@/settings/models/outcome";
import { OccurenceType } from "@/settings/models/occurence-type";

export class StaffDiscipline extends ResourceModel<StaffDiscipline> {
    public staffDetailsId?: number;
    public staffDetails?: StaffDetails;
    
    public outcomeDetails?: string;
    public occurenceDetails?: string;
    public occurenceStartDate?: Date;
    public occurenceEndDate?: Date;

    public outcomeId?: number;
    public outcome?: Outcome;
    public occurenceTypeId?: number;
    public occurenceType?: OccurenceType;

    constructor(model?: Partial<StaffDiscipline>) {
        super(model);
    }
}
