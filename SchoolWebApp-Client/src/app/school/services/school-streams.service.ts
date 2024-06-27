import { ResourceService } from '@/core/services/resource.service';
import { Injectable } from '@angular/core';
import { SchoolStream } from '../models/school-stream';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class SchoolStreamsService extends ResourceService<SchoolStream> {
  constructor(private http: HttpClient) {
      super(http, SchoolStream);
  }
}