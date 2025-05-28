import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {LearningMode} from '@/school/models/learning-mode';
import {LearningModesService} from '@/school/services/learning-modes.service';
import {Gender} from '@/settings/models/gender';
import {Nationality} from '@/settings/models/nationality';
import {Religion} from '@/settings/models/religion';
import {GenderService} from '@/settings/services/gender.service';
import {NationalitiesService} from '@/settings/services/nationalities.service';
import {ReligionsService} from '@/settings/services/religions.service';
import {StudentDetails} from '@/students/models/student-details';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {DatePipe} from '@angular/common';
import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-students-add-form',
    templateUrl: './students-add-form.component.html',
    styleUrl: './students-add-form.component.scss'
})
export class StudentsAddFormComponent implements OnInit {
    dashboardTitle = 'Students add';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/studentDetails'], title: 'Student'},
        {link: ['/add'], title: 'Add'}
    ];

    currentDate: Date;
    studentForm: FormGroup;
    editMode = false;
    readonly = false;
    student: StudentDetails;
    studentId: number = 0;
    students: StudentDetails[] = [];
    studentImageUrl: string = '../../../../../../../assets/img/user_image.png';

    learningModes: LearningMode[] = [];
    nationalities: Nationality[] = [];
    religions: Religion[] = [];
    genders: Gender[] = [];

    status: Status = Status.Active;
    querySource: string = '';
    statusVals = Status;
    statusValues;

    constructor(
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private studentsSvc: StudentDetailsService,
        private learningModesSvc: LearningModesService,
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
        return this.studentForm.controls;
    }

    resetForm = () => this.studentForm.reset();

    loadDropdowns = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            this.status = params['status']
                ? parseInt(params['status'])
                : Status.Active;
            this.querySource = params['querySource']
                ? params['querySource']
                : '';
            if (params['action'] == 'view') {
                this.readonly = true;
            } else if (params['action'] == 'edit') {
                this.readonly = false;
                this.editMode = true;
            }

            let learningModesReq = this.learningModesSvc.get('/learningModes');
            let nationalitiesReq = this.nationalitiesSvc.get('/nationalities');
            let religionsReq = this.religionsSvc.get('/religions');
            let gendersReq = this.gendersSvc.get('/genders');
            let studentsReq = this.studentsSvc.getById(
                this.studentId,
                '/students'
            );

            forkJoin([
                learningModesReq,
                nationalitiesReq,
                religionsReq,
                gendersReq,
                this.studentId && this.studentId > 0 ? studentsReq : of(null)
            ]).subscribe(
                (res) => {
                    this.learningModes = res[0];
                    this.nationalities = res[1];
                    this.religions = res[2];
                    this.genders = res[3];

                    if (this.studentId && this.studentId > 0) {
                        this.student = res[4];
                        this.studentImageUrl = this.student?.staffImageAsBase64
                            ? this.student.staffImageAsBase64
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
        this.studentForm = this.formBuilder.group({
            staffImageAsBase64: [''],
            fullName: ['', [Validators.required]],
            upi: ['', [Validators.required]],
            status: [this.statusValues[0], [Validators.required]],
            nationalityId: [null, [Validators.required]],
            religionId: [null, [Validators.required]],
            genderId: [null, [Validators.required]],

            dateOfBirth: [null],
            admissionDate: [null],
            applicationDate: [null],
            learningModeId: [null, [Validators.required]],
            address: [''],
            phoneNumber: [''],
            email: ['', [Validators.email]],
            otherDetails: [''],
            healthConcerns: [''],
            studentImage: ['']
        });
    }

    onFileChange(event: any) {
        const reader = new FileReader();
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.studentImageUrl = reader.result as string;
                this.studentForm.patchValue({
                    staffImageAsBase64: reader.result
                });
            };
        }
    }

    onSubmit = () => {
        if (this.studentForm.invalid) {
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
                    this.student = Object.assign(
                        this.student,
                        this.studentForm.value
                    );
                    let birthDate = this.studentForm.get('dateOfBirth').value;
                    let admissionDate =
                        this.studentForm.get('admissionDate').value;
                    let applicationDate =
                        this.studentForm.get('applicationDate').value;

                    this.student.dateOfBirth = !birthDate
                        ? null
                        : new Date(birthDate);
                    this.student.admissionDate = !admissionDate
                        ? null
                        : new Date(admissionDate);
                    this.student.applicationDate = !applicationDate
                        ? null
                        : new Date(applicationDate);
                }
                let reqToProcess = this.editMode
                    ? this.studentsSvc.update('/students', this.student)
                    : this.studentsSvc.create(
                          '/students',
                          new StudentDetails(this.studentForm.value)
                      );

                let replyMsg = `Student ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    ([addedStudent]) => {
                        this.toastr.success(replyMsg);
                        this.studentForm.reset();

                        if (this.editMode) {
                            this.editMode = false;
                            let retUrl = `/students/details?source=${this.querySource}&status=${this.status}`;
                            this.router.navigateByUrl(retUrl);
                        } else {
                            this.editMode = false;
                            let retUrl =
                                '/students/manage/add?id=' +
                                addedStudent.id +
                                '&action=manage';
                            this.router.navigateByUrl(retUrl);
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
        this.studentForm.setValue({
            staffImageAsBase64: this.student?.staffImageAsBase64,
            fullName: this.student?.fullName,
            upi: this.student?.upi,
            status: this.statusValues[this.student?.status],
            nationalityId: this.student?.nationalityId,
            religionId: this.student?.religionId,
            genderId: this.student?.genderId,

            dateOfBirth: this.student?.dateOfBirth
                ? this.datePipe.transform(
                      this.student?.dateOfBirth,
                      'yyyy-MM-dd'
                  )
                : null,
            admissionDate: this.student?.admissionDate
                ? this.datePipe.transform(
                      this.student?.admissionDate,
                      'yyyy-MM-dd'
                  )
                : null,
            applicationDate: this.student?.applicationDate
                ? this.datePipe.transform(
                      this.student?.applicationDate,
                      'yyyy-MM-dd'
                  )
                : null,
            learningModeId: this.student?.learningModeId,
            address: this.student?.address,
            phoneNumber: this.student?.phoneNumber,
            email: this.student?.email,
            otherDetails: this.student?.otherDetails,
            healthConcerns: this.student?.healthConcerns,
            studentImage: ''
        });
    };

    toggleEditStudent = () => {
        this.readonly = !this.readonly;
        this.editMode = !this.editMode;
    };
}
