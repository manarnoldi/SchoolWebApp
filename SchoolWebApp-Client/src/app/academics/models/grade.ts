import { ResourceModel } from "@/core/models/ResourceModel";
import { Curriculum } from "./curriculum";

export class Grade extends ResourceModel<Grade> {
    public name?: string;
    public abbr?: string;

    public minScore?: number;
    public maxScore?: number;
    public points?: number;
    
    public remarksSwa?: string;
    public remarksEng?: string;

    public curriculumId?: string;
    public curriculum?: Curriculum;

    constructor(model?: Partial<Grade>) {
        super(model);
      }
}