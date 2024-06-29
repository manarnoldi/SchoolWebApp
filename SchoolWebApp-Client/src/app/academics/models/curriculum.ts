import { ResourceModel } from "@/core/models/ResourceModel";

export class Curriculum extends ResourceModel<Curriculum> {
    public name?: string;
    public code?: string;
    public description?: string;

    constructor(model?: Partial<Curriculum>) {
        super(model);
      }
}