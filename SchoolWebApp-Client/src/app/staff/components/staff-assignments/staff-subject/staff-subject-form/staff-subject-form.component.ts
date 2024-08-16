import {Subject} from '@/academics/models/subject';
import {LearningLevel} from '@/class/models/learning-level';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {AcademicYear} from '@/school/models/academic-year';
import {YearClassStreamComponent} from '@/shared/directives/year-class-stream/year-class-stream.component';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffSubject} from '@/staff/models/staff-subject';
import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';

@Component({
    selector: 'app-staff-subject-form',
    templateUrl: './staff-subject-form.component.html',
    styleUrl: './staff-subject-form.component.scss'
})
export class StaffSubjectFormComponent implements OnInit, AfterViewInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @ViewChild('yearClassStream')
    yearClassStreamComponent: YearClassStreamComponent;
    buttonSubmitActive: boolean = true;
    @Input() staffSubject: StaffSubject;
    @Input() statuses;
    @Input() staff: StaffDetails;

    @Input() subjects: Subject[] = [];

    @Input() academicYears: AcademicYear[] = [];
    @Input() learningLevels: LearningLevel[] = [];
    @Input() schoolStreams: SchoolStream[] = [];

    action: string = 'add';

    @Output() addItemEvent = new EventEmitter<StaffSubject>();
    @Output() errorEvent = new EventEmitter<string>();

    staffSubjectForm: FormGroup;

    constructor(
        private formBuilder: FormBuilder,
        private toastrSvc: ToastrService,
        private schoolClassSvc: SchoolClassesService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    ngAfterViewInit(): void {
        this.yearClassStreamComponent.initializeFormControl();
    }
    refreshItems = () => {
        this.staffSubjectForm = this.formBuilder.group({
            staffDetailsId: [this.staff?.id, [Validators.required]],
            description: [''],
            subjectId: [null]
        });
    };

    setFormControls = (staffSubject: StaffSubject) => {
        this.schoolClassSvc
            .getById(staffSubject.schoolClassId, '/schoolClasses')
            .subscribe(
                (sClass: SchoolClass) => {
                    this.staffSubjectForm.patchValue({
                        description: staffSubject.description,
                        subjectId: staffSubject.subjectId ?? null,
                        staffDetailsId: staffSubject.staffDetailsId ?? null
                    });
                    this.yearClassStreamComponent.setFormControls({
                        academicYearId: sClass.academicYearId,
                        learningLevelId: sClass.learningLevelId,
                        schoolStreamId: sClass.schoolStreamId
                    });
                },
                (err) => {
                    this.toastrSvc.error(err.error?.message);
                }
            );
    };

    get f() {
        return this.staffSubjectForm.controls;
    }

    closeStaffSubjectForm = () => {
        this.resetFormControls();
    };

    resetFormControls() {
        this.action = 'add';
        this.staffSubjectForm.reset();
        this.staffSubjectForm.patchValue({staffDetailsId: this.staff?.id});
    }

    yearClassStreamUpdated = (yearClassStream) => {
        this.yearClassStreamComponent
            .checkIfExists(
                yearClassStream.academicYearId,
                yearClassStream.learningLevelId,
                yearClassStream.schoolStreamId
            )
            .subscribe(
                (schoolCl) => {
                    this.buttonSubmitActive = true;
                    if (!schoolCl) {
                        this.toastrSvc.error(
                            'The class selected is not added yet. Ask administrator to register the class!'
                        );
                        this.buttonSubmitActive = false;
                    }
                },
                (err) => {
                    this.buttonSubmitActive = false;
                    this.toastrSvc.error(
                        'The class selected is not added yet. Ask administrator to register the class!'
                    );
                }
            );
    };

    goToRegisteredClasses = () => {
        this.closeButton.nativeElement.click();
        this.router.navigate(['/class/classes']);
    };

    onSubmit = () => {
        this.yearClassStreamComponent
            ?.checkIfExists(
                this.staffSubjectForm.value?.academicYearId,
                this.staffSubjectForm.value?.learningLevelId,
                this.staffSubjectForm.value?.schoolStreamId
            )
            .subscribe(
                (schoolCl) => {
                    if (this.action == 'edit') {
                        let staffSubjectId = this.staffSubject.id;
                        this.staffSubject = new StaffSubject(
                            this.staffSubjectForm.value
                        );
                        this.staffSubject.schoolClassId = parseInt(schoolCl.id);
                        this.staffSubject.id = staffSubjectId;
                    } else {
                        this.staffSubject = new StaffSubject(
                            this.staffSubjectForm.value
                        );
                        this.staffSubject.schoolClassId = parseInt(schoolCl.id);
                    }
                    
                    this.addItemEvent.emit(this.staffSubject);
                },
                (err) => {
                    this.toastrSvc.error(err.error?.message);
                }
            );
    };
}
