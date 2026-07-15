export interface CareerDetailDto {
  id: number;
  name: string;
  description: string;
  responsibilities: string;
  minSalary: number;
  maxSalary: number;
  demandLevel: string;
  iconUrl?: string | null;
}

export interface ListCareerDto {
  id: number;
  name: string;
  shortDescription?: string | null;
  salary: number;
  iconUrl?: string | null;
  demandLevel?: string | null;
}
