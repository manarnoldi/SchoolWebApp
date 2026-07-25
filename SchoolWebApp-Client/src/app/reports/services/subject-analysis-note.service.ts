import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {SubjectAnalysisNote} from '../models/subject-analysis-note';

@Injectable({providedIn: 'root'})
export class SubjectAnalysisNoteService {
    constructor(private http: HttpClient) {}

    // Returns the saved note or null (the API replies with an empty 200 body
    // when none exists yet).
    getByParams(
        academicYearId: number,
        schoolClassId: number,
        subjectId: number,
        schoolExamId: number
    ): Observable<SubjectAnalysisNote | null> {
        let params = new HttpParams()
            .set('academicYearId', String(academicYearId))
            .set('schoolClassId', String(schoolClassId))
            .set('subjectId', String(subjectId))
            .set('schoolExamId', String(schoolExamId));
        return this.http.get<SubjectAnalysisNote | null>('/subjectAnalysisNotes/byParams', {params});
    }

    upsert(note: SubjectAnalysisNote): Observable<SubjectAnalysisNote> {
        return this.http.put<SubjectAnalysisNote>('/subjectAnalysisNotes/upsert', note);
    }
}
