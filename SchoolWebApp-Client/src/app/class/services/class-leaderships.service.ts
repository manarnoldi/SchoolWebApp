import { Injectable } from '@angular/core';
import { ClassLeadershipRole } from '../models/class-leadership-role';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ClassLeadershipsService extends ResourceService<ClassLeadershipRole> {
  constructor(private http: HttpClient) {
      super(http, ClassLeadershipRole);
  }
}