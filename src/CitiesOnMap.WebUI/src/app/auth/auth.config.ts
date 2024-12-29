import { AuthConfig } from 'angular-oauth2-oidc';

export const customAuthConfig: AuthConfig = {
  issuer: "https://localhost:40443/",
  redirectUri: "http://localhost:4200/",
  tokenEndpoint: "https://localhost:40443/api/connect/token",
  clientId: "angular-app",
  responseType: "password",
  scope: "profile",
  silentRefreshRedirectUri: "http://localhost:4200/silent-refresh.html",
  oidc: false,
  disablePKCE: false,
  showDebugInformation: true,
}
export const googleAuthConfig: AuthConfig = {
  redirectUri: "http://localhost:4200/callback",
  clientId: "angular-app",
  loginUrl: "https://localhost:40443/api/connect/authorize",
  tokenEndpoint: "https://localhost:40443/api/connect/token",
  responseType: "code",
  scope: "https://www.googleapis.com/auth/userinfo.email",
  silentRefreshRedirectUri: "http://localhost:4200/silent-refresh.html",
  oidc: true,
  disablePKCE: false,
  showDebugInformation: true,
  skipIssuerCheck: true,
  strictDiscoveryDocumentValidation: false
}
