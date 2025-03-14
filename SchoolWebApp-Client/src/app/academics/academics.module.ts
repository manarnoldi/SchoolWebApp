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
import { SubjectsComponent } from './components/subjects/subjects.component';
import { SubjectsAddFormComponent } from './components/subjects/subjects-add-form/subjects-add-form.component';
import { SubjectsTableComponent } from './components/subjects/subjects-table/subjects-table.component';
import { ExamsComponent } from './components/exams/exams.component';
import { ExamListSearchFormComponent } from './components/exams/exam-list-search-form/exam-list-search-form.component';
import { ExamAddFormComponent } from './components/exams/exam-add-form/exam-add-form.component';
import { ExamTableComponent } from './components/exams/exam-table/exam-table.component';



@NgModule({
  declarations: [
    CurriculumComponent,
    ExamTypesComponent,
    ExamsComponent,
    AcademicsComponent,
    SubjectGroupsComponent,
    SubjectGroupsAddFormComponent,
    GradesComponent,
    GradesAddFormComponent,
    SubjectsComponent,
    SubjectsAddFormComponent,
    SubjectsTableComponent,
    ExamListSearchFormComponent,
    ExamAddFormComponent,
    ExamTableComponent,
    ExamsComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ],
  exports:[SubjectsTableComponent]
})
export class AcademicsModule { }
