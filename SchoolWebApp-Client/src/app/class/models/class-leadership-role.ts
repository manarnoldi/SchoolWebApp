import {PersonType} from '@/core/enums/personTypes';
import {ResourceModel} from '@/core/models/ResourceModel';

export class ClassLeadershipRole extends ResourceModel<ClassLeadershipRole> {
    public name?: string;
    public description?: string;
    public personType?: PersonType;
    public rank?: number;

    constructor(model?: Partial<ClassLeadershipRole>) {
        super(model);
    }
}
