import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Designation} from '@/settings/models/designation';
import {EmploymentType} from '@/settings/models/employment-type';
import {Gender} from '@/settings/models/gender';
import {Nationality} from '@/settings/models/nationality';
import {Religion} from '@/settings/models/religion';
import {StaffCategory} from '@/settings/models/staff-category';
import {DesignationsService} from '@/settings/services/designations.service';
import {EmploymentTypeService} from '@/settings/services/employment-type.service';
import {GenderService} from '@/settings/services/gender.service';
import {NationalitiesService} from '@/settings/services/nationalities.service';
import {ReligionsService} from '@/settings/services/religions.service';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {DatePipe} from '@angular/common';
import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-staff-details-form',
    templateUrl: './staff-details-form.component.html',
    styleUrl: './staff-details-form.component.scss'
})
export class StaffDetailsFormComponent implements OnInit {
    dashboardTitle = 'School staff';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/staffDetails'], title: 'Staff'},
        {link: ['/add'], title: 'Add'}
    ];

    currentDate: Date;
    staffForm: FormGroup;
    editMode = false;
    readonly = false;
    staff: StaffDetails;
    staffId: number = 0;
    staffs: StaffDetails[] = [];
    staffImageUrl: string = '../../../../../../assets/img/user_image.png';

    staffCategories: StaffCategory[] = [];
    designations: Designation[] = [];
    employmentTypes: EmploymentType[] = [];
    nationalities: Nationality[] = [];
    religions: Religion[] = [];
    genders: Gender[] = [];

    statusVals = Status;
    statusValues;

    constructor(
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private staffsSvc: StaffDetailsService,
        private staffCategoriesSvc: StaffCategoriesService,
        private designationsSvc: DesignationsService,
        private employmentTypesSvc: EmploymentTypeService,
        private nationalitiesSvc: NationalitiesService,
        private religionsSvc: ReligionsService,
        private gendersSvc: GenderService,
        private router: Router,
        private route: ActivatedRoute,
        private datePipe: DatePipe
    ) {
        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
        );
    }

    get f() {
        return this.staffForm.controls;
    }

    resetForm = () => this.staffForm.reset();

    loadDropdowns = () => {
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            if (params['action'] == 'view') {
                this.readonly = true;
            } else if (params['action'] == 'edit') {
                this.readonly = false;
                this.editMode = true;
            }

            let designationsReq = this.designationsSvc.get('/designations');
            let employmentTypesReq =
                this.employmentTypesSvc.get('/employmentTypes');
            let nationalitiesReq = this.nationalitiesSvc.get('/nationalities');
            let religionsReq = this.religionsSvc.get('/religions');
            let gendersReq = this.gendersSvc.get('/genders');
            let staffCategoriesReq =
                this.staffCategoriesSvc.get('/staffCategories');
            let staffsReq = this.staffsSvc.getById(
                this.staffId,
                '/staffDetails'
            );

            forkJoin([
                designationsReq,
                employmentTypesReq,
                nationalitiesReq,
                religionsReq,
                gendersReq,
                staffCategoriesReq,
                this.staffId && this.staffId > 0 ? staffsReq : of(null)
            ]).subscribe(
                (res) => {
                    this.designations = res[0];
                    this.employmentTypes = res[1];
                    this.nationalities = res[2];
                    this.religions = res[3];
                    this.genders = res[4];
                    this.staffCategories = res[5];

                    if (this.staffId && this.staffId > 0) {
                        this.staff = res[6];
                        this.staffImageUrl = this.staff?.staffImageAsBase64
                            ? this.staff.staffImageAsBase64
                            : '../../../../../../assets/img/user_image.png';
                    }
                    if (
                        params['action'] == 'edit' ||
                        params['action'] == 'view'
                    ) {
                        this.setFormValues();
                    }
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    ngOnInit(): void {
        this.currentDate = new Date();
        this.loadDropdowns();
        this.staffForm = this.formBuilder.group({
            staffImageAsBase64: [''],
            fullName: ['', [Validators.required]],
            status: [this.statusValues[0], [Validators.required]],
            nationalityId: [null, [Validators.required]],
            religionId: [null, [Validators.required]],
            genderId: [null, [Validators.required]],

            dateOfBirth: [null],
            upi: ['', [Validators.required]],
            idNumber: [''],
            nhifNo: [''],
            nssfNo: [''],
            kraPinNo: [''],

            employmentDate: [null],
            endofEmploymentDate: [null],
            currentlyEmployed: [true],
            staffCategoryId: [null, [Validators.required]],
            designationId: [null, [Validators.required]],
            employmentTypeId: [null, [Validators.required]],

            address: [''],
            phoneNumber: [''],
            email: ['', [Validators.email]],
            otherDetails: [''],
            staffImage: ['']
        });
    }

    onFileChange(event: any) {
        const reader = new FileReader();
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.staffImageUrl = reader.result as string;
                this.staffForm.patchValue({
                    staffImageAsBase64: reader.result
                });
            };
        }
    }

    onSubmit = () => {
        if (this.staffForm.invalid) {
            return;
        }
        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${
                this.editMode ? 'edit' : 'add'
            } record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                if (this.editMode) {
                    this.staff = Object.assign(
                        {id: this.staff.id},
                        this.staffForm.value
                    );
                    let birthDate = this.staffForm.get('dateOfBirth').value;
                    let empDate = this.staffForm.get('employmentDate').value;
                    let endOfEmpDate = this.staffForm.get(
                        'endofEmploymentDate'
                    ).value;

                    this.staff.dateOfBirth = !birthDate
                        ? null
                        : new Date(birthDate);
                    this.staff.employmentDate = !empDate
                        ? null
                        : new Date(empDate);
                    this.staff.endofEmploymentDate = !endOfEmpDate
                        ? null
                        : new Date(endOfEmpDate);
                }
                let reqToProcess = this.editMode
                    ? this.staffsSvc.update('/staffDetails', new StaffDetails(this.staff))
                    : this.staffsSvc.create(
                          '/staffDetails',
                          new StaffDetails(this.staffForm.value)
                      );

                let replyMsg = `Staff ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.staffForm.reset();
                        this.router.navigateByUrl('/staff/details');
                    },
                    (err) => {
                        this.toastr.error(err.error);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };

    setFormValues = () => {
        this.staffForm.setValue({
            staffImageAsBase64: this.staff?.staffImageAsBase64,
            fullName: this.staff?.fullName,
            status: this.statusValues[this.staff?.status],
            nationalityId: this.staff?.nationalityId,
            religionId: this.staff?.religionId,
            genderId: this.staff?.genderId,

            dateOfBirth: this.staff?.dateOfBirth
                ? this.datePipe.transform(this.staff?.dateOfBirth, 'yyyy-MM-dd')
                : null,
            upi: this.staff?.upi,
            idNumber: this.staff?.idNumber,
            nhifNo: this.staff?.nhifNo,
            nssfNo: this.staff?.nssfNo,
            kraPinNo: this.staff?.kraPinNo,

            employmentDate: this.staff?.employmentDate
                ? this.datePipe.transform(
                      this.staff?.employmentDate,
                      'yyyy-MM-dd'
                  )
                : null,
            endofEmploymentDate: this.staff?.endofEmploymentDate
                ? this.datePipe.transform(
                      this.staff?.endofEmploymentDate,
                      'yyyy-MM-dd'
                  )
                : null,
            currentlyEmployed: this.staff?.currentlyEmployed,
            staffCategoryId: this.staff?.staffCategoryId,
            designationId: this.staff?.designationId,
            employmentTypeId: this.staff?.employmentTypeId,

            address: this.staff?.address,
            phoneNumber: this.staff?.phoneNumber,
            email: this.staff?.email,
            otherDetails: this.staff?.otherDetails,
            staffImage: ''
        });
    };

    toggleEditStaff = () => {
        this.readonly = !this.readonly;
        this.editMode = !this.editMode;
    };

    // parseIssueType(typeString: string): Status {
    //     const type = Status[typeString];
    //     if (type === undefined) {
    //         return Status.Active;
    //     }
    //     return type;
    // }
}
