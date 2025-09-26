import {NgModule} from '@angular/core';
import {AppComponent} from './app.component';
import {ToastrModule} from 'ngx-toastr';

import {DatePipe, registerLocaleData} from '@angular/common';
import localeEn from '@angular/common/locales/en';

import {CoreModule} from './core/core.module';
import {
    NgbActiveModal,
    NgbModule,
    NgbTimepickerConfig
} from '@ng-bootstrap/ng-bootstrap';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {environment} from 'environments/environment';
import {AuthInterceptor} from './core/interceptors/authconfig.interceptor';
import {LoaderInterceptor} from './core/interceptors/loader.interceptor';
import {SettingsModule} from './settings/settings.module';
import {NgxSpinnerModule} from 'ngx-spinner';
import {NgIdleModule} from '@ng-idle/core';
import {SchoolModule} from './school/school.module';
import {AcademicsModule} from './academics/academics.module';
import {StaffModule} from './staff/staff.module';
import {StudentsModule} from './students/students.module';
import {ClassModule} from './class/class.module';
import {ReportsModule} from './reports/reports.module';
import { CbeModule } from './cbe/cbe.module';
registerLocaleData(localeEn, 'en-EN');

@NgModule({
    declarations: [AppComponent],
    imports: [
        NgxSpinnerModule,
        CoreModule,
        SettingsModule,
        SchoolModule,
        ClassModule,
        AcademicsModule,
        StaffModule,
        ReportsModule,
        StudentsModule,
        CbeModule,
        ToastrModule.forRoot({
            timeOut: 10000,
            // positionClass: 'toast-top-full-width',
            preventDuplicates: true,
            closeButton: true,
            easing: 'ease-in',
            easeTime: 1000,
            progressBar: true,
            tapToDismiss: false
        }),
        NgbModule,
        NgIdleModule.forRoot()
    ],
    providers: [
        NgbActiveModal,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: LoaderInterceptor,
            multi: true
        },
        {provide: 'BASE_API_URL', useValue: environment.baseUrl},
        DatePipe,
        {
            provide: NgbTimepickerConfig,
            useFactory: () => {
                const config = new NgbTimepickerConfig();
                config.spinners = false;
                config.meridian = false;
                config.size = 'small';
                config.minuteStep = 10;
                return config;
            }
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
