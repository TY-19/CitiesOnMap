export interface TokensModel {
  accessToken: string | null;
  accessTokenExpiration?: Date;
  refreshToken: string | null;
  refreshTokenExpiration?: Date;
}
