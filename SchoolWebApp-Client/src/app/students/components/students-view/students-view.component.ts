import { BreadCrumb } from '@/core/models/bread-crumb';
import { StudentDetailsService } from '@/students/services/student-details.service';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-students-view',
  templateUrl: './students-view.component.html',
  styleUrl: './students-view.component.scss'
})
export class StudentsViewComponent implements OnInit {

  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/students/details'], title: 'Student: Students list'}
  ];

  dashboardTitle = 'Student: Students list';

  students;

  constructor(
      private studentsSvc: StudentDetailsService,
      private toarst: ToastrService
  ) {}

  ngOnInit(): void {
      this.loadStudents();
  }

  loadStudents = () => {
      this.studentsSvc.get('/students').subscribe(
          (res) => {
              this.students = [...res];
          },
          (err) => {
              this.toarst.error(err.error);
          }
      );
  };

}
