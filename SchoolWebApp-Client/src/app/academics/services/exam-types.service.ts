import { Injectable } from '@angular/core';
import { ExamType } from '../models/exam-type';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class ExamTypesService extends ResourceService<ExamType> {
  constructor(private http: HttpClient) {
      super(http, ExamType);
  }
}