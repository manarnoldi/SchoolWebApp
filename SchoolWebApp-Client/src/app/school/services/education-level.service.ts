import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {EducationLevel} from '../models/educationLevel';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class EducationLevelService extends ResourceService<EducationLevel> {
    constructor(private http: HttpClient) {
        super(http, EducationLevel);
    }

    educationLevelsByCurriculum = (
        curriculumId: number
    ): Observable<EducationLevel[]> => {
        return this.get(
            `/educationLevels/byCurriculumId?curriculumId=${curriculumId ?? ''}`
        ).pipe(map((educationLevels) => educationLevels));
    };
}
