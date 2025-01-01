import { AuthProviderConfig } from "../_models/auth/authProviderConfig";

export const googleConfig: AuthProviderConfig = {
  authUrl: 'https://accounts.google.com/o/oauth2/v2/auth',
  clientId: '334636357841-d5kh1tm8jncngj4jugk31li3ef6rv6es.apps.googleusercontent.com',
  redirectUri: 'http://localhost:4200/callback',
  scope: 'https://www.googleapis.com/auth/userinfo.email',
  statePrefix: "google:",
}
