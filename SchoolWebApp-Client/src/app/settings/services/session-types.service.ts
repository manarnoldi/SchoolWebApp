import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SessionType } from '../models/session-type';

@Injectable({
  providedIn: 'root'
})

export class SessionTypesService extends ResourceService<SessionType> {
  constructor(private http: HttpClient) {
      super(http, SessionType);
  }
}
