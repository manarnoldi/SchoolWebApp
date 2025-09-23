import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Gender} from '@/settings/models/gender';
import {Nationality} from '@/settings/models/nationality';
import {Occupation} from '@/settings/models/occupation';
import {Relationship} from '@/settings/models/relationship';
import {Religion} from '@/settings/models/religion';
import {GenderService} from '@/settings/services/gender.service';
import {NationalitiesService} from '@/settings/services/nationalities.service';
import {OccupationsService} from '@/settings/services/occupations.service';
import {RelationshipsService} from '@/settings/services/relationships.service';
import {ReligionsService} from '@/settings/services/religions.service';
import {Parent} from '@/students/models/parent';
import {StudentDetails} from '@/students/models/student-details';
import {StudentParent} from '@/students/models/student-parent';
import {ParentsService} from '@/students/services/parents.service';
import {StudentParentsService} from '@/students/services/student-parents.service';
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

    routerLink: string = '/students/parents';
    queryParams;
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
    relationShips: Relationship[] = [];
    studentParent: StudentParent;
    studentId: number = 0;

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
        private relationshipsSvc: RelationshipsService,
        private studentParentsSvc: StudentParentsService,
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
            this.studentId = parseInt(params['sId']);
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
            let relationShipsReq = this.relationshipsSvc.get('/relationShips');
            let studentParentReq = this.studentParentsSvc.getByIds(
                [this.parentId, this.studentId],
                '/students/studentParents'
            );

            forkJoin([
                occupationsReq,
                nationalitiesReq,
                religionsReq,
                gendersReq,
                this.parentId && this.parentId > 0 ? parentsReq : of(null),
                this.parentId && this.parentId > 0 ? studentsReq : of(null),
                relationShipsReq,
                this.parentId &&
                this.parentId > 0 &&
                this.studentId &&
                this.studentId > 0
                    ? studentParentReq
                    : of(null)
            ]).subscribe(
                (res) => {
                    this.occupations = res[0];
                    this.nationalities = res[1];
                    this.religions = res[2];
                    this.genders = res[3];
                    this.relationShips = res[6];

                    if (this.parentId && this.parentId > 0) {
                        this.parent = res[4];
                        this.parentImageUrl = this.parent?.staffImageAsBase64
                            ? this.parent.staffImageAsBase64
                            : '../../../../../../assets/img/user_image.png';
                        this.parentStudents = res[5];
                        if (this.studentId && this.studentId > 0) {
                            this.studentParent = res[7];
                        }
                    }
                    if (this.studentId && this.studentId > 0) {
                        this.routerLink = '/students/manage/add';
                        this.queryParams = {
                            id: this.studentId,
                            action: 'manage',
                            activeNav: 'parents'
                        };
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
            pickup: [false],
            relationShipId: [null],
            otherDetailsSP: ['']
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
                let toSubmitDetails = new Parent(this.parentForm.value);

                if (this.editMode) toSubmitDetails.id = this.parent.id;

                let reqToProcess = this.editMode
                    ? this.parentsSvc.update('/parents', toSubmitDetails)
                    : this.parentsSvc.create('/parents', toSubmitDetails);

                let replyMsg = `Parent ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        let studParentApp = new StudentParent();
                        if (this.studentId && this.studentId > 0) {
                            studParentApp.studentId = this.studentId;
                            studParentApp.parentId = this.editMode
                                ? parseInt(this.parent.id)
                                : parseInt(res[0].id);
                            studParentApp.relationShipId =
                                this.parentForm.get('relationShipId').value;
                            studParentApp.otherDetails =
                                this.parentForm.get('otherDetailsSP').value;

                            let reqToProcessSP = this.editMode
                                ? this.studentParentsSvc.update(
                                      '/students/studentParent/',
                                      studParentApp
                                  )
                                : this.studentParentsSvc.create(
                                      '/students/studentParent/',
                                      studParentApp
                                  );
                            forkJoin([reqToProcessSP]).subscribe(
                                (res) => {
                                    this.editMode = false;
                                    this.toastr.success(replyMsg);
                                    this.parentForm.reset();
                                    this.router.navigateByUrl(
                                        '/students/manage/add?id=' +
                                            this.studentId +
                                            '&action=manage&activeNav=parents'
                                    );
                                },
                                (err) => {
                                    this.toastr.error(err.error);
                                }
                            );
                        } else {
                            this.editMode = false;
                            this.toastr.success(replyMsg);
                            this.parentForm.reset();
                            this.router.navigateByUrl('/students/parents');
                        }
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
            pickup: this.parent?.pickup,
            relationShipId: this.studentParent
                ? this.studentParent?.relationShipId
                : null,
            otherDetailsSP: this.studentParent
                ? this.studentParent?.otherDetails
                : null
        });
    };

    toggleEditParent = () => {
        this.readonly = !this.readonly;
        this.editMode = !this.editMode;
    };
}
