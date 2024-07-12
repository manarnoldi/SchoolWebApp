import { ResourceModel } from "@/core/models/ResourceModel";

export class ClassLeadershipRole extends ResourceModel<ClassLeadershipRole> {
    public name?: string;
    public description?: string;

    constructor(model?: Partial<ClassLeadershipRole>) {
        super(model);
      }
}