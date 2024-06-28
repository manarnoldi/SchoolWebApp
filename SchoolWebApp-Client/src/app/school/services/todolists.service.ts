import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TodoList } from '../models/todolist';

@Injectable({
  providedIn: 'root'
})

export class TodolistsService extends ResourceService<TodoList> {
  constructor(private http: HttpClient) {
      super(http, TodoList);
  }
}
