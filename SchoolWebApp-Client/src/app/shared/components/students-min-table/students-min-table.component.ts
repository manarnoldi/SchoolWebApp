import {Status} from '@/core/enums/status';
import {LearningMode} from '@/school/models/learning-mode';
import {LearningModesService} from '@/school/services/learning-modes.service';
import {StudentDetails} from '@/students/models/student-details';
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

@Component({
    selector: 'app-students-min-table',
    templateUrl: './students-min-table.component.html',
    styleUrl: './students-min-table.component.scss'
})
export class StudentsMinTableComponent implements OnInit {
    @Input() students: StudentDetails[] = [];
    @Input() tableTitle: string = 'Parent students list';
    @Input() showCheckBoxes: boolean = false;
    @Input() disabled: Boolean = false;
    @Input() showMinimum: Boolean = false;

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    @Output() studentClickedEvent = new EventEmitter<number>();

    @ViewChild('checkAllStudents', {static: false}) checkAll: ElementRef;

    tableHeaders = [];

    page = 1;
    pageSize = 10;

    statusVals = Status;
    statusValues;

    learningModes: LearningMode[] = [];

    constructor(
        private learningModesSvc: LearningModesService,
        private toastr: ToastrService
    ) {}

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    ngOnInit(): void {
        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
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
            this.students.forEach((c) => {
                c.isSelected = true;
            });
        } else {
            this.students.forEach((c) => {
                c.isSelected = false;
            });
        }
    };

    studentClicked = (studentId: number) => {
        this.studentClickedEvent.emit(studentId);
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.students.some((s) => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    };
}
