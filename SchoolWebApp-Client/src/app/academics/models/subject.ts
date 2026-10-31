import {ResourceModel} from '@/core/models/ResourceModel';
import {SubjectGroup} from './subject-group';
import {Department} from '@/school/models/department';
import {StaffDetails} from '@/staff/models/staff-details';

export class Subject extends ResourceModel<Subject> {
    public code?: string;
    public name?: string;
    public abbr?: string;
    public rank?: number;
    public numOfLessons?: number;
    public description?: string;
    public optional?: boolean;
    public examinable?: boolean;
    // Teachers keep schemes, lesson plans and a record of work for it; off leaves it out of Teaching Management.
    public requiresTeachingDocuments?: boolean;

    public subjectGroupId?: number;
    public subjectGroup?: SubjectGroup;

    public departmentId?: number;
    public department?: Department;

    public staffDetailsId?: number;
    public staffDetails?: StaffDetails;

    public isSelected?: Boolean = false;
    public isOriginallySelected?: Boolean = false;

    constructor(model?: Partial<Subject>) {
        super(model);
    }
}
