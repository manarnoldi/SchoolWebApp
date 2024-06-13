import { ResourceModel } from "@/core/models/ResourceModel";

export class Gender extends ResourceModel<Gender> {
    public name?: string;
    public description?: string;

    constructor(model?: Partial<Gender>) {
        super(model);
      }
}