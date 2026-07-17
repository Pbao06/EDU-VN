export interface RefreshTokenDto {
  refreshToken: string;
}

export interface LogoutDto {
  userId: string;
  deviceInfo?: string | null;
}
