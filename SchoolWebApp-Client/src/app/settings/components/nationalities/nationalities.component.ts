import {BreadCrumb} from '@/core/models/bread-crumb';
import {Nationality} from '@/settings/models/nationality';
import {NationalitiesService} from '@/settings/services/nationalities.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-nationalities',
    templateUrl: './nationalities.component.html',
    styleUrl: './nationalities.component.scss'
})
export class NationalitiesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    nationalityForm: FormGroup;

    buttonTitle: string = 'Add nationality';
    tableModel: string = 'nationality';
    tableTitle: string = 'Nationalities list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    nationality: Nationality;
    isAuthLoading: boolean;
    nationalities: Nationality[] = [];
    tblShowViewButton: false;

    constructor(
        private nationalitiesSvc: NationalitiesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}

    closeResult = '';
    dashboardTitle = 'Nationalities list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/nationalities'], title: 'Settings:Nationalities'}
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
                this.nationalitiesSvc.delete('/nationalities', id).subscribe(
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
        this.nationalitiesSvc.getById(id, '/nationalities').subscribe(
            (res) => {
                this.nationality = new Nationality(res);
                this.nationalityForm.setValue({
                    name: this.nationality.name,
                    rank: this.nationality.rank,
                    description: this.nationality.description
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
        if (this.nationalityForm.invalid) {
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
                    this.nationality.name =
                        this.nationalityForm.get('name').value;
                    this.nationality.rank =
                        this.nationalityForm.get('rank').value;
                    this.nationality.description =
                        this.nationalityForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.nationalitiesSvc.update(
                          '/nationalities',
                          this.nationality
                      )
                    : this.nationalitiesSvc.create(
                          '/nationalities',
                          new Nationality(this.nationalityForm.value)
                      );

                let replyMsg = `Nationality ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.nationalityForm.reset();
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
        return this.nationalityForm.controls;
    }

    refreshItems() {
        this.nationalityForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.nationalitiesSvc.get('/nationalities').subscribe(
            (res) => {
                this.nationalities = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the nationalities. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    resetForm() {
        this.nationalityForm.reset();
        this.editMode = false;
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
