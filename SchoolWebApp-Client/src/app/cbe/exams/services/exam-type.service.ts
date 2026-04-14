import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ExamType} from '../models/exam-type';

@Injectable({
    providedIn: 'root'
})
export class ExamTypeService extends ResourceService<ExamType> {
    constructor(private http: HttpClient) {
        super(http, ExamType);
    }
}
