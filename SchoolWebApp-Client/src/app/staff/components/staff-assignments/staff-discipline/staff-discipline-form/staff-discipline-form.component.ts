import {DateLessThanOrEqualsValidator} from '@/core/validators.ts/DateValidators';
import {OccurenceType} from '@/settings/models/occurence-type';
import {Outcome} from '@/settings/models/outcome';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffDiscipline} from '@/staff/models/staff-discipline';
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
    selector: 'app-staff-discipline-form',
    templateUrl: './staff-discipline-form.component.html',
    styleUrl: './staff-discipline-form.component.scss'
})
export class StaffDisciplineFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() staffDiscipline: StaffDiscipline;
    @Input() statuses;
    @Input() staff: StaffDetails;

    @Input() outcomes: Outcome[] = [];
    @Input() occurenceTypes: OccurenceType[] = [];
    action: string = 'add';

    @Output() addItemEvent = new EventEmitter<StaffDiscipline>();
    @Output() errorEvent = new EventEmitter<string>();

    staffDisciplineForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.staffDisciplineForm = this.formBuilder.group({
            staffDetailsId: [this.staff?.id, [Validators.required]],
            occurenceStartDate: [
                formatDate(new Date(), 'yyyy-MM-dd', 'en'),
                [
                    Validators.required,
                    DateLessThanOrEqualsValidator('occurenceEndDate', 'greater')
                ]
            ],
            occurenceEndDate: [
                formatDate(new Date(), 'yyyy-MM-dd', 'en'),
                [Validators.required]
            ],
            occurenceDetails: [''],
            outcomeDetails: [''],
            outcomeId: [null],
            occurenceTypeId: [null]
        });
    };

    setFormControls = (staffDiscipline: StaffDiscipline) => {
        this.staffDisciplineForm.setValue({
            occurenceStartDate: formatDate(
                new Date(staffDiscipline.occurenceStartDate),
                'yyyy-MM-dd',
                'en'
            ),
            occurenceEndDate: formatDate(
                new Date(staffDiscipline.occurenceEndDate),
                'yyyy-MM-dd',
                'en'
            ),
            occurenceDetails: staffDiscipline.occurenceDetails,
            outcomeDetails: staffDiscipline.outcomeDetails,
            staffDetailsId: staffDiscipline.staffDetailsId ?? null,
            outcomeId: staffDiscipline.outcomeId ?? null,
            occurenceTypeId: staffDiscipline.occurenceTypeId ?? null
        });
    };

    get f() {
        return this.staffDisciplineForm.controls;
    }

    closeStaffDisciplineForm = () => {
        this.closeButton.nativeElement.click();
        this.refreshItems();
        this.resetFormControls();
    };

    resetFormControls() {
        this.action = 'add';
        this.staffDisciplineForm.reset();
    }

    onSubmit = () => {
        if (this.action == 'edit') {
            let staffDisciplineId = this.staffDiscipline.id;
            this.staffDiscipline = new StaffDiscipline(
                this.staffDisciplineForm.value
            );
            this.staffDiscipline.id = staffDisciplineId;
        } else {
            this.staffDiscipline = new StaffDiscipline(
                this.staffDisciplineForm.value
            );
        }
        this.addItemEvent.emit(this.staffDiscipline);
    };
}
