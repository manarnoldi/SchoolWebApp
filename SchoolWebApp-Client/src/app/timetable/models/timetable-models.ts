// Enum values match the API (SchoolWebApp.Core.Entities.Timetable).
export enum BellSlotKind {
    Lesson = 0,
    Break = 1,
    Lunch = 2,
    Activity = 3
}

export enum DayPart {
    Morning = 0,
    Afternoon = 1
}

export enum SubjectPlacement {
    Any = 0,
    PreferMorning = 1,
    MorningOnly = 2,
    PreferAfternoon = 3,
    AfternoonOnly = 4
}

export const SLOT_KINDS: {value: BellSlotKind; label: string}[] = [
    {value: BellSlotKind.Lesson, label: 'Lesson'},
    {value: BellSlotKind.Break, label: 'Break'},
    {value: BellSlotKind.Lunch, label: 'Lunch'},
    {value: BellSlotKind.Activity, label: 'Activity'}
];

export const PLACEMENTS: {value: SubjectPlacement; label: string}[] = [
    {value: SubjectPlacement.Any, label: 'Any time'},
    {value: SubjectPlacement.PreferMorning, label: 'Prefer morning'},
    {value: SubjectPlacement.MorningOnly, label: 'Morning only'},
    {value: SubjectPlacement.PreferAfternoon, label: 'Prefer afternoon'},
    {value: SubjectPlacement.AfternoonOnly, label: 'Afternoon only'}
];

// ISO day numbers, as the API stores them.
export const WEEK_DAYS: {value: number; short: string; name: string}[] = [
    {value: 1, short: 'Mon', name: 'Monday'},
    {value: 2, short: 'Tue', name: 'Tuesday'},
    {value: 3, short: 'Wed', name: 'Wednesday'},
    {value: 4, short: 'Thu', name: 'Thursday'},
    {value: 5, short: 'Fri', name: 'Friday'},
    {value: 6, short: 'Sat', name: 'Saturday'}
];

export interface TimetableLookups {
    levels: {id: number; name: string; educationLevel: string; rank: number}[];
    subjects: {id: number; name: string; abbr: string}[];
    teachers: {id: number; name: string}[];
    sessions: {id: number; name: string; startDate: string; endDate: string}[];
    currentSessionId: number | null;
}

export interface BellSlot {
    id: number;
    startTime: string;
    endTime: string;
    kind: BellSlotKind;
    part: DayPart;
    label: string | null;
}

export interface FixedSlot {
    id: number;
    day: number;
    slotIndex: number;
    learningLevelId: number | null;
    subjectId: number | null;
    label: string | null;
}

export interface BellSchedule {
    id: number;
    name: string;
    description: string | null;
    days: number[];
    slots: BellSlot[];
    learningLevelIds: number[];
    fixedSlots: FixedSlot[];
}

export interface SubjectRule {
    id: number;
    subjectId: number;
    learningLevelId: number | null;
    excluded: boolean;
    placement: SubjectPlacement;
    doubleLessons: number;
    maxPerDay: number;
    avoidLastLesson: boolean;
}

export interface OptionGroup {
    id: number;
    name: string;
    learningLevelId: number | null;
    subjectIds: number[];
}

export interface TeacherUnavailability {
    id: number;
    day: number;
    startTime: string;
    endTime: string;
    reason: string | null;
}

export interface TeacherRule {
    staffDetailsId: number;
    staffName: string | null;
    maxLessonsPerDay: number | null;
    maxConsecutiveLessons: number | null;
    notes: string | null;
    unavailable: TeacherUnavailability[];
}

export interface Readiness {
    sessionId: number;
    ready: boolean;
    problems: string[];
    warnings: string[];
    leftOut: string[];
    classes: {
        schoolClassId: number;
        className: string;
        learningLevelId: number;
        bellScheduleName: string | null;
        lessonsNeeded: number;
        slotsAvailable: number;
        subjectsWithoutTeacher: string[];
        subjects: {subjectId: number; subjectName: string; lessonsPerWeek: number; staffDetailsId: number | null;
            teacherName: string | null; excluded: boolean}[];
    }[];
    teachers: {staffDetailsId: number; name: string; lessonsAssigned: number; slotsAvailable: number; classes: number}[];
}
