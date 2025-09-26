import { ResourceModel } from "@/core/models/ResourceModel";

export class Competency extends ResourceModel<Competency> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<Competency>) {
        super(model);
      }
}