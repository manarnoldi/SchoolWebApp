import { NgModule } from '@angular/core';
import { ParentsComponent } from './parents.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { ParentAddFormComponent } from './components/parent-add-form/parent-add-form.component';
import { ParentViewComponent } from './components/parent-view/parent-view.component';
import { ParentsListComponent } from './components/parents-list/parents-list.component';
import { ParentsTableComponent } from './components/parents-table/parents-table.component';
import { DataTablesModule } from 'angular-datatables';
import { StudentsModule } from '@/students/students.module';

@NgModule({
  declarations: [
    ParentsComponent,
    ParentAddFormComponent,
    ParentsListComponent,
    ParentViewComponent,
    ParentsTableComponent
  ],
  imports: [
    CoreModule,
    SharedModule,
    DataTablesModule,
    StudentsModule
  ]
})
export class ParentsModule { }
