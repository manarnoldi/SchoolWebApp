import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SchoolClass } from '../models/school-class';

@Injectable({
  providedIn: 'root'
})

export class SchoolClassesService extends ResourceService<SchoolClass> {
  constructor(private http: HttpClient) {
      super(http, SchoolClass);
  }
}
