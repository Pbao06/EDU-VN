import apiClient from '../lib/apiClient';
import { AuthResponseDto } from '../types/Auth/auth-response';
import { LoginDto, RegisterDto } from '../types/Auth/register';
import { RefreshTokenDto } from '../types/Auth/refresh-token';

export const authService = {
  register: async (data: RegisterDto): Promise<AuthResponseDto> => {

    return (await apiClient.post<AuthResponseDto>('/api/auth/register', data)); // data là dữ liệu phía client gửi cho server 
    // post<authResponseDto> là gói dữ liệu xem dto trả về cái gì 
  },

  login: async (data: LoginDto): Promise<AuthResponseDto> => {
    const res= await apiClient.post<AuthResponseDto>('/api/auth/login', data);
    console.log("BACKEND LOGIN RESPONSE:", res);
    return res;
    
  },

  refreshToken: async (data: RefreshTokenDto): Promise<AuthResponseDto> => {
    return (await apiClient.post<AuthResponseDto>('/api/auth/refresh-token', data));
  },
};
