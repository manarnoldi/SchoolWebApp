import { Injectable } from '@angular/core';
import { ResourceService } from './resource.service';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class UsersService extends ResourceService<User> {
  constructor(private http: HttpClient) {
      super(http, User);
  }
}
