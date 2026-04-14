import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CoCurriculumScore} from '../models/co-curriculum-score';

@Injectable({providedIn: 'root'})
export class CoCurriculumScoreService extends ResourceService<CoCurriculumScore> {
    constructor(private http: HttpClient) {
        super(http, CoCurriculumScore);
    }
}
