import {ResourceModel} from '@/core/models/ResourceModel';
import {StudentDetails} from './student-details';
import {SchoolClass} from '@/class/models/school-class';

export class StudentClass extends ResourceModel<StudentClass> {
    public description?: string;

    public studentId?: number;
    public student?: StudentDetails;

    public schoolClassId?: number;
    public schoolClass?: SchoolClass;

    public isSelected?: boolean = false;
    public isOriginallySelected?: boolean = false;

    public remarks?: string = '';
    public hasRecord?: boolean = false;

    public timeIn? = { hour: 8, minute: 0 };
    public timeOut? = { hour: 17, minute: 0 };

    constructor(model?: Partial<StudentClass>) {
        super(model);
    }
}
