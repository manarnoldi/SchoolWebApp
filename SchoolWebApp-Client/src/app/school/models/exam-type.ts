import { ResourceModel } from "@/core/models/ResourceModel";

export class ExamType extends ResourceModel<ExamType> {
    public name?: string;
    public description?: string;

    constructor(model?: Partial<ExamType>) {
        super(model);
      }
}