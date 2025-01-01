export interface TokensModel {
  userName: string | null;
  accessToken: string | null;
  accessTokenExpiration?: Date;
  refreshToken: string | null;
  refreshTokenExpiration?: Date;
}
