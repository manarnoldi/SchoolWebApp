import {Curriculum} from '@/academics/models/curriculum';
import {SchoolClass} from '@/class/models/school-class';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {StudentAttendanceSearch} from '@/students/models/student-attendance-search';
import {StudentClass} from '@/students/models/student-class';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-students-attendances-search-form',
    templateUrl: './students-attendances-search-form.component.html',
    styleUrl: './students-attendances-search-form.component.scss'
})
export class StudentsAttendancesSearchFormComponent implements OnInit {
    @Input() curricula: Curriculum[] = [];
    @Input() educationLevels: EducationLevel[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() studentClasses: StudentClass[] = [];

    @Output() searchItemEvent = new EventEmitter<StudentAttendanceSearch>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() educationLevelYearChangedEvent =
        new EventEmitter<EducationLevelYear>();
    @Output() schoolClassChangedEvent = new EventEmitter<number>();
    @Output() dateChangedEvent = new EventEmitter<Date>();

    studentAttendanceSearchForm: FormGroup;
    saSearch: StudentAttendanceSearch;
    today = new Date().toISOString().split('T')[0];

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentAttendanceSearchForm = this.formBuilder.group({
            curriculumId: [null,[Validators.required]],
            educationLevelId: [null,[Validators.required]],
            academicYearId: [null,[Validators.required]],
            schoolClassId: [null,  [Validators.required]],
            attendanceDate: [this.today]
        });
    };

    curriculumChanged = () => {
        this.studentAttendanceSearchForm.get('educationLevelId').reset();
        this.studentAttendanceSearchForm.get('schoolClassId').reset();
        let curriculumId =
            this.studentAttendanceSearchForm.get('curriculumId').value;
        if (!curriculumId || curriculumId == '') return;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    educationLevelYearChanged = () => {
        this.studentAttendanceSearchForm.get('schoolClassId').reset();
        let educationLevelId =
            this.studentAttendanceSearchForm.get('educationLevelId').value;
        let academicYearId =
            this.studentAttendanceSearchForm.get('academicYearId').value;
        if (
            !educationLevelId ||
            educationLevelId == '' ||
            !academicYearId ||
            academicYearId == ''
        )
            return;

        let ely: EducationLevelYear = new EducationLevelYear();
        ely.educationLevelId = educationLevelId;
        ely.academicYearId = academicYearId;
        this.educationLevelYearChangedEvent.emit(ely);
    };

    schoolClassChanged = () => {
        let schoolClassId =
            this.studentAttendanceSearchForm.get('schoolClassId').value;
        if (!schoolClassId || schoolClassId == '') return;
        this.schoolClassChangedEvent.emit(schoolClassId);
    };

    onSubmit = () => {
        this.saSearch = new StudentAttendanceSearch(
            this.studentAttendanceSearchForm.value
        );
        this.searchItemEvent.emit(this.saSearch);
    };

    dateChanged = () => {
        let selectedDate =
            this.studentAttendanceSearchForm.get('attendanceDate').value;
        this.dateChangedEvent.emit(selectedDate);
    };
}
