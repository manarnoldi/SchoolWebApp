import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Designation } from '../models/designation';

@Injectable({
  providedIn: 'root'
})
export class DesignationsService extends ResourceService<Designation> {
  constructor(private http: HttpClient) {
      super(http, Designation);
  }
}