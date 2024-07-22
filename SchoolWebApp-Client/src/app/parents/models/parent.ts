import {Status} from '@/core/enums/status';
import {ResourceModel} from '@/core/models/ResourceModel';
import {LearningMode} from '@/school/models/learning-mode';
import {Gender} from '@/settings/models/gender';
import {Nationality} from '@/settings/models/nationality';
import { Occupation } from '@/settings/models/occupation';
import {Religion} from '@/settings/models/religion';

export class Parent extends ResourceModel<Parent> {
    //Personal details
    public staffImageAsBase64?: string;   
    public fullName?: string;
    public upi?: string;
    public dateOfBirth?: Date;
    public address?: string;
    public phoneNumber?: string;
    public email?: string;
    public status?: Status;
    public otherDetails?: string;
    public nationalityId?: number;
    public nationality?: Nationality;
    public religionId?: number;
    public religion?: Religion;
    public genderId?: number;
    public gender?: Gender;
    
    public notifiable?: boolean;
    public payer?: boolean;
    public pickup?: boolean;
    public occupationId?: number;
    public occupation?: Occupation;

    constructor(model?: Partial<Parent>) {
        super(model);
    }
}
