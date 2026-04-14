import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Exam} from '../models/exam';

@Injectable({
    providedIn: 'root'
})
export class ExamService extends ResourceService<Exam> {
    constructor(private http: HttpClient) {
        super(http, Exam);
    }
}
