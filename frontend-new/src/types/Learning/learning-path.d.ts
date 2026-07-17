export interface LearningPathDto {
  id: number;
  userId: string;
  careerId: number;
  careerName: string;
  careerIconUrl: string;
  title: string;
  isActive: boolean;
  createdAt: string;
  completedAt?: string | null;
  totalSubjects: number;
  completedSubjects: number;
  overallProgress: number;
}

export interface SubjectSummaryDto {
  id: number;
  code: string;
  name: string;
  description: string;
  type: string;
  priority: number;
  reason: string;
  totalTopics: number;
  completedTopics: number;
  subjectProgress: number;
  isCompleted: boolean;
  isInProgress: boolean;
}

export interface LearningPathDetailDto {
  id: number;
  userId: string;
  careerId: number;
  careerName: string;
  careerIconUrl: string;
  title: string;
  isActive: boolean;
  createdAt: string;
  completedAt?: string | null;
  totalSubjects: number;
  completedSubjects: number;
  overallProgress: number;
  subjects: SubjectSummaryDto[];
}

export interface TopicSummaryDto {
  id: number;
  name: string;
  description: string;
  difficultyLevel: number;
  totalQuestions: number;
  completedQuestions: number;
  topicProgress: number;
  isCompleted: boolean;
  isInProgress: boolean;
  lastAccessedAt?: string | null;
}

export interface SubjectDetailDto {
  id: number;
  code: string;
  name: string;
  description: string;
  type: string;
  priority: number;
  reason: string;
  totalTopics: number;
  completedTopics: number;
  subjectProgress: number;
  isCompleted: boolean;
  isInProgress: boolean;
  topics: TopicSummaryDto[];
}

export interface LearningAnswerDto {
  id: number;
  content: string;
  isCorrect: boolean;
  explanation: string;
  learningQuestionId: number;
}

export interface LearningQuestionDto {
  id: number;
  content: string;
  explanation: string;
  hint: string;
  difficulty: number;
  topicId: number;
  answers: LearningAnswerDto[];
  userAnswerId?: number | null;
  isUserCorrect?: boolean | null;
}

export interface TopicDetailDto {
  id: number;
  name: string;
  description: string;
  difficultyLevel: number;
  subjectId: number;
  subjectName: string;
  totalQuestions: number;
  completedQuestions: number;
  topicProgress: number;
  isCompleted: boolean;
  isInProgress: boolean;
  lastAccessedAt?: string | null;
  questions: LearningQuestionDto[];
}

export interface CreateLearningPathDto {
  careerId: number;
  title?: string | null;
}

export interface CreateLearningPathResponseDto {
  learningPathId: number;
  message: string;
  learningPath: LearningPathDto;
}

export interface SubmitTopicAnswersDto {
  topicId: number;
  learningPathId: number;
  answers: Record<number, number>;
}

export interface SubmitTopicAnswersResponseDto {
  success: boolean;
  message: string;
  totalQuestions: number;
  correctAnswers: number;
  score: number;
  isTopicCompleted: boolean;
  topicProgress: TopicSummaryDto;
  subjectProgress?: SubjectSummaryDto | null;
}
