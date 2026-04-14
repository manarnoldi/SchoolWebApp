import {ResourceModel} from '@/core/models/ResourceModel';

export class LessonAllocation extends ResourceModel<LessonAllocation> {
    public lessonsPerWeek?: number;
    public description?: string;
    public subjectId?: number;
    public subject?: any;
    public learningLevelId?: number;
    public learningLevel?: any;

    // Used by add/edit form for cascading filters
    public academicYearId?: number;
    public curriculumId?: number;

    constructor(model?: Partial<LessonAllocation>) {
        super(model);
    }
}
