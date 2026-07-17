export interface AdminSubjectDto {
  id: number;
  code: string;
  name: string;
  description: string;
  type: string;
}

export interface CreateSubjectDto {
  code: string;
  name: string;
  description: string;
  type: string;
}

export interface UpdateSubjectDto {
  code: string;
  name: string;
  description: string;
  type: string;
}
