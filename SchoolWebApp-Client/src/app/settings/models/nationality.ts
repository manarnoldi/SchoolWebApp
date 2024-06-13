import { ResourceModel } from "@/core/models/ResourceModel";

export class Nationality extends ResourceModel<Nationality> {
    public name?: string;
    public description?: string;

    constructor(model?: Partial<Nationality>) {
        super(model);
      }
}