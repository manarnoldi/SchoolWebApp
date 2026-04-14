import {ResourceModel} from '@/core/models/ResourceModel';
import {Strand} from './strand';

export class SubStrand extends ResourceModel<SubStrand> {
    public name?: string;
    public code?: string;
    public description?: string;
    public rank?: number;
    public strandId?: number;
    public strand?: Strand;

    // Used by add/edit form for cascading filters
    public academicYearId?: number;
    public curriculumId?: number;
    public learningLevelId?: number;
    public subjectId?: number;

    constructor(model?: Partial<SubStrand>) {
        super(model);
    }
}
