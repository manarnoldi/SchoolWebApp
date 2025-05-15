import { NgModule } from '@angular/core';
import { SchoolComponent } from './school.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { LearningModesComponent } from './components/learning-modes/learning-modes.component';
import { SchoolDetailsComponent } from './components/school-details/school-details.component';
import { EducationLevelTypesComponent } from './components/education-level-types/education-level-types.component';
import { AcademicYearsComponent } from './components/academic-years/academic-years.component';
import { TodolistsComponent } from './components/todolists/todolists.component';
import { TodolistFormComponent } from './components/todolists/todolist-form/todolist-form.component';
import { DashboardSummaryComponent } from './components/dashboard-summary/dashboard-summary.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { EducationLevelsComponent } from './components/education-levels/education-levels.component';
import { EducationLevelFormComponent } from './components/education-levels/education-level-form/education-level-form.component';
import { DepartmentsComponent } from './components/departments/departments.component';
import { DepartmentsAddFormComponent } from './components/departments/departments-add-form/departments-add-form.component';
import { EventsComponent } from './components/events/events.component';
import { EventsAddFormComponent } from './components/events/events-add-form/events-add-form.component';
import { EventsTableComponent } from './components/events/events-table/events-table.component';
import { EventsDashboardComponent } from './components/events/events-dashboard/events-dashboard.component';
import { EventsDashboardItemComponent } from './components/events/events-dashboard/events-dashboard-item/events-dashboard-item.component';

@NgModule({
  declarations: [
    SchoolComponent,
    LearningModesComponent,
    SchoolDetailsComponent,    
    EducationLevelTypesComponent,
    AcademicYearsComponent,
    TodolistsComponent,
    TodolistFormComponent,
    DashboardComponent,
    DashboardSummaryComponent,    
    EducationLevelsComponent,
    EducationLevelFormComponent,
    DepartmentsComponent,
    DepartmentsAddFormComponent,
    EventsComponent,
    EventsAddFormComponent,
    EventsTableComponent,
    EventsDashboardComponent,
    EventsDashboardItemComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class SchoolModule { }
