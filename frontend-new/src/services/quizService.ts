import apiClient from '../lib/apiClient';
import { CareerResultDto, QuizResultDto, QuizSubmitRequestDto, QuizDto } from '@/types/Recommendation/quiz'

export const quizService = {
  getUserQuiz: async (): Promise<QuizDto> => {
    return await apiClient.get<QuizDto>('/api/quiz');
  },

  submitQuiz: async (data: QuizSubmitRequestDto): Promise<QuizResultDto> => {
    return await apiClient.post<QuizResultDto>('/api/quiz/submit', data);
  },

  getQuizResult: async (id: number): Promise<QuizResultDto> => {
    return await apiClient.get<QuizResultDto>(`/api/quiz/results/${id}`);
  },
};

