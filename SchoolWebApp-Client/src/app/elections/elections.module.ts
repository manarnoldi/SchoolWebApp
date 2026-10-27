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
import {StationsComponent} from './components/stations/stations.component';
import {PollingComponent} from './components/polling/polling.component';
import {StationScreenComponent} from './components/station-screen/station-screen.component';
import {ResultsComponent} from './components/results/results.component';
import {ResultsHomeComponent} from './components/results-home/results-home.component';
import {LeadershipComponent} from './components/leadership/leadership.component';

@NgModule({
    declarations: [
        ElectionsComponent,
        ElectionListComponent,
        ElectionDetailComponent,
        ElectionFormComponent,
        PositionFormComponent,
        NominationsComponent,
        NominateFormComponent,
        VoterRegisterComponent,
        StationsComponent,
        PollingComponent,
        StationScreenComponent,
        ResultsComponent,
        ResultsHomeComponent,
        LeadershipComponent
    ],
    imports: [CommonModule, RouterModule, SharedModule]
})
export class ElectionsModule {}
