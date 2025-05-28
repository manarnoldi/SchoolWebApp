import {Injectable} from '@angular/core';
import {StudentFormerSchool} from '../models/student-former-school';
import {HttpClient} from '@angular/common/http';
import {ResourceService} from '@/core/services/resource.service';
import { map, Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StudentFormerSchoolsService extends ResourceService<StudentFormerSchool> {
    constructor(private http: HttpClient) {
        super(http, StudentFormerSchool);
    }

    getBySearch(
        studentId: number,
        curriculumId: number
    ): Observable<StudentFormerSchool[]> {
        let searchStr = `/formerSchools/search?studentId=${studentId ?? ''}&curriculumId=${curriculumId ?? ''}`;
        return this.get(searchStr).pipe(map((formerSchools) => formerSchools));
    }
}
