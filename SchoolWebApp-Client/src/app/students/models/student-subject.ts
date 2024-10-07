import {ResourceModel} from '@/core/models/ResourceModel';
import {StudentClass} from './student-class';
import {Subject} from '@/academics/models/subject';

export class StudentSubject extends ResourceModel<StudentSubject> {
    public description?: string;

    public studentClassId?: number;
    public studentClass?: StudentClass;

    public subjectId?: number;
    public subject?: Subject;

    constructor(model?: Partial<StudentSubject>) {
        super(model);
    }
}
