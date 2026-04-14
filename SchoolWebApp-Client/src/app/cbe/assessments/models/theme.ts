import { Curriculum } from '@/academics/models/curriculum';
import { Subject } from '@/academics/models/subject';
import { LearningLevel } from '@/class/models/learning-level';
import {ResourceModel} from '@/core/models/ResourceModel';

export class Theme extends ResourceModel<Theme> {
    public name?: string;
    public code?: string;
    public description?: string;
    public rank?: number;

    public curriculumId?: number;
    public curriculum?: Curriculum;

    public learningLevelId?: number;
    public learningLevel?: LearningLevel;

    public subjectId?: number;
    public subject?: Subject;

    constructor(model?: Partial<Theme>) {
        super(model);
    }
}
