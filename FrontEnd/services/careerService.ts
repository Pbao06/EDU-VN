import apiClient from '../lib/apiClient';
import { CareerDetailDto } from '../types/Recommendation/career-detail';

// Define the interface for list career if not present in types yet
export interface ListCareer {
  id: number;
  code: string;
  name: string;
  description: string;
  iconUrl: string;
}

export const careerService = {
  getListCareer: async (fieldId: number): Promise<ListCareer[]> => {
    return  (await apiClient.get<ListCareer[]>(`/api/career/GetListCareer?fieldId=${fieldId}`));
    
  },

  getDetailCareer: async (id: number): Promise<CareerDetailDto> => {
    return (await apiClient.get<CareerDetailDto>(`/api/career/GetDetailCareer/${id}`));
  },
};

