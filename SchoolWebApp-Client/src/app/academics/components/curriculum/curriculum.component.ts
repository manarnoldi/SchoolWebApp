import { BreadCrumb } from '@/core/models/bread-crumb';
import { Curriculum } from '@/academics/models/curriculum';
import { CurriculumService } from '@/academics/services/curriculum.service';
import { TableButtonComponent } from '@/shared/directives/table-button/table-button.component';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription, forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-curriculum',
  templateUrl: './curriculum.component.html',
  styleUrl: './curriculum.component.scss'
})
export class CurriculumComponent  implements OnInit{
  @ViewChild('closebutton') closeButton;
  @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/settings/curricula'], title: 'Settings: Curricula'}
  ];

  dashboardTitle = 'Settings:  Curricula';
  tableTitle: string = ' Curricula list';
  tableHeaders: string[] = ['Name', 'Code', 'Description', 'Action'];

  curriculum: Curriculum;
  curricula: Curriculum[] = [];
  tblShowViewButton: true;
  editMode = false;
  isAuthLoading: boolean;
  page = 1;
  pageSize = 10;
  collectionSize = 0;
  pageSubscription: Subscription;
  pageSizeSubscription: Subscription;
  tableModel: string = 'curriculum';

  curriculumForm: FormGroup;

  constructor(
      private curriculaSvc: CurriculumService,
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
      return this.curriculumForm.controls;
  }

  refreshItems() {
      this.curriculumForm = this.formBuilder.group({
          name: ['', [Validators.required]],
          code: [''],
          description: ['']
      });

      let curriculaRequest =
          this.curriculaSvc.get('/curricula');

      forkJoin([curriculaRequest]).subscribe(
          (res) => {
              this.collectionSize = res[0].length;
              this.curricula = res[0].slice(
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
      this.curriculaSvc.getById(id, '/curricula').subscribe(
          (res) => {
              this.curriculum = new Curriculum(res);
              this.curriculumForm.setValue({
                  name: this.curriculum.name,
                  code: this.curriculum.code,
                  description: this.curriculum.description
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
              this.curriculaSvc.delete('/curricula', id).subscribe(
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
      if (this.curriculumForm.invalid) {
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
                  this.curriculum.name =
                      this.curriculumForm.get('name').value;
                  this.curriculum.code=
                      this.curriculumForm.get('code').value;
                  this.curriculum.description =
                      this.curriculumForm.get('description').value;
              }

              let reqToProcess = this.editMode
                  ? this.curriculaSvc.update(
                        '/curricula',
                        this.curriculum
                    )
                  : this.curriculaSvc.create(
                        '/curricula',
                        new Curriculum(this.curriculumForm.value)
                    );

              let replyMsg = `Curriculum ${
                  this.editMode ? 'updated' : 'created'
              } successfully!`;

              forkJoin([reqToProcess]).subscribe(
                  (res) => {
                      this.editMode = false;
                      this.toastr.success(replyMsg);
                      this.refreshItems();
                      this.curriculumForm.reset();
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
      this.curriculumForm.reset();
  }
}
