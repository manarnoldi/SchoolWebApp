import {Status} from '@/core/enums/status';
import {Nationality} from '@/settings/models/nationality';
import {Occupation} from '@/settings/models/occupation';
import {StudentParent} from '@/students/models/student-parent';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
    selector: 'app-student-parents-table',
    templateUrl: './student-parents-table.component.html',
    styleUrl: './student-parents-table.component.scss'
})
export class StudentParentsTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student parents list';
    @Input() studentParents: StudentParent[] = [];
    @Input() showLoginControls: Boolean = false;

    @Input() occupations: Occupation[] = [];
    @Input() nationalities: Nationality[] = [];

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<{}>();

    page = 1;
    pageSize = 10;

    constructor() {}

    statusVals = Status;
    statusValues;

    ngOnInit(): void {
        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
        );
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    tableHeaders: string[] = [
        'Full name',
        'Occupation',
        'Nationality',
        'Notifiable',
        'Payer',
        'Picker',
        'Status',
        'Relationship',
        'Action'
    ];

    deleteItem = (parentId: number, studentId: number) => {
        this.deleteItemEvent.emit({
            parentId: parentId,
            studentId: studentId
        });
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };

    viewItem = (id: number) => {
        this.viewItemEvent.emit(id);
    };
}
