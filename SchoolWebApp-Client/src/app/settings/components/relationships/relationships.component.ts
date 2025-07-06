import {BreadCrumb} from '@/core/models/bread-crumb';
import {Relationship} from '@/settings/models/relationship';
import {RelationshipsService} from '@/settings/services/relationships.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-relationships',
    templateUrl: './relationships.component.html',
    styleUrl: './relationships.component.scss'
})
export class RelationshipsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    relationshipForm: FormGroup;

    buttonTitle: string = 'Add relationship';
    tableModel: string = 'relationship';
    tableTitle: string = 'Relationships list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Description', 'Action'];

    editMode = false;
    relationship: Relationship;
    isAuthLoading: boolean;
    relationships: Relationship[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private relationshipsSvc: RelationshipsService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Relationships list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/relationships'], title: 'Settings:Relationships'}
    ];

    deleteItem(id) {
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
                this.relationshipsSvc.delete('/relationships', id).subscribe(
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

    editItem(id) {
        this.relationshipsSvc.getById(id, '/relationships').subscribe(
            (res) => {
                this.relationship = new Relationship(res);
                this.relationshipForm.setValue({
                    name: this.relationship.name,
                    description: this.relationship.description
                });
                this.editMode = true;
                this.settingsTblBtn.onButtonClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    onSubmit() {
        if (this.relationshipForm.invalid) {
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
                    this.relationship.name =
                        this.relationshipForm.get('name').value;
                    this.relationship.description =
                        this.relationshipForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.relationshipsSvc.update(
                          '/relationships',
                          this.relationship
                      )
                    : this.relationshipsSvc.create(
                          '/relationships',
                          new Relationship(this.relationshipForm.value)
                      );

                let replyMsg = `Relationship ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.relationshipForm.reset();
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

    get f() {
        return this.relationshipForm.controls;
    }

    refreshItems() {
        this.relationshipForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: ['']
        });

        this.relationshipsSvc.get('/relationships').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.relationships = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the relationships. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    ngOnInit(): void {
        this.refreshItems();
    }

    resetForm() {
        this.relationshipForm.reset();
        this.editMode = false;
    }
}
