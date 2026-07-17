export interface AdminQuizDto {
  id: number;
  code: string;
  title: string;
  description?: string | null;
  fieldId?: number | null;
  fieldName?: string | null;
  durationMinutes: number;
}

export interface CreateQuizDto {
  code: string;
  title: string;
  description?: string | null;
  fieldId?: number | null;
  durationMinutes: number;
}

export interface UpdateQuizDto {
  code: string;
  title: string;
  description?: string | null;
  fieldId?: number | null;
  durationMinutes: number;
}

export interface AdminRecommendationQuestionDto {
  id: number;
  content: string;
  quizId: number;
  quizTitle: string;
}

export interface CreateRecommendationQuestionDto {
  content: string;
  quizId: number;
}

export interface UpdateRecommendationQuestionDto {
  content: string;
  quizId: number;
}

export interface AdminRecommendationAnswerDto {
  id: number;
  content: string;
  recommendationQuestionId: number;
  questionContent: string;
}

export interface CreateRecommendationAnswerDto {
  content: string;
  recommendationQuestionId: number;
}

export interface UpdateRecommendationAnswerDto {
  content: string;
  recommendationQuestionId: number;
}
