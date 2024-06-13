import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StaffCategory } from '../models/staff-category';

@Injectable({
  providedIn: 'root'
})

export class StaffCategoriesService extends ResourceService<StaffCategory> {
  constructor(private http: HttpClient) {
      super(http, StaffCategory);
  }
}