import {ResourceModel} from '@/core/models/ResourceModel';

export class BroadOutcome extends ResourceModel<BroadOutcome> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public educationLevelId?: number;
    public educationLevel?: any;
    public subjectId?: number;
    public subject?: any;

    constructor(model?: Partial<BroadOutcome>) {
        super(model);
    }
}
