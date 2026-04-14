import { Curriculum } from '@/academics/models/curriculum';
import { Subject } from '@/academics/models/subject';
import { LearningLevel } from '@/class/models/learning-level';
import {ResourceModel} from '@/core/models/ResourceModel';
import { AcademicYear } from '@/school/models/academic-year';
export class Strand extends ResourceModel<Strand> {
    public name?: string;
    public code?: string;
    public description?: string
    public rank?: number;

    public curriculumId?: number;
    public curriculum?: Curriculum;

    public learningLevelId?: number;
    public learningLevel?: LearningLevel;
    
    public academicYearId?: number;
    public academicYear?: AcademicYear;

    public subjectId?: number;
    public subject?: Subject;

    public themeId?: number;
    public theme?: any;

    constructor(model?: Partial<Strand>) {
        super(model);
    }
}
