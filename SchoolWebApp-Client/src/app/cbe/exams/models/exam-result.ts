import {ResourceModel} from '@/core/models/ResourceModel';

export class ExamResult extends ResourceModel<ExamResult> {
    public score?: number;
    public description?: string;
    public studentId?: number;
    public student?: any;
    public examId?: number;
    public exam?: any;

    constructor(model?: Partial<ExamResult>) {
        super(model);
    }
}
