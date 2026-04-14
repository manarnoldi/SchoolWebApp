import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ExamResult} from '../models/exam-result';

@Injectable({
    providedIn: 'root'
})
export class ExamResultService extends ResourceService<ExamResult> {
    constructor(private http: HttpClient) {
        super(http, ExamResult);
    }
}
