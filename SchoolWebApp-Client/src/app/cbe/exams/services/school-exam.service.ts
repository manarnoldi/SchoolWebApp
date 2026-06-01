import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {SchoolExam} from '../models/school-exam';

@Injectable({
    providedIn: 'root'
})
export class SchoolExamService extends ResourceService<SchoolExam> {
    constructor(private http: HttpClient) {
        super(http, SchoolExam);
    }

    /**
     * Release (or revert the release of) a school exam. Releasing publishes
     * its results to the dashboard summary and is the hook for parent
     * notifications.
     */
    release(id: number, release: boolean = true): Observable<SchoolExam> {
        return this.http.post<SchoolExam>(
            `/schoolExams/${id}/release?release=${release}`,
            {}
        );
    }
}
