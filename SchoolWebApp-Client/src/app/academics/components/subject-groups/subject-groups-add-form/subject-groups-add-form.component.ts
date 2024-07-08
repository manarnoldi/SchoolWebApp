import { Curriculum } from '@/academics/models/curriculum';
import { SubjectGroup } from '@/academics/models/subject-group';
import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-subject-groups-add-form',
    templateUrl: './subject-groups-add-form.component.html',
    styleUrl: './subject-groups-add-form.component.scss'
})
export class SubjectGroupsAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() subjectGroup: SubjectGroup;
    @Input() curricula: Curriculum[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<SubjectGroup>();
    @Output() errorEvent = new EventEmitter<string>();

    subjectGroupForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.subjectGroupForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: [''],
            description: [''],
            curriculumId: [null, [Validators.required]]
        });
    };

    setFormControls = (subjectGroup: SubjectGroup) => {
        this.subjectGroupForm.setValue({
            name: subjectGroup.name,
            abbreviation: subjectGroup.abbreviation,
            description: subjectGroup.description,
            curriculumId: subjectGroup?.curriculumId
        });
    };

    get f() {
        return this.subjectGroupForm.controls;
    }

    closeSubjectGroupForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (subjectGroup: SubjectGroup, action: string) => {
        this.subjectGroup = subjectGroup;
        this.setFormControls(subjectGroup);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.subjectGroupForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let subjectGroupId = this.subjectGroup.id;
            this.subjectGroup = new SubjectGroup(
                this.subjectGroupForm.value
            );
            this.subjectGroup.id = subjectGroupId;
        } else {
            this.subjectGroup = new SubjectGroup(
                this.subjectGroupForm.value
            );
        }
        this.addItemEvent.emit(this.subjectGroup);
    };
}
