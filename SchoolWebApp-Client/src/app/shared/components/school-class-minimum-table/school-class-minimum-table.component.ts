import {SchoolClass} from '@/class/models/school-class';
import {
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
    selector: 'app-school-class-minimum-table',
    templateUrl: './school-class-minimum-table.component.html',
    styleUrl: './school-class-minimum-table.component.scss'
})
export class SchoolClassMinimumTableComponent {
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() disabled: boolean = false;
    @Input() minimumTable: Boolean = false;
    @ViewChild('checkAllClasses', {static: false}) checkAll: ElementRef;

    @Output() schoolClassClickedEvent =new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    checkAllClicked = (inputSelectAll: any) => {
        if (inputSelectAll?.target?.checked) {
            this.schoolClasses.forEach((c) => {
                c.isSelected = true;
            });
        } else {
            this.schoolClasses.forEach((c) => {
                c.isSelected = false;
            });
        }
    };

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    schoolClassClicked = (schoolClassId: number) => {
        this.schoolClassClickedEvent.emit(schoolClassId);
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.schoolClasses.some((s) => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    };
}
