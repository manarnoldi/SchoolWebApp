import { ResourceModel } from "@/core/models/ResourceModel";

export class EducationLevelType extends ResourceModel<EducationLevelType> {
    public name?: string;
    public abbr?: string;
    public description?: string;

    constructor(model?: Partial<EducationLevelType>) {
        super(model);
      }
}