import { ResourceModel } from "@/core/models/ResourceModel";

export class SessionType extends ResourceModel<SessionType> {
    public name?: string;
    public rank?: number;
    public description?: string;
    public abbreviation?: string;

    constructor(model?: Partial<SessionType>) {
        super(model);
      }
}