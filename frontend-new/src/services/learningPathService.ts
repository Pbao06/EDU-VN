import apiClient from '../lib/apiClient';
import { 
  LearningPathDto, 
  LearningPathDetailDto, 
  CreateLearningPathResponseDto 
} from '../types/Learning/learning-path';

export const learningPathService = {
  startLearningPath: async (careerId: number, title?: string): Promise<CreateLearningPathResponseDto> => {
    return (await apiClient.post<CreateLearningPathResponseDto>(`/api/learningpath/${careerId}/start`, { title }));
    
  },
  getUserLearningPaths: async (): Promise<LearningPathDto[]> => {
    return (await apiClient.get<LearningPathDto[]>('/api/learningpath/user'))!;
    
  },

  getLearningPathDetail: async (learningPathId: number): Promise<LearningPathDetailDto> => {
    const response=await apiClient.get<LearningPathDetailDto>(`/api/learningpath/${learningPathId}`);
    return response;
    
  },
};

