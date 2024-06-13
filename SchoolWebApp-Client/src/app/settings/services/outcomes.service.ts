import { Injectable } from '@angular/core';
import { Outcome } from '../models/outcome';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { OccurenceType } from '../models/occurence-type';

@Injectable({
  providedIn: 'root'
})
  
export class OutcomesService extends ResourceService<Outcome> {
  constructor(private http: HttpClient) {
      super(http, OccurenceType);
  }
}