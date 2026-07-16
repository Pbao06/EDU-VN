import apiClient from '../lib/apiClient';
import { 
  LearningPathDto, 
  LearningPathDetailDto, 
  CreateLearningPathResponseDto 
} from '../types/Learning/learning-path';

export const learningPathService = {
  startLearningPath: async (careerId: number, title?: string): Promise<CreateLearningPathResponseDto> => {
    return (await apiClient.post<CreateLearningPathResponseDto>(`/api/learning-path/${careerId}/start`, { title }));
    
  },
  getUserLearningPaths: async (): Promise<LearningPathDto[]> => {
    return (await apiClient.get<LearningPathDto[]>('/api/learning-path/user'))!;
    
  },

  getLearningPathDetail: async (learningPathId: number): Promise<LearningPathDetailDto> => {
    return (await apiClient.get<LearningPathDetailDto>(`/api/learning-path/${learningPathId}`));
    
  },
  // Các phương thức dưới đây thuộc về SubjectController hoặc TopicController
  // Đã xóa khỏi learningPathService để tránh gọi sai endpoint
};

