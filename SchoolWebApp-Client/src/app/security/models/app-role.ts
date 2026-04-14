export class AppRole {
    id: number;
    name: string;
    normalizedName: string;

    constructor(model?: Partial<AppRole>) {
        Object.assign(this, model);
    }
}
