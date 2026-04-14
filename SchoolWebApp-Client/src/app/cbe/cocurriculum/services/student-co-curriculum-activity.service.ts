import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentCoCurriculumActivity} from '../models/student-co-curriculum-activity';

@Injectable({providedIn: 'root'})
export class StudentCoCurriculumActivityService extends ResourceService<StudentCoCurriculumActivity> {
    constructor(private http: HttpClient) {
        super(http, StudentCoCurriculumActivity);
    }
}
