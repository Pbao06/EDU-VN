export interface RecommendationAnswerDto {
  id: number;
  content: string;
  recommendationQuestionId: number;
}

export interface RecommendationQuestionDto {
  id: number;
  content: string;
  quizId: number;
  answers: RecommendationAnswerDto[];
}

export interface QuizDto {
  id: number;
  code: string;
  title: string;
  description: string;
  fieldId: number;
  fieldName: string;
  durationMinutes: number;
  questions?: RecommendationQuestionDto[] | null;
}

export interface QuizSubmitRequestDto {
  answers: Record<number, number>;
}

export interface CareerResultDto {
  careerId: number;
  careerName: string;
  fieldName: string;
  description: string;
  minSalary: number;
  maxSalary: number;
  matchPercentage: number;
  explanation: string;
}

export interface QuizResultDto {
  quizId: number;
  quizTitle: string;
  careers: CareerResultDto[];
  submittedAt: string;
}
