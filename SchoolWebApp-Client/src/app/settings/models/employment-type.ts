import { ResourceModel } from "@/core/models/ResourceModel";

export class EmploymentType extends ResourceModel<EmploymentType> {
    public name?: string;
    public description?: string;

    constructor(model?: Partial<EmploymentType>) {
        super(model);
      }
}