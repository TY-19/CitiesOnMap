import {Injectable} from "@angular/core";
import {TokensModel} from "../_models/auth/tokensModel";

@Injectable({
  providedIn: 'root',
})

export class LocalStorageService {
  private readonly userNameKey: string = 'userName';
  private readonly accessTokenKey: string = 'access_token';
  private readonly accessTokenExpirationKey: string = 'access_token_expiration';
  private readonly refreshTokenKey: string = 'refresh_token';
  private readonly refreshTokenExpirationKey: string = 'refresh_token_expiration';
  private readonly codeVerifierKey: string = 'pkce_code_verifier';
  private readonly stateKey: string = 'oauth_request_state';

  get userName(): string | null {
    return localStorage.getItem(this.userNameKey);
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
  get codeVerifier(): string | null {
    return localStorage.getItem(this.codeVerifierKey);
  }

  storeTokens(tokens: TokensModel): void {
    if(tokens.userName) {
      localStorage.setItem(this.userNameKey, tokens.userName);
    }
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
  removeTokensFormStorage() {
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.accessTokenExpirationKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.refreshTokenExpirationKey);
    localStorage.removeItem(this.stateKey);
    localStorage.removeItem(this.codeVerifierKey);
  }
  storeCodeRequestParameters(codeVerifier: string, state: string): void {
    localStorage.setItem(this.codeVerifierKey, codeVerifier);
    localStorage.setItem(this.stateKey, state);
  }
  clearCodeRequestParameters(): void {
    localStorage.removeItem(this.codeVerifierKey);
    localStorage.removeItem(this.stateKey);
  }
}
