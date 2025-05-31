export class ReportName {
    public id?: number;
    public title?: string;
    public category?: string;

    constructor(rn?: ReportName) {
        if (rn) {
            Object.assign(this, rn);
        }
    }
}
