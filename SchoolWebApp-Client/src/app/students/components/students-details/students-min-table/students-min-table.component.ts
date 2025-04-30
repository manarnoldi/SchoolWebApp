import {Status} from '@/core/enums/status';
import {LearningMode} from '@/school/models/learning-mode';
import {LearningModesService} from '@/school/services/learning-modes.service';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StudentDetails} from '@/students/models/student-details';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-students-min-table',
    templateUrl: './students-min-table.component.html',
    styleUrl: './students-min-table.component.scss'
})
export class StudentsMinTableComponent implements OnInit {
    @Input() students: StudentDetails[] = [];
    @Input() tableTitle: string = 'Parent students list';
    
    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    tableHeaders: string[] = [
        'Admission no',
        'Full name',
        'Admission date',
        'Learning mode',
        'Status'
    ];

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
        private toastr:ToastrService
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
}
