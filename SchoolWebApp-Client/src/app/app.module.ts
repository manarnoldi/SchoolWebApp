import {NgModule} from '@angular/core';
import {AppComponent} from './app.component';
import {ToastrModule} from 'ngx-toastr';

import {DatePipe, registerLocaleData} from '@angular/common';
import localeEn from '@angular/common/locales/en';

import { CoreModule } from './core/core.module';
import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { environment } from 'environments/environment';
import { AuthInterceptor } from './core/interceptors/authconfig.interceptor';
import { LoaderInterceptor } from './core/interceptors/loader.interceptor';

registerLocaleData(localeEn, 'en-EN');

@NgModule({
    declarations: [
        AppComponent        
    ],
    imports: [               
        CoreModule,                
        ToastrModule.forRoot({
            timeOut: 10000,
            positionClass: 'toast-top-full-width',
            preventDuplicates: true,
            closeButton: true,
            easing: 'ease-in',
            easeTime: 1000,
            progressBar: true
        }), NgbModule        
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
        DatePipe
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
