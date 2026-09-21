import {ResourceModel} from '@/core/models/ResourceModel';

export class ExamType extends ResourceModel<ExamType> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public abbreviation?: string;
    // Results appear on the report form, in a column of their own.
    public showOnReportForm?: boolean;
    // A term may hold more than one exam of this type (e.g. weekly marathons).
    public allowMultiplePerTerm?: boolean;

    constructor(model?: Partial<ExamType>) {
        super(model);
    }
}
