import {ResourceModel} from '@/core/models/ResourceModel';
import {ExamType} from './exam-type';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {Subject} from './subject';
import {ExamName} from './exam-name';

export class Exam extends ResourceModel<Exam> {
    public name?: string;
    public examMark?: number;
    public contributingMark?: number;

    public examTypeId?: number;
    public examType?: ExamType;

    public examNameId?: number;
    public examName?: ExamName;

    public schoolClassId?: number;
    public schoolClass?: SchoolClass;

    public sessionId?: number;
    public session?: Session;

    public subjectId?: number;
    public subject?: Subject;

    public examStartDate: Date;
    public examEndDate: Date;
    public examMarkEntryEndDate: Date;

    public otherDetails?: string;

    constructor(model?: Partial<Exam>) {
        super(model);
    }
}
