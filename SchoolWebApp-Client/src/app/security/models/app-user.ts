export class AppUser {
    id: number;
    userName: string;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
    password?: string;

    constructor(model?: Partial<AppUser>) {
        Object.assign(this, model);
        this.roles = this.roles || [];
    }
}
