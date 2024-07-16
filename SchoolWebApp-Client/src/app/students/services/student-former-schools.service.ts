import {Injectable} from '@angular/core';
import {StudentFormerSchool} from '../models/student-former-school';
import {HttpClient} from '@angular/common/http';
import {ResourceService} from '@/core/services/resource.service';

@Injectable({
    providedIn: 'root'
})
export class StudentFormerSchoolsService extends ResourceService<StudentFormerSchool> {
    constructor(private http: HttpClient) {
        super(http, StudentFormerSchool);
    }
}
