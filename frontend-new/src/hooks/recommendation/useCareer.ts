import { useState } from 'react';
import { careerService } from '../../services/careerService';

export const useCareer = () => {
  const [loading, setLoading] = useState(false);

  const getListCareer = async (fieldId: number) => {
    setLoading(true);
    try {
      return await careerService.getListCareer(fieldId);
    } finally {
      setLoading(false);
    }
  };

  const getDetailCareer = async (id: number) => {
    setLoading(true);
    try {
      return await careerService.getDetailCareer(id);
    } finally {
      setLoading(false);
    }
  };

  return { getListCareer, getDetailCareer, loading };
};
