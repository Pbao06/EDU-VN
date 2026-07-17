export interface RegisterDto {
  email: string;
  password: string;
  fullName: string;
  confirmPassword: string;
}

export interface LoginDto {
  email: string;
  password: string;
}
