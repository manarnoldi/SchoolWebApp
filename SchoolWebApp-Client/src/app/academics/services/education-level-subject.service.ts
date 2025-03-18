import {ResourceService} from '@/core/services/resource.service';
import {Injectable} from '@angular/core';
import {EducationLevelSubject} from '../models/education-level-subject';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class EducationLevelSubjectService extends ResourceService<EducationLevelSubject> {
    constructor(private http: HttpClient) {
        super(http, EducationLevelSubject);
    }
}
