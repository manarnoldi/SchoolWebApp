import { ResourceModel } from "@/core/models/ResourceModel";

export class Religion extends ResourceModel<Religion> {
    public name?: string;
    public description?: string;

    constructor(model?: Partial<Religion>) {
        super(model);
      }
}