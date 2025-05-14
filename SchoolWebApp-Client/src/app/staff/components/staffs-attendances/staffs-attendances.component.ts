import {BreadCrumb} from '@/core/models/bread-crumb';
import {EmploymentType} from '@/settings/models/employment-type';
import {StaffCategory} from '@/settings/models/staff-category';
import {EmploymentTypeService} from '@/settings/services/employment-type.service';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {StaffAttendance} from '@/staff/models/staff-attendance';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {StaffAttendanceSearch} from '@/staff/models/staff-attendance-search';
import {DatePipe} from '@angular/common';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {StaffsAttendancesTableComponent} from './staffs-attendances/staffs-attendances-table/staffs-attendances-table.component';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-staffs-attendances',
    templateUrl: './staffs-attendances.component.html',
    styleUrl: './staffs-attendances.component.scss'
})
export class StaffsAttendancesComponent implements OnInit {
    @ViewChild(StaffsAttendancesTableComponent)
    staffsAttendancesTableComponent: StaffsAttendancesTableComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/staff/staff-attendances'], title: 'Staff: Attendances'}
    ];

    dashboardTitle = 'Staff: Attendances';
    staffCategories: StaffCategory[] = [];
    employmentTypes: EmploymentType[] = [];
    staffs: StaffDetails[] = [];
    attendanceDate: Date;
    saSearch: StaffAttendanceSearch;
    doneLoading = false;

    constructor(
        private toastr: ToastrService,
        private staffCategoriesSvc: StaffCategoriesService,
        private employmentTypesSvc: EmploymentTypeService,
        private staffsSvc: StaffDetailsService,
        private datePipe: DatePipe,
        private staffAttendancesSvc: StaffAttendancesService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    loadInitials = () => {
        let staffCategoriesReq =
            this.staffCategoriesSvc.get('/staffCategories');
        let employmentTypesReq =
            this.employmentTypesSvc.get('/employmentTypes');

        forkJoin([staffCategoriesReq, employmentTypesReq]).subscribe({
            next: ([staffCategories, employmentTypes]) => {
                this.staffCategories = staffCategories;
                this.employmentTypes = employmentTypes;
                this.doneLoading = true;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    dateChanged = (selectedDate: Date) => {
        this.staffs = [];
        this.attendanceDate = selectedDate;
    };

    employmentTypeChanged = (employTypeId: number) => {
        this.staffs = [];
    };
    staffCategoryChanged = (sCategoryId: number) => {
        this.staffs = [];
    };

    searchForDataMethod = (saSearch: StaffAttendanceSearch) => {
        this.staffs = [];
        this.saSearch = saSearch;

        this.attendanceDate = saSearch.attendanceDate;
        let searchStr = `/staffDetails/staffSearch?staffCategoryId=${saSearch.staffCategoryId ?? ''}&employmentTypeId=${saSearch.employmentTypeId ?? ''}`;
        this.staffsSvc.get(searchStr).subscribe({
            next: (staffs) => {
                if (staffs.length <= 0) {
                    this.toastr.info(
                        'No records found with the search parameters selected!'
                    );
                } else {
                    let checkStaffAttendancesReq = [];
                    staffs.forEach((sc) => {
                        let attendDate = this.datePipe.transform(
                            this.attendanceDate,
                            'yyyy-MM-dd'
                        );
                        let reqString =
                            '/staffAttendances/byStaffIdAttendanceDate/' +
                            sc.id +
                            '/' +
                            attendDate;
                        checkStaffAttendancesReq.push(
                            this.staffAttendancesSvc.getObjectBySearch(
                                reqString
                            )
                        );
                    });

                    forkJoin(checkStaffAttendancesReq).subscribe({
                        next: (saRes: StaffAttendance[]) => {
                            for (let i = 0; i < saRes.length; i++) {
                                if (saRes[i].id) {
                                    let [hourIn, minuteIn] = saRes[i].timeIn
                                        ? saRes[i].timeIn.split(':').map(Number)
                                        : [8, 0];
                                    let [hourOut, minuteOut] = saRes[i].timeIn
                                        ? saRes[i].timeIn.split(':').map(Number)
                                        : [17, 0];

                                    staffs[i].isSelected = saRes[i].present;
                                    staffs[i].remarks = saRes[i].remarks;
                                    staffs[i].hasRecord = true;
                                    staffs[i].timeIn = {
                                        hour: hourIn,
                                        minute: minuteIn
                                    };
                                    staffs[i].timeOut = {
                                        hour: hourOut,
                                        minute: minuteOut
                                    };
                                }
                            }
                            this.staffs = staffs.sort((a, b) =>
                                a.upi.localeCompare(b.upi)
                            );
                            this.staffsAttendancesTableComponent.staffs =
                                this.staffs;
                            this.staffsAttendancesTableComponent.updateCheckAll();
                        },
                        error: (err) => {
                            this.toastr.error(err.error);
                        }
                    });
                }
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    deleteAttendance(id: number) {
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
                const staff = this.staffs.find((i) => i.id == id.toString());
                let attendDate = this.datePipe.transform(
                    this.attendanceDate,
                    'yyyy-MM-dd'
                );
                this.staffAttendancesSvc
                    .getObjectBySearch(
                        '/staffAttendances/byStaffIdAttendanceDate/' +
                            id +
                            '/' +
                            attendDate
                    )
                    .subscribe({
                        next: (att) => {
                            this.staffAttendancesSvc
                                .delete('/staffAttendances', parseInt(att.id))
                                .subscribe(
                                    (res) => {
                                        if (staff) {
                                            staff.hasRecord = false;
                                            staff.isSelected = false;
                                            staff.isOriginallySelected = false;
                                            staff.remarks = '';
                                        }
                                        this.toastr.success(
                                            'Record deleted successfully!'
                                        );
                                    },
                                    (err) => {
                                        this.toastr.error(err);
                                    }
                                );
                        },
                        error: (err) => {
                            this.toastr.error(err);
                        }
                    });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }
    submitAttendances = () => {
        if (this.staffs.length <= 0)
        this.toastr.error('There are no records to save!');
      
        Swal.fire({
            title: `Update staff attendances records?`,
            text: `Confirm if you want to batch update staff attendances.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Update Attendances`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let staffAttendances: StaffAttendance[] = [];
                this.staffs.forEach((st) => {
                    let sa = new StaffAttendance();
                    let dateIn = new Date(
                        1970,
                        0,
                        1,
                        st.timeIn.hour,
                        st.timeIn.minute
                    );
                    let dateOut = new Date(
                        1970,
                        0,
                        1,
                        st.timeIn.hour,
                        st.timeIn.minute
                    );

                    sa.date = this.attendanceDate;
                    sa.present = st.isSelected;
                    sa.remarks = st.remarks;
                    sa.timeIn = this.datePipe.transform(dateIn, 'HH:mm:ss');
                    sa.timeOut = this.datePipe.transform(dateOut, 'HH:mm:ss');
                    sa.staffDetailsId = parseInt(st.id);

                    staffAttendances.push(sa);
                });

                this.staffAttendancesSvc
                    .createBatch(
                        '/staffAttendances/batch',
                        staffAttendances
                    )
                    .subscribe({
                        next: (res) => {
                            this.toastr.success(
                                'Staffattendances updated successfully.'
                            );
                            this.searchForDataMethod(this.saSearch);
                        },
                        error: (err) => {
                            this.toastr.error(err.error?.message);
                        }
                    });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
