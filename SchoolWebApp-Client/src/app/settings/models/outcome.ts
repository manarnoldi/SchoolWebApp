import { ResourceModel } from "@/core/models/ResourceModel";

export class Outcome extends ResourceModel<Outcome> {
    public name?: string;
    public rank?: string;
    public description?: string;

    constructor(model?: Partial<Outcome>) {
        super(model);
      }
}