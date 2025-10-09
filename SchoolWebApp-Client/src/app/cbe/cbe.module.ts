import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompetenciesComponent } from './assessments/components/competencies/competencies.component';
import { AssessmentTypesComponent } from './assessments/components/assessment-types/assessment-types.component';
import { SharedModule } from '@/shared/shared.module';
import { CbeComponent } from './cbe.component';
import { GeneralOutcomesComponent } from './assessments/components/general-outcomes/general-outcomes.component';
import { GeneralOutcomeAddFormComponent } from './assessments/components/general-outcomes/general-outcome-add-form/general-outcome-add-form.component';
import { StrandsComponent } from './assessments/components/strands/strands.component';
import { StrandAddFormComponent } from './assessments/components/strands/strand-add-form/strand-add-form.component';



@NgModule({
  declarations: [
    CbeComponent,
    CompetenciesComponent,
    AssessmentTypesComponent,
    GeneralOutcomesComponent,
    GeneralOutcomeAddFormComponent,
    StrandAddFormComponent,
    StrandsComponent
  ],
  imports: [
    CommonModule, SharedModule
  ]
})
export class CbeModule { }
