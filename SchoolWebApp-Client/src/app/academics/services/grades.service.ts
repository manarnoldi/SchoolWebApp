import { Injectable } from '@angular/core';
import { Grade } from '../models/grade';
import { HttpClient } from '@angular/common/http';
import { ResourceService } from '@/core/services/resource.service';

@Injectable({
  providedIn: 'root'
})

export class GradesService extends ResourceService<Grade> {
  constructor(private http: HttpClient) {
      super(http, Grade);
  }
}