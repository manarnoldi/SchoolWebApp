export class AppUser {
    id: number;
    userName: string;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
    password?: string;

    personId?: number | null;
    personName?: string | null;
    personUPI?: string | null;
    personType?: string | null;

    constructor(model?: Partial<AppUser>) {
        Object.assign(this, model);
        this.roles = this.roles || [];
    }
}

export class AvailablePerson {
    id: number;
    fullName: string;
    upi: string;
    email?: string | null;
    personType: string;
}
