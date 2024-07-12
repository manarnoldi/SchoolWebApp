import { Injectable } from '@angular/core';
import { ClassLeadershipRole } from '../models/class-leadership-role';
import { HttpClient } from '@angular/common/http';
import { ResourceService } from '@/core/services/resource.service';

@Injectable({
  providedIn: 'root'
})

export class ClassLeadershipRolesService extends ResourceService<ClassLeadershipRole> {
  constructor(private http: HttpClient) {
      super(http, ClassLeadershipRole);
  }
}