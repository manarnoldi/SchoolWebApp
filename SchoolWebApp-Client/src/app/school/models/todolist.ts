import { ResourceModel } from "@/core/models/ResourceModel";

export class TodoList extends ResourceModel<TodoList> {
    public itemName?: string;
    public completeBy?: Date;
    public completed?: boolean;
    public timeToDeadline: number;

    public staffDetailsId?: number;

    constructor(model?: Partial<TodoList>) {
        super(model);
    }
}
