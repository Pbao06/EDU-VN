import apiClient from '../lib/apiClient';

export const quizService = {
  getAvailableQuizzes: async (fieldId: number): Promise<any[]> => {
    return (await apiClient.get<any[]>(`/api/quiz/available?fieldId=${fieldId}`));
    
  },
  getQuizQuestions: async (id: number): Promise<any> => {
    return (await apiClient.get<any>(`/api/quiz/${id}/questions`));
    
  },
  submitQuiz: async (id: number, data: any): Promise<any> => {
    return (await apiClient.post<any>(`/api/quiz/${id}/submit`, data));
    
  },
  getQuizResult: async (id: number): Promise<any> => {
    return await apiClient.get<any>(`/api/quiz/results/${id}`);
  },
};

