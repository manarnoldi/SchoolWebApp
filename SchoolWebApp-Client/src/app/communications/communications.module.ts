import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {SharedModule} from '@/shared/shared.module';
import {CommunicationsComponent} from './communications.component';
import {CommunicationSettingsComponent} from './components/communication-settings/communication-settings.component';
import {MessageTemplatesComponent} from './components/message-templates/message-templates.component';
import {ComposeMessageComponent} from './components/compose-message/compose-message.component';
import {MessageQueueComponent} from './components/message-queue/message-queue.component';
import {SmsReportComponent} from './components/sms-report/sms-report.component';

@NgModule({
    declarations: [
        CommunicationsComponent,
        CommunicationSettingsComponent,
        MessageTemplatesComponent,
        ComposeMessageComponent,
        MessageQueueComponent,
        SmsReportComponent
    ],
    imports: [CommonModule, RouterModule, SharedModule]
})
export class CommunicationsModule {}
