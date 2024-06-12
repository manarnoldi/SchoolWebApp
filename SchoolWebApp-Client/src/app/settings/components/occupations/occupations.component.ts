import { BreadCrumb } from '@/core/models/bread-crumb';
import { Occupation } from '@/settings/models/occupation';
import { OccupationsService } from '@/settings/services/occupations.service';
import { SettingsTableComponent } from '@/shared/directives/settings-table/settings-table.component';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription, forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-occupations',
  templateUrl: './occupations.component.html',
  styleUrl: './occupations.component.scss'
})
export class OccupationsComponent implements OnInit{
  @ViewChild('closebutton') closeButton;
  @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
  page = 1;
  pageSize = 10;
  pageSubscription: Subscription;
  pageSizeSubscription: Subscription;

  occupationForm: FormGroup;

  buttonTitle: string = 'Add occupation';
  tableModel: string = 'occupation';
  tableTitle: string = 'Occupations list';
  tableHeaders: string[] = ['Name', 'Description', 'Action'];

  editMode = false;
  occupation: Occupation;
  isAuthLoading: boolean;
  occupations: Occupation[] = [];
  tblShowViewButton: false;

  collectionSize = 0;

  constructor(
      private occupationsSvc: OccupationsService,
      private toastr: ToastrService,
      private formBuilder: FormBuilder,
      private tableSettingsSvc: TableSettingsService
  ) {}
  closeResult = '';
  dashboardTitle = 'Occupations list';
  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/settings/occupations'], title: 'Settings:Occupations'}
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
              this.occupationsSvc.delete('/occupations', id).subscribe(
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
      this.occupationsSvc.getById(id, '/occupations').subscribe(
          (res) => {
              this.occupation = new Occupation(res);
              this.occupationForm.setValue({
                  name: this.occupation.name,
                  description: this.occupation.description
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
      if (this.occupationForm.invalid) {
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
                  this.occupation.name =
                      this.occupationForm.get('name').value;
                  this.occupation.description =
                      this.occupationForm.get('description').value;
              }

              let reqToProcess = this.editMode
                  ? this.occupationsSvc.update(
                        '/occupations',
                        this.occupation
                    )
                  : this.occupationsSvc.create(
                        '/occupations',
                        new Occupation(this.occupationForm.value)
                    );

              let replyMsg = `Occupation ${
                  this.editMode ? 'updated' : 'created'
              } successfully!`;
              forkJoin([reqToProcess]).subscribe(
                  (res) => {
                      this.editMode = false;
                      this.toastr.success(replyMsg);
                      this.refreshItems();
                      this.occupationForm.reset();
                      this.closeButton.nativeElement.click();
                  },
                  (err) => {
                      this.toastr.error(err.error);
                  }
              );
          } else if (result.dismiss === Swal.DismissReason.cancel) {
          }
      });
  }

  get f() {
      return this.occupationForm.controls;
  }

  refreshItems() {
      this.occupationForm = this.formBuilder.group({
          name: ['', [Validators.required]],
          description: ['']
      });

      this.occupationsSvc.get('/occupations').subscribe(
          (res) => {
              this.collectionSize = res.length;
              this.occupations = res.slice(
                  (this.page - 1) * this.pageSize,
                  (this.page - 1) * this.pageSize + this.pageSize
              );
              this.isAuthLoading = false;
              this.editMode = false;
          },
          (err) => {
              this.toastr.error(
                  'An error occured while fetching the occupations. Contact system administrator.'
              );
              this.isAuthLoading = false;
          }
      );
  }

  ngOnInit(): void {
      this.pageSubscription = this.tableSettingsSvc.page.subscribe(
          (page) => (this.page = page)
      );
      this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
          (pageSize) => (this.pageSize = pageSize)
      );
      this.refreshItems();
  }

  resetForm() {
      this.occupationForm.reset();
      this.editMode = false;
  }
}
