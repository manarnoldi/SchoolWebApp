import { ResourceService } from '@/core/services/resource.service';
import { Injectable } from '@angular/core';
import { StudentClass } from '../models/student-class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
  
export class StudentClassService extends ResourceService<StudentClass> {
  constructor(private http: HttpClient) {
      super(http, StudentClass);
  }
}