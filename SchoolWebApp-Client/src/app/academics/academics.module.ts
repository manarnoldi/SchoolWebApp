import { NgModule } from '@angular/core';
import { ExamTypesComponent } from './components/exam-types/exam-types.component';
import { CurriculumComponent } from './components/curriculum/curriculum.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { AcademicsComponent } from './academics.component';
import { SubjectGroupsComponent } from './components/subject-groups/subject-groups.component';
import { SubjectGroupsAddFormComponent } from './components/subject-groups/subject-groups-add-form/subject-groups-add-form.component';



@NgModule({
  declarations: [
    CurriculumComponent,
    ExamTypesComponent,
    AcademicsComponent,
    SubjectGroupsComponent,
    SubjectGroupsAddFormComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class AcademicsModule { }
