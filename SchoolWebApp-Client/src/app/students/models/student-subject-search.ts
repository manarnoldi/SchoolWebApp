import {Subject} from '@/academics/models/subject';

import {ResourceModel} from '@/core/models/ResourceModel';

export class StudentSubjectSearch {
    public curriculumId?: number;
    public educationLevelId?: number;
    public academicYearId?: number;
    public schoolClassId?: number;
    public studentClassId?: number;

    constructor(ssSeach?: StudentSubjectSearch) {
        if (ssSeach) {
            Object.assign(this, ssSeach);
        }
    }
}
