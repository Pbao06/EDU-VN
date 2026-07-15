export interface OnBoardingDto {
  fullName: string;
  userType: string;
  mainGoal?: string | null;
  fieldId?: number | null;
}

export interface OnboardingStatusDto {
  isCompleted: boolean;
  onboardingData?: OnBoardingDto | null;
}
