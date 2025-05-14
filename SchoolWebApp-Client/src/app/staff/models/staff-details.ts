import {Status} from '@/core/enums/status';
import {ResourceModel} from '@/core/models/ResourceModel';
import { Designation } from '@/settings/models/designation';
import { EmploymentType } from '@/settings/models/employment-type';
import {Gender} from '@/settings/models/gender';
import {Nationality} from '@/settings/models/nationality';
import {Religion} from '@/settings/models/religion';
import { StaffCategory } from '@/settings/models/staff-category';

export class StaffDetails extends ResourceModel<StaffDetails> {
    //Core details
    public staffImageAsBase64?: string;    
    public fullName?: string;    
    public status?: Status;
    public nationalityId?: number;
    public nationality?: Nationality;
    public religionId?: number;
    public religion?: Religion;
    public genderId?: number;
    public gender?: Gender;
    

    //Personal details
    public dateOfBirth?: Date;
    public upi?: string;
    public idNumber?: string;
    public nhifNo?: string;
    public nssfNo?: string;
    public kraPinNo?: string;

    //Employment details
    public employmentDate?: Date;
    public endofEmploymentDate?: Date;
    public currentlyEmployed?: boolean;
    public staffCategoryId?: number;
    public staffCategory?: StaffCategory;
    public designationId?: number;
    public designation?: Designation;
    public employmentTypeId?: number;
    public employmentType?: EmploymentType;

    //Contact details
    public address?: string;
    public phoneNumber?: string;
    public email?: string;
    public otherDetails?: string;

    public isSelected?: boolean = false;
    public isOriginallySelected?: boolean = false;

    public remarks?: string = '';
    public hasRecord?: boolean = false;

    public timeIn? = { hour: 8, minute: 0 };
    public timeOut? = { hour: 17, minute: 0 };

    constructor(model?: Partial<StaffDetails>) {
        super(model);
    }
}
