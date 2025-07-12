import { ResourceModel } from "@/core/models/ResourceModel";

export class LearningMode extends ResourceModel<LearningMode> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<LearningMode>) {
        super(model);
      }
}