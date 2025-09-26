import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {AssessmentType} from '../models/assessment-type';

@Injectable({
    providedIn: 'root'
})
export class AssessmentTypeService extends ResourceService<AssessmentType> {
    constructor(private http: HttpClient) {
        super(http, AssessmentType);
    }
}
