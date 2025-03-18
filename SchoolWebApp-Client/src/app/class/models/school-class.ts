import {ResourceModel} from '@/core/models/ResourceModel';
import {SchoolStream} from './school-stream';
import {LearningLevel} from '@/class/models/learning-level';
import {AcademicYear} from '@/school/models/academic-year';
import {Person} from '@/school/models/person';

export class SchoolClass extends ResourceModel<SchoolClass> {
    public name?: string;
    public rank?: string;
    public description?: string;

    public learningLevelId?: string;
    public learningLevel?: LearningLevel;

    public schoolStreamId?: string;
    public schoolStream?: SchoolStream;

    public academicYearId?: string;
    public academicYear?: AcademicYear;

    public schoolClassLeaders?: Person[];

    public isSelected?: boolean = false;

    constructor(model?: Partial<SchoolClass>) {
        super(model);
    }
}
