import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ButtonComponent} from './directives/button/button.component';
import {DashboardHeaderComponent} from './directives/dashboard-header/dashboard-header.component';
import {DropdownComponent} from './directives/dropdown/dropdown.component';
import {DropdownMenuComponent} from './directives/dropdown-menu/dropdown-menu.component';
import {ItemLinkComponent} from './directives/item-link/item-link.component';
import {SettingsControlsComponent} from './directives/settings-controls/settings-controls.component';
import {SettingsTableComponent} from './directives/settings-table/settings-table.component';
import {TableActionComponent} from './directives/table-action/table-action.component';
import {TableHeadingComponent} from './directives/table-heading/table-heading.component';
import {TablePagingComponent} from './directives/table-paging/table-paging.component';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {CoreModule} from '@/core/core.module';
import {TableButtonComponent} from './directives/table-button/table-button.component';

@NgModule({
    declarations: [
        ButtonComponent,
        DashboardHeaderComponent,
        DropdownComponent,
        DropdownMenuComponent,
        ItemLinkComponent,
        SettingsControlsComponent,
        SettingsTableComponent,
        TableActionComponent,
        TableHeadingComponent,
        TablePagingComponent,
        TableButtonComponent
    ],
    imports: [CommonModule, NgbModule, CoreModule],
    exports: [
        ButtonComponent,
        DashboardHeaderComponent,
        DropdownComponent,
        DropdownMenuComponent,
        ItemLinkComponent,
        SettingsControlsComponent,
        SettingsTableComponent,
        TableActionComponent,
        TableHeadingComponent,
        TablePagingComponent,
        TableButtonComponent,
        NgbModule
    ]
})
export class SharedModule {}
