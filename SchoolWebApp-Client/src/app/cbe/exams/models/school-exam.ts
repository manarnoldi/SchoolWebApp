import {ResourceModel} from '@/core/models/ResourceModel';

/**
 * The exam "event" header (type, term, schedule + release workflow).
 * The per-class-per-subject detail lives on {@link Exam}, which carries a
 * schoolExamId pointing back here.
 */
export class SchoolExam extends ResourceModel<SchoolExam> {
    public examStartDate?: string;
    public examEndDate?: string;
    public examMarkEntryEndDate?: string;
    public description?: string;
    public examTypeId?: number;
    public examType?: any;
    public sessionId?: number;
    public session?: any;

    // Release workflow
    public isReleased?: boolean;
    public releasedBy?: string;
    public releasedDate?: string;
    public parentsNotified?: boolean;
    public parentsNotifiedDate?: string;

    constructor(model?: Partial<SchoolExam>) {
        super(model);
    }
}
