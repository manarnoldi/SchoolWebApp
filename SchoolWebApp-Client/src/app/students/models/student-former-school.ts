import {ResourceModel} from '@/core/models/ResourceModel';
import { StudentDetails } from './student-details';
import { Curriculum } from '@/academics/models/curriculum';
import { EducationLevel } from '@/school/models/educationLevel';

export class StudentFormerSchool extends ResourceModel<StudentFormerSchool> {
    public description?: string;
    public schoolName?: string;
    public classDetails?: string;
    public score?: string;
    public position?: string;

    public studentId?: number;
    public student?: StudentDetails;
    public curriculumId?: number;
    public curriculum?: Curriculum;
    public educationLevelId?: number;
    public educationLevel?: EducationLevel;

    constructor(model?: Partial<StudentFormerSchool>) {
        super(model);
    }
}
