import { ResourceModel } from "@/core/models/ResourceModel";
import { StaffDetails } from "@/staff/models/staff-details";

export class Department extends ResourceModel<Department> {
    public name?: string;
    public code?: string;
    public description?: string;
    public staffDetailsId?: number;
    public staffDetails?: StaffDetails;

    constructor(model?: Partial<Department>) {
        super(model);
    }
}