import {ResourceModel} from '@/core/models/ResourceModel';
import {StaffDetails} from './staff-details';

export class StaffAttendance extends ResourceModel<StaffAttendance> {
    public staffDetailsId?: number;
    public staffDetails?: StaffDetails;
    public date?: Date;
    public present?: boolean;
    public remarks?: string;

    public timeIn?: string;
    public timeOut?: string;
    
    constructor(model?: Partial<StaffAttendance>) {
        super(model);
    }
}
