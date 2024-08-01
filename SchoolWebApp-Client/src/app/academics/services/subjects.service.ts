import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Subject } from '../models/subject';

@Injectable({
  providedIn: 'root'
})

export class SubjectsService extends ResourceService<Subject> {
  constructor(private http: HttpClient) {
      super(http,  Subject);
  }
}