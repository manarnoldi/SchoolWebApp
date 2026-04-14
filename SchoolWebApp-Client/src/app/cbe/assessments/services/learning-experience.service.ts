import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {LearningExperience} from '../models/learning-experience';

@Injectable({
    providedIn: 'root'
})
export class LearningExperienceService extends ResourceService<LearningExperience> {
    constructor(private http: HttpClient) {
        super(http, LearningExperience);
    }
}
