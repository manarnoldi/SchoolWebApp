import { GeneralOutcome } from '@/cbe/assessments/models/general-outcome';
import { EducationLevelType } from '@/school/models/education-level-types';
import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
    selector: 'app-general-outcome-add-form',
    templateUrl: './general-outcome-add-form.component.html',
    styleUrl: './general-outcome-add-form.component.scss'
})
export class GeneralOutcomeAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() generalOutcome: GeneralOutcome;
    @Input() educationLevelTypes: EducationLevelType[];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<GeneralOutcome>();
    @Output() errorEvent = new EventEmitter<string>();

    generalOutcomeForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.generalOutcomeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: [''],
            educationLevelTypeId: [null, [Validators.required]]
        });
    };

    setFormControls = (generalOutcome: GeneralOutcome) => {
        this.generalOutcomeForm.setValue({
            name: generalOutcome.name,
            rank: generalOutcome.rank,
            description: generalOutcome.description,
            educationLevelTypeId: generalOutcome.educationLevelTypeId ?? null
        });
    };

    get f() {
        return this.generalOutcomeForm.controls;
    }

    closeGeneralOutcomeForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (generalOutcome: GeneralOutcome, action: string) => {
        this.generalOutcome = generalOutcome;
        this.setFormControls(generalOutcome);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.generalOutcomeForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let generalOutcomeId = this.generalOutcome.id;
            this.generalOutcome = new GeneralOutcome(
                this.generalOutcomeForm.value
            );
            this.generalOutcome.id = generalOutcomeId;
        } else {
            this.generalOutcome = new GeneralOutcome(
                this.generalOutcomeForm.value
            );
        }
        this.addItemEvent.emit(this.generalOutcome);
    };
}
