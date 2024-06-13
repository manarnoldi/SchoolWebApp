import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@/core/core.module';
import { SharedModule } from '@/shared/shared.module';
import { DesignationsComponent } from './components/designations/designations.component';
import { SettingsComponent } from './settings.component';
import { OccupationsComponent } from './components/occupations/occupations.component';
import { EmploymentTypesComponent } from './components/employment-types/employment-types.component';
import { GenderComponent } from './components/gender/gender.component';

@NgModule({
  declarations: [
    DesignationsComponent,
    OccupationsComponent,
    EmploymentTypesComponent,
    GenderComponent,
    SettingsComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule
  ]
})
export class SettingsModule { }
