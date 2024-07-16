import {Status} from '@/core/enums/status';
import {ResourceModel} from '@/core/models/ResourceModel';
import {LearningMode} from '@/school/models/learning-mode';
import {Gender} from '@/settings/models/gender';
import {Nationality} from '@/settings/models/nationality';
import {Religion} from '@/settings/models/religion';

export class StudentDetails extends ResourceModel<StudentDetails> {
    //Personal details
    public staffImageAsBase64?: string;
    public fullName?: string;
    public status?: Status;
    public nationalityId?: number;
    public nationality?: Nationality;
    public religionId?: number;
    public religion?: Religion;
    public genderId?: number;
    public gender?: Gender;
    public dateOfBirth?: Date;
    public upi?: string;

    //Admission details
    public admissionDate?: Date;
    public applicationDate?: Date;
    public learningModeId?: number;
    public learningMode?: LearningMode;

    //Contact details
    public address?: string;
    public phoneNumber?: string;
    public email?: string;
    public otherDetails?: string;
    public healthConcerns?: string;

    constructor(model?: Partial<StudentDetails>) {
        super(model);
    }
}
