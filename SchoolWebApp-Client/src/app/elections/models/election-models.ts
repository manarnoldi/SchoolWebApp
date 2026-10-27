// Mirrors SchoolWebApp.Core.Entities.Elections (enums sent as numbers).

export enum ElectionStage {
    Draft = 0,
    Nominations = 1,
    CandidatesFinal = 2,
    Polling = 3,
    PollingClosed = 4,
    Declared = 5
}

export enum VoterRegisterMode {
    Frozen = 0,
    Live = 1
}

export enum PositionScope {
    Single = 0,
    PerEducationLevel = 1,
    PerGrade = 2,
    PerClass = 3
}

export enum PositionGenderRule {
    None = 0,
    SingleGender = 1,
    SeatsPerGender = 2
}

export const STAGE_LABELS: {[s: number]: string} = {
    [ElectionStage.Draft]: 'Draft',
    [ElectionStage.Nominations]: 'Nominations open',
    [ElectionStage.CandidatesFinal]: 'Candidates final',
    [ElectionStage.Polling]: 'Polling open',
    [ElectionStage.PollingClosed]: 'Polling closed',
    [ElectionStage.Declared]: 'Results declared'
};

export const STAGE_BADGES: {[s: number]: string} = {
    [ElectionStage.Draft]: 'bg-secondary',
    [ElectionStage.Nominations]: 'bg-info',
    [ElectionStage.CandidatesFinal]: 'bg-primary',
    [ElectionStage.Polling]: 'bg-success',
    [ElectionStage.PollingClosed]: 'bg-warning text-dark',
    [ElectionStage.Declared]: 'bg-dark'
};

// The button wording for moving to a stage.
export const STAGE_ACTIONS: {[s: number]: string} = {
    [ElectionStage.Draft]: 'Back to draft',
    [ElectionStage.Nominations]: 'Open nominations',
    [ElectionStage.CandidatesFinal]: 'Close nominations',
    [ElectionStage.Polling]: 'Open polling',
    [ElectionStage.PollingClosed]: 'Close polling',
    [ElectionStage.Declared]: 'Declare results'
};

export const SCOPE_LABELS: {[s: number]: string} = {
    [PositionScope.Single]: 'One for the whole school',
    [PositionScope.PerEducationLevel]: 'One per school section',
    [PositionScope.PerGrade]: 'One per grade',
    [PositionScope.PerClass]: 'One per class'
};

export const GENDER_RULE_LABELS: {[s: number]: string} = {
    [PositionGenderRule.None]: 'No gender rule',
    [PositionGenderRule.SingleGender]: 'Candidates of one gender',
    [PositionGenderRule.SeatsPerGender]: 'Seats for each gender'
};

export interface IdName {
    id: number;
    name: string;
}

export interface ElectionSection {
    id: number;
    name: string;
    grades: IdName[];
}

export interface ElectionOptions {
    canManage: boolean;
    academicYears: IdName[];
    currentAcademicYearId: number | null;
    sections: ElectionSection[];
    genders: IdName[];
    defaultRegisterMode: VoterRegisterMode;
    defaultRefreshSeconds: number;
}

export interface ElectionSummary {
    id: number;
    name: string;
    electionType: number;
    academicYearId: number;
    academicYearName: string;
    stage: ElectionStage;
    positions: number;
    contests: number;
    pollingStart: string | null;
    pollingEnd: string | null;
}

export interface ElectionContest {
    id: number;
    name: string;
    educationLevelId: number | null;
    learningLevelId: number | null;
    schoolClassId: number | null;
}

export interface ElectionPosition {
    id: number;
    name: string;
    runningMateTitle: string | null;
    scope: PositionScope;
    seats: number;
    genderRule: PositionGenderRule;
    genderId: number | null;
    genderName: string | null;
    votersSameGenderOnly: boolean;
    rank: number;
    voterLevelIds: number[];
    candidateLevelIds: number[];
    contests: ElectionContest[];
}

