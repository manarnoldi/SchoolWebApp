import {ResourceModel} from '@/core/models/ResourceModel';

export class Value extends ResourceModel<Value> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<Value>) {
        super(model);
    }
}
