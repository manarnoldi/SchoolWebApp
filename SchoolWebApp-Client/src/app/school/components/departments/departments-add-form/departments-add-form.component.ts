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
            code: ['', [Validators.required]],
            description: [''],
            staffDetailsId: [null, [Validators.required]]
        });
    };

    setFormControls = (department: Department) => {
        this.departmentForm.setValue({
            name: department?.name,
            code: department?.code,
            description: department?.description,
            staffDetailsId: 0
        });
        var staffDId = this.staffDetails.find(
            (s) => s.id == department?.staffDetailsId.toString()
        );
        this.departmentForm.patchValue({
            staffDetailsId: staffDId
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
        this.department = new Department(this.departmentForm.value);
        this.department.staffDetailsId =
            this.departmentForm.get('staffDetailsId').value?.id;
        if (this.editMode) this.department.id = departmentId;
        this.addItemEvent.emit(this.department);
    };
}
