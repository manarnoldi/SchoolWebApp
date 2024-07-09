import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SchoolEvent } from '../models/schoolEvent';

@Injectable({
  providedIn: 'root'
})

export class EventsService extends ResourceService<SchoolEvent> {
  constructor(private http: HttpClient) {
      super(http, SchoolEvent);
  }
}