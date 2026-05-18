import {Injectable} from '@angular/core';

// Snapshot of the last filter selections used on the StudentsSubjectsComponent
// page. Kept on a singleton root-provided service so navigating away (e.g. to
// /students/students-subjects/add) and back restores selections without
// forcing the user to redo every dropdown.
export interface StudentsSubjectsFilterState {
    curriculumId?: number | null;
    academicYearId?: number | null;
    educationLevelId?: number | null;
    schoolClassId?: number | null;
    selectedStudentClassId?: number | null;
}

@Injectable({providedIn: 'root'})
export class StudentsSubjectsStateService {
    private state: StudentsSubjectsFilterState | null = null;

    set(state: StudentsSubjectsFilterState): void {
        // Shallow-merge so callers can update one field without erasing others.
        this.state = {...(this.state ?? {}), ...state};
    }

    get(): StudentsSubjectsFilterState | null {
        return this.state;
    }

    clear(): void {
        this.state = null;
    }

    hasUsableFilter(): boolean {
        return !!(this.state && this.state.curriculumId);
    }
}
