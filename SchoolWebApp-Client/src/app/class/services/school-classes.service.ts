import {ResourceService} from '@/core/services/resource.service';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {SchoolClass} from '../models/school-class';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class SchoolClassesService extends ResourceService<SchoolClass> {
    constructor(private http: HttpClient) {
        super(http, SchoolClass);
    }

    public getByYearClassStream(url, params?: any): Observable<SchoolClass> {
        let httpParams = new HttpParams();
        if (params) {
            Object.keys(params).forEach((key) => {
                httpParams = httpParams.append(
                    key,
                    JSON.stringify(params[key])
                );
            });
        }
        return this.http.get<SchoolClass>(`${url}`, {params: httpParams});
    }

    public getByEducationLevelandYear = (
        educationLevelId: number,
        academicYearId: number
    ): Observable<SchoolClass[]> => {
        return this.get(
            `/schoolClasses/byEducationLevelYearId?educationLevelId=${educationLevelId ?? ''}&academicYearId=${academicYearId ?? ''}`
        ).pipe(map((educationLevels) => educationLevels));
    };
}
