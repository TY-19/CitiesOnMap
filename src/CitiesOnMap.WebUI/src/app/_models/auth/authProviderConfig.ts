export interface AuthProviderConfig {
  authUrl: string;
  clientId: string;
  redirectUri: string;
  scope: string;
  statePrefix: string;
}
