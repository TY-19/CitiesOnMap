import { AuthConfig } from 'angular-oauth2-oidc';

export const customAuthConfig: AuthConfig = {
  issuer: "https://localhost:40443/",
  redirectUri: "http://localhost:4200/",
  tokenEndpoint: "https://localhost:40443/api/connect/token",
  clientId: "angular-app",
  responseType: "password",
  scope: "demo_api",
  silentRefreshRedirectUri: "http://localhost:4200/silent-refresh.html",
  oidc: false,
  disablePKCE: false,
  showDebugInformation: true,
}
export const googleAuthConfig: AuthConfig = {
  issuer: "https://localhost:40443/",
  redirectUri: "http://localhost:4200/",
  clientId: "angular-app",
  responseType: "code",
  scope: "demo_api",
  silentRefreshRedirectUri: "http://localhost:4200/silent-refresh.html",
  disablePKCE: false,
  showDebugInformation: true,
}
