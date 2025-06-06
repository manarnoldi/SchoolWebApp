import {ResourceModel} from '@/core/models/ResourceModel';

export class SchoolStream extends ResourceModel<SchoolStream> {
    public name?: string;
    public description?: string;
    public abbreviation?: string;
    public rank?: number;
    
    constructor(model?: Partial<SchoolStream>) {
        super(model);
    }
}
