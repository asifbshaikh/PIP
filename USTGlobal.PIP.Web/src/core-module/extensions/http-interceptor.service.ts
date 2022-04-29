import { Injectable } from '@angular/core';
import {
    HttpInterceptor,
    HttpRequest,
    HttpResponse,
    HttpHandler,
    HttpEvent,
    HttpErrorResponse
} from '@angular/common/http';

import { Observable, throwError } from 'rxjs';
import { map, catchError, retry, finalize } from 'rxjs/operators';
import { MessageService } from 'primeng/api';
import { SpinnerService } from '../spinner/spinner.service';
import { Router } from '@angular/router';

@Injectable()
export class HttpClientInterceptor implements HttpInterceptor {
    constructor(
        private messageService: MessageService,
        public spinnerService: SpinnerService,
        private router: Router
    ) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.spinnerService.start();
        if (!request.headers.has('Content-Type')) {
            if (!request.url.includes('uploadMultipleUsers')) {
                request = request.clone({ headers: request.headers.set('Content-Type', 'application/json') });
            }
        }

        request = request.clone({ headers: request.headers.set('observe', 'response') });
        request = request.clone({ headers: request.headers.set('Accept', 'application/json') });

        return next.handle(request).pipe(
            retry(1),
            map((event: HttpEvent<any>) => {
                let successMessage = '';
                if (event instanceof HttpResponse) {
                    if (event.body && event.body.success) {
                        successMessage = `Success Code: ${event.status}\nMessage: ${event.statusText}`;
                        this.messageService.add({ severity: 'success', detail: successMessage });
                    }
                    return event;
                }
            }),
            catchError((error: HttpErrorResponse) => {
                let errorMessage = '';
                if (error.status === 401) {
                    this.router.navigate(['/unauthorised']);
                } else {
                    if (error.error instanceof ErrorEvent) {
                        // client-side error
                        errorMessage = `Error: ${error.error.message}`;
                    } else {
                        // server-side error
                        errorMessage = `Error Code: ${error.status}\nMessage: ${error.statusText}`;
                    }
                    this.messageService.add({ severity: 'error', detail: errorMessage });
                    return throwError(errorMessage);
                }
            }),
            finalize(() => this.spinnerService.stop())
        );
    }
}
