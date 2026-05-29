import {Curriculum} from '@/academics/models/curriculum';
import {SchoolClass} from '@/class/models/school-class';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {StudentSubjectSearch} from '@/students/models/student-subject-search';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
    selector: 'app-students-subjects-search-form',
    templateUrl: './students-subjects-search-form.component.html',
    styleUrl: './students-subjects-search-form.component.scss'
})
export class StudentsSubjectsSearchFormComponent implements OnInit {
    @Input() curricula: Curriculum[] = [];
    @Input() educationLevels: EducationLevel[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() studentClasses: StudentClass[] = [];
    // When false, hides the per-student dropdown - useful when the parent
    // page renders the full student list instead (with click-to-select).
    @Input() showStudent: boolean = true;

    @Output() searchItemEvent = new EventEmitter<StudentSubjectSearch>();
    // Emit null when the user clears the upstream dropdown so the parent can
    // drop dependent state (rows, button-gating ids, etc.) instead of being
    // stuck with stale selections after a -Curriculum-/-Level-/-Class- pick.
    @Output() curriculumChangedEvent = new EventEmitter<number | null>();
    @Output() educationLevelYearChangedEvent =
        new EventEmitter<EducationLevelYear | null>();
    @Output() schoolClassChangedEvent = new EventEmitter<number | null>();
    @Output() studentChangedEvent = new EventEmitter<void>();

    studentsSubjectsSearchForm: FormGroup;
    ssSearch: StudentSubjectSearch;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentsSubjectsSearchForm = this.formBuilder.group({
            curriculumId: [null],
            educationLevelId: [null],
            academicYearId: [null],
            schoolClassId: [null],
            studentClassId: [null]
        });
    };

    curriculumChanged = () => {
        let curriculumId =
            this.studentsSubjectsSearchForm.get('curriculumId').value;
        // Always emit — null signals "user cleared the dropdown" so the
        // parent can drop dependent state instead of holding stale ids.
        if (!curriculumId || curriculumId == '') {
            // Cascade-clear downstream controls so re-selecting an upstream
            // value can't leave a stale child selection that fails to fire a
            // (change) event the second time around.
            this.studentsSubjectsSearchForm.patchValue({
                academicYearId: null,
                educationLevelId: null,
                schoolClassId: null,
                studentClassId: null
            });
            this.curriculumChangedEvent.emit(null);
            return;
        }
        this.curriculumChangedEvent.emit(curriculumId);
    };

    educationLevelYearChanged = () => {
        let educationLevelId =
            this.studentsSubjectsSearchForm.get('educationLevelId').value;
        let academicYearId =
            this.studentsSubjectsSearchForm.get('academicYearId').value;
        if (
            !educationLevelId ||
            educationLevelId == '' ||
            !academicYearId ||
            academicYearId == ''
        ) {
            // Drop the now-orphaned class/student selection so re-picking
            // year+level later doesn't show a stale child silently.
            this.studentsSubjectsSearchForm.patchValue({
                schoolClassId: null,
                studentClassId: null
            });
            this.educationLevelYearChangedEvent.emit(null);
            return;
        }

        let ely: EducationLevelYear = new EducationLevelYear();
        ely.educationLevelId = educationLevelId;
        ely.academicYearId = academicYearId;
        this.educationLevelYearChangedEvent.emit(ely);
    };

    schoolClassChanged = () => {
        this.studentsSubjectsSearchForm.get('studentClassId').reset();
        let schoolClassId =
            this.studentsSubjectsSearchForm.get('schoolClassId').value;
        if (!schoolClassId || schoolClassId == '') {
            this.schoolClassChangedEvent.emit(null);
            return;
        }
        this.schoolClassChangedEvent.emit(schoolClassId);
    };

    studentsChanged = () => {
        this.studentChangedEvent.emit();
    };

    onSubmit = () => {
        this.ssSearch = new StudentSubjectSearch(
            this.studentsSubjectsSearchForm.value
        );
        this.searchItemEvent.emit(this.ssSearch);
    };

    // Replays a previously-saved selection into the dropdowns. Used by the
    // parent to restore filter state when the user navigates back to this
    // page. Only patches the FormGroup - the parent re-runs the data-loading
    // cascade so the dependent dropdowns repopulate in the correct order.
    setFormValues = (values: {
        curriculumId?: number | null;
        academicYearId?: number | null;
        educationLevelId?: number | null;
        schoolClassId?: number | null;
    }) => {
        if (!this.studentsSubjectsSearchForm) return;
        this.studentsSubjectsSearchForm.patchValue({
            curriculumId: values.curriculumId ?? null,
            academicYearId: values.academicYearId ?? null,
            educationLevelId: values.educationLevelId ?? null,
            schoolClassId: values.schoolClassId ?? null
        });
    };
}
