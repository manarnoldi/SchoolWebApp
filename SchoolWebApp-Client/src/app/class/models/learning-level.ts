import { ResourceModel } from "@/core/models/ResourceModel";
import { EducationLevel } from "@/school/models/educationLevel";

export class LearningLevel extends ResourceModel<LearningLevel> {
    public name?: string;
    public rank?: number;
    public description?: string;
    public educationLevelId?: number;
    public educationLevel?: EducationLevel;

    constructor(model?: Partial<LearningLevel>) {
        super(model);
      }
}