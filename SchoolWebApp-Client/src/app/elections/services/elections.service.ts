import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {
    ElectionDetail,
    ElectionOptions,
    ElectionStage,
    ElectionSummary,
    SaveElection,
    SavePosition
} from '../models/election-models';

@Injectable({providedIn: 'root'})
export class ElectionsService {
    constructor(private http: HttpClient) {}

    // Lookups and whether the caller can manage elections. Not cached: another
    // user can log in without the page reloading.
    options(): Observable<ElectionOptions> {
        return this.http.get<ElectionOptions>('/elections/options');
    }

    list(): Observable<ElectionSummary[]> {
        return this.http.get<ElectionSummary[]>('/elections');
    }

    get(id: number): Observable<ElectionDetail> {
        return this.http.get<ElectionDetail>(`/elections/${id}`);
    }

    create(dto: SaveElection): Observable<ElectionDetail> {
        return this.http.post<ElectionDetail>('/elections', dto);
    }

    update(id: number, dto: SaveElection): Observable<ElectionDetail> {
        return this.http.put<ElectionDetail>(`/elections/${id}`, dto);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`/elections/${id}`);
    }

    changeStage(id: number, toStage: ElectionStage, note: string | null): Observable<ElectionDetail> {
        return this.http.post<ElectionDetail>(`/elections/${id}/stage`, {toStage, note});
    }

    addPosition(electionId: number, dto: SavePosition): Observable<ElectionDetail> {
        return this.http.post<ElectionDetail>(`/elections/${electionId}/positions`, dto);
    }

    updatePosition(positionId: number, dto: SavePosition): Observable<ElectionDetail> {
        return this.http.put<ElectionDetail>(`/elections/positions/${positionId}`, dto);
    }

    deletePosition(positionId: number): Observable<ElectionDetail> {
        return this.http.delete<ElectionDetail>(`/elections/positions/${positionId}`);
    }
}
