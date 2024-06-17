import { NgModule } from '@angular/core';
import { SchoolComponent } from './school.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { LearningModesComponent } from './components/learning-modes/learning-modes.component';

@NgModule({
  declarations: [
    SchoolComponent,
    LearningModesComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class SchoolModule { }
