import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CoCurriculumActivity} from '../models/co-curriculum-activity';

@Injectable({
    providedIn: 'root'
})
export class CoCurriculumActivityService extends ResourceService<CoCurriculumActivity> {
    constructor(private http: HttpClient) {
        super(http, CoCurriculumActivity);
    }
}
