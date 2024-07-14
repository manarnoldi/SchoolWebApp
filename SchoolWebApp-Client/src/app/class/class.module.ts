import { NgModule } from '@angular/core';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { LearningLevelsComponent } from './components/learning-levels/learning-levels.component';
import { ClassComponent } from './class.component';
import { LearningLevelsFormComponent } from './components/learning-levels/learning-levels-form/learning-levels-form.component';
import { SchoolClassAddFormComponent } from './components/school-class/school-class-add-form/school-class-add-form.component';
import { SchoolClassComponent } from './components/school-class/school-class.component';
import { SchoolStreamsComponent } from './components/school-streams/school-streams.component';
import { SessionFormComponent } from './components/sessions/session-form/session-form.component';
import { SessionsComponent } from './components/sessions/sessions.component';
import { ClassLeadershipRolesComponent } from './components/class-leadership-roles/class-leadership-roles.component';
import { ClassDetailsViewComponent } from './components/school-class/class-details/class-details-view/class-details-view.component';
import { ClassLeadershipTableComponent } from './components/school-class/class-details/class-leadership-table/class-leadership-table.component';
import { ClassLeadershipAddFormComponent } from './components/school-class/class-details/class-leadership-add-form/class-leadership-add-form.component';
import { ClassDetailsComponent } from './components/school-class/class-details/class-details.component';

@NgModule({
  declarations: [
    ClassComponent,
    LearningLevelsComponent,
    LearningLevelsFormComponent,
    SchoolStreamsComponent,
    SessionsComponent,
    SessionFormComponent,
    SchoolClassComponent,
    SchoolClassAddFormComponent,
    ClassLeadershipRolesComponent,
    ClassDetailsViewComponent,
    ClassLeadershipTableComponent,
    ClassLeadershipAddFormComponent,
    ClassDetailsComponent
  ],
  imports: [
    CoreModule,
    SharedModule
  ]
})
export class ClassModule { }
