import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentAssessment} from '../models/student-assessment';

@Injectable({
    providedIn: 'root'
})
export class StudentAssessmentService extends ResourceService<StudentAssessment> {
    constructor(private http: HttpClient) {
        super(http, StudentAssessment);
    }
}
