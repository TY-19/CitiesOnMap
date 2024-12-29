import {HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // const oAuthService = inject(OAuthService);
  //
  //
  // if (token) {
  //   req = req.clone({
  //     setHeaders: {
  //       'Authorization': `Bearer ${token}`
  //     }
  //   });
  // }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        // oAuthService.logOut();
        console.log("Logout performed.")
      }
      return throwError(() => error);
    })
  );
};
