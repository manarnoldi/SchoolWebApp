import {Roles} from '../enums/roles';
import {ResourceModel} from './ResourceModel';

export class User extends ResourceModel<User> {
    id: string;
    userName: string;
    email: string;
    password: string;
    roles: Roles[];
    normalizedUserName: string;
    normalizedEmail: string;
    emailConfirmed: boolean;
    // passwordHash: string;
    // securityStamp: string;
    // concurrencyStamp: string;
    phoneNumber: string;
    phoneNumberConfirmed: boolean;
    twoFactorEnabled: boolean;
    // lockoutEnd: string;
    // lockoutEnabled: boolean;
    // accessFailedCount: number;
    firstName: string;
    lastName: string;
    staffId: number;
    status: boolean;

    currentUserParent: boolean;
    currentUserStudent: boolean;
    currentUserTeacher: boolean;
    currentUserAdministrator: boolean;
    currentUserHeadTeacher: boolean;
    currentUserVisitor: boolean;
    currentUserAccounts: boolean;

    constructor(model?: Partial<User>) {
        super(model);
    }
}
