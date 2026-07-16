export interface AdminCareerSubjectDto {
  careerId: number;
  careerName: string;
  subjectId: number;
  subjectName: string;
  priority: number;
  reason: string;
}

export interface CreateCareerSubjectDto {
  careerId: number;
  subjectId: number;
  priority: number;
  reason: string;
}

export interface UpdateCareerSubjectDto {
  careerId: number;
  subjectId: number;
  priority: number;
  reason: string;
}
