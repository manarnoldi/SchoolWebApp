import {Injectable} from '@angular/core';
import {LearningLevel} from '../models/learning-level';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LearningLevelsService extends ResourceService<LearningLevel> {
    constructor(private http: HttpClient) {
        super(http, LearningLevel);
    }

    public getLearningLevelsByCurriculum = (
        curriculumId: number
    ): Observable<LearningLevel[]> => {
        return this.get(
            `/learningLevels/byCurriculumId?curriculumId=${curriculumId ?? ''}`
        ).pipe(map((learningLevels) => learningLevels));
    };
}
