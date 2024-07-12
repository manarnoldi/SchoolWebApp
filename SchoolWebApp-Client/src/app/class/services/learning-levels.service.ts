import {Injectable} from '@angular/core';
import {LearningLevel} from '../models/learning-level';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class LearningLevelsService extends ResourceService<LearningLevel> {
    constructor(private http: HttpClient) {
        super(http, LearningLevel);
    }
}
