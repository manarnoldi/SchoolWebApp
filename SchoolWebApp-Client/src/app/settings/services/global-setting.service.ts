import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {GlobalSetting} from '../models/global-setting';
import {Observable} from 'rxjs';

@Injectable({providedIn: 'root'})
export class GlobalSettingService extends ResourceService<GlobalSetting> {
    constructor(private http: HttpClient) {
        super(http, GlobalSetting);
    }

    getByModule = (module: string): Observable<GlobalSetting[]> => {
        return this.get(`/globalSettings/byModule/${module}`);
    };

    getByKey = (module: string, key: string): Observable<any> => {
        return this.http.get(`/globalSettings/byKey/${module}/${key}`);
    };

    upsert = (setting: any): Observable<any> => {
        return this.http.put('/globalSettings/upsert', setting);
    };
}
