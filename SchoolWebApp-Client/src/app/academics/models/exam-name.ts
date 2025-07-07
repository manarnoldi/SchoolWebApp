import {ResourceModel} from '@/core/models/ResourceModel';
import { ExamType } from './exam-type';

export class ExamName extends ResourceModel<ExamName> {
    public name?: string;
    public examTypeId?: number;
    public examType?: ExamType;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<ExamName>) {
        super(model);
    }
}
