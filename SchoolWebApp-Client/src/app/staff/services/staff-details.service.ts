import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StaffDetails } from '../models/staff-details';

@Injectable({
  providedIn: 'root'
})
  
export class StaffDetailsService extends ResourceService<StaffDetails> {
  constructor(private http: HttpClient) {
      super(http, StaffDetails);
  }
}