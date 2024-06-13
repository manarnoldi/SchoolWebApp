import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Nationality } from '../models/nationality';

@Injectable({
  providedIn: 'root'
})

export class NationalitiesService extends ResourceService<Nationality> {
  constructor(private http: HttpClient) {
      super(http, Nationality);
  }
}
