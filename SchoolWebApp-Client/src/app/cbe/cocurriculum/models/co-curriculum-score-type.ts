import {ResourceModel} from '@/core/models/ResourceModel';

export class CoCurriculumScoreType extends ResourceModel<CoCurriculumScoreType> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<CoCurriculumScoreType>) {
        super(model);
    }
}
