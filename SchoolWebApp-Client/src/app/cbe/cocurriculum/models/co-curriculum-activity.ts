import {ResourceModel} from '@/core/models/ResourceModel';

export class CoCurriculumActivity extends ResourceModel<CoCurriculumActivity> {
    public name?: string;
    public rank?: number;
    public description?: string;

    constructor(model?: Partial<CoCurriculumActivity>) {
        super(model);
    }
}
