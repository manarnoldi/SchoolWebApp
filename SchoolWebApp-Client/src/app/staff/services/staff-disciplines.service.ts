import { Injectable } from '@angular/core';
import { StaffDiscipline } from '../models/staff-discipline';
import { HttpClient } from '@angular/common/http';
import { ResourceService } from '@/core/services/resource.service';

@Injectable({
  providedIn: 'root'
})

export class StaffDisciplinesService extends ResourceService<StaffDiscipline> {
  constructor(private http: HttpClient) {
      super(http, StaffDiscipline);
  }
}
