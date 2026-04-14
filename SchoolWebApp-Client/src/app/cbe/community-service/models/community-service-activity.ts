import {ResourceModel} from '@/core/models/ResourceModel';

export class CommunityServiceActivity extends ResourceModel<CommunityServiceActivity> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<CommunityServiceActivity>) {
        super(model);
    }
}
