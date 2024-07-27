import { ResourceModel } from "@/core/models/ResourceModel";
import { StudentDetails } from "./student-details";
import { Relationship } from "@/settings/models/relationship";
import { Parent } from "./parent";

export class StudentParent extends ResourceModel<StudentParent> {
    public OtherDetails?: string;

    public studentId?: number;
    public student?: StudentDetails;

    public parentId?: number;
    public parent?: Parent;

    public relationShipId?: number;
    public relationShip?: Relationship;

    constructor(model?: Partial<StudentParent>) {
        super(model);
    }
}