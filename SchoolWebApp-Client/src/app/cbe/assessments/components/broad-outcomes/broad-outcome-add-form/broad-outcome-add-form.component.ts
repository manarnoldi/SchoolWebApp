import { BroadOutcome } from '@/cbe/assessments/models/broad-outcome';
import { EducationLevel } from '@/school/models/educationLevel';
import { Subject } from '@/academics/models/subject';
import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
    selector: 'app-broad-outcome-add-form',
    templateUrl: './broad-outcome-add-form.component.html',
    styleUrl: './broad-outcome-add-form.component.scss'
})
export class BroadOutcomeAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() broadOutcome: BroadOutcome;
    @Input() educationLevels: EducationLevel[];
    @Input() subjects: Subject[];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<BroadOutcome>();
    @Output() errorEvent = new EventEmitter<string>();

    broadOutcomeForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.broadOutcomeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: [''],
            rank: [0, [Validators.required]],
            educationLevelId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]]
        });
    };

    setFormControls = (broadOutcome: BroadOutcome) => {
        this.broadOutcomeForm.setValue({
            name: broadOutcome.name,
            description: broadOutcome.description,
            rank: broadOutcome.rank,
            educationLevelId: broadOutcome.educationLevelId ?? null,
            subjectId: broadOutcome.subjectId ?? null
        });
    };

    get f() {
        return this.broadOutcomeForm.controls;
    }

    closeBroadOutcomeForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (broadOutcome: BroadOutcome, action: string) => {
        this.broadOutcome = broadOutcome;
        this.setFormControls(broadOutcome);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.broadOutcomeForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let broadOutcomeId = this.broadOutcome.id;
            this.broadOutcome = new BroadOutcome(
                this.broadOutcomeForm.value
            );
            this.broadOutcome.id = broadOutcomeId;
        } else {
            this.broadOutcome = new BroadOutcome(
                this.broadOutcomeForm.value
            );
        }
        this.addItemEvent.emit(this.broadOutcome);
    };
}
