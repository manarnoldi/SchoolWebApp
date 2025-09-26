import { ResourceModel } from "@/core/models/ResourceModel";

export class AssessmentType extends ResourceModel<AssessmentType> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<AssessmentType>) {
        super(model);
      }
}