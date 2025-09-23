export class ReportName {
    public id?: number;
    public title?: string;
    public category?: string;
    public code?: string;
    public rank?: number;
    public subReport?: ReportName;

    constructor(rn?: ReportName) {
        if (rn) {
            Object.assign(this, rn);
        }
    }
}
