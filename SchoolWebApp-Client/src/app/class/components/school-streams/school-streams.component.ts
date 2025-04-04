import { BreadCrumb } from '@/core/models/bread-crumb';
import { SchoolStream } from '@/class/models/school-stream';
import { SchoolStreamsService } from '@/class/services/school-streams.service';
import { TableButtonComponent } from '@/shared/directives/table-button/table-button.component';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription, forkJoin } from 'rxjs';
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
    tableHeaders: string[] = ['Name', 'Abbreviation', 'Description', 'Action'];

    schoolStream: SchoolStream;
    schoolStreams: SchoolStream[] = [];
    tblShowViewButton: true;
    editMode = false;
    isAuthLoading: boolean;
    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;
    tableModel: string = 'schoolStream';

    schoolStreamForm: FormGroup;

    constructor(
        private schoolStreamsSvc: SchoolStreamsService,
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.refreshItems();
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }

    get f() {
        return this.schoolStreamForm.controls;
    }

    refreshItems() {
        this.schoolStreamForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: [''],
            description: ['']
        });

        let schoolStreamsRequest =
            this.schoolStreamsSvc.get('/schoolStreams');

        forkJoin([schoolStreamsRequest]).subscribe(
            (res) => {
                this.collectionSize = res[0].length;
                this.schoolStreams = res[0];
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
                if (this.editMode) {
                    this.schoolStream.name =
                        this.schoolStreamForm.get('name').value;
                    this.schoolStream.abbreviation =
                        this.schoolStreamForm.get('abbreviation').value;
                    this.schoolStream.description =
                        this.schoolStreamForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.schoolStreamsSvc.update(
                          '/schoolStreams',
                          this.schoolStream
                      )
                    : this.schoolStreamsSvc.create(
                          '/schoolStreams',
                          new SchoolStream(this.schoolStreamForm.value)
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
