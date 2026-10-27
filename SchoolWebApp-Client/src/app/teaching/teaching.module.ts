import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {SharedModule} from '@/shared/shared.module';
import {ApprovalsModule} from '@/approvals/approvals.module';
import {TeachingComponent} from './teaching.component';
import {TeachingAssignmentsComponent} from './components/teaching-assignments/teaching-assignments.component';
import {SchemesOfWorkComponent} from './components/schemes-of-work/schemes-of-work.component';
import {LessonPlansComponent} from './components/lesson-plans/lesson-plans.component';
import {RecordOfWorkComponent} from './components/record-of-work/record-of-work.component';
import {TrackerComponent} from './components/tracker/tracker.component';
import {SchemeImportComponent} from './components/scheme-import/scheme-import.component';
import {CurriculumViewComponent} from './components/curriculum-view/curriculum-view.component';
import {TermPickerComponent} from './components/term-picker/term-picker.component';
import {FillHeightDirective} from './components/fill-height.directive';

@NgModule({
    declarations: [
        TeachingComponent,
        TeachingAssignmentsComponent,
        SchemesOfWorkComponent,
        LessonPlansComponent,
        RecordOfWorkComponent,
        TrackerComponent,
        SchemeImportComponent,
        CurriculumViewComponent,
        TermPickerComponent,
        FillHeightDirective
    ],
    imports: [CommonModule, RouterModule, SharedModule, ApprovalsModule]
})
export class TeachingModule {}
