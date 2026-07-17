export interface AdminCareerDto {
  id: number;
  code: string;
  name: string;
  fieldId: number;
  fieldName: string;
  description: string;
  responsibilities: string;
  minSalary: number;
  maxSalary: number;
  demandLevel: string;
  iconUrl: string;
  popularityScore: number;
}

export interface CreateCareerDto {
  code: string;
  name: string;
  fieldId: number;
  description: string;
  responsibilities: string;
  minSalary: number;
  maxSalary: number;
  demandLevel: string;
  iconUrl: string;
  popularityScore: number;
}

export interface UpdateCareerDto {
  code: string;
  name: string;
  fieldId: number;
  description: string;
  responsibilities: string;
  minSalary: number;
  maxSalary: number;
  demandLevel: string;
  iconUrl: string;
  popularityScore: number;
}
