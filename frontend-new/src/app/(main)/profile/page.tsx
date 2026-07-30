"use client";
import Profile from "@/components/shared/ProfileComponent";
import { useProfile } from "@/hooks/learning/useProfile";
import { useEffect, useState } from "react";
import { learningPathService, toLearningPathView } from "@/services/learningPathService";
import { useLearningPaths } from "@/hooks/learning/useLearningPaths";
import type { LearningPathView } from "@/types/Learning/learning-path";
import { useRouter } from "next/navigation";
import { EditProfileModal, EditProfileFormData, FieldOption } from "@/components/shared/EditUserProfile";
import type { editprofile } from "@/types/Profile/userProfile";

// Danh sách map giữa ID và Tên Field
const FIELD_OPTIONS: FieldOption[] = [
  { id: 1, name: "Software Engineering" },
  { id: 2, name: "Marketing" },
  { id: 3, name: "Design" },
  { id: 4, name: "Business" },
];

// 1. ĐƯA MOCK DATA LÊN ĐÂY ĐỂ KHÔNG BỊ LỖI "CHƯA KHAI BÁO"
const mockLearningPath = [
  { id: "1", name: "Backend Developer", progress: 72, subject: "ASP.NET Core", action: "Continue" as const },
  { id: "2", name: "Frontend Developer", progress: 18, subject: "JavaScript", action: "Continue" as const },
  { id: "3", name: "AI Engineer", progress: 0, subject: "—", action: "Start" as const },
];

const mockAchievement = {
  completedTopics: 35,
  completedSubjects: 8,
  totalLearningPaths: 3,
};

export default function ProfilePage() {
  const { getInfoUser, editInfoUser, loading } = useProfile();
  const [userData, setUserData] = useState<any>(null);
  const { paths, refetch, loading: pathLoading } = useLearningPaths(); 
  const [learningPathData, setLearningPathData] = useState<LearningPathView[] | null>(null);
  const router = useRouter();
  
  // 2. SỬA CHÍNH TẢ THÀNH Modal
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const res = await getInfoUser();
        console.log("APi dc goi la : ", res);
        setUserData(res);
      } catch (error) {
        console.error(" error khi tai thong tin: ", error);
      }
    };
    fetchProfile();
  }, [getInfoUser]); 
  
  useEffect(() => {
    refetch(); 
  }, [refetch]);

  useEffect(() => {
    if (paths.length > 0) {
      setLearningPathData(paths.map(toLearningPathView));
      console.log("API learningPath gọi dc là : ", paths); 
    }
  }, [paths]);

  const handleContinuePath = (id: number) => {
    router.push(`/learningpath/${id}`);
  };

  const handleSaveProfile = async (formData: EditProfileFormData) => {
    try {
      const payload: editprofile = {
        fullName: formData.fullName,
        useType: formData.useType,
        mainGoal: formData.mainGoal,
        fieldId: formData.fieldId,
        updateAt: new Date().toISOString()
      };
      console.log(" Gui API ve server la : ", payload);
      
      await editInfoUser(payload);
      
      setUserData((prev: any) => ({
        ...prev,
        ...payload,
        fieldName: FIELD_OPTIONS.find(f => f.id === payload.fieldId)?.name || prev.fieldName
      }));
    } catch (error) {
      console.error("Lỗi khi lưu profile", error);
    }
  };

  if (pathLoading || !userData || !learningPathData) {
    return <div>Loading...</div>;
  }

  // 3. BỌC BẰNG THẺ <> ... </>
  return (
    <>
      <Profile
        personalInfo={userData}              
        learningPath={learningPathData}      
        achievement={mockAchievement}        
        onEditProfile={() => {
          console.log("Đã bật state lên true!");
          setIsEditModalOpen(true);
        }}
        onChangePassword={() => console.log("change password clicked")}
        onContinuePath={handleContinuePath}
      />
      <EditProfileModal
        isOpen={isEditModalOpen} 
        onClose={() => setIsEditModalOpen(false)}
        onSave={handleSaveProfile}
        fieldOptions={FIELD_OPTIONS}
        // 4. TRUYỀN DỮ LIỆU THẬT THAY VÌ { ... }
        initialData={{
          fullName: userData.fullName || "",
          useType: userData.useType || "Learner",
          mainGoal: userData.mainGoal || "",
          fieldId: userData.fieldId || 1,
        }}
      />
    </>
  );
}