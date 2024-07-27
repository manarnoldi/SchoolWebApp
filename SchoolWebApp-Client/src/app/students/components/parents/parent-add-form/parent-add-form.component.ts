import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Parent} from '@/parents/models/parent';
import {ParentsService} from '@/parents/services/parents.service';
import {Gender} from '@/settings/models/gender';
import {Nationality} from '@/settings/models/nationality';
import {Occupation} from '@/settings/models/occupation';
import {Religion} from '@/settings/models/religion';
import {GenderService} from '@/settings/services/gender.service';
import {NationalitiesService} from '@/settings/services/nationalities.service';
import {OccupationsService} from '@/settings/services/occupations.service';
import {ReligionsService} from '@/settings/services/religions.service';
import {StudentDetails} from '@/students/models/student-details';
import {DatePipe} from '@angular/common';
import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-parent-add-form',
    templateUrl: './parent-add-form.component.html',
    styleUrl: './parent-add-form.component.scss'
})
export class ParentAddFormComponent implements OnInit {
    dashboardTitle = 'School parent';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/parents/details'], title: 'Parent'},
        {link: ['/add'], title: 'Add'}
    ];

    currentDate: Date;
    parentForm: FormGroup;
    editMode = false;
    readonly = false;
    parent: Parent;
    parentId: number = 0;
    parents: Parent[] = [];
    parentImageUrl: string = '../../../../../../assets/img/user_image.png';

    occupations: Occupation[] = [];
    nationalities: Nationality[] = [];
    religions: Religion[] = [];
    genders: Gender[] = [];
    parentStudents: StudentDetails[] = [];

    statusVals = Status;
    statusValues;

    constructor(
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private parentsSvc: ParentsService,
        private occupationsSvc: OccupationsService,
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
        return this.parentForm.controls;
    }

    resetForm = () => this.parentForm.reset();

    loadDropdowns = () => {
        this.route.queryParams.subscribe((params) => {
            this.parentId = params['id'];
            if (params['action'] == 'view') {
                this.readonly = true;
            } else if (params['action'] == 'edit') {
                this.readonly = false;
                this.editMode = true;
            }

            let occupationsReq = this.occupationsSvc.get('/occupations');
            let nationalitiesReq = this.nationalitiesSvc.get('/nationalities');
            let religionsReq = this.religionsSvc.get('/religions');
            let gendersReq = this.gendersSvc.get('/genders');
            let parentsReq = this.parentsSvc.getById(this.parentId, '/parents');
            let studentsReq = this.parentsSvc.get(
                '/parents/parentStudents/' + this.parentId
            );

            forkJoin([
                occupationsReq,
                nationalitiesReq,
                religionsReq,
                gendersReq,
                this.parentId && this.parentId > 0 ? parentsReq : of(null),
                this.parentId && this.parentId > 0 ? studentsReq : of(null)
            ]).subscribe(
                (res) => {
                    this.occupations = res[0];
                    this.nationalities = res[1];
                    this.religions = res[2];
                    this.genders = res[3];

                    if (this.parentId && this.parentId > 0) {
                        this.parent = res[4];
                        this.parentImageUrl = this.parent?.staffImageAsBase64
                            ? this.parent.staffImageAsBase64
                            : '../../../../../../assets/img/user_image.png';
                        this.parentStudents = res[5];
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
        this.parentForm = this.formBuilder.group({
            staffImageAsBase64: [''],
            fullName: ['', [Validators.required]],
            upi: ['', [Validators.required]],
            dateOfBirth: [null],
            address: [''],
            phoneNumber: [''],
            email: ['', [Validators.email]],
            status: [this.statusValues[0], [Validators.required]],
            otherDetails: [''],
            parentImage: [''],
            nationalityId: [null, [Validators.required]],
            religionId: [null, [Validators.required]],
            genderId: [null, [Validators.required]],
            occupationId: [null, [Validators.required]],
            notifiable: [false],
            payer: [false],
            pickup: [false]
        });
    }

    onFileChange(event: any) {
        const reader = new FileReader();
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.parentImageUrl = reader.result as string;
                this.parentForm.patchValue({
                    parentImageAsBase64: reader.result
                });
            };
        }
    }

    onSubmit = () => {
        if (this.parentForm.invalid) {
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
                    this.parent = Object.assign(
                        this.parent,
                        this.parentForm.value
                    );
                    let birthDate = this.parentForm.get('dateOfBirth').value;

                    this.parent.dateOfBirth = !birthDate
                        ? null
                        : new Date(birthDate);
                }
                let reqToProcess = this.editMode
                    ? this.parentsSvc.update('/parents', this.parent)
                    : this.parentsSvc.create(
                          '/parents',
                          new Parent(this.parentForm.value)
                      );

                let replyMsg = `Parent ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.parentForm.reset();
                        this.router.navigateByUrl('/parents/details');
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
        this.parentForm.setValue({
            staffImageAsBase64: this.parent?.staffImageAsBase64,
            fullName: this.parent?.fullName,
            upi: this.parent?.upi,
            dateOfBirth: this.parent?.dateOfBirth
                ? this.datePipe.transform(
                      this.parent?.dateOfBirth,
                      'yyyy-MM-dd'
                  )
                : null,
            address: this.parent?.address,
            phoneNumber: this.parent?.phoneNumber,
            email: this.parent?.email,
            status: this.statusValues[this.parent?.status],
            otherDetails: this.parent?.otherDetails,
            parentImage: [''],
            nationalityId: this.parent?.nationalityId,
            religionId: this.parent?.religionId,
            genderId: this.parent?.genderId,
            occupationId: this.parent?.occupationId,
            notifiable: this.parent?.notifiable,
            payer: this.parent?.payer,
            pickup: this.parent?.pickup
        });
    };

    toggleEditParent = () => {
        this.readonly = !this.readonly;
        this.editMode = !this.editMode;
    };
}
