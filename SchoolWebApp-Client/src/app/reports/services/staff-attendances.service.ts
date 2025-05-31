import { ResourceService } from '@/core/services/resource.service';
import { Injectable } from '@angular/core';
import { StaffAttendancesReport } from '../models/staff-attendances-report';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class StaffAttendancesService extends ResourceService<StaffAttendancesReport> {
  constructor(private http: HttpClient) {
      super(http, StaffAttendancesReport);
  }
}