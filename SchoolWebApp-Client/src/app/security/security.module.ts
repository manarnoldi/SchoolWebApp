import {NgModule} from '@angular/core';
import {CoreModule} from '@/core/core.module';
import {SharedModule} from '@/shared/shared.module';
import {SecurityComponent} from './security.component';
import {UsersComponent} from './components/users/users.component';
import {RolesComponent} from './components/roles/roles.component';
import {MenuPermissionsComponent} from './components/menu-permissions/menu-permissions.component';
import {LogsComponent} from './components/logs/logs.component';
import {AuditLogsComponent} from './components/audit-logs/audit-logs.component';

@NgModule({
    declarations: [
        SecurityComponent,
        UsersComponent,
        RolesComponent,
        MenuPermissionsComponent,
        LogsComponent,
        AuditLogsComponent
    ],
    imports: [
        CoreModule,
        SharedModule
    ]
})
export class SecurityModule {}
