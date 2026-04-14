import {ResourceModel} from '@/core/models/ResourceModel';

export class ExamType extends ResourceModel<ExamType> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public abbreviation?: string;
    public internal?: boolean;

    constructor(model?: Partial<ExamType>) {
        super(model);
    }
}
