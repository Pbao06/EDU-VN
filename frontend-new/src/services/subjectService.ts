import apiClient from '../lib/apiClient';

export const subjectService = {
  getSubjectDetail: async (subjectId: number): Promise<any> => {
    return await apiClient.get<any>(`/api/subject/${subjectId}`);
    
  },
};

