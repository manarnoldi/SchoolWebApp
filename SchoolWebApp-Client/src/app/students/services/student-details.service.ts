import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentDetails} from '../models/student-details';
import {Status} from '@/core/enums/status';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StudentDetailsService extends ResourceService<StudentDetails> {
    constructor(private http: HttpClient) {
        super(http, StudentDetails);
    }

    public getBySearchDetails = (
        status: Status
    ): Observable<StudentDetails[]> => {
        let searchString = `/students/studentSearch?status=${status ?? ''}`;
        return this.get(searchString).pipe(map((students) => students));
    };
}
