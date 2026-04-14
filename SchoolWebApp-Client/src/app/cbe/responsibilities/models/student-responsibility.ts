import {ResourceModel} from '@/core/models/ResourceModel';

export class StudentResponsibility extends ResourceModel<StudentResponsibility> {
    public academicYearId?: number;
    public academicYear?: any;
    public studentId?: number;
    public student?: any;
    public responsibilitySocialSkillId?: number;
    public responsibilitySocialSkill?: any;
    public description?: string;

    constructor(model?: Partial<StudentResponsibility>) {
        super(model);
    }
}
