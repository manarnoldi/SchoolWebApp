import { ResourceService } from '@/core/services/resource.service';
import { Injectable } from '@angular/core';
import { StudentClass } from '../models/student-class';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
  
export class StudentClassService extends ResourceService<StudentClass> {
  constructor(private http: HttpClient) {
      super(http, StudentClass);
  }

  getByStudentYearId(
          studentId: number,
          yearId: number
      ): Observable<StudentClass[]> {
          let searchStr = `/studentClasses/byStudentYearId/${studentId}/${yearId}`;
          return this.get(searchStr).pipe(map((studentClasses) => studentClasses));
      }
}