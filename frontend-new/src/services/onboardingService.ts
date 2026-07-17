import apiClient from '../lib/apiClient';

export const onboardingService = {
  completeOnboarding: async (data: any): Promise<any> => {
    return await apiClient.post<any>('/api/onboarding/complete', data);
    
  },
  getStatus: async (): Promise<any> => {
    return await apiClient.get<any>('/api/onboarding/status');
    
  },
};
