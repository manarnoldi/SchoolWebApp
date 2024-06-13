import { ResourceModel } from "@/core/models/ResourceModel";

export class OccurenceType extends ResourceModel<OccurenceType> {
    public name?: string;
    public description?: string;
    public abbreviation?: string;

    constructor(model?: Partial<OccurenceType>) {
        super(model);
      }
}