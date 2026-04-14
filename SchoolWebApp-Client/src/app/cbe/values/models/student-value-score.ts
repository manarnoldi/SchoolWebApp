import {ResourceModel} from '@/core/models/ResourceModel';

export class StudentValueScore extends ResourceModel<StudentValueScore> {
    public studentId?: number;
    public student?: any;
    public valueId?: number;
    public value?: any;
    public sessionId?: number;
    public session?: any;
    public valueScoreId?: number;
    public valueScore?: any;
    public description?: string;

    constructor(model?: Partial<StudentValueScore>) {
        super(model);
    }
}
