import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {SharedModule} from '@/shared/shared.module';
import {TimetableComponent} from './timetable.component';
import {TimetableSetupComponent} from './components/setup/timetable-setup.component';
import {BellSchedulesComponent} from './components/bell-schedules/bell-schedules.component';
import {SubjectRulesComponent} from './components/subject-rules/subject-rules.component';
import {OptionGroupsComponent} from './components/option-groups/option-groups.component';
import {TeacherRulesComponent} from './components/teacher-rules/teacher-rules.component';
import {ReadinessComponent} from './components/readiness/readiness.component';

@NgModule({
    declarations: [
        TimetableComponent,
        TimetableSetupComponent,
        BellSchedulesComponent,
        SubjectRulesComponent,
        OptionGroupsComponent,
        TeacherRulesComponent,
        ReadinessComponent
    ],
    imports: [CommonModule, RouterModule, SharedModule]
})
export class TimetableModule {}
