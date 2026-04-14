import {ResourceModel} from '@/core/models/ResourceModel';

export class CoCurriculumScore extends ResourceModel<CoCurriculumScore> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public abbreviation?: string;
    public coCurriculumScoreTypeId?: number;
    public coCurriculumScoreType?: any;

    constructor(model?: Partial<CoCurriculumScore>) {
        super(model);
    }
}
