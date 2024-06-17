import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LearningMode } from '../models/learning-mode';

@Injectable({
  providedIn: 'root'
})
export class LearningModesService extends ResourceService<LearningMode> {
  constructor(private http: HttpClient) {
      super(http, LearningMode);
  }
}