export interface ElectionStageChange {
    fromStage: ElectionStage;
    toStage: ElectionStage;
    note: string | null;
    at: string | null;
    by: string | null;
}

export interface ElectionDetail extends ElectionSummary {
    description: string | null;
    registerMode: VoterRegisterMode;
    registerFrozenAt: string | null;
    dashboardRefreshSeconds: number;
    declaredAt: string | null;
    declaredBy: string | null;
    positionsEditable: boolean;
    candidates: number;
    pendingNominations: number;
    nextStages: ElectionStage[];
    positionList: ElectionPosition[];
    history: ElectionStageChange[];
}

export interface SaveElection {
    name: string;
    description: string | null;
    academicYearId: number | null;
    registerMode: VoterRegisterMode;
    pollingStart: string | null;
    pollingEnd: string | null;
    dashboardRefreshSeconds: number;
}

export interface SavePosition {
    name: string;
    runningMateTitle: string | null;
    scope: PositionScope;
    seats: number;
    genderRule: PositionGenderRule;
    genderId: number | null;
    votersSameGenderOnly: boolean;
    rank: number;
    voterLevelIds: number[];
    candidateLevelIds: number[];
}

// The API answers a refused change with a plain message.
export function apiError(err: any, fallback: string): string {
    if (typeof err?.error === 'string' && err.error) return err.error;
    return err?.error?.message || err?.error?.title || fallback;
}

// Nominations

export enum CandidateStatus {
    Nominated = 0,
    Approved = 1,
    Rejected = 2,
    Withdrawn = 3
}

export const CANDIDATE_STATUS_LABELS: {[s: number]: string} = {
    [CandidateStatus.Nominated]: 'Awaiting approval',
    [CandidateStatus.Approved]: 'Approved',
    [CandidateStatus.Rejected]: 'Rejected',
    [CandidateStatus.Withdrawn]: 'Withdrawn'
};

export const CANDIDATE_STATUS_BADGES: {[s: number]: string} = {
    [CandidateStatus.Nominated]: 'bg-warning text-dark',
    [CandidateStatus.Approved]: 'bg-success',
    [CandidateStatus.Rejected]: 'bg-danger',
    [CandidateStatus.Withdrawn]: 'bg-secondary'
};

export interface Candidate {
    id: number;
    contestId: number;
    studentId: number;
    studentName: string;
    upi: string;
    className: string;
    genderName: string;
    runningMateStudentId: number | null;
    runningMateName: string | null;
    runningMateClassName: string | null;
    slogan: string | null;
    photoUrl: string | null;
    runningMatePhotoUrl: string | null;
    status: CandidateStatus;
    statusNote: string | null;
    ballotOrder: number;
    nominatedBy: string | null;
    nominatedAt: string | null;
}

export interface NominationContest {
    id: number;
    name: string;
    candidates: Candidate[];
}

export interface NominationPosition {
    id: number;
    name: string;
    runningMateTitle: string | null;
    seats: number;
    genderRule: PositionGenderRule;
    genderName: string | null;
    contests: NominationContest[];
}

export interface Nominations {
    electionId: number;
    electionName: string;
    stage: ElectionStage;
    canManage: boolean;
    canNominate: boolean;
    canChangePhotos: boolean;
    positions: NominationPosition[];
}

export interface EligibleStudent {
    studentId: number;
    name: string;
    upi: string;
    className: string;
    genderName: string;
    taken: string | null;
}

export interface SaveCandidate {
    studentId: number;
    runningMateStudentId: number | null;
    slogan: string | null;
}

// Voter register

export interface RegisterSummary {
    mode: VoterRegisterMode;
    frozenAt: string | null;
    students: number;
    contests: {contestId: number; positionName: string; name: string; voters: number}[];
}

export interface RegisterVoter {
    studentId: number;
    name: string;
    upi: string;
    className: string;
    genderName: string;
}
