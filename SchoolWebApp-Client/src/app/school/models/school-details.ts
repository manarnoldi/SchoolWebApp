import {ResourceModel} from '@/core/models/ResourceModel';

export class SchoolDetails extends ResourceModel<SchoolDetails> {
    public name?: string;
    public address?: string;
    public telephone?: string;
    public email?: string;
    public motto?: string;
    public mission?: string;
    public vision?: string;
    
    public initials?: string;
    public website?: string;
    public logoUrl?: string;
    public logoAsBase64?: string;

    public prePrimary?: boolean;
    public lowerPrimary?: boolean;
    public upperPrimary?: boolean;
    public juniorSchool?: boolean;
    public seniorSchool?: boolean;

    public otherDetails?: string;
    public reportHeader?: string;
    public reportTitle?: string;
    public reportSubTitle?: string;
    public reportTitleDetails?: string;

    constructor(model?: Partial<SchoolDetails>) {
        super(model);
    }
}
