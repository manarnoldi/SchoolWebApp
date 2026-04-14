import {ResourceModel} from '@/core/models/ResourceModel';

export class Exam extends ResourceModel<Exam> {
    public examMark?: number;
    public examStartDate?: string;
    public examEndDate?: string;
    public examMarkEntryEndDate?: string;
    public description?: string;
    public examTypeId?: number;
    public examType?: any;
    public schoolClassId?: number;
    public schoolClass?: any;
    public sessionId?: number;
    public session?: any;
    public subjectId?: number;
    public subject?: any;

    constructor(model?: Partial<Exam>) {
        super(model);
    }
}
