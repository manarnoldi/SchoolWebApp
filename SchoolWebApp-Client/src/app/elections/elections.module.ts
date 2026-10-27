import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {SharedModule} from '@/shared/shared.module';
import {ElectionsComponent} from './elections.component';
import {ElectionListComponent} from './components/election-list/election-list.component';
import {ElectionDetailComponent} from './components/election-detail/election-detail.component';
import {ElectionFormComponent} from './components/election-form/election-form.component';
import {PositionFormComponent} from './components/position-form/position-form.component';

@NgModule({
    declarations: [
        ElectionsComponent,
        ElectionListComponent,
        ElectionDetailComponent,
        ElectionFormComponent,
        PositionFormComponent
    ],
    imports: [CommonModule, RouterModule, SharedModule]
})
export class ElectionsModule {}
