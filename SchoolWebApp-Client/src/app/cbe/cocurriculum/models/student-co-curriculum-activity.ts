import {ResourceModel} from '@/core/models/ResourceModel';

export class StudentCoCurriculumActivity extends ResourceModel<StudentCoCurriculumActivity> {
    public studentId?: number;
    public student?: any;
    public coCurriculumActivityId?: number;
    public coCurriculumActivity?: any;
    public description?: string;

    constructor(model?: Partial<StudentCoCurriculumActivity>) {
        super(model);
    }
}
