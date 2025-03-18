import {EducationLevelSubject} from '@/academics/models/education-level-subject';
import {Subject} from '@/academics/models/subject';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {SubjectsMinTableComponent} from '@/shared/components/subjects-min-table/subjects-min-table.component';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-education-level-subjects-form',
    templateUrl: './education-level-subjects-form.component.html',
    styleUrl: './education-level-subjects-form.component.scss'
})
export class EducationLevelSubjectsFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @ViewChild(SubjectsMinTableComponent)
    subjectsMinTableComponent: SubjectsMinTableComponent;

    @Input() academicYears: AcademicYear[] = [];
    @Input() educationLevels: EducationLevel[] = [];
    @Input() subjects: Subject[] = [];
    @Input() isLoading: boolean = true;

    educationLevelSubject: EducationLevelSubject;

    editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<EducationLevelSubject[]>();
    @Output() selectChangeEvent = new EventEmitter<EducationLevelYear>();
    @Output() errorEvent = new EventEmitter<string>();

    educationLevelSubjectForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.educationLevelSubjectForm = this.formBuilder.group({
            academicYearId: [null, Validators.required],
            educationLevelId: [null, Validators.required],
            description: ['']
        });
        this.subjects.forEach((s) => (s.isSelected = false));
        if (this.subjectsMinTableComponent)
            this.subjectsMinTableComponent.checkAll.nativeElement.checked =
                false;
    };

    get f() {
        return this.educationLevelSubjectForm.controls;
    }

    setFormControls = (educationLevelSubject: EducationLevelSubject) => {
        this.educationLevelSubjectForm.setValue({
            academicYearId: educationLevelSubject?.academicYearId ?? null,
            educationLevelId: educationLevelSubject?.educationLevelId ?? null,
            description: educationLevelSubject?.description ?? ''
        });
    };

    closeEducationLevelSubjectsForm = () => {
        this.closeButton.nativeElement.click();
        this.refreshItems();
    };

    viewItem = (
        educationLevelSubject: EducationLevelSubject,
        action: string
    ) => {
        this.educationLevelSubject = educationLevelSubject;
        this.setFormControls(educationLevelSubject);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.educationLevelSubjectForm.reset();
    }

    onSubmit = () => {
        let educationLevelSubjects: EducationLevelSubject[] = [];
        this.subjects.forEach((s) => {
            if (s.isSelected && !s.isOriginallySelected) {
                educationLevelSubjects.push(
                    new EducationLevelSubject({
                        subjectId: parseInt(s.id),
                        academicYearId: parseInt(
                            this.educationLevelSubjectForm.value?.academicYearId
                        ),
                        educationLevelId: parseInt(
                            this.educationLevelSubjectForm.value
                                ?.educationLevelId
                        )
                    })
                );
            }
        });
        this.addItemEvent.emit(educationLevelSubjects);
    };

    eduLevelYearChanged = () => {
        let yearId = this.educationLevelSubjectForm.get('academicYearId').value;
        let eduLevelId =
            this.educationLevelSubjectForm.get('educationLevelId').value;

        this.subjects.forEach((s) => (s.isSelected = false));
        if (this.subjectsMinTableComponent)
            this.subjectsMinTableComponent.checkAll.nativeElement.checked =
                false;
        if (!yearId || !eduLevelId) {
        } else {
            let el = new EducationLevelYear();
            el.academicYearId = yearId;
            el.educationLevelId = eduLevelId;
            this.selectChangeEvent.emit(el);
        }
    };
}
