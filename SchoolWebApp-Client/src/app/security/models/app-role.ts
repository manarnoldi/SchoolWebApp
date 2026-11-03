export class AppRole {
    id: number;
    name: string;
    normalizedName: string;
    userCount?: number;

    constructor(model?: Partial<AppRole>) {
        Object.assign(this, model);
    }
}
