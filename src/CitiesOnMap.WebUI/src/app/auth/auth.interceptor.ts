import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { AuthService } from "./auth.service";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService: AuthService = inject(AuthService);
  let token: string | null = authService.accessToken;
  if(token) {
    if (authService.isAccessTokenExpired()) {
      authService.refreshTokens()
        .subscribe(res => token = res.accessToken)
    }
    req = req.clone({
      setHeaders: {
        'Authorization': `Bearer ${token}`
      }
    });
  }

  return next(req)
};
