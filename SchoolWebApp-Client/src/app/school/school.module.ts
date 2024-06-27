import { NgModule } from '@angular/core';
import { SchoolComponent } from './school.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { LearningModesComponent } from './components/learning-modes/learning-modes.component';
import { ExamTypesComponent } from './components/exam-types/exam-types.component';
import { SchoolDetailsComponent } from './components/school-details/school-details.component';

@NgModule({
  declarations: [
    SchoolComponent,
    LearningModesComponent,
    ExamTypesComponent,
    SchoolDetailsComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class SchoolModule { }
