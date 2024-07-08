import { NgModule } from '@angular/core';
import { ExamTypesComponent } from './components/exam-types/exam-types.component';
import { CurriculumComponent } from './components/curriculum/curriculum.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { AcademicsComponent } from './academics.component';
import { SubjectGroupsComponent } from './components/subject-groups/subject-groups.component';
import { SubjectGroupsAddFormComponent } from './components/subject-groups/subject-groups-add-form/subject-groups-add-form.component';
import { GradesComponent } from './components/grades/grades.component';
import { GradesAddFormComponent } from './components/grades/grades-add-form/grades-add-form.component';



@NgModule({
  declarations: [
    CurriculumComponent,
    ExamTypesComponent,
    AcademicsComponent,
    SubjectGroupsComponent,
    SubjectGroupsAddFormComponent,
    GradesComponent,
    GradesAddFormComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class AcademicsModule { }
