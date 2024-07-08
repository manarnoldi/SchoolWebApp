import { NgModule } from '@angular/core';
import { SchoolComponent } from './school.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { LearningModesComponent } from './components/learning-modes/learning-modes.component';
import { SchoolDetailsComponent } from './components/school-details/school-details.component';
import { SchoolStreamsComponent } from './components/school-streams/school-streams.component';
import { EducationLevelTypesComponent } from './components/education-level-types/education-level-types.component';
import { AcademicYearsComponent } from './components/academic-years/academic-years.component';
import { TodolistsComponent } from './components/todolists/todolists.component';
import { TodolistFormComponent } from './components/todolists/todolist-form/todolist-form.component';
import { DashboardSummaryComponent } from './components/dashboard-summary/dashboard-summary.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { CurriculumComponent } from '../academics/components/curriculum/curriculum.component';
import { SessionsComponent } from './components/sessions/sessions.component';
import { SessionFormComponent } from './components/sessions/session-form/session-form.component';
import { EducationLevelsComponent } from './components/education-levels/education-levels.component';
import { EducationLevelFormComponent } from './components/education-levels/education-level-form/education-level-form.component';
import { LearningLevelsComponent } from './components/learning-levels/learning-levels.component';
import { LearningLevelsFormComponent } from './components/learning-levels/learning-levels-form/learning-levels-form.component';
import { DepartmentsComponent } from './components/departments/departments.component';
import { DepartmentsAddFormComponent } from './components/departments/departments-add-form/departments-add-form.component';

@NgModule({
  declarations: [
    SchoolComponent,
    LearningModesComponent,
    SchoolDetailsComponent,
    SchoolStreamsComponent,
    EducationLevelTypesComponent,
    AcademicYearsComponent,
    TodolistsComponent,
    TodolistFormComponent,
    DashboardComponent,
    DashboardSummaryComponent,
    SessionsComponent,
    SessionFormComponent,
    EducationLevelsComponent,
    EducationLevelFormComponent,
    LearningLevelsComponent,
    LearningLevelsFormComponent,
    DepartmentsComponent,
    DepartmentsAddFormComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class SchoolModule { }
