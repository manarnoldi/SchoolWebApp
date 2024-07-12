import {AcademicYear} from '@/school/models/academic-year';
import {SchoolEvent} from '@/school/models/schoolEvent';
import {Session} from '@/class/models/session';
import {DatePipe, formatDate} from '@angular/common';
import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';

@Component({
    selector: 'app-events-add-form',
    templateUrl: './events-add-form.component.html',
    styleUrl: './events-add-form.component.scss'
})
export class EventsAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() event: SchoolEvent;
    @Input() sessions: Session[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<SchoolEvent>();
    @Output() errorEvent = new EventEmitter<string>();

    eventForm: FormGroup;
    sessionsForFiltering: Session[] = [];

    constructor(
        private formBuilder: FormBuilder,
        private datePipe: DatePipe
    ) {}

    ngOnInit(): void {
        this.initializeForm();
    }

    initializeForm = () => {
        this.sessionsForFiltering = [];
        this.eventForm = this.formBuilder.group({
            eventName: ['', [Validators.required]],
            eventLocation: ['', [Validators.required]],
            startDate: [
                this.datePipe.transform(new Date(), 'yyyy-MM-dd'),
                [Validators.required]
            ],
            endDate: [
                this.datePipe.transform(new Date(), 'yyyy-MM-dd'),
                [Validators.required]
            ],
            description: [''],
            sessionId: [null, [Validators.required]],
            academicYearId: [null, [Validators.required]]
        });
    };

    loadSessions = () => {
        this.eventForm.controls['sessionId'].reset();
        let acadYearId = this.eventForm.get('academicYearId').value;
        this.sessionsForFiltering = [];
        this.sessionsForFiltering = this.sessions;
        this.sessionsForFiltering = this.sessionsForFiltering.filter(
            (s) => s.academicYearId == acadYearId
        );
    };

    setFormControls = (event: SchoolEvent) => {
        let acadYearId = this.sessions.find(
            (s) => s.id == event.sessionId.toString()
        ).academicYearId;
        this.sessionsForFiltering = this.sessions;
        this.sessionsForFiltering = this.sessionsForFiltering.filter(
            (s) => s.academicYearId == acadYearId
        );
        this.eventForm.setValue({
            eventName: event?.eventName,
            eventLocation: event?.eventLocation,
            startDate: formatDate(new Date(event.startDate), 'yyyy-MM-dd', 'en'),
            endDate: formatDate(new Date(event.endDate), 'yyyy-MM-dd', 'en'),
            description: event?.description,
            sessionId: event?.sessionId ?? null,
            academicYearId: acadYearId
        });
    };

    get f() {
        return this.eventForm.controls;
    }

    closeEventForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (event: SchoolEvent, action: string) => {
        this.event = event;
        this.setFormControls(event);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls = () => {
        this.editMode = false;
        this.sessionsForFiltering = [];
        this.eventForm.reset();
    };

    onSubmit = () => {
        let eventId = this.event?.id;
        this.event = new SchoolEvent(this.eventForm.value);
        this.event.sessionId = this.eventForm.get('sessionId').value;
        if (this.editMode) this.event.id = eventId;
        this.addItemEvent.emit(this.event);
    };
}
