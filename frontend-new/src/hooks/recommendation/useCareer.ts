import { useState,useCallback } from 'react';
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

  const getDetailCareer = useCallback(async (id: number) => {
    setLoading(true);
    try {
      const data = await careerService.getDetailCareer(id);
      return data;
    } 
     finally {
      setLoading(false);
    }
  }, []); // Mảng dependency rỗng nghĩa là hàm này chỉ khởi tạo một lần duy nhất, không bao giờ bị đổi địa chỉ nữa.

  const getListCareerPublic = async () => {
    setLoading(true);
    try {
      return await careerService.getListCareerPublic();
    } finally {
      setLoading(false);
    }
  };

  return { getListCareer, getDetailCareer, getListCareerPublic, loading };
};
