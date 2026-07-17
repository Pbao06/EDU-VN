export interface AdminAnswerCareerWeightDto {
  id: number;
  recommendationAnswerId: number;
  answerContent: string;
  careerId: number;
  careerName: string;
  weight: number;
}

export interface CreateAnswerCareerWeightDto {
  recommendationAnswerId: number;
  careerId: number;
  weight: number;
}

export interface UpdateAnswerCareerWeightDto {
  recommendationAnswerId: number;
  careerId: number;
  weight: number;
}
