import {NgModule} from '@angular/core';
import {CoreModule} from '@/core/core.module';
import {SharedModule} from '@/shared/shared.module';
import {StudentsComponent} from './students.component';
import {StudentDetailsComponent} from './components/student-details/student-details.component';
import {StudentsViewComponent} from './components/students-view/students-view.component';
import {StudentsTableComponent} from './components/students-table/students-table.component';
import { DataTablesModule } from 'angular-datatables';

@NgModule({
    declarations: [
        StudentsComponent,
        StudentDetailsComponent,
        StudentsViewComponent,
        StudentsTableComponent
    ],
    imports: [DataTablesModule, CoreModule, SharedModule]
})
export class StudentsModule {}
