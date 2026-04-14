import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentCoCurriculumScore} from '../models/student-co-curriculum-score';

@Injectable({providedIn: 'root'})
export class StudentCoCurriculumScoreService extends ResourceService<StudentCoCurriculumScore> {
    constructor(private http: HttpClient) {
        super(http, StudentCoCurriculumScore);
    }
}
