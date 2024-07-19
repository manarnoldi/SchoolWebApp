import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {AcademicYear} from '@/school/models/academic-year';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-year-class-stream',
    templateUrl: './year-class-stream.component.html',
    styleUrl: './year-class-stream.component.scss'
})
export class YearClassStreamComponent implements OnInit {
    @Input() f: FormGroup;
    @Input() academicYears: AcademicYear[];
    @Input() learningLevels: LearningLevel[];
    @Input() schoolStreams: SchoolStream[];

    // @Output() setControlsEvent = new EventEmitter<any>();

    constructor(private fb: FormBuilder) {}

    ngOnInit(): void {
        this.initializeFormControl();
    }

    initializeFormControl = () => {
        this.f.addControl('learningLevelId', this.fb.control(null, Validators.required));
        this.f.addControl('schoolStreamId', this.fb.control(null, Validators.required));
        this.f.addControl('academicYearId', this.fb.control(null, Validators.required));
    };

    setFormControls = (item: any) => {
        this.f.patchValue({
            learningLevelId: item.learningLevelId,
            schoolStreamId: item.schoolStreamId,
            academicYearId: item.academicYearId
        });
    };
}
