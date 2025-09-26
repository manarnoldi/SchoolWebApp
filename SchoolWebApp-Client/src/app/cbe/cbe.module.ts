import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompetenciesComponent } from './assessments/components/competencies/competencies.component';
import { AssessmentTypesComponent } from './assessments/components/assessment-types/assessment-types.component';
import { SharedModule } from '@/shared/shared.module';
import { CbeComponent } from './cbe.component';



@NgModule({
  declarations: [
    CbeComponent,
    CompetenciesComponent,
    AssessmentTypesComponent
  ],
  imports: [
    CommonModule, SharedModule
  ]
})
export class CbeModule { }
