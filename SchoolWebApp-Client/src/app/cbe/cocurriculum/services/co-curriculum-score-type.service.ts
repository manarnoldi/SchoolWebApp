import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CoCurriculumScoreType} from '../models/co-curriculum-score-type';

@Injectable({
    providedIn: 'root'
})
export class CoCurriculumScoreTypeService extends ResourceService<CoCurriculumScoreType> {
    constructor(private http: HttpClient) {
        super(http, CoCurriculumScoreType);
    }
}
