import { NgModule } from '@angular/core';
import { ExamTypesComponent } from './components/exam-types/exam-types.component';
import { CurriculumComponent } from './components/curriculum/curriculum.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { AcademicsComponent } from './academics.component';



@NgModule({
  declarations: [
    CurriculumComponent,
    ExamTypesComponent,
    AcademicsComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class AcademicsModule { }
