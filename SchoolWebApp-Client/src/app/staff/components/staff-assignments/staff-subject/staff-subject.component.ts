import {StaffDetails} from '@/staff/models/staff-details';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {StaffSubjectFormComponent} from './staff-subject-form/staff-subject-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StaffSubject} from '@/staff/models/staff-subject';
import {ToastrService} from 'ngx-toastr';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {ActivatedRoute} from '@angular/router';
import {StaffSubjectsService} from '@/staff/services/staff-subjects.service';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Subject} from '@/academics/models/subject';
import {AcademicYear} from '@/school/models/academic-year';
import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';

@Component({
    selector: 'app-staff-subject',
    templateUrl: './staff-subject.component.html',
    styleUrl: './staff-subject.component.scss'
})
export class StaffSubjectComponent implements OnInit {
    @Input() statuses;
    @Input() staff: StaffDetails;
    @Input() subjects: Subject[];
    @ViewChild(StaffSubjectFormComponent)
    staffSubjectFormComponent: StaffSubjectFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    staffId: number = 0;
    staffSubject: StaffSubject;
    staffSubjects: StaffSubject[];
    academicYears: AcademicYear[];
    learningLevels: LearningLevel[];
    schoolStreams: SchoolStream[];

    constructor(
        private toastr: ToastrService,
        private staffSubjectsSvc: StaffSubjectsService,
        private route: ActivatedRoute,
        private academicYearsSvc: AcademicYearsService,
        private learningLevelsSvc: LearningLevelsService,
        private schoolStreamsSvc: SchoolStreamsService
    ) {}

    ngOnInit(): void {
        this.loadStaffSubjects();
    }

    loadStaffSubjects = () => {
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            let subjectsByStaffDetailsIdReq = this.staffSubjectsSvc.get(
                '/staffSubjects/byStaffDetailsId/' + this.staffId.toString()
            );
            let academicYearsReq = this.academicYearsSvc.get('/academicYears');
            let learningLevelsReq =
                this.learningLevelsSvc.get('/learningLevels');
            let schoolStreamsReq = this.schoolStreamsSvc.get('/schoolStreams');

            forkJoin([
                subjectsByStaffDetailsIdReq,
                academicYearsReq,
                learningLevelsReq,
                schoolStreamsReq
            ]).subscribe(
                ([
                    staffSubjects,
                    academicYears,
                    learningLevels,
                    schoolStreams
                ]) => {
                    this.staffSubjects = staffSubjects;
                    this.academicYears = academicYears;
                    this.learningLevels = learningLevels;
                    this.schoolStreams = schoolStreams;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    editItem(id: number, action = 'edit') {
        this.staffSubjectsSvc.getById(id, '/staffSubjects').subscribe(
            (res) => {
                let staffSubjectId = res.id;
                this.staffSubject = new StaffSubject(res);
                this.staffSubject.id = staffSubjectId;
                this.staffSubjectFormComponent.setFormControls(
                    this.staffSubject
                );
                this.staffSubjectFormComponent.action = action;
                this.staffSubjectFormComponent.staffSubject = this.staffSubject;
                this.tableButton.onClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(id: number) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.staffSubjectsSvc.delete('/staffSubjects', id).subscribe(
                    (res) => {
                        this.loadStaffSubjects();
                        this.toastr.success('Record deleted successfully!');
                    },
                    (err) => {
                        this.toastr.error(err);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    AddStaffSubject = (staffSubject: StaffSubject) => {
        Swal.fire({
            title: `${this.staffSubjectFormComponent.action == 'edit' ? 'Update' : 'Add'} Staff subject record?`,
            text: `Confirm if you want to ${
                this.staffSubjectFormComponent.action == 'edit' ? 'update' : 'add'
            } staff subject.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.staffSubjectFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StaffSubject(staffSubject);
                if (this.staffSubjectFormComponent.action == 'edit')
                    app.id = staffSubject.id;
                let reqToProcess = this.staffSubjectFormComponent.action == 'edit'
                    ? this.staffSubjectsSvc.update('/staffSubjects', app)
                    : this.staffSubjectsSvc.create('/staffSubjects', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.staffSubjectFormComponent.action = 'add';
                        this.toastr.success('Staff subject saved successfully');
                        this.staffSubjectFormComponent.closeButton.nativeElement.click();
                        this.loadStaffSubjects();
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
