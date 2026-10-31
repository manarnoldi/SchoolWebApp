import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {TeachingSession} from '@/teaching/models/teaching-models';

export type AssessmentTrackStatus = 'UpToDate' | 'Incomplete' | 'Behind' | 'NotStarted' | 'NothingDue' | 'NoScheme';

export const TRACK_STATUS_LABELS: Record<AssessmentTrackStatus, string> = {
    UpToDate: 'Up to date',
    Incomplete: 'Incomplete',
    Behind: 'Behind',
    NotStarted: 'Not started',
    NothingDue: 'Nothing due yet',
    NoScheme: 'No scheme'
};

export const TRACK_STATUS_CLASSES: Record<AssessmentTrackStatus, string> = {
    UpToDate: 'bg-success-subtle text-success-emphasis',
    Incomplete: 'bg-warning-subtle text-warning-emphasis',
    Behind: 'bg-danger-subtle text-danger-emphasis',
    NotStarted: 'text-bg-danger',
    NothingDue: 'bg-light text-muted border',
    NoScheme: 'bg-secondary-subtle text-secondary-emphasis'
};

export const TRACK_STATUS_HELP: Record<AssessmentTrackStatus, string> = {
    UpToDate: 'Every outcome due has been graded for the class',
    Incomplete: 'Every outcome due has grades, but some only for part of the class',
    Behind: 'Some outcomes due have no grades yet',
    NotStarted: 'Outcomes are due and none has been graded',
    NothingDue: 'The scheme has nothing due for assessment yet',
    NoScheme: 'No scheme of work this term, so nothing says what is due'
};

export interface AssessmentTrackerRow {
    staffDetailsId: number;
    staffName: string;
    subjectId: number;
    subjectName: string;
    subjectRank: number;
    schoolClassId: number;
    className: string;
    learningLevelId: number;
    learningLevelName: string;
    learningLevelRank: number;
    departmentId: number;
    departmentName: string | null;
    schemeOfWorkId: number | null;
    learners: number;
    subStrandsPlanned: number;
    subStrandsDue: number;
    outcomesDue: number;
    outcomesGraded: number;
    outcomesPartlyGraded: number;
    outcomesOverdue: number;
    gradesEntered: number;
    gradesExpected: number;
    outcomesGradedThisTerm: number;
    nextSubStrand: string | null;
    nextDueAt: string | null;
    lastEntry: string | null;
    status: AssessmentTrackStatus;
}

export interface AssessmentTracker {
    session: TeachingSession;
    curriculumId: number;
    scope: 'school' | 'department' | 'own';
    graceDays: number;
    rows: AssessmentTrackerRow[];
}

export interface AssessmentTrackerDetail {
    learners: number;
    subStrands: {
        strandId: number;
        strandName: string;
        subStrandId: number;
        subStrandName: string;
        lastWeek: number | null;
        dueAt: string | null;
        due: boolean;
        inScheme: boolean;
        outcomes: {id: number; name: string; graded: number}[];
    }[];
    learnersMissing: {studentId: number; name: string; upi: string | null; missing: number}[];
}

@Injectable({providedIn: 'root'})
export class AssessmentTrackerService {
    constructor(private http: HttpClient) {}

    tracker(sessionId: number): Observable<AssessmentTracker> {
        return this.http.get<AssessmentTracker>('/assessmentTracker', {params: {sessionId}});
    }

    detail(sessionId: number, r: AssessmentTrackerRow): Observable<AssessmentTrackerDetail> {
        return this.http.get<AssessmentTrackerDetail>('/assessmentTracker/detail', {
            params: {sessionId, schoolClassId: r.schoolClassId, subjectId: r.subjectId, staffDetailsId: r.staffDetailsId}
        });
    }
}
