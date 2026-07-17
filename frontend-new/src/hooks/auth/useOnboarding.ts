import { useState } from 'react';
import { onboardingService } from '../../services/onboardingService';

export const useOnboarding = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const completeOnboarding = async (data: any) => {
    setLoading(true);
    setError(null);
    try {
      return await onboardingService.completeOnboarding(data);
    } catch (err: any) {
      setError(err.message || 'Failed to complete onboarding');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const getStatus = async () => {
    setLoading(true);
    try {
      return await onboardingService.getStatus();
    } finally {
      setLoading(false);
    }
  };

  return { completeOnboarding, getStatus, loading, error };
};
