import { NgModule } from '@angular/core';
import { SchoolComponent } from './school.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { LearningModesComponent } from './components/learning-modes/learning-modes.component';
import { ExamTypesComponent } from './components/exam-types/exam-types.component';
import { SchoolDetailsComponent } from './components/school-details/school-details.component';
import { SchoolStreamsComponent } from './components/school-streams/school-streams.component';
import { EducationLevelTypesComponent } from './components/education-level-types/education-level-types.component';
import { AcademicYearsComponent } from './components/academic-years/academic-years.component';

@NgModule({
  declarations: [
    SchoolComponent,
    LearningModesComponent,
    ExamTypesComponent,
    SchoolDetailsComponent,
    SchoolStreamsComponent,
    EducationLevelTypesComponent,
    AcademicYearsComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class SchoolModule { }
