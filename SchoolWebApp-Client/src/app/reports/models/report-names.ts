export class ReportName {
    public id?: number;
    public title?: string;
    public category?: string;
    public code?: string;
    public rank?: number;

    constructor(rn?: ReportName) {
        if (rn) {
            Object.assign(this, rn);
        }
    }
}
