import {Injectable} from '@angular/core';
import {StaffDiscipline} from '../models/staff-discipline';
import {HttpClient} from '@angular/common/http';
import {ResourceService} from '@/core/services/resource.service';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StaffDisciplinesService extends ResourceService<StaffDiscipline> {
    constructor(private http: HttpClient) {
        super(http, StaffDiscipline);
    }

    getByDateFromDateToStaffId(
        staffId: number,
        dateFrom: string,
        dateTo: string
    ): Observable<StaffDiscipline[]> {
        let searchStr = `/StaffDisciplines/byDateFromDateTo/${staffId}/${dateFrom}/${dateTo}`;
        return this.get(searchStr).pipe(
            map((staffDisciplines) => staffDisciplines)
        );
    }
}
