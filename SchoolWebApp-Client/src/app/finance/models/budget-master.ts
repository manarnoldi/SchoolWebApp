import {ResourceModel} from '@/core/models/ResourceModel';

export enum BudgetMasterStatus {
    Draft = 0,
    Open = 1,
    Closed = 2
}

export class BudgetMaster extends ResourceModel<BudgetMaster> {
    public name?: string;
    public code?: string;
    public description?: string;
    public academicYearId?: number;
    public academicYearName?: string;
    public startDate?: string;
    public endDate?: string;
    public status?: BudgetMasterStatus;
    public budgetCount?: number;

    constructor(model?: Partial<BudgetMaster>) {
        super(model);
    }
}

export enum BudgetAmendmentStatus {
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

export class BudgetAmendmentLine {
    public id?: number;
    public budgetAmendmentId?: number;
    public accountId?: number;
    public accountCode?: string;
    public accountName?: string;
    public previousAmount?: number;
    public newAmount?: number;
    public delta?: number;
    public notes?: string;
}

export class BudgetAmendment extends ResourceModel<BudgetAmendment> {
    public budgetId?: number;
    public budgetName?: string;
    public referenceNumber?: string;
    public amendmentDate?: string;
    public reason?: string;
    public status?: BudgetAmendmentStatus;
    public approvedById?: number;
    public approvedByName?: string;
    public approvedDate?: string;
    public lines!: BudgetAmendmentLine[];

    constructor(model?: Partial<BudgetAmendment>) {
        super(model);
        if (!this.lines) this.lines = [];
    }
}
