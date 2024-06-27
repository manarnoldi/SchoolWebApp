import { Injectable } from '@angular/core';
import { EducationLevelType } from '../models/education-level-types';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class EducationLevelTypesService extends ResourceService<EducationLevelType> {
  constructor(private http: HttpClient) {
      super(http, EducationLevelType);
  }
}