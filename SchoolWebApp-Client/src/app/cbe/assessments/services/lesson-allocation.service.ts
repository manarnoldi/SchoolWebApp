import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {LessonAllocation} from '../models/lesson-allocation';

@Injectable({
    providedIn: 'root'
})
export class LessonAllocationService extends ResourceService<LessonAllocation> {
    constructor(private http: HttpClient) {
        super(http, LessonAllocation);
    }
}
