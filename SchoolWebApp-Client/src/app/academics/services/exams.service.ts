import { ResourceService } from '@/core/services/resource.service';
import { Injectable } from '@angular/core';
import { Exam } from '../models/exam';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ExamsService  extends ResourceService<Exam> {
  constructor(private http: HttpClient) {
      super(http, Exam);
  }
}
