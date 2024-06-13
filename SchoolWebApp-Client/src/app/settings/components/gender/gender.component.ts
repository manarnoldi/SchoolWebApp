import { BreadCrumb } from '@/core/models/bread-crumb';
import { Gender } from '@/settings/models/gender';
import { GenderService } from '@/settings/services/gender.service';
import { SettingsTableComponent } from '@/shared/directives/settings-table/settings-table.component';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription, forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-gender',
  templateUrl: './gender.component.html',
  styleUrl: './gender.component.scss'
})
export class GenderComponent implements OnInit{
  @ViewChild('closebutton') closeButton;
  @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
  page = 1;
  pageSize = 10;
  pageSubscription: Subscription;
  pageSizeSubscription: Subscription;

  genderForm: FormGroup;

  buttonTitle: string = 'Add gender';
  tableModel: string = 'gender';
  tableTitle: string = 'Gender list';
  tableHeaders: string[] = ['Name', 'Description', 'Action'];

  editMode = false;
  gender: Gender;
  isAuthLoading: boolean;
  genders: Gender[] = [];
  tblShowViewButton: false;

  collectionSize = 0;

  constructor(
      private gendersSvc: GenderService,
      private toastr: ToastrService,
      private formBuilder: FormBuilder,
      private tableSettingsSvc: TableSettingsService
  ) {}
  closeResult = '';
  dashboardTitle = 'Gender list';
  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/settings/genders'], title: 'Settings:Gender'}
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
              this.gendersSvc.delete('/genders', id).subscribe(
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
      this.gendersSvc.getById(id, '/genders').subscribe(
          (res) => {
              this.gender = new Gender(res);
              this.genderForm.setValue({
                  name: this.gender.name,
                  description: this.gender.description
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
      if (this.genderForm.invalid) {
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
                  this.gender.name =
                      this.genderForm.get('name').value;
                  this.gender.description =
                      this.genderForm.get('description').value;
              }

              let reqToProcess = this.editMode
                  ? this.gendersSvc.update(
                        '/genders',
                        this.gender
                    )
                  : this.gendersSvc.create(
                        '/genders',
                        new Gender(this.genderForm.value)
                    );

              let replyMsg = `Gender ${
                  this.editMode ? 'updated' : 'created'
              } successfully!`;
              forkJoin([reqToProcess]).subscribe(
                  (res) => {
                      this.editMode = false;
                      this.toastr.success(replyMsg);
                      this.refreshItems();
                      this.genderForm.reset();
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
      return this.genderForm.controls;
  }

  refreshItems() {
      this.genderForm = this.formBuilder.group({
          name: ['', [Validators.required]],
          description: ['']
      });

      this.gendersSvc.get('/genders').subscribe(
          (res) => {
              this.collectionSize = res.length;
              this.genders = res.slice(
                  (this.page - 1) * this.pageSize,
                  (this.page - 1) * this.pageSize + this.pageSize
              );
              this.isAuthLoading = false;
              this.editMode = false;
          },
          (err) => {
              this.toastr.error(
                  'An error occured while fetching the gender. Contact system administrator.'
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
      this.genderForm.reset();
      this.editMode = false;
  }
}
