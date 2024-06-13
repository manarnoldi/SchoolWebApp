import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Religion } from '../models/religion';

@Injectable({
  providedIn: 'root'
})
  
export class ReligionsService extends ResourceService<Religion> {
  constructor(private http: HttpClient) {
      super(http, Religion);
  }
}