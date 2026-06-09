import {ResourceService} from '@/core/services/resource.service';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {GlobalSetting} from '../models/global-setting';
import {Observable, catchError, of, throwError} from 'rxjs';

@Injectable({providedIn: 'root'})
export class GlobalSettingService extends ResourceService<GlobalSetting> {
    constructor(private http: HttpClient) {
        super(http, GlobalSetting);
    }

    getByModule = (module: string): Observable<GlobalSetting[]> => {
        return this.get(`/globalSettings/byModule/${module}`);
    };

    // A setting that has never been saved yields 404 from the API. Treat that
    // as "unset" (null) so callers fall back to their default instead of the
    // whole forkJoin rejecting; genuine errors (500 etc.) still propagate.
    getByKey = (module: string, key: string): Observable<any> => {
        return this.http.get(`/globalSettings/byKey/${module}/${key}`).pipe(
            catchError((err: HttpErrorResponse) =>
                err.status === 404 ? of(null) : throwError(() => err))
        );
    };

    upsert = (setting: any): Observable<any> => {
        return this.http.put('/globalSettings/upsert', setting);
    };
}
