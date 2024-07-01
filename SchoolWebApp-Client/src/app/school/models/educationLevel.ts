import { Curriculum } from '@/academics/models/curriculum';
import {ResourceModel} from '@/core/models/ResourceModel';
import { EducationLevelType } from './education-level-types';

export class EducationLevel extends ResourceModel<EducationLevel> {
    public name?: string;
    public description?: string;
    public abbr?: string;
    public numOfYears?: number;

    public educationLevelTypeId?: number;
    public educationLevelType?: EducationLevelType;
    public curriculumId?: number;
    public curriculum?: Curriculum;

    constructor(model?: Partial<EducationLevel>) {
        super(model);
    }
}
