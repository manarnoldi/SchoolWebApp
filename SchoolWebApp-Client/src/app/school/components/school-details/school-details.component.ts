import {Roles} from '@/core/enums/roles';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AuthService} from '@/core/services/auth.service';
import {SchoolDetails} from '@/school/models/school-details';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-school-details',
    templateUrl: './school-details.component.html',
    styleUrl: './school-details.component.scss'
})
export class SchoolDetailsComponent implements OnInit {
    dashboardTitle = 'Dashboard details';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/details'], title: 'School:Details'}
    ];
    schoolDetailsForm: FormGroup;
    schoolDetail: SchoolDetails;
    schoolDetails: SchoolDetails[] = [];

    canAddEdit = false;
    readonly = true;
    editMode = false;

    schoolLogoUrl = '';

    constructor(
        private fb: FormBuilder,
        private toastr: ToastrService,
        private schoolDetailsService: SchoolDetailsService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
        this.checkIfCanAddEdit();
        this.schoolDetailsForm.disable();
    }

    onSubmit() {
        if (this.schoolDetailsForm.valid) {
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
                    let toSubmitDetails = new SchoolDetails(
                        this.schoolDetailsForm.value
                    );

                    toSubmitDetails.id = this.editMode
                        ? this.schoolDetail.id
                        : '0';
                    let reqToProcess = this.editMode
                        ? this.schoolDetailsService.update(
                              '/schooldetails',
                              toSubmitDetails
                          )
                        : this.schoolDetailsService.create(
                              '/schooldetails',
                              toSubmitDetails
                          );

                    let replyMsg = `School details ${
                        this.editMode ? 'updated' : 'created'
                    } successfully!`;

                    forkJoin([reqToProcess]).subscribe(
                        (res) => {
                            this.toastr.success(replyMsg);
                            if (this.editMode) {
                                this.schoolDetails = this.schoolDetails.map(
                                    (r) =>
                                        r.id === this.schoolDetail.id
                                            ? this.schoolDetail
                                            : r
                                );
                                this.editMode = true;
                                this.readonly = true;
                            } else {
                                this.editMode = true;
                                this.readonly = true;
                            }
                            this.schoolDetailsForm.disable();
                        },
                        (err) => {
                            this.toastr.error(err.error);
                        }
                    );
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                }
            });
        } else {
            console.log(this.schoolDetailsForm);
        }
    }

    checkIfCanAddEdit() {
        this.authService.getCurrentUser().roles.forEach((r) => {
            if (r.toString() == Roles[Roles.Administrator])
                this.canAddEdit = true;
        });
    }

    get f() {
        return this.schoolDetailsForm.controls;
    }

    refreshItems() {
        this.schoolDetailsService.get('/schoolDetails').subscribe(
            (res) => {
                if (res.length > 0) {
                    this.editMode = true;
                    this.schoolDetail = new SchoolDetails(res[0]);
                    this.schoolLogoUrl = res[0].logoAsBase64;
                    this.schoolDetailsForm.setValue({
                        name: this.schoolDetail.name,
                        email: this.schoolDetail.email,
                        address: this.schoolDetail.address,
                        telephone: this.schoolDetail.telephone,
                        motto: this.schoolDetail.motto,
                        vision: this.schoolDetail.vision,
                        mission: this.schoolDetail.mission,
                        initials: this.schoolDetail.initials,
                        website: this.schoolDetail.website,
                        logoUrl: this.schoolDetail.logoUrl,
                        logoAsBase64: this.schoolDetail.logoAsBase64,
                        prePrimary: this.schoolDetail.prePrimary,
                        lowerPrimary: this.schoolDetail.lowerPrimary,
                        upperPrimary: this.schoolDetail.upperPrimary,
                        juniorSchool: this.schoolDetail.juniorSchool,
                        seniorSchool: this.schoolDetail.seniorSchool,
                        otherDetails: this.schoolDetail.otherDetails,
                        reportHeader: this.schoolDetail.reportHeader,
                        reportTitle: this.schoolDetail.reportTitle,
                        reportSubTitle: this.schoolDetail.reportSubTitle,
                        reportTitleDetails: this.schoolDetail.reportTitleDetails
                    });
                }
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );

        this.schoolDetailsForm = this.fb.group({
            name: ['', [Validators.required]],
            email: ['', [Validators.email, Validators.required]],
            address: ['', [Validators.required]],
            telephone: ['', [Validators.required]],
            motto: ['', [Validators.required]],
            vision: ['', [Validators.required]],
            mission: ['', [Validators.required]],
            initials: ['', [Validators.required]],
            website: ['', [Validators.required]],
            logoUrl: [''],
            logoAsBase64: [''],
            prePrimary: [false],
            lowerPrimary: [false],
            upperPrimary: [false],
            juniorSchool: [false],
            seniorSchool: [false],
            otherDetails: [''],
            reportHeader: [''],
            reportTitle: [''],
            reportSubTitle: [''],
            reportTitleDetails: ['']
        });
    }

    editSchoolDetails() {
        if (this.canAddEdit) {
            if (this.readonly) {
                this.schoolDetailsForm.enable();
                this.readonly = false;
            } else {
                this.schoolDetailsForm.disable();
                this.readonly = true;
            }
        } else {
            this.schoolDetailsForm.disable();
            this.toastr.error(
                'Your role does not allow you to update school details. Contact system administrator.'
            );
        }
    }

    onFileChange(event: any) {
        const reader = new FileReader();
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.schoolLogoUrl = reader.result as string;
                this.schoolDetailsForm.patchValue({
                    logoAsBase64: reader.result
                });
            };
        }
    }
}
