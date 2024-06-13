import { BreadCrumb } from '@/core/models/bread-crumb';
import { OccurenceType } from '@/settings/models/occurence-type';
import { OccurenceTypeService } from '@/settings/services/occurence-type.service';
import { TableButtonComponent } from '@/shared/directives/table-button/table-button.component';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription, forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-occurence-types',
  templateUrl: './occurence-types.component.html',
  styleUrl: './occurence-types.component.scss'
})
export class OccurenceTypesComponent implements OnInit{
  @ViewChild('closebutton') closeButton;
  @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/settings/occurenceTypes'], title: 'Settings: Occurence types'}
  ];

  dashboardTitle = 'Settings: Occurence types';
  tableTitle: string = 'Occurence types list';
  tableHeaders: string[] = [
      'Name',        
      'Abbreviation',
      'Description',
      'Action'
  ];

  occurenceType: OccurenceType;
  occurenceTypes: OccurenceType[] = [];
  tblShowViewButton: true;
  editMode = false;
  isAuthLoading: boolean;
  page = 1;
  pageSize = 10;
  collectionSize = 0;
  pageSubscription: Subscription;
  pageSizeSubscription: Subscription;
  tableModel: string = 'occurenceType';

  occurenceTypeForm: FormGroup;

  constructor(
      private occurenceTypesSvc: OccurenceTypeService,
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
      return this.occurenceTypeForm.controls;
  }

  refreshItems() {
      this.occurenceTypeForm = this.formBuilder.group({
          name: ['', [Validators.required]],
          abbreviation: [''],
          description: ['']
      });

      let occurenceTypesRequest = this.occurenceTypesSvc.get('/occurenceTypes');

      forkJoin([occurenceTypesRequest]).subscribe(
          (res) => {
              this.collectionSize = res[0].length;
              this.occurenceTypes = res[0].slice(
                  (this.page - 1) * this.pageSize,
                  (this.page - 1) * this.pageSize + this.pageSize
              );
              this.isAuthLoading = false;
              this.editMode = false;
          },
          (err) => {
              this.toastr.error(err.error);
          }
      );
  }

  editItem(id: number) {
      this.occurenceTypesSvc.getById(id, '/occurenceTypes').subscribe(
          (res) => {
              this.occurenceType = new OccurenceType(res);
              this.occurenceTypeForm.setValue({
                  name: this.occurenceType.name,
                  abbreviation: this.occurenceType.abbreviation,
                  description: this.occurenceType.description
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
              this.occurenceTypesSvc.delete('/occurenceTypes', id).subscribe(
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
      if (this.occurenceTypeForm.invalid) {
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
                  this.occurenceType.name =
                      this.occurenceTypeForm.get('name').value;
                  this.occurenceType.abbreviation =
                      this.occurenceTypeForm.get('abbreviation').value;
                  this.occurenceType.description =
                      this.occurenceTypeForm.get('description').value;
              }

              let reqToProcess = this.editMode
                  ? this.occurenceTypesSvc.update(
                        '/occurenceTypes',
                        this.occurenceType
                    )
                  : this.occurenceTypesSvc.create(
                        '/occurenceTypes',
                        new OccurenceType(this.occurenceTypeForm.value)
                    );

              let replyMsg = `Occurence type ${
                  this.editMode ? 'updated' : 'created'
              } successfully!`;

              forkJoin([reqToProcess]).subscribe(
                  (res) => {
                      this.editMode = false;
                      this.toastr.success(replyMsg);
                      this.refreshItems();
                      this.occurenceTypeForm.reset();
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
      this.occurenceTypeForm.reset();
  }
}
