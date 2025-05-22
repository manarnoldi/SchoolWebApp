import {Injectable} from '@angular/core';
import {ClassLeadershipRole} from '../models/class-leadership-role';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {ClassLeadership} from '../models/class-leadership';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ClassLeadershipsService extends ResourceService<ClassLeadershipRole> {
    constructor(private http: HttpClient) {
        super(http, ClassLeadershipRole);
    }

    public getBySchoolClassId = (
        schoolClassId: number
    ): Observable<ClassLeadership[]> => {
        return this.get(
            `/schoolClassLeaders/bySchoolClassId/${schoolClassId}`
        ).pipe(map((classLeaderships) => classLeaderships));
    };
}
