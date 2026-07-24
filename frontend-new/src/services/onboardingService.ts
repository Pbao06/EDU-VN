import apiClient from '../lib/apiClient';
import { OnBoardingDto, OnboardingStatusDto } from '@/types/Auth/onboarding';
export const onboardingService = {
  completeOnboarding: async (data: any): Promise<OnBoardingDto> => {
    return await apiClient.post<any>('/api/onboarding/complete', data);
    
  },
  getStatus: async (): Promise<OnboardingStatusDto> => {
    return await apiClient.get<any>('/api/onboarding/status');
  },
};
