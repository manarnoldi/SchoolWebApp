import {ResourceModel} from '@/core/models/ResourceModel';

export class GlobalSetting extends ResourceModel<GlobalSetting> {
    public module?: string;
    public settingKey?: string;
    public settingValue?: string;
    public description?: string;

    constructor(model?: Partial<GlobalSetting>) {
        super(model);
    }
}
