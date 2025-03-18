import { ResourceModel } from "@/core/models/ResourceModel";
import { AcademicYear } from "@/school/models/academic-year";
import { Subject } from "./subject";
import { EducationLevel } from "@/school/models/educationLevel";

export class EducationLevelSubject extends ResourceModel<EducationLevelSubject> {
    public educationLevelId: number;
    public educationLevel?: EducationLevel;

    public subjectId: number;
    public subject?: Subject;

    public academicYearId: number;
    public academicYear?: AcademicYear;

    public description?: string;

    constructor(model?: Partial<EducationLevelSubject>) {
        super(model);
      }
}