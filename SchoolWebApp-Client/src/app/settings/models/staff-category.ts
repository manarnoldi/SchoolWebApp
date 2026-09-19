import { ResourceModel } from "@/core/models/ResourceModel";

export class StaffCategory extends ResourceModel<StaffCategory> {
    public name?: string;
    public code?: string;
    public forTeaching?: boolean;
    public rank?: number;
    public description?: string;
    public abbreviation?: string;
    // Expense account this category's salaries are charged to in the payroll journal.
    public salaryExpenseAccountId?: number | null;

    constructor(model?: Partial<StaffCategory>) {
        super(model);
      }
}