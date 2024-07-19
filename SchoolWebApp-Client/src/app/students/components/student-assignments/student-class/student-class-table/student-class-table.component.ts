import { LearningLevel } from '@/class/models/learning-level';
import { SchoolStream } from '@/class/models/school-stream';
import { AcademicYear } from '@/school/models/academic-year';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import { StudentClass } from '@/students/models/student-class';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-student-class-table',
  templateUrl: './student-class-table.component.html',
  styleUrl: './student-class-table.component.scss'
})
export class StudentClassTableComponent  implements OnInit {
  @Input() tableTitle: string = 'Student classes list';
  @Input() studentClasses: StudentClass[] = [];
  @Input() academicYears: AcademicYear[] = [];
  @Input() schoolStreams: SchoolStream[] = [];
  @Input() learningLevels: LearningLevel[] = [];
  @Input() showLoginControls: Boolean = false;

  @Output() viewItemEvent = new EventEmitter<number>();
  @Output() editItemEvent = new EventEmitter<number>();
  @Output() deleteItemEvent = new EventEmitter<number>();

  page = 1;
  pageSize = 10;
  collectionSize = 0;
  pageSubscription: Subscription;
  pageSizeSubscription: Subscription;

  constructor(private tableSettingsSvc: TableSettingsService) {}

  ngOnInit(): void {
      this.pageSubscription = this.tableSettingsSvc.page.subscribe(
          (page) => (this.page = page)
      );
      this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
          (pageSize) => (this.pageSize = pageSize)
      );
  }

  tableHeaders: string[] = [
      'Student Full Name',
      'Academic Year',
      'Class',
      'Stream',        
      'Description',
      'Action'
  ];

  deleteItem = (id: number) => {
      this.deleteItemEvent.emit(id);
  };

  editItem = (id: number) => {
      this.editItemEvent.emit(id);
  };

  viewItem = (id: number) => {
      this.viewItemEvent.emit(id);
  };

}
