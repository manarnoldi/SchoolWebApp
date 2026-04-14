export class MenuPermission {
    id: number;
    roleId: string;
    menuPath: string;
    menuName: string;

    constructor(model?: Partial<MenuPermission>) {
        Object.assign(this, model);
    }
}
