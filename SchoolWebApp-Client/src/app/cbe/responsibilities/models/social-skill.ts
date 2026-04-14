import {ResourceModel} from '@/core/models/ResourceModel';

export class SocialSkill extends ResourceModel<SocialSkill> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<SocialSkill>) {
        super(model);
    }
}
