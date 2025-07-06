import {ResourceModel} from '@/core/models/ResourceModel';

export class ExamName extends ResourceModel<ExamName> {
    public name?: string;
    public examTypeId?: number;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<ExamName>) {
        super(model);
    }
}
