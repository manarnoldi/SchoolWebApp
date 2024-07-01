import { ResourceService } from '@/core/services/resource.service';
import { Injectable } from '@angular/core';
import { Session } from '../models/session';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
  
export class SessionsService extends ResourceService<Session> {
  constructor(private http: HttpClient) {
      super(http, Session);
  }
}