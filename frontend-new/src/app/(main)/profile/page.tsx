"use client";
import Profile from "@/components/shared/ProfileComponent";
import { useProfile } from "@/hooks/learning/useProfile";
import { useEffect, useState } from "react";
import {learningPathService,toLearningPathView} from "@/services/learningPathService";
import {useLearningPaths} from "@/hooks/learning/useLearningPaths";
import type { LearningPathView } from "@/types/Learning/learning-path";
import path from "path";
export default function ProfilePage()
{
   const { getInfoUser, loading } = useProfile();
   const[userData,setUserData]=useState<any>(null); // lưu lại dữ liệu
    const { paths, refetch, loading: pathLoading } = useLearningPaths(); // gọi khai báo các hàm cần dùng ra 
    const [learningPathData, setLearningPathData] = useState<LearningPathView[] | null>(null);
      useEffect(()=>{
        const fetchProfile= async ()=>{
            try{
                const res = await getInfoUser();
                console.log("APi dc goi la : ",res);
                setUserData(res);
            }
            catch(error){
                console.error(" error khi tai thong tin: ",error);
            }
        };
        fetchProfile();
   },[getInfoUser]); 
  
   // goi API learningPath - only trigger fetch khong nhan ket qua truc tiep 
   useEffect(()=>{
    refetch(); // refetch là hàm gọi APi đc set nickname == refetch bên hook 
   },[refetch]);
   // map lai moi khi paths trong hook thay doi sau khi refetch xong 
   useEffect(()=>{
    if(paths.length>0)
    {
      setLearningPathData(paths.map(toLearningPathView));
      console.log("API learningPath gọi dc là : ",paths); // vì refetch sẽ lấy data mới bỏ vào paths
    }
   },[paths]);


    // ⏳ chờ API xong mới render, tránh personalInfo bị null lúc đầu
   if ( pathLoading || !userData || !learningPathData) {
    return <div>Loading...</div>;
  }

    return (
    <Profile
      personalInfo={userData}              // ✅ data thật từ API
      learningPath={learningPathData}       // 🔧 mock tạm, test personalInfo trước
      achievement={mockAchievement}         // 🔧 mock tạm
      onEditProfile={() => console.log("edit profile clicked")}
      onChangePassword={() => console.log("change password clicked")}
      onContinuePath={(id) => console.log("continue path:", id)}
    />);

}
// Mock data tạm để test UI phần chưa nối API
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