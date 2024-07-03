import { BreadCrumb } from '@/core/models/bread-crumb';
import { StaffDetails } from '@/staff/models/staff-details';
import { StaffDetailsService } from '@/staff/services/staff-details.service';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-staff-details',
  templateUrl: './staff-details.component.html',
  styleUrl: './staff-details.component.scss'
})
export class StaffDetailsComponent implements OnInit {
  dashboardTitle = 'Staff details list';
  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/staff/details'], title: 'School:staff details'}
  ];

  searchedStaffs: StaffDetails[] = [];
  staffs: StaffDetails[] = [];

  constructor(
      private staffsSvc: StaffDetailsService,
      private toastr: ToastrService
  ) {}

  ngOnInit(): void {
      this.refreshItems();
  }

  editItem(id: number) {}

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
              this.staffsSvc.delete('/staffDetails', id).subscribe(
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

  refreshItems = () => {
      let staffsRequest = this.staffsSvc.get('/staffDetails');
      forkJoin([staffsRequest]).subscribe(
          (res) => {
              this.staffs = res[0];
              this.searchedStaffs = res[0];
          },
          (err) => {
              this.toastr.error(err.error);
          }
      );
  };

  resetForm = () => {};

}
