import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentValueScore} from '../models/student-value-score';

@Injectable({providedIn: 'root'})
export class StudentValueScoreService extends ResourceService<StudentValueScore> {
    constructor(private http: HttpClient) {
        super(http, StudentValueScore);
    }
}
