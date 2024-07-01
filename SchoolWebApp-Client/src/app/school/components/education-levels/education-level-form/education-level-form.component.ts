import { Curriculum } from '@/academics/models/curriculum';
import { EducationLevelType } from '@/school/models/education-level-types';
import { EducationLevel } from '@/school/models/educationLevel';
import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-education-level-form',
    templateUrl: './education-level-form.component.html',
    styleUrl: './education-level-form.component.scss'
})
export class EducationLevelFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() educationLevel: EducationLevel;
    @Input() educationLevelTypes: EducationLevelType[];
    @Input() curricula: Curriculum[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<EducationLevel>();
    @Output() errorEvent = new EventEmitter<string>();

    educationLevelForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.educationLevelForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbr: ['', [Validators.required]],
            numOfYears: [null, [Validators.required]],
            description: [''],
            educationLevelTypeId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]]
        });
    };

    setFormControls = (educationLevel: EducationLevel) => {
        this.educationLevelForm.setValue({
            name: educationLevel.name,
            abbr: educationLevel.abbr,
            numOfYears: educationLevel.numOfYears,
            description: educationLevel.description,
            educationLevelTypeId: educationLevel.educationLevelTypeId ?? null,
            curriculumId: educationLevel.curriculumId ?? null
        });
    };

    get f() {
        return this.educationLevelForm.controls;
    }

    closeEducationLevelForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (educationLevel: EducationLevel, action: string) => {
        this.educationLevel = educationLevel;
        this.setFormControls(educationLevel);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.educationLevelForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let educationLevelId = this.educationLevel.id;
            this.educationLevel = new EducationLevel(this.educationLevelForm.value);
            this.educationLevel.id = educationLevelId;
        } else {
            this.educationLevel = new EducationLevel(this.educationLevelForm.value);
        }
        this.addItemEvent.emit(this.educationLevel);
    };
}
