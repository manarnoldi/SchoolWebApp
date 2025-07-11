import { ResourceModel } from "@/core/models/ResourceModel";

export class Occupation extends ResourceModel<Occupation> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<Occupation>) {
        super(model);
      }
}