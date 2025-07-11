import { ResourceModel } from "@/core/models/ResourceModel";

export class Relationship extends ResourceModel<Relationship> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<Relationship>) {
        super(model);
      }
}