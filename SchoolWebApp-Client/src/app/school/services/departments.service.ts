import { Injectable } from '@angular/core';
import { Department } from '../models/department';
import { HttpClient } from '@angular/common/http';
import { ResourceService } from '@/core/services/resource.service';

@Injectable({
  providedIn: 'root'
})
  
export class DepartmentsService extends ResourceService<Department> {
  constructor(private http: HttpClient) {
      super(http, Department);
  }
}