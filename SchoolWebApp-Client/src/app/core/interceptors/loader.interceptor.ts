import {Injectable} from '@angular/core';
import {
    HttpErrorResponse,
    HttpEvent,
    HttpHandler,
    HttpInterceptor,
    HttpRequest
} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {catchError, finalize} from 'rxjs/operators';
import {NgxSpinnerService} from 'ngx-spinner';
@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
    constructor(private spinner: NgxSpinnerService) {}
    handleError(error: HttpErrorResponse) {
        return throwError(error);
    }
    intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        this.spinner.show();
        return next.handle(req).pipe(
            finalize(() => this.spinner.hide()),
            catchError(this.handleError)
        );
    }
}
