import apiClient from '../lib/apiClient';

export const subjectService = {
  getSubjectDetail: async (learningPathId: number, subjectId: number): Promise<any> => {
    return await apiClient.get<any>(`/api/subject/${learningPathId}/subject/${subjectId}`);
    
  },
};

