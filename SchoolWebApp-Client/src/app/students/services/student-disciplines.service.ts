import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentDiscipline} from '../models/student-discipline';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StudentDisciplinesService extends ResourceService<StudentDiscipline> {
    constructor(private http: HttpClient) {
        super(http, StudentDiscipline);
    }

    getByDateFromDateToStudentId(
        studentId: number,
        dateFrom: string,
        dateTo: string
    ): Observable<StudentDiscipline[]> {
        let searchStr = `/studentDisciplines/byDateFromDateTo/${studentId}/${dateFrom}/${dateTo}`;
        return this.get(searchStr).pipe(
            map((studentDisciplines) => studentDisciplines)
        );
    }
}
