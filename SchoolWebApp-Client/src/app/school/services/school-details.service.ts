import { Injectable } from '@angular/core';
import { SchoolDetails } from '../models/school-details';
import { HttpClient } from '@angular/common/http';
import { ResourceService } from '@/core/services/resource.service';

@Injectable({
  providedIn: 'root'
})

export class SchoolDetailsService extends ResourceService<SchoolDetails> {
  constructor(private http: HttpClient) {
      super(http, SchoolDetails);
  }
}