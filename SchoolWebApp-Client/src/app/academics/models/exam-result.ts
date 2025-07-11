import {ResourceModel} from '@/core/models/ResourceModel';
import {StudentDetails} from '@/students/models/student-details';
import {Exam} from './exam';
import {StudentSubject} from '@/students/models/student-subject';

export class ExamResult extends ResourceModel<ExamResult> {
    public score?: number;

    public studentId?: number;
    public student?: StudentDetails;

    public examId?: number;
    public exam?: Exam;

    constructor(model?: Partial<ExamResult>) {
        super(model);
    }
}
