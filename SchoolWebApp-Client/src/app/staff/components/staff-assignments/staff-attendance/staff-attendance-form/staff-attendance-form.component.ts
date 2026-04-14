import {StaffAttendance} from '@/staff/models/staff-attendance';
import {StaffDetails} from '@/staff/models/staff-details';
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
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {ToastrService} from 'ngx-toastr';

@Component({
    selector: 'app-staff-attendance-form',
    templateUrl: './staff-attendance-form.component.html',
    styleUrl: './staff-attendance-form.component.scss'
})
export class StaffAttendanceFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() staffAttendance: StaffAttendance;
    @Input() statuses;
    @Input() staff: StaffDetails;
    editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<StaffAttendance>();
    @Output() errorEvent = new EventEmitter<string>();

    staffAttendanceForm: FormGroup;
    requireAbsenceReason: boolean = false;

    constructor(
        private formBuilder: FormBuilder,
        private globalSettingSvc: GlobalSettingService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.globalSettingSvc.getByKey('General', 'RequireAbsenceReason').subscribe({
            next: (setting) => {
                this.requireAbsenceReason = setting?.settingValue === 'true';
            },
            error: () => {}
        });
        this.refreshItems();
    }

    refreshItems = () => {
        this.staffAttendanceForm = this.formBuilder.group({
            date: [
                formatDate(new Date(), 'yyyy-MM-dd', 'en'),
                [Validators.required]
            ],
            present: [true],
            remarks: [''],
            staffDetailsId: [this.staff?.id, [Validators.required]]
        });
    };

    setFormControls = (staffAttendance: StaffAttendance) => {
        this.staffAttendanceForm.setValue({
            date: formatDate(
                new Date(staffAttendance.date),
                'yyyy-MM-dd',
                'en'
            ),
            present: staffAttendance.present,
            remarks: staffAttendance.remarks,
            staffDetailsId: staffAttendance.staffDetailsId ?? null
        });
    };

    get f() {
        return this.staffAttendanceForm.controls;
    }

    closeStaffAttendanceForm = () => {
        this.closeButton.nativeElement.click();
        this.refreshItems();
    };

    viewItem = (staffAttendance: StaffAttendance, action: string) => {
        this.staffAttendance = staffAttendance;
        this.setFormControls(staffAttendance);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.staffAttendanceForm.reset();
    }

    onSubmit = () => {
        let formValue = this.staffAttendanceForm.value;
        let isPresent = formValue.present === null ? false : formValue.present;
        let remarks = (formValue.remarks || '').trim();

        if (this.requireAbsenceReason && !isPresent && !remarks) {
            this.toastr.warning('A reason/remark is required for absent records.');
            return;
        }

        if (this.editMode) {
            let staffAttendanceId = this.staffAttendance.id;
            this.staffAttendance = new StaffAttendance(formValue);
            this.staffAttendance.id = staffAttendanceId;
        } else {
            this.staffAttendance = new StaffAttendance(formValue);
        }
        this.staffAttendance.present = isPresent;
        this.addItemEvent.emit(this.staffAttendance);
    };
}
