import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AcademicYear } from '../models/academic-years';

@Injectable({
  providedIn: 'root'
})

export class AcademicYearsService extends ResourceService<AcademicYear> {
  constructor(private http: HttpClient) {
      super(http, AcademicYear);
  }
}