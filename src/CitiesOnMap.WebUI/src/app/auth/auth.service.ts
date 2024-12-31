import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import pkceChallenge from "pkce-challenge";
import { baseUrl } from "../app.config";
import { TokensModel } from "../_models/tokensModel";
import {BehaviorSubject, catchError, finalize, Observable, tap, throwError } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private refreshLock: boolean = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  private refreshQueue: Array<any> = [];
  private readonly accessTokenKey: string = 'access_token';
  private readonly accessTokenExpirationKey: string = 'access_token_expiration';
  private readonly refreshTokenKey: string = 'refresh_token';
  private readonly refreshTokenExpirationKey: string = 'refresh_token_expiration';
  private readonly codeVerifierKey: string = 'pkce_code_verifier';
  private readonly stateKey: string = 'oauth-request-state';
  get isAuthorized(): boolean {
    return localStorage.getItem(this.accessTokenKey) !== null;
  }
  get accessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }
  get accessTokenExpiration(): Date | null {
    let dateString = localStorage.getItem(this.accessTokenExpirationKey);
    if(!dateString) {
      return null;
    }
    return new Date(dateString)
  }
  get refreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }
  get refreshTokenExpiration(): Date | null {
    let dateString = localStorage.getItem(this.refreshTokenExpirationKey);
    if(!dateString) {
      return null;
    }
    return new Date(dateString)
  }
  constructor(
    private http: HttpClient,
  ) {
  }
  registerUser(userName: string, email: string, password: string): Observable<object> {
    let url = baseUrl + "/account/register";
    let body: object = {
      userName: userName,
      email: email,
      password: password
    };
    return this.http.post(url, body);
  }
  startAuthorization(provider: string): void {
    this.startGoogleAuthorization();
  }
  loginExternal(provider: string, code: string): Observable<TokensModel> {
    let url = baseUrl + "/account/login/" + provider;
    let body: object = {
      code: code,
      codeVerifier: localStorage.getItem(this.codeVerifierKey)
    };
    return this.http.post<TokensModel>(url, body)
      .pipe(tap(tokens => this.storeTokens(tokens)));
  }
  startGoogleAuthorization(): void {
    pkceChallenge().then(({ code_verifier, code_challenge }) => {
      const googleAuthUrl = 'https://accounts.google.com/o/oauth2/v2/auth';
      const clientId = '334636357841-d5kh1tm8jncngj4jugk31li3ef6rv6es.apps.googleusercontent.com';
      const redirectUri = 'http://localhost:4200/callback';
      const scope = 'https://www.googleapis.com/auth/userinfo.email';
      const state: string = "google:" + Math.random() + Date.now().toString();
      const responseType = "code";
      const codeChallengeMethod = 'S256';

      localStorage.setItem(this.codeVerifierKey, code_verifier);
      localStorage.setItem(this.stateKey, state);

      const authUrl = new URL(googleAuthUrl);
      authUrl.searchParams.set('client_id', clientId);
      authUrl.searchParams.set('redirect_uri', redirectUri);
      authUrl.searchParams.set('scope', scope);
      authUrl.searchParams.set('state', state);
      authUrl.searchParams.set('response_type', responseType);
      authUrl.searchParams.set('code_challenge', code_challenge);
      authUrl.searchParams.set('code_challenge_method', codeChallengeMethod);

      window.location.href = authUrl.toString();
    });
  }
  storeTokens(tokens: TokensModel) {
    if(tokens.accessToken) {
      localStorage.setItem(this.accessTokenKey, tokens.accessToken);
    }
    if(tokens.accessTokenExpiration) {
      localStorage.setItem(this.accessTokenExpirationKey, tokens.accessTokenExpiration.toString());
    }
    if(tokens.refreshToken) {
      localStorage.setItem(this.refreshTokenKey, tokens.refreshToken);
    }
    if(tokens.refreshTokenExpiration) {
      localStorage.setItem(this.refreshTokenExpirationKey, tokens.refreshTokenExpiration.toString());
    }
  }
  refreshTokens(): Observable<TokensModel> {
    if(this.refreshLock === true) {
      return new Observable<TokensModel>((observer) => {
        this.refreshQueue.push(() => {
          this.refreshTokens()
            .subscribe((tokens) => {
              observer.next(tokens)
            });
        });
      });
    } else {
      this.refreshLock = true;
      this.refreshTokenSubject.next(null);

      let url = baseUrl + "/api/account/refresh"
      let body = {
        accessToken: this.accessToken,
        refreshToken: this.refreshToken
      };
      return this.http.post<TokensModel>(url, body)
        .pipe(
          tap(res => {
            this.storeTokens(res);
            this.refreshTokenSubject.next(res.accessToken);
            this.refreshQueue.forEach(request => request());
            this.refreshQueue = [];
            this.refreshLock = false;
          }),
          catchError(error => {
            console.log(error);
            this.refreshLock = false;
            return throwError(() => error);
          }),
          finalize(() => {
            this.refreshLock = false;
          })
        );
    }
  }
  getUserInfo() {
    let url = baseUrl + "/account/profile";
    this.http.get(url)
      .subscribe(res => console.log(res));
  }
  logOut(): void {
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.accessTokenExpirationKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.refreshTokenExpirationKey);
  }
}
