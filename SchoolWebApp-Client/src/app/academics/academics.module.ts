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
import { ExamsComponent } from './components/exams/exams.component';
import { ExamAddFormComponent } from './components/exams/exam-add-form/exam-add-form.component';
import { ExamTableComponent } from './components/exams/exam-table/exam-table.component';
import { EducationLevelSubjectsComponent } from './components/subjects/education-level-subjects/education-level-subjects.component';
import { EducationLevelSubjectsTableComponent } from './components/subjects/education-level-subjects/education-level-subjects-table/education-level-subjects-table.component';
import { EducationLevelSubjectsFormComponent } from './components/subjects/education-level-subjects/education-level-subjects-form/education-level-subjects-form.component';
import { ExamResultsComponent } from './components/exam-results/exam-results.component';
import { ExamResultsTableComponent } from './components/exam-results/exam-results-table/exam-results-table.component';

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
    ExamAddFormComponent,
    ExamTableComponent,
    ExamsComponent,
    EducationLevelSubjectsComponent,
    EducationLevelSubjectsTableComponent,
    EducationLevelSubjectsFormComponent,
    ExamResultsComponent,
    ExamResultsTableComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class AcademicsModule { }
