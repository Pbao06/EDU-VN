export interface AdminTopicDto {
  id: number;
  subjectId: number;
  subjectName: string;
  name: string;
  description: string;
  difficultyLevel: number;
}

export interface CreateTopicDto {
  subjectId: number;
  name: string;
  description: string;
  difficultyLevel: number;
}

export interface UpdateTopicDto {
  subjectId: number;
  name: string;
  description: string;
  difficultyLevel: number;
}
