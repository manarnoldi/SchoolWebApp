import { BreadCrumb } from '@/core/models/bread-crumb';
import { Outcome } from '@/settings/models/outcome';
import { OutcomesService } from '@/settings/services/outcomes.service';
import { SettingsTableComponent } from '@/shared/directives/settings-table/settings-table.component';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription, forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-outcomes',
  templateUrl: './outcomes.component.html',
  styleUrl: './outcomes.component.scss'
})
export class OutcomesComponent implements OnInit {
  @ViewChild('closebutton') closeButton;
  @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
  page = 1;
  pageSize = 10;
  pageSubscription: Subscription;
  pageSizeSubscription: Subscription;

  outcomeForm: FormGroup;

  buttonTitle: string = 'Add outcome';
  tableModel: string = 'outcome';
  tableTitle: string = 'Outcomes list';
  tableHeaders: string[] = ['Name', 'Description', 'Action'];

  editMode = false;
  outcome: Outcome;
  isAuthLoading: boolean;
  outcomes: Outcome[] = [];
  tblShowViewButton: false;

  collectionSize = 0;

  constructor(
      private outcomesSvc: OutcomesService,
      private toastr: ToastrService,
      private formBuilder: FormBuilder,
      private tableSettingsSvc: TableSettingsService
  ) {}
  closeResult = '';
  dashboardTitle = 'Outcomes list';
  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/settings/outcomes'], title: 'Settings:Outcomes'}
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
              this.outcomesSvc.delete('/outcomes', id).subscribe(
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
      this.outcomesSvc.getById(id, '/outcomes').subscribe(
          (res) => {
              this.outcome = new Outcome(res);
              this.outcomeForm.setValue({
                  name: this.outcome.name,
                  description: this.outcome.description
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
      if (this.outcomeForm.invalid) {
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
                  this.outcome.name =
                      this.outcomeForm.get('name').value;
                  this.outcome.description =
                      this.outcomeForm.get('description').value;
              }

              let reqToProcess = this.editMode
                  ? this.outcomesSvc.update(
                        '/outcomes',
                        this.outcome
                    )
                  : this.outcomesSvc.create(
                        '/outcomes',
                        new Outcome(this.outcomeForm.value)
                    );

              let replyMsg = `Outcome ${
                  this.editMode ? 'updated' : 'created'
              } successfully!`;
              forkJoin([reqToProcess]).subscribe(
                  (res) => {
                      this.editMode = false;
                      this.toastr.success(replyMsg);
                      this.refreshItems();
                      this.outcomeForm.reset();
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
      return this.outcomeForm.controls;
  }

  refreshItems() {
      this.outcomeForm = this.formBuilder.group({
          name: ['', [Validators.required]],
          description: ['']
      });

      this.outcomesSvc.get('/outcomes').subscribe(
          (res) => {
              this.collectionSize = res.length;
              this.outcomes = res.slice(
                  (this.page - 1) * this.pageSize,
                  (this.page - 1) * this.pageSize + this.pageSize
              );
              this.isAuthLoading = false;
              this.editMode = false;
          },
          (err) => {
              this.toastr.error(
                  'An error occured while fetching the outcomes. Contact system administrator.'
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
      this.outcomeForm.reset();
      this.editMode = false;
  }
}
