import { ResourceModel } from "@/core/models/ResourceModel";

export class ExamType extends ResourceModel<ExamType> {
    public name?: string;
    public description?: string;
    public abbreviation?: string;
    public featured?: string;

    constructor(model?: Partial<ExamType>) {
        super(model);
      }
}