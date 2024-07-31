import {Nationality} from '@/settings/models/nationality';
import {Occupation} from '@/settings/models/occupation';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentParent} from '@/students/models/student-parent';
import {ToastrService} from 'ngx-toastr';
import {StudentParentsService} from '@/students/services/student-parents.service';
import {ActivatedRoute} from '@angular/router';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Parent} from '@/students/models/parent';
import {OccupationsService} from '@/settings/services/occupations.service';
import {NationalitiesService} from '@/settings/services/nationalities.service';
import {StudentParentsExistingFormComponent} from './student-parents-existing-form/student-parents-existing-form.component';
import {Relationship} from '@/settings/models/relationship';
import {ParentsService} from '@/students/services/parents.service';
import {RelationshipsService} from '@/settings/services/relationships.service';
import {StudentDetails} from '@/students/models/student-details';

@Component({
    selector: 'app-student-parents',
    templateUrl: './student-parents.component.html',
    styleUrl: './student-parents.component.scss'
})
export class StudentParentsComponent implements OnInit {
    @Input() statuses;
    @Input() student: StudentDetails;

    occupations: Occupation[];
    nationalities: Nationality[];
    parents: Parent[];
    relationships: Relationship[];

    @ViewChild(StudentParentsExistingFormComponent)
    studentParentsExistingFormComponent: StudentParentsExistingFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    studentId: number = 0;
    studentParent: StudentParent;
    studentParents: StudentParent[] = [];
    constructor(
        private toastr: ToastrService,
        private studentParentsSvc: StudentParentsService,
        private occupationsSvc: OccupationsService,
        private parentsSvc: ParentsService,
        private relationShipsSvc: RelationshipsService,
        private nationalitiesSvc: NationalitiesService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStudentParents();
    }

    loadStudentParents = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            let parentsByStudentIdReq = this.studentParentsSvc.get(
                '/students/studentParents/' + this.studentId.toString()
            );
            let occupationsReq = this.occupationsSvc.get('/occupations/');
            let nationalitiesReq = this.nationalitiesSvc.get('/nationalities/');
            let parentsReq = this.parentsSvc.get('/parents/');
            let relationShipsReq = this.relationShipsSvc.get('/relationShips/');

            forkJoin([
                parentsByStudentIdReq,
                occupationsReq,
                nationalitiesReq,
                parentsReq,
                relationShipsReq
            ]).subscribe(
                ([
                    studentParents,
                    occupations,
                    nationalities,
                    parents,
                    relationShips
                ]) => {
                    this.studentParents = studentParents;
                    this.occupations = occupations;
                    this.nationalities = nationalities;
                    this.parents = parents;
                    this.relationships = relationShips;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    editItem(studentId: number, parentId: number, action = 'edit') {
        this.studentParentsSvc
            .getByIds([studentId, parentId], '/students/studentParents/')
            .subscribe(
                (res) => {
                    let studentId = res.studentId;
                    let parentId = res.parentId;
                    this.studentParent = new StudentParent(res);
                    this.studentParent.parentId = parentId;
                    this.studentParent.studentId = studentId;
                    this.studentParentsExistingFormComponent.action = action;
                    this.tableButton.onClick();
                },
                (err) => {
                    this.toastr.error(err);
                }
            );
    }

    deleteItem(studentParentId) {
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
                this.studentParentsSvc
                    .deleteByIds('/students/studentParent', [
                        studentParentId?.parentId,
                        studentParentId?.studentId
                    ])
                    .subscribe(
                        (res) => {
                            this.loadStudentParents();
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

    AddStudentParent = (studentParent: StudentParent) => {
        Swal.fire({
            title: `${this.studentParentsExistingFormComponent.action == 'edit' ? 'Update' : 'Add'} Staff attendance record?`,
            text: `Confirm if you want to ${
                this.studentParentsExistingFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } staff attendance.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentParentsExistingFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentParent(studentParent);
                if (this.studentParentsExistingFormComponent.action == 'edit')
                    app.id = studentParent.id;
                let reqToProcess =
                    this.studentParentsExistingFormComponent.action == 'edit'
                        ? this.studentParentsSvc.updateByIds(
                              '/students/studentParent/',
                              app,
                              [app.parentId, app.studentId]
                          )
                        : this.studentParentsSvc.create(
                              '/students/studentParent',
                              app
                          );

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.studentParentsExistingFormComponent.action = 'add';
                        this.toastr.success(
                            'Student parent saved successfully'
                        );
                        this.studentParentsExistingFormComponent.closeButton.nativeElement.click();
                        this.loadStudentParents();
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
