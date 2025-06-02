import {StudentClass} from '@/students/models/student-class';
import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';

@Component({
    selector: 'app-students-attendances-table',
    templateUrl: './students-attendances-table.component.html',
    styleUrl: './students-attendances-table.component.scss'
})
export class StudentsAttendancesTableComponent implements OnInit {
    @Input() tableTitle: string = 'Students attendance list';
    @Input() studentClasses: StudentClass[] = [];
    @Input() currentDate: Date = new Date();
    @Input() disabled: Boolean = false;

    @Output() deleteItemEvent = new EventEmitter<number>();

    @ViewChild('checkAllStudents', {static: false}) checkAll: ElementRef;

    tableHeaders: string[] = ['Adm No', 'Student Full Name', 'Date'];

    page = 1;
    pageSize = 20;

    constructor() {}

    updateCheckAll = () => {
        if (this.checkAll) {
            if (this.studentClasses.length <= 0) {
                this.checkAll.nativeElement.checked = false;
            } else if (this.studentClasses.some((s) => !s.isSelected)) {
                this.checkAll.nativeElement.checked = false;
            } else {
                this.checkAll.nativeElement.checked = true;
            }
        }
    };

    ngOnInit(): void {
        this.updateCheckAll();
    }

    checkAllClicked = (inputSelectAll: any) => {
        if (inputSelectAll?.target?.checked) {
            this.studentClasses.forEach((c) => {
                c.isSelected = true;
            });
        } else {
            this.studentClasses.forEach((c) => {
                c.isSelected = false;
            });
        }
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.studentClasses.some((s) => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    };

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };
}
