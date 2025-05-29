import { Curriculum } from '@/academics/models/curriculum';
import { Grade } from '@/academics/models/grade';
import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-grades-add-form',
    templateUrl: './grades-add-form.component.html',
    styleUrl: './grades-add-form.component.scss'
})
export class GradesAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() grade: Grade;
    @Input() curricula: Curriculum[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<Grade>();
    @Output() errorEvent = new EventEmitter<string>();

    gradeForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.gradeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            abbr: ['', [Validators.required]],
            minScore: [0, [Validators.required]],
            maxScore: [0, [Validators.required]],
            points: [0, [Validators.required]],
            remarksSwa: [''],
            remarksEng: [''],
            curriculumId: [null, [Validators.required]]
        });
    };

    setFormControls = (grade: Grade) => {
        this.gradeForm.setValue({
            name: grade?.name,
            rank: grade?.rank,
            abbr: grade?.abbr,
            minScore: grade?.minScore,
            maxScore: grade?.maxScore,
            points: grade?.points,
            remarksSwa: grade?.remarksSwa,
            remarksEng: grade?.remarksEng,
            curriculumId: grade?.curriculumId
        });
    };

    get f() {
        return this.gradeForm.controls;
    }

    closeGradeForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (grade: Grade, action: string) => {
        this.grade = grade;
        this.setFormControls(grade);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.gradeForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let gradeId = this.grade.id;
            this.grade = new Grade(this.gradeForm.value);
            this.grade.id = gradeId;
        } else {
            this.grade = new Grade(this.gradeForm.value);
        }
        this.addItemEvent.emit(this.grade);
    };
}
