import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OccurenceType } from '../models/occurence-type';

@Injectable({
  providedIn: 'root'
})

export class OccurenceTypeService extends ResourceService<OccurenceType> {
  constructor(private http: HttpClient) {
      super(http, OccurenceType);
  }
}
