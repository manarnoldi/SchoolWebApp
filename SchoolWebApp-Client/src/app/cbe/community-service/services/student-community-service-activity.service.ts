import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StudentCommunityServiceActivity} from '../models/student-community-service-activity';

@Injectable({providedIn: 'root'})
export class StudentCommunityServiceActivityService extends ResourceService<StudentCommunityServiceActivity> {
    constructor(private http: HttpClient) {
        super(http, StudentCommunityServiceActivity);
    }
}
