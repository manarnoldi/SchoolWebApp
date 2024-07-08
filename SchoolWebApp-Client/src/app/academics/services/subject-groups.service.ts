import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SubjectGroup } from '../models/subject-group';

@Injectable({
  providedIn: 'root'
})

export class SubjectGroupsService extends ResourceService<SubjectGroup> {
  constructor(private http: HttpClient) {
      super(http, SubjectGroup);
  }
}