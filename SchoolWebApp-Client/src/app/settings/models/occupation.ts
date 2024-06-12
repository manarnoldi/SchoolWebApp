import { ResourceModel } from "@/core/models/ResourceModel";

export class Occupation extends ResourceModel<Occupation> {
    public name?: string;
    public description?: string;

    constructor(model?: Partial<Occupation>) {
        super(model);
      }
}