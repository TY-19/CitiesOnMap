import {HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";
import { AuthService } from "./auth.service";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  let token = authService.accessToken;
  const expirationTime = authService.accessTokenExpiration?.getTime();
  if(token && expirationTime) {
    if (new Date().getTime() > expirationTime - 30 * 1000) {
      authService.refreshTokens()
        .subscribe(res => token = res.accessToken ?? "")
    }
    req = req.clone({
      setHeaders: {
        'Authorization': `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        authService.logOut();
        console.log("Logout performed.")
      }
      return throwError(() => error);
    })
  );
};
