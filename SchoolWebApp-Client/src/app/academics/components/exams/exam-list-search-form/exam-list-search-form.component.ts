import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumYear} from '@/academics/models/curriculum-year';
import {ExamSearch} from '@/academics/models/exam-search';
import {Subject} from '@/academics/models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {AcademicYear} from '@/school/models/academic-year';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-exam-list-search-form',
    templateUrl: './exam-list-search-form.component.html',
    styleUrl: './exam-list-search-form.component.scss'
})
export class ExamListSearchFormComponent implements OnInit {
    @Input() academicYears: AcademicYear[] = [];
    @Input() curricula: Curriculum[] = [];
    @Input() sessions: Session[] = [];
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() subjects: Subject[] = [];

    @Output() searchItemEvent = new EventEmitter<ExamSearch>();
    @Output() curriculumYearChangedEvent = new EventEmitter<CurriculumYear>();
    @Output() clearListEvent = new EventEmitter<void>();

    examListSearchForm: FormGroup;
    examSearch: ExamSearch;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.examListSearchForm = this.formBuilder.group({
            examTypeId: [null],
            academicYearId: [null],
            schoolClassId: [null],
            sessionId: [null],
            subjectId: [null],
            curriculumId: [null]
        });
    };

    clearList = () => {
        this.clearListEvent.emit();
     };
    
    onSubmit = () => {
        this.examSearch = new ExamSearch(this.examListSearchForm.value);
        this.searchItemEvent.emit(this.examSearch);
    };

    academicYearCurriculumChanged = () => {
        let curriculumYearId =
            this.examListSearchForm.get('curriculumId').value;
        let academicYearId =
            this.examListSearchForm.get('academicYearId').value;

        if (
            !curriculumYearId ||
            curriculumYearId == null ||
            !academicYearId ||
            academicYearId == null
        ) {
            return;
        }
        const curriculumYear = new CurriculumYear(
            curriculumYearId,
            academicYearId
        );
        this.curriculumYearChangedEvent.emit(curriculumYear);
    };
}
