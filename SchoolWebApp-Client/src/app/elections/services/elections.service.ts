import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {
    Ballot,
    CandidateStatus,
    ElectionDetail,
    ElectionOptions,
    ElectionStage,
    ElectionSummary,
    EligibleStudent,
    Leadership,
    MyStation,
    Nominations,
    RegisterSummary,
    RegisterVoter,
    SaveCandidate,
    Results,
    SaveElection,
    SavePosition,
    SaveStation,
    StationRegister,
    Stations
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

    nominations(electionId: number): Observable<Nominations> {
        return this.http.get<Nominations>(`/elections/${electionId}/nominations`);
    }

    eligible(contestId: number, role: 'candidate' | 'mate'): Observable<EligibleStudent[]> {
        return this.http.get<EligibleStudent[]>(`/elections/contests/${contestId}/eligible`, {params: {role}});
    }

    nominate(contestId: number, dto: SaveCandidate): Observable<Nominations> {
        return this.http.post<Nominations>(`/elections/contests/${contestId}/candidates`, dto);
    }

    updateCandidate(candidateId: number, dto: SaveCandidate): Observable<Nominations> {
        return this.http.put<Nominations>(`/elections/candidates/${candidateId}`, dto);
    }

    setCandidateStatus(candidateId: number, status: CandidateStatus, note: string | null): Observable<Nominations> {
        return this.http.post<Nominations>(`/elections/candidates/${candidateId}/status`, {status, note});
    }

    deleteCandidate(candidateId: number): Observable<Nominations> {
        return this.http.delete<Nominations>(`/elections/candidates/${candidateId}`);
    }

    uploadPhoto(candidateId: number, file: File, mate: boolean): Observable<Nominations> {
        const form = new FormData();
        form.append('file', file);
        return this.http.post<Nominations>(`/elections/candidates/${candidateId}/photo`, form, {params: {mate}});
    }

    deletePhoto(candidateId: number, mate: boolean): Observable<Nominations> {
        return this.http.delete<Nominations>(`/elections/candidates/${candidateId}/photo`, {params: {mate}});
    }

    register(electionId: number): Observable<RegisterSummary> {
        return this.http.get<RegisterSummary>(`/elections/${electionId}/register`);
    }

    registerContest(electionId: number, contestId: number): Observable<RegisterVoter[]> {
        return this.http.get<RegisterVoter[]>(`/elections/${electionId}/register/contests/${contestId}`);
    }

    stations(electionId: number): Observable<Stations> {
        return this.http.get<Stations>(`/elections/${electionId}/stations`);
    }

    addStation(electionId: number, dto: SaveStation): Observable<Stations> {
        return this.http.post<Stations>(`/elections/${electionId}/stations`, dto);
    }

    updateStation(stationId: number, dto: SaveStation): Observable<Stations> {
        return this.http.put<Stations>(`/elections/stations/${stationId}`, dto);
    }

    deleteStation(stationId: number): Observable<Stations> {
        return this.http.delete<Stations>(`/elections/stations/${stationId}`);
    }

    generateStations(electionId: number): Observable<Stations> {
        return this.http.post<Stations>(`/elections/${electionId}/stations/generate`, {});
    }

    myStations(): Observable<MyStation[]> {
        return this.http.get<MyStation[]>('/elections/polling/mine');
    }

    stationRegister(stationId: number): Observable<StationRegister> {
        return this.http.get<StationRegister>(`/elections/stations/${stationId}/register`);
    }

    ballot(stationId: number, studentId: number): Observable<Ballot> {
        return this.http.get<Ballot>(`/elections/stations/${stationId}/ballot/${studentId}`);
    }

    cast(stationId: number, studentId: number, choices: {contestId: number; candidateIds: number[]}[]): Observable<{voted: string; races: number}> {
        return this.http.post<{voted: string; races: number}>(`/elections/stations/${stationId}/cast`, {studentId, choices});
    }

    results(electionId: number): Observable<Results> {
        return this.http.get<Results>(`/elections/${electionId}/results`);
    }

    resultsCsv(electionId: number): Observable<Blob> {
        return this.http.get(`/elections/${electionId}/results/csv`, {responseType: 'blob'});
    }

    tieBreak(contestId: number, candidateIds: number[]): Observable<Results> {
        return this.http.post<Results>(`/elections/contests/${contestId}/tiebreak`, {candidateIds});
    }

    leadership(electionId: number): Observable<Leadership> {
        return this.http.get<Leadership>(`/elections/${electionId}/leadership`);
    }

    recordLeadership(electionId: number, positions: {positionId: number; classLeadershipRoleId: number | null; responsibilityId: number | null}[]):
        Observable<{added: number; leadership: Leadership}> {
        return this.http.post<{added: number; leadership: Leadership}>(`/elections/${electionId}/leadership`, {positions});
    }
}
