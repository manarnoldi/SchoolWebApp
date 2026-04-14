import {ResourceModel} from '@/core/models/ResourceModel';

export class StudentCommunityServiceActivity extends ResourceModel<StudentCommunityServiceActivity> {
    public studentId?: number;
    public student?: any;
    public communityServiceActivityId?: number;
    public communityServiceActivity?: any;
    public sessionId?: number;
    public session?: any;
    public academicYearId?: number;
    public academicYear?: any;
    public description?: string;

    constructor(model?: Partial<StudentCommunityServiceActivity>) {
        super(model);
    }
}
