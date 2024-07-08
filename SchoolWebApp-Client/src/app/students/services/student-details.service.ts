import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StudentDetails } from '../models/student-details';

@Injectable({
  providedIn: 'root'
})

export class StudentDetailsService extends ResourceService<StudentDetails> {
  constructor(private http: HttpClient) {
      super(http, StudentDetails);
  }
}