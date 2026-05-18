import {Department} from '@/school/models/department';
import {StaffDetails} from '@/staff/models/staff-details';
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
    selector: 'app-departments-add-form',
    templateUrl: './departments-add-form.component.html',
    styleUrl: './departments-add-form.component.scss'
})
export class DepartmentsAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() department: Department;
    @Input() staffDetails: StaffDetails[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<Department>();
    @Output() errorEvent = new EventEmitter<string>();

    departmentForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.initializeForm();
    }

    initializeForm = () => {
        this.departmentForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            code: [{value: 'Auto', disabled: true}],
            description: [''],
            staffDetailsId: [null]
        });
    };

    setFormControls = (department: Department) => {
        this.departmentForm.setValue({
            name: department?.name,
            code: department?.code || 'Auto',
            description: department?.description,
            staffDetailsId: department?.staffDetailsId
        });
    };

    get f() {
        return this.departmentForm.controls;
    }

    closeDepartmentForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (department: Department, action: string) => {
        this.department = department;
        this.setFormControls(department);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.departmentForm.reset();
    }

    onSubmit = () => {
        let departmentId = this.department?.id;
        let raw = this.departmentForm.getRawValue();
        this.department = new Department(raw);
        // Backend auto-generates the code on create; clear the placeholder text
        if (!this.editMode || !this.department.code || this.department.code === 'Auto') {
            this.department.code = '';
        }
        // staffDetailsId is now bound directly to the id via ng-select bindValue
        let staffVal = this.departmentForm.get('staffDetailsId').value;
        this.department.staffDetailsId = (staffVal && typeof staffVal === 'object') ? staffVal.id : staffVal;
        if (this.editMode) this.department.id = departmentId;
        this.addItemEvent.emit(this.department);
    };
}
