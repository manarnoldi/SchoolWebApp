import {ResourceModel} from '@/core/models/ResourceModel';
import { Session } from 'protractor';

export class SchoolEvent extends ResourceModel<SchoolEvent> {
    public eventName?: string;
    public eventLocation?: string;
    public startDate?: Date;
    public endDate?: Date;
    public description?: string;

    public sessionId?: number;
    public session?: Session;

    constructor(model?: Partial<SchoolEvent>) {
        super(model);
    }
}
