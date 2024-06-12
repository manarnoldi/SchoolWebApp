import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {LoginComponent} from './auth/login/login.component';
import {FooterComponent} from './modules/main/footer/footer.component';
import {HeaderComponent} from './modules/main/header/header.component';
import {MainComponent} from './modules/main/main.component';
import {MenuSidebarComponent} from './modules/main/menu-sidebar/menu-sidebar.component';
import {BlankComponent} from './pages/blank/blank.component';
import {ProfileComponent} from './pages/profile/profile.component';
import {MenuItemComponent} from './components/menu-item/menu-item.component';
import {SidebarSearchComponent} from './components/sidebar-search/sidebar-search.component';
import {ControlSidebarComponent} from './modules/main/control-sidebar/control-sidebar.component';
import {LanguageComponent} from './modules/main/header/language/language.component';
import {MessagesComponent} from './modules/main/header/messages/messages.component';
import {NotificationsComponent} from './modules/main/header/notifications/notifications.component';
import {UserComponent} from './modules/main/header/user/user.component';
import {DashboardComponent} from './pages/dashboard/dashboard.component';
import {MainMenuComponent} from './pages/main-menu/main-menu.component';
import {SubMenuComponent} from './pages/main-menu/sub-menu/sub-menu.component';
import {authReducer} from './store/auth/reducer';
import {uiReducer} from './store/ui/reducer';
import {StoreModule} from '@ngrx/store';
import {ForgotPasswordComponent} from './auth/forgot-password/forgot-password.component';
import {RecoverPasswordComponent} from './auth/recover-password/recover-password.component';
import {RegisterComponent} from './auth/register/register.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { ProfabricComponentsModule } from '@profabric/angular-components';
import { AppRoutingModule } from '@/app-routing.module';

@NgModule({
    declarations: [
        MainComponent,
        LoginComponent,
        HeaderComponent,
        FooterComponent,
        MenuSidebarComponent,
        BlankComponent,
        ProfileComponent,
        DashboardComponent,
        MessagesComponent,
        NotificationsComponent,
        UserComponent,
        LanguageComponent,
        MainMenuComponent,
        SubMenuComponent,
        MenuItemComponent,
        ControlSidebarComponent,
        SidebarSearchComponent,
        RegisterComponent,
        ForgotPasswordComponent,
        RecoverPasswordComponent
    ],
    imports: [
        ProfabricComponentsModule,
        AppRoutingModule,
        CommonModule,
        BrowserModule,
        HttpClientModule,
        BrowserAnimationsModule,
        StoreModule.forRoot({auth: authReducer, ui: uiReducer}),
        ReactiveFormsModule,
        FormsModule
    ],
    exports: [
        MainComponent,
        LoginComponent,
        HeaderComponent,
        FooterComponent,
        MenuSidebarComponent,
        BlankComponent,
        ProfileComponent,
        DashboardComponent,
        MessagesComponent,
        NotificationsComponent,
        UserComponent,
        LanguageComponent,
        MainMenuComponent,
        SubMenuComponent,
        MenuItemComponent,
        ControlSidebarComponent,
        SidebarSearchComponent,
        RegisterComponent,
        ForgotPasswordComponent,
        RecoverPasswordComponent,
        AppRoutingModule,
        ReactiveFormsModule,
        FormsModule
    ]
})
export class CoreModule {}