import {Curriculum} from '@/academics/models/curriculum';
import {DateLessThanOrEqualsValidator} from '@/core/validators.ts/DateValidators';
import {AcademicYear} from '@/school/models/academic-year';
import {Session} from '@/school/models/session';
import {SessionType} from '@/settings/models/session-type';
import {formatDate} from '@angular/common';
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
    selector: 'app-session-form',
    templateUrl: './session-form.component.html',
    styleUrl: './session-form.component.scss'
})
export class SessionFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() session: Session;
    @Input() academicYears: AcademicYear[];
    @Input() curricula: Curriculum[] = [];
    @Input() sessionTypes: SessionType[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<Session>();
    @Output() errorEvent = new EventEmitter<string>();

    sessionForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.sessionForm = this.formBuilder.group({
            sessionName: ['', [Validators.required]],
            abbreviation: ['', [Validators.required]],
            startDate: [
                formatDate(new Date(), 'yyyy-MM-dd', 'en'),
                [
                    Validators.required,
                    DateLessThanOrEqualsValidator('endDate', 'greater')
                ]
            ],
            endDate: [
                formatDate(new Date(), 'yyyy-MM-dd', 'en'),
                [Validators.required]
            ],
            status: [false],
            academicYearId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]],
            sessionTypeId: [null, [Validators.required]]
        });
    };

    setFormControls = (session: Session) => {
        this.sessionForm.setValue({
            sessionName: session.sessionName,
            abbreviation: session.abbreviation,
            startDate: formatDate(
                new Date(session.startDate),
                'yyyy-MM-dd',
                'en'
            ),
            endDate: formatDate(new Date(session.endDate), 'yyyy-MM-dd', 'en'),
            status: session.status,
            academicYearId: session.academicYearId ?? null,
            curriculumId: session.curriculumId ?? null,
            sessionTypeId: session.sessionTypeId ?? null
        });
    };

    get f() {
        return this.sessionForm.controls;
    }

    closeSessionForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (session: Session, action: string) => {
        this.session = session;
        this.setFormControls(session);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.sessionForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let sessionId = this.session.id;
            this.session = new Session(this.sessionForm.value);
            this.session.id = sessionId;
        } else {
            this.session = new Session(this.sessionForm.value);
        }
        this.addItemEvent.emit(this.session);
    };
}
