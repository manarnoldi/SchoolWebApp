import { Injectable } from '@angular/core';
import { Curriculum } from '../models/curriculum';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
  
export class CurriculumService extends ResourceService<Curriculum> {
  constructor(private http: HttpClient) {
      super(http, Curriculum);
  }
}