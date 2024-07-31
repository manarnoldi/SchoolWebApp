import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StudentDiscipline } from '../models/student-discipline';

@Injectable({
  providedIn: 'root'
})
export class StudentDisciplinesService extends ResourceService<StudentDiscipline> {
  constructor(private http: HttpClient) {
      super(http, StudentDiscipline);
  }
}
