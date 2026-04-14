import {ResourceModel} from '@/core/models/ResourceModel';

export class Responsibility extends ResourceModel<Responsibility> {
    public name?: string;
    public rank?: number;
    public description?: string;
    public category?: string;

    constructor(model?: Partial<Responsibility>) {
        super(model);
    }
}
