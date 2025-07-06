import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ExamName } from '../models/exam-name';

@Injectable({
  providedIn: 'root'
})
export class ExamNamesService  extends ResourceService<ExamName> {
  constructor(private http: HttpClient) {
      super(http, ExamName);
  }
}