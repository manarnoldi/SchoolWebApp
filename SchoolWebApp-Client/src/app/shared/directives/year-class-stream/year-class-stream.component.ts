import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {AcademicYear} from '@/school/models/academic-year';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

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
    @Input() action: string = 'edit';

    @Output() controlsChanged = new EventEmitter<any>();

    constructor(
        private fb: FormBuilder,
        private schoolClassSvc: SchoolClassesService
    ) {}

    ngOnInit(): void {
        this.initializeFormControl();
    }

    initializeFormControl = () => {
        this.f.addControl(
            'learningLevelId',
            this.fb.control(null, Validators.required)
        );
        this.f.addControl(
            'schoolStreamId',
            this.fb.control(null, Validators.required)
        );
        this.f.addControl(
            'academicYearId',
            this.fb.control(null, Validators.required)
        );
    };

    setFormControls = (item: any) => {
        this.f.patchValue({
            learningLevelId: item.learningLevelId,
            schoolStreamId: item.schoolStreamId,
            academicYearId: item.academicYearId
        });
    };

    controlsUpdated = () => {
        if (
            this.f.value.academicYearId &&
            this.f.value.learningLevelId &&
            this.f.value.schoolStreamId
        ) {
            this.controlsChanged.emit({
                academicYearId: this.f.controls.academicYearId.value,
                learningLevelId: this.f.controls.learningLevelId.value,
                schoolStreamId: this.f.controls.schoolStreamId.value
            });
        }
    };

    checkIfExists = (
        academicYearId: number,
        learningLevelId: number,
        schoolStreamId: number
    ) => {
        let urlToSend =
            '/schoolClasses/byYearClassStream?academicYearId=' +
            academicYearId +
            '&learningLevelId=' +
            learningLevelId +
            '&schoolStreamId=' +
            schoolStreamId;
        return this.schoolClassSvc.getByYearClassStream(urlToSend);
    };
}
