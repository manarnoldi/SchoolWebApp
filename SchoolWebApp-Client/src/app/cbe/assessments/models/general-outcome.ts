import { Curriculum } from '@/academics/models/curriculum';
import {ResourceModel} from '@/core/models/ResourceModel';
import { EducationLevelType } from '@/school/models/education-level-types';

export class GeneralOutcome extends ResourceModel<GeneralOutcome> {
    public name?: string;
    public description?: string
    public rank?: number;

    public educationLevelTypeId?: number;
    public educationLevelType?: EducationLevelType;

    constructor(model?: Partial<GeneralOutcome>) {
        super(model);
    }
}
