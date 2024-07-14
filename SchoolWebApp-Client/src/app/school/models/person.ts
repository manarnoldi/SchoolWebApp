import { Status } from "@/core/enums/status";
import { ResourceModel } from "@/core/models/ResourceModel";
import { Gender } from "@/settings/models/gender";
import { Nationality } from "@/settings/models/nationality";
import { Religion } from "@/settings/models/religion";

export class Person extends ResourceModel<Person> {
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
    public address?: string;
    public phoneNumber?: string;
    public email?: string;
    public otherDetails?: string;

    constructor(model?: Partial<Person>) {
        super(model);
    }
}
