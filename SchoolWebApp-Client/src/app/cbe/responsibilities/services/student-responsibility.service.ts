import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentResponsibility} from '../models/student-responsibility';

@Injectable({providedIn: 'root'})
export class StudentResponsibilityService extends ResourceService<StudentResponsibility> {
    constructor(private http: HttpClient) {
        super(http, StudentResponsibility);
    }
}
