import { Injectable } from '@angular/core';
import { StudentParent } from '../models/student-parent';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class StudentParentsService extends ResourceService<StudentParent> {
  constructor(private http: HttpClient) {
      super(http, StudentParent);
  }
}
