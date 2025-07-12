import { ResourceModel } from "@/core/models/ResourceModel";

export class EducationLevelType extends ResourceModel<EducationLevelType> {
    public name?: string;
    public rank?: number;
    public abbr?: string;
    public description?: string;

    constructor(model?: Partial<EducationLevelType>) {
        super(model);
      }
}