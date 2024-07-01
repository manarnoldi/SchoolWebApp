import {ResourceModel} from '@/core/models/ResourceModel';

export class AcademicYear extends ResourceModel<AcademicYear> {
    public name?: string;
    public description?: string;
    public abbreviation?: string;
    public startDate?: Date;
    public endDate?: Date;
    public status?: boolean;

    constructor(model?: Partial<AcademicYear>) {
        super(model);
    }
}
