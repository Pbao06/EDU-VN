import apiClient from '../lib/apiClient';

export const topicService = {
  getTopicDetail: async (topicId: number): Promise<any> => {
    return await apiClient.get<any>(`/api/topic/${topicId}`);
    
  },
  submitTopic: async (data: any): Promise<any> => {
    return await apiClient.post<any>('/api/topic/submit', data);
    
  },
};
