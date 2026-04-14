import {ResourceModel} from '@/core/models/ResourceModel';

export class SpecificOutcome extends ResourceModel<SpecificOutcome> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public subStrandId?: number;
    public subStrand?: any;
    public sessionId?: number;
    public session?: any;
    public broadOutcomeId?: number;
    public broadOutcome?: any;
    public generalOutcomeId?: number;
    public generalOutcome?: any;

    // Used by add/edit form for cascading filters
    public academicYearId?: number;
    public curriculumId?: number;
    public learningLevelId?: number;
    public subjectId?: number;
    public strandId?: number;

    constructor(model?: Partial<SpecificOutcome>) {
        super(model);
    }
}
