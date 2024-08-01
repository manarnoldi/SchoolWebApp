import {ClassLeadership} from '@/class/models/class-leadership';
import {ClassLeadershipRole} from '@/class/models/class-leadership-role';
import {SchoolClass} from '@/class/models/school-class';
import {Person} from '@/school/models/person';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {ParentsService} from '@/students/services/parents.service';
import {StudentDetailsService} from '@/students/services/student-details.service';
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
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-class-leadership-add-form',
    templateUrl: './class-leadership-add-form.component.html',
    styleUrl: './class-leadership-add-form.component.scss'
})
export class ClassLeadershipAddFormComponent implements OnInit {
    @Input() schoolClass: SchoolClass;
    @Input() classLeadershipRoles: ClassLeadershipRole[] = [];
    @Input() persons: Person[] = [];
    @Input() personTypes;
    @Input() classLeadership: ClassLeadership;

    @Output() addItemEvent = new EventEmitter<ClassLeadership>();
    @Output() errorEvent = new EventEmitter<string>();

    loading: boolean = true;

    @ViewChild('closeButton') closeButton: ElementRef;

    editMode: boolean = false;
    classLeadershipForm: FormGroup;
    classLeadershipRole: ClassLeadershipRole;
    constructor(
        private formBuilder: FormBuilder,
        private toastr: ToastrService,
        private parentsSvc: ParentsService,
        private studentsSvc: StudentDetailsService,
        private staffSvc: StaffDetailsService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.classLeadershipForm = this.formBuilder.group({
            schoolClassId: [this.schoolClass?.id, [Validators.required]],
            description: [''],
            personId: [null, [Validators.required]],
            classLeadershipRoleId: [null, [Validators.required]]
        });
    };

    onSubmit = () => {
        if (this.editMode) {
            let classLeadershipId = this.classLeadership.id;
            this.classLeadership = new ClassLeadership(
                this.classLeadershipForm.value
            );
            this.classLeadership.id = classLeadershipId;
        } else {
            this.classLeadership = new ClassLeadership(
                this.classLeadershipForm.value
            );
        }
        this.classLeadership.personId =
            this.classLeadershipForm.get('personId').value.id;
        this.classLeadership.classLeadershipRoleId =
            this.classLeadershipForm.get('classLeadershipRoleId').value.id;
        this.addItemEvent.emit(this.classLeadership);
    };

    setFormControls = (classLeadership: ClassLeadership) => {
        this.classLeadershipForm.setValue({
            schoolClassId: classLeadership.schoolClassId,
            description: classLeadership.description,
            personId: classLeadership.person ?? null,
            classLeadershipRoleId: classLeadership.classLeadershipRole ?? null
        });
        this.loadPersons();
    };

    get f() {
        return this.classLeadershipForm.controls;
    }

    closeClassLeadershipForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.classLeadershipForm.reset();
        this.classLeadershipForm.patchValue({
            schoolClassId: this.schoolClass?.id
        });
    }

    loadPersons = () => {
        this.persons = [];
        this.classLeadershipForm.get('personId').reset();

        if (this.classLeadershipForm.get('classLeadershipRoleId').value) {
            this.classLeadershipRole = this.classLeadershipForm.get(
                'classLeadershipRoleId'
            ).value;
            let personReq;
            if (
                this.classLeadershipForm.get('classLeadershipRoleId').value
                    .personType === 0
            )
                personReq = this.studentsSvc.get('/students?active=true');
            else if (
                this.classLeadershipForm.get('classLeadershipRoleId').value
                    .personType === 1
            )
                personReq = this.staffSvc.get('/staffDetails?active=true');
            else if (
                this.classLeadershipForm.get('classLeadershipRoleId').value
                    .personType === 2
            )
                personReq = this.parentsSvc.get('/parents?active=true');

            forkJoin([personReq]).subscribe(
                (res) => {
                    this.loading = false;
                    this.persons = res[0];

                    if (this.classLeadership) {
                        this.classLeadershipForm.patchValue({
                            personId: this.persons.find(
                                (p) => p.id === this.classLeadership.personId
                            )
                        });
                    }
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        }
    };
}
