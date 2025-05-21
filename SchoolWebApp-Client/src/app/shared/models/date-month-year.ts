export class DateMonthYear {
    public dateFrom?: Date;
    public dateTo?: Date;
    public month?: number;
    public year?: number;

    constructor(dateFrom?: Date, dateTo?: Date, month?: number, year?: number) {
        this.dateFrom = dateFrom;
        this.dateTo = dateTo;
        this.month = month;
        this.year = year;
    }
}
