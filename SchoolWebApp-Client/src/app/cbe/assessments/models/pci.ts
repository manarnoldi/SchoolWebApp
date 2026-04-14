import {ResourceModel} from '@/core/models/ResourceModel';

export class PCI extends ResourceModel<PCI> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public subStrandId?: number;
    public subStrand?: any;

    // Used by add/edit form for cascading filters
    public academicYearId?: number;
    public curriculumId?: number;
    public learningLevelId?: number;
    public subjectId?: number;
    public strandId?: number;

    constructor(model?: Partial<PCI>) {
        super(model);
    }
}
