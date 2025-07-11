import { ResourceModel } from "@/core/models/ResourceModel";

export class Designation extends ResourceModel<Designation> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<Designation>) {
        super(model);
      }
}