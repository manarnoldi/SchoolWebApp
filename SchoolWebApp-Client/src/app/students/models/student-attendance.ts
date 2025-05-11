import {ResourceModel} from '@/core/models/ResourceModel';
import { StudentClass } from './student-class';

export class StudentAttendance extends ResourceModel<StudentAttendance> {
    public studentClassId?: number;
    public studentClass?: StudentClass;
    public date?: Date;
    public present?: boolean;
    public remarks?: string;

    public timeIn?: string;
    public timeOut?: string;

    constructor(model?: Partial<StudentAttendance>) {
        super(model);
    }
}
