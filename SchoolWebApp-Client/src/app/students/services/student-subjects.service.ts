import {Injectable} from '@angular/core';
import { StudentSubject } from '../models/student-subject';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class StudentSubjectsService extends ResourceService<StudentSubject> {
    constructor(private http: HttpClient) {
        super(http, StudentSubject);
    }
}
