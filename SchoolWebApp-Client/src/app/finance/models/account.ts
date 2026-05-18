import {ResourceModel} from '@/core/models/ResourceModel';

export enum AccountType {
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Income = 4,
    Expense = 5
}

export class Account extends ResourceModel<Account> {
    public code?: string;
    public name?: string;
    public accountType?: AccountType;
    public parentAccountId?: number;
    public parentAccountName?: string;
    public description?: string;
    public isActive?: boolean;

    constructor(model?: Partial<Account>) {
        super(model);
    }
}
