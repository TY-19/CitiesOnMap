import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import {AuthConfig, OAuthService } from "angular-oauth2-oidc";
import {customAuthConfig, googleAuthConfig} from "./auth.config";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private activeProvider: 'google' | 'custom' | null = null;
  private options?: AuthConfig;
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService
  ) {

  }

  configureProvider(provider: string): void {
    switch (provider) {
      case 'google':
        if(this.activeProvider !== 'google') {
          this.activeProvider = 'google';
          this.options = googleAuthConfig;
        }
        break;
      case 'custom':
        if(this.activeProvider !== 'custom') {
          this.activeProvider = 'custom';
          this.options = customAuthConfig;
        }
        break;
      default:
        if(this.activeProvider !== 'custom') {
          this.activeProvider = 'custom';
          this.options = customAuthConfig;
        }
        break;
    }
    this.oauthService.configure(this.options!);
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }
  login(userName: string, password: string) {
    this.configureProvider('custom');
    this.oauthService
      .fetchTokenUsingPasswordFlow(userName, password)
      .then((res) => {
        console.log("Successfully get access token");
      })
  }
  loginOauth(provider: string) {
    this.configureProvider(provider);
    this.oauthService.initCodeFlow();
  }
  logout() {
    this.oauthService.logOut();
  }

  get isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }
}
