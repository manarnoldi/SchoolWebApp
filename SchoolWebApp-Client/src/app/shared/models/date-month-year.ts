export class DateMonthYear {
    public dateFrom?: Date;
    public dateTo?: Date;
    public month?: number;
    public year?: number;

     constructor(model?: DateMonthYear) {
        Object.assign(this, model);
    }
}
