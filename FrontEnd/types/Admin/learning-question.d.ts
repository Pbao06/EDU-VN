export interface AdminLearningQuestionDto {
  id: number;
  content: string;
  explanation?: string | null;
  hint?: string | null;
  difficulty: number;
  topicId: number;
  topicName: string;
}

export interface CreateLearningQuestionDto {
  content: string;
  explanation?: string | null;
  hint?: string | null;
  difficulty: number;
  topicId: number;
}

export interface UpdateLearningQuestionDto {
  content: string;
  explanation?: string | null;
  hint?: string | null;
  difficulty: number;
  topicId: number;
}

export interface AdminLearningAnswerDto {
  id: number;
  content: string;
  isCorrect: boolean;
  explanation?: string | null;
  learningQuestionId: number;
  questionContent: string;
}

export interface CreateLearningAnswerDto {
  content: string;
  isCorrect: boolean;
  explanation?: string | null;
  learningQuestionId: number;
}

export interface UpdateLearningAnswerDto {
  content: string;
  isCorrect: boolean;
  explanation?: string | null;
  learningQuestionId: number;
}
