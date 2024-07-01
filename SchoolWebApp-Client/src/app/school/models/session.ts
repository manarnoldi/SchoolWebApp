import { Curriculum } from "@/academics/models/curriculum";
import { ResourceModel } from "@/core/models/ResourceModel";
import { SessionType } from "@/settings/models/session-type";
import { AcademicYear } from "./academic-years";

export class Session extends ResourceModel<Session> {
    public sessionName?: string;
    public abbreviation?: string;
    public startDate?: Date;
    public endDate?: Date;
    public status?: boolean;
    public academicYearId?: number;
    public academicYear?: AcademicYear;
    public curriculumId?: number;
    public curriculum?: Curriculum;
    public sessionTypeId?: number;
    public sessionType?: SessionType;

    constructor(model?: Partial<Session>) {
        super(model);
    }
}