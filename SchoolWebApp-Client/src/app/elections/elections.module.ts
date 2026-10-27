import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {SharedModule} from '@/shared/shared.module';
import {ElectionsComponent} from './elections.component';
import {ElectionListComponent} from './components/election-list/election-list.component';
import {ElectionDetailComponent} from './components/election-detail/election-detail.component';
import {ElectionFormComponent} from './components/election-form/election-form.component';
import {PositionFormComponent} from './components/position-form/position-form.component';
import {NominationsComponent} from './components/nominations/nominations.component';
import {NominateFormComponent} from './components/nominate-form/nominate-form.component';
import {VoterRegisterComponent} from './components/voter-register/voter-register.component';

@NgModule({
    declarations: [
        ElectionsComponent,
        ElectionListComponent,
        ElectionDetailComponent,
        ElectionFormComponent,
        PositionFormComponent,
        NominationsComponent,
        NominateFormComponent,
        VoterRegisterComponent
    ],
    imports: [CommonModule, RouterModule, SharedModule]
})
export class ElectionsModule {}
