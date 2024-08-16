import { Injectable } from '@angular/core';
import { StaffSubject } from '../models/staff-subject';
import { HttpClient } from '@angular/common/http';
import { ResourceService } from '@/core/services/resource.service';

@Injectable({
  providedIn: 'root'
})

export class StaffSubjectsService extends ResourceService<StaffSubject> {
  constructor(private http: HttpClient) {
      super(http, StaffSubject);
  }
}