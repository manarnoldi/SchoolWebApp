import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@/core/core.module';
import { SharedModule } from '@/shared/shared.module';
import { DesignationsComponent } from './components/designations/designations.component';
import { SettingsComponent } from './settings.component';

@NgModule({
  declarations: [
    DesignationsComponent,
    SettingsComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule
  ]
})
export class SettingsModule { }
