import { Injectable } from '@angular/core';
import { StudentAttendance } from '../models/student-attendance';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class StudentAttendancesService extends ResourceService<StudentAttendance> {
  constructor(private http: HttpClient) {
      super(http, StudentAttendance);
  }
}