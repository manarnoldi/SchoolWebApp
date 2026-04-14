import {ResourceModel} from '@/core/models/ResourceModel';

export class ValueScore extends ResourceModel<ValueScore> {
    public name?: string;
    public rank?: number;
    public abbreviation?: string;
    public description?: string;

    constructor(model?: Partial<ValueScore>) {
        super(model);
    }
}
