import { Injectable } from '@angular/core';
import { Relationship } from '../models/relationship';
import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
  
export class RelationshipsService extends ResourceService<Relationship> {
  constructor(private http: HttpClient) {
      super(http, Relationship);
  }
}