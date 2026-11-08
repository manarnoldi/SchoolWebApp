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
import {TimetableViewerComponent} from './components/viewer/timetable-viewer.component';
import {TimetablesComponent} from './components/timetables/timetables.component';
import {TimetableDetailComponent} from './components/timetable-detail/timetable-detail.component';
import {PublishedTimetableComponent} from './components/published/published-timetable.component';

@NgModule({
    declarations: [
        TimetableComponent,
        TimetableSetupComponent,
        BellSchedulesComponent,
        SubjectRulesComponent,
        OptionGroupsComponent,
        TeacherRulesComponent,
        ReadinessComponent,
        TimetableViewerComponent,
        TimetablesComponent,
        TimetableDetailComponent,
        PublishedTimetableComponent
    ],
    imports: [CommonModule, RouterModule, SharedModule]
})
export class TimetableModule {}
