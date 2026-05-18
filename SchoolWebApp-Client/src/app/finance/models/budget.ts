import {ResourceModel} from '@/core/models/ResourceModel';

export class BudgetLine extends ResourceModel<BudgetLine> {
    public budgetId?: number;
    public accountId?: number;
    public accountCode?: string;
    public accountName?: string;
    public accountType?: string;
    public budgetedAmount?: number;
    public amendedAmount?: number;
    public effectiveAmount?: number;
    public actualAmount?: number;
    public variance?: number;
    public notes?: string;

    constructor(model?: Partial<BudgetLine>) {
        super(model);
    }
}

export class Budget extends ResourceModel<Budget> {
    public name?: string;
    public description?: string;
    public startDate?: string;
    public endDate?: string;
    public academicYearId?: number;
    public academicYearName?: string;
    public departmentId?: number;
    public departmentName?: string;
    public budgetMasterId?: number;
    public budgetMasterName?: string;
    public isActive?: boolean;
    public totalBudgeted?: number;
    public totalAmendments?: number;
    public totalEffective?: number;
    public lines!: BudgetLine[];

    constructor(model?: Partial<Budget>) {
        super(model);
        if (!this.lines) this.lines = [];
    }
}
