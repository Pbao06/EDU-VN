export interface AdminFieldDto {
  id: number;
  code: string;
  name: string;
  description: string;
}

export interface CreateFieldDto {
  code: string;
  name: string;
  description: string;
}

export interface UpdateFieldDto {
  code: string;
  name: string;
  description: string;
}
