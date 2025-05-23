import { ResourceService } from '@/core/services/resource.service';
import { Injectable } from '@angular/core';
import { Session } from '../models/session';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
  
export class SessionsService extends ResourceService<Session> {
  constructor(private http: HttpClient) {
      super(http, Session);
  }

  public getByCurriculumYear = (
          curriculumId: number,
          academicYearId: number
      ): Observable<Session[]> => {
          return this.get(
              `/sessions/byCurriculumYearId?curriculumId=${curriculumId ?? ''}&academicYearId=${academicYearId ?? ''}`
          ).pipe(map((educationLevels) => educationLevels));
      };
}