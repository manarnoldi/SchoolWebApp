import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@/core/core.module';
import { SharedModule } from '@/shared/shared.module';
import { DesignationsComponent } from './components/designations/designations.component';
import { SettingsComponent } from './settings.component';
import { OccupationsComponent } from './components/occupations/occupations.component';
import { EmploymentTypesComponent } from './components/employment-types/employment-types.component';
import { GenderComponent } from './components/gender/gender.component';
import { NationalitiesComponent } from './components/nationalities/nationalities.component';
import { OccurenceTypesComponent } from './components/occurence-types/occurence-types.component';
import { OutcomesComponent } from './components/outcomes/outcomes.component';
import { RelationshipsComponent } from './components/relationships/relationships.component';
import { ReligionsComponent } from './components/religions/religions.component';
import { SessionTypesComponent } from './components/session-types/session-types.component';
import { StaffCategoriesComponent } from './components/staff-categories/staff-categories.component';

@NgModule({
  declarations: [
    DesignationsComponent,
    OccupationsComponent,
    EmploymentTypesComponent,
    GenderComponent,
    NationalitiesComponent,
    SettingsComponent,
    OccurenceTypesComponent,
    OutcomesComponent,
    RelationshipsComponent,
    ReligionsComponent,
    SessionTypesComponent,
    StaffCategoriesComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class SettingsModule { }
