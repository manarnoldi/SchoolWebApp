import { ResourceModel } from "@/core/models/ResourceModel";

export class StaffCategory extends ResourceModel<StaffCategory> {
    public name?: string;
    public code?: string;
    public rank?: number;
    public description?: string;
    public abbreviation?: string;

    constructor(model?: Partial<StaffCategory>) {
        super(model);
      }
}