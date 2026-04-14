import {ResourceModel} from '@/core/models/ResourceModel';

export class StudentCoCurriculumScore extends ResourceModel<StudentCoCurriculumScore> {
    public studentCoCurriculumActivityId?: number;
    public studentCoCurriculumActivity?: any;
    public coCurriculumScoreId?: number;
    public coCurriculumScore?: any;
    public description?: string;

    constructor(model?: Partial<StudentCoCurriculumScore>) {
        super(model);
    }
}
