import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {CoreModule} from '@/core/core.module';
import {SharedModule} from '@/shared/shared.module';
import {ApprovalWorkflowsComponent} from './components/approval-workflows/approval-workflows.component';
import {ApprovalWebpartComponent} from './components/approval-webpart/approval-webpart.component';
import {ApprovalHistoryComponent} from './components/approval-history/approval-history.component';

@NgModule({
    declarations: [
        ApprovalWorkflowsComponent,
        ApprovalWebpartComponent,
        ApprovalHistoryComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        CoreModule,
        SharedModule
    ],
    exports: [
        ApprovalWebpartComponent,
        ApprovalHistoryComponent
    ]
})
export class ApprovalsModule {}
