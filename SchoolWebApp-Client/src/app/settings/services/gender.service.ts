import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Gender } from '../models/gender';

@Injectable({
  providedIn: 'root'
})
export class GenderService extends ResourceService<Gender> {
  constructor(private http: HttpClient) {
      super(http, Gender);
  }
}
