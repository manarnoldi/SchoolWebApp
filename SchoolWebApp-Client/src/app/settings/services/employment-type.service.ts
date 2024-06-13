import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EmploymentType } from '../models/employment-type';

@Injectable({
  providedIn: 'root'
})

export class EmploymentTypeService extends ResourceService<EmploymentType> {
  constructor(private http: HttpClient) {
      super(http, EmploymentType);
  }
}
