import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StudentAttendance} from '@/students/models/student-attendance';
import {StudentClass} from '@/students/models/student-class';
import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-students-attendances-table',
    templateUrl: './students-attendances-table.component.html',
    styleUrl: './students-attendances-table.component.scss',
    providers: [TableSettingsService]
})
export class StudentsAttendancesTableComponent implements OnInit {
    @Input() tableTitle: string = 'Students attendance list';
    @Input() studentClasses: StudentClass[] = [];
    @Input() currentDate: Date = new Date();
    @Input() disabled: Boolean = false;

    @Output() deleteItemEvent = new EventEmitter<number>();
    
    @ViewChild('checkAllStudents', { static: false }) checkAll: ElementRef;
    
   

    page = 1;
    pageSize = 20;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    constructor(private tableSettingsSvc: TableSettingsService) {}

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
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
        this.tableSettingsSvc.changePageSize(20);
        this.updateCheckAll();
    }

    tableHeaders: string[] = ['Adm No', 'Student Full Name', 'Date'];

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
