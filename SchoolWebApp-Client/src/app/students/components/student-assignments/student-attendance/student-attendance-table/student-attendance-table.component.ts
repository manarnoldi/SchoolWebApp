import {SchoolClass} from '@/class/models/school-class';
import {StudentAttendance} from '@/students/models/student-attendance';
import {StudentDetails} from '@/students/models/student-details';
import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-student-attendance-table',
    templateUrl: './student-attendance-table.component.html',
    styleUrl: './student-attendance-table.component.scss'
})
export class StudentAttendanceTableComponent {
    @Input() tableTitle: string = 'Student attendance list ';
    @Input() studentAttendances: StudentAttendance[] = [];
    @Input() showEditDeleteControls: Boolean = true;
    @Input() showMinimum = false;

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    // tableHeaders: string[] = [
    //     'Student Full Name',
    //     'Adm no',
    //     'Class',
    //     'Stream',
    //     'Year',
    //     'Date',
    //     'Present?',
    //     'Remarks'
    // ];

     pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };
}
