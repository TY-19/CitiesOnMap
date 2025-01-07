import {HttpClient, HttpResponseBase} from "@angular/common/http";
import { Injectable } from "@angular/core";
import pkceChallenge from "pkce-challenge";
import {BehaviorSubject, catchError, finalize, Observable, tap, throwError } from "rxjs";
import {googleConfig} from "./auth.config";
import {LocalStorageService} from "../_common/localStorage.service";
import { baseUrl } from "../app.config";
import { TokensModel } from "../_models/auth/tokensModel";
import {AuthProviderConfig} from "../_models/auth/authProviderConfig";
import {RegistrationRequest} from "../_models/auth/registrationRequest";
import {CodeExchangeModel} from "../_models/auth/codeExchangeModel";
import {RefreshTokenModel} from "../_models/auth/refreshTokenModel";
import {LoginRequest} from "../_models/auth/loginRequest";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly markAsExpiredForMilliseconds: number = 30000;
  private refreshLock: boolean = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  private refreshQueue: Array<any> = [];

  get isAuthorized(): boolean {
    return this.localStorageService.accessToken !== null;
  }
  get accessToken(): string | null {
    return this.localStorageService.accessToken;
  }
  get userName(): string | null {
    return  this.localStorageService.userName;
  }

  constructor(private http: HttpClient,
              private localStorageService: LocalStorageService
  ) {
  }

  registerUser(userName: string, email: string, password: string): Observable<TokensModel> {
    const url: string = baseUrl + "/account/register";
    const body: RegistrationRequest = {
      userName: userName,
      email: email,
      password: password
    };
    return this.http.post<TokensModel>(url, body);
  }

  login(userNameOrEmail: string, password: string): Observable<TokensModel> {
    const url: string = baseUrl + "/account/login";
    const body: LoginRequest = {
      userName: null,
      email: null,
      password: password
    };
    if(userNameOrEmail.includes("@")) {
      body.email = userNameOrEmail;
    } else {
      body.userName = userNameOrEmail;
    }
    return this.http.post<TokensModel>(url, body)
      .pipe(tap(tokens => this.localStorageService.storeTokens(tokens)));
  }

  startExternalAuthorization(provider: string): void {
    const providerConfig: AuthProviderConfig | null = this.selectProviderConfig(provider);
    if(providerConfig == null) {
      console.log(`Provider ${provider} is not configured.`);
      return;
    }
    pkceChallenge().then(({ code_verifier, code_challenge }) => {
      const state: string = providerConfig.statePrefix + Math.random() + Date.now().toString();
      this.localStorageService.storeCodeRequestParameters(code_verifier, state);
      window.location.href = this.buildAuthUrl(providerConfig, code_challenge, state)
    });
  }
  private selectProviderConfig(provider: string): AuthProviderConfig | null {
    switch (provider) {
      case "Google":
        return googleConfig;
      default:
        return null;
    }
  }
  private buildAuthUrl(providerConfig: AuthProviderConfig, codeChallenge: string, state: string) {
    const authUrl = new URL(providerConfig.authUrl);
    authUrl.searchParams.set('client_id', providerConfig.clientId);
    authUrl.searchParams.set('redirect_uri', providerConfig.redirectUri);
    authUrl.searchParams.set('scope', providerConfig.scope);
    authUrl.searchParams.set('state', state);
    authUrl.searchParams.set('response_type', "code");
    authUrl.searchParams.set('code_challenge', codeChallenge);
    authUrl.searchParams.set('code_challenge_method', "S256");
    return authUrl.toString();
  }
  verifyState(state?: string): boolean {
    const stateVerifier = this.localStorageService.oauthRequestState;
    return stateVerifier !== null && state !== undefined && stateVerifier === state;
  }

  loginExternal(provider: string, code: string): Observable<TokensModel> {
    const url = baseUrl + "/account/login/" + provider;
    const body: CodeExchangeModel = {
      code: code,
      codeVerifier: this.localStorageService.codeVerifier ?? ""
    };
    return this.http.post<TokensModel>(url, body)
      .pipe(tap(tokens => {
        this.localStorageService.storeTokens(tokens);
        this.localStorageService.clearCodeRequestParameters();
      }));
  }

  isAccessTokenExpired(): boolean {
    return this.isExpired(this.localStorageService.accessTokenExpiration);
  }
  isRefreshTokenExpired(): boolean {
    return this.isExpired(this.localStorageService.refreshTokenExpiration);
  }
  private isExpired(expiration: Date | null): boolean {
    const expirationTime: number | undefined = expiration?.getTime();
    return !expirationTime || new Date().getTime() > expirationTime - this.markAsExpiredForMilliseconds;
  }

  refreshTokens(): Observable<TokensModel> {
    if(this.refreshLock) {
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

      const url = baseUrl + "/account/refresh"
      const body: RefreshTokenModel = {
        userName: this.localStorageService.userName ?? "",
        refreshToken: this.localStorageService.refreshToken ?? ""
      };
      return this.http.post<TokensModel>(url, body)
        .pipe(
          tap(res => {
            this.localStorageService.storeTokens(res);
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
    const url = baseUrl + "/account/profile";
    this.http.get(url)
      .subscribe(res => console.log(res));
  }
  logOut(): Observable<HttpResponseBase> {
    const url: string = baseUrl + "/account/revoke";
    const body: RefreshTokenModel = {
      userName: this.localStorageService.userName ?? "",
      refreshToken: this.localStorageService.refreshToken ?? ""
    };

    return this.http.post(url, body, { observe: "response" })
      .pipe(tap(() => {
        this.localStorageService.clearStorage();
      }));
  }
}
