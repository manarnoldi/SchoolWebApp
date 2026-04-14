import {ResourceModel} from '@/core/models/ResourceModel';

export class StudentAssessment extends ResourceModel<StudentAssessment> {
    public studentId?: number;
    public student?: any;
    public schoolClassId?: number;
    public schoolClass?: any;
    public specificOutcomeId?: number;
    public specificOutcome?: any;
    public subStrandId?: number;
    public subStrand?: any;
    public strandId?: number;
    public strand?: any;
    public gradeId?: number;
    public grade?: any;
    public sessionId?: number;
    public session?: any;
    public assessmentTypeId?: number;
    public assessmentType?: any;
    public assessmentDate?: string;
    public staffDetailsId?: number;
    public staffDetails?: any;
    public description?: string;

    constructor(model?: Partial<StudentAssessment>) {
        super(model);
    }
}
