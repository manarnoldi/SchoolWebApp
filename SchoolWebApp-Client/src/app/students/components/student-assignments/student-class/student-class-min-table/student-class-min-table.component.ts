import {Status} from '@/core/enums/status';
import {LearningMode} from '@/school/models/learning-mode';
import {LearningModesService} from '@/school/services/learning-modes.service';
import {TableSettingsService} from '@/shared/services/table-settings.service';
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
import {ToastrService} from 'ngx-toastr';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-student-class-min-table',
    templateUrl: './student-class-min-table.component.html',
    styleUrl: './student-class-min-table.component.scss'
})
export class StudentClassMinTableComponent implements OnInit {
    @Input() studentClasses: StudentClass[] = [];
    @Input() tableTitle: string = 'Parent students list';
    @Input() showCheckBoxes: boolean = false;
    @Input() disabled: Boolean = false;
    @Input() showMinimum: Boolean = false;

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    @ViewChild('checkAllStudents', {static: false}) checkAll: ElementRef;

    tableHeaders = [];

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    statusVals = Status;
    statusValues;

    learningModes: LearningMode[] = [];

    constructor(
        private tableSettingsSvc: TableSettingsService,
        private learningModesSvc: LearningModesService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
        );

        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );

        this.learningModesSvc.get('/learningModes').subscribe(
            (learningModes) => {
                this.learningModes = learningModes;
            },
            (err) => {
                this.toastr.error(err.error.message);
            }
        );

        this.tableHeaders = [
            {name: 'Admission no', min: false},
            {name: 'Full', min: false},
            {name: 'Admission date', min: this.showMinimum},
            {name: 'Learning mode', min: this.showMinimum},
            {name: 'Status', min: false}
        ];
    }

    viewStudent = (id: number) => {
        this.viewItemEvent.emit(id);
    };

    editStudent = (id: number) => {
        this.editItemEvent.emit(id);
    };

    deleteStudent = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

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
}
