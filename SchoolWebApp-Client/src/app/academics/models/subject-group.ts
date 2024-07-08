import { ResourceModel } from "@/core/models/ResourceModel";
import { Curriculum } from "./curriculum";

export class SubjectGroup extends ResourceModel<SubjectGroup> {
    public name?: string;
    public description?: string;
    public abbreviation?: string;

    public curriculumId?: number;
    public curriculum?: Curriculum;

    constructor(model?: Partial<SubjectGroup>) {
        super(model);
      }
}