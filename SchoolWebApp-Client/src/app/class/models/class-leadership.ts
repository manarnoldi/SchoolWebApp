import { ResourceModel } from "@/core/models/ResourceModel";
import { SchoolClass } from "./school-class";
import { ClassLeadershipRole } from "./class-leadership-role";
import { Person } from "@/school/models/person";

export class ClassLeadership extends ResourceModel<ClassLeadership> {
    public name?: string;
    public description?: string;

    public schoolClassId?: string;
    public schoolClass?: SchoolClass;

    public personId?: string;
    public person?: Person;

    public classLeadershipRoleId?: string;
    public classLeadershipRole?: ClassLeadershipRole;

    constructor(model?: Partial<ClassLeadership>) {
        super(model);
    }
}
