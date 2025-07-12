import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolStream} from '@/class/models/school-stream';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-school-streams',
    templateUrl: './school-streams.component.html',
    styleUrl: './school-streams.component.scss'
})
export class SchoolStreamsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/schoolStreams'], title: 'Settings: School streams'}
    ];

    dashboardTitle = 'Settings: School streams';
    tableTitle: string = 'School streams list';
    tableHeaders: string[] = [
        'Ref#',
        'Name',
        'Abbreviation',
        'Rank',
        'Description',
        'Action'
    ];

    schoolStream: SchoolStream;
    schoolStreams: SchoolStream[] = [];
    tblShowViewButton: true;
    editMode = false;
    isAuthLoading: boolean;
    page = 1;
    pageSize = 10;

    tableModel: string = 'schoolStream';

    schoolStreamForm: FormGroup;

    constructor(
        private schoolStreamsSvc: SchoolStreamsService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    get f() {
        return this.schoolStreamForm.controls;
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    refreshItems() {
        this.schoolStreamForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: ['', [Validators.required]],
            abbreviation: [''],
            description: ['']
        });

        let schoolStreamsRequest = this.schoolStreamsSvc.get('/schoolStreams');

        forkJoin([schoolStreamsRequest]).subscribe(
            (res) => {
                this.schoolStreams = res[0].sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.schoolStreamsSvc.getById(id, '/schoolStreams').subscribe(
            (res) => {
                this.schoolStream = new SchoolStream(res);
                this.schoolStreamForm.setValue({
                    name: this.schoolStream.name,
                    abbreviation: this.schoolStream.abbreviation,
                    rank: this.schoolStream.rank,
                    description: this.schoolStream.description
                });
                this.editMode = true;
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
                this.schoolStreamsSvc.delete('/schoolStreams', id).subscribe(
                    (res) => {
                        this.refreshItems();
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

    onSubmit() {
        if (this.schoolStreamForm.invalid) {
            return;
        }

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${
                this.editMode ? 'edit' : 'add'
            } record.`,
            width: 400,
            heightAuto: true,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let toSubmitDetails = new SchoolStream(
                    this.schoolStreamForm.value
                );
                if (this.editMode) toSubmitDetails.id = this.schoolStream.id;

                let reqToProcess = this.editMode
                    ? this.schoolStreamsSvc.update(
                          '/schoolStreams',
                          toSubmitDetails
                      )
                    : this.schoolStreamsSvc.create(
                          '/schoolStreams',
                          toSubmitDetails
                      );

                let replyMsg = `School stream ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.schoolStreamForm.reset();
                        this.closeButton.nativeElement.click();
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    resetForm() {
        this.schoolStreamForm.reset();
    }
}
