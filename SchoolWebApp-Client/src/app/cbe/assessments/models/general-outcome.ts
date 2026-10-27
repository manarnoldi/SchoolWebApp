import {ResourceModel} from '@/core/models/ResourceModel';
import { EducationLevelType } from '@/school/models/education-level-types';
import {EducationLevel} from '@/school/models/educationLevel';

export class GeneralOutcome extends ResourceModel<GeneralOutcome> {
    public name?: string;
    public description?: string
    public rank?: number;

    public educationLevelTypeId?: number;
    public educationLevelType?: EducationLevelType;
    // Unset: the outcome serves every level of the type that has none of its own.
    public educationLevelId?: number;
    public educationLevel?: EducationLevel;

    constructor(model?: Partial<GeneralOutcome>) {
        super(model);
    }
}
