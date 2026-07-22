"use client";
import React, { useEffect, useState } from "react";
import { useParams,useRouter } from "next/navigation";
import { useCareer } from "@/hooks/recommendation/useCareer";
import CareerDetail, { 
  CareerDetailProps, 
  CareerOutlook, 
  LearningPathPreviewItem,
  DemandLevel 
} from "@/components/shared/CareerDetail";
import {useLearningPaths} from '@/hooks/learning/useLearningPaths';
import { Sparkles } from "lucide-react";

export default function CareerDetailPage() {
  const params = useParams();
  const router = useRouter();// dùng cho chuyển trang 
  const { getDetailCareer } = useCareer(); 
  const [careerData, setCareerData] = useState<CareerDetailProps | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const {startLearningPath,loading:isCreatingPath}=useLearningPaths();
// Ép kiểu ID an toàn cho Next.js App Router
  const rawId = params?.id;
  const careerId = rawId ? parseInt(Array.isArray(rawId) ? rawId[0] : rawId, 10) : 0;
  useEffect(() => {
    const fetchCareerDetail = async () => {
        if (!careerId) {
          setError("Invalid career ID");
          setLoading(false);
          return;
        }
      try {
        

        const data = await getDetailCareer(careerId);
        
        // Map API data to CareerDetail props
        const demandLevelMap: Record<string, DemandLevel> = {
          "High": "high",
          "Medium": "medium",
          "Low": "low",
          "high": "high",
          "medium": "medium",
          "low": "low"
        };

        // Parse responsibilities from string to array
        const responsibilitiesArray = data.responsibilities 
          ? data.responsibilities.split('\n').filter((r: string) => r.trim())
          : [];

        // Create career outlook based on demand level
        const outlook: CareerOutlook = {
          demandLabel: data.demandLevel === "High" ? "Very High" : 
                      data.demandLevel === "Medium" ? "Medium" : "Low",
          demandStars: data.demandLevel === "High" ? 5 : 
                       data.demandLevel === "Medium" ? 3 : 1,
          growthLabel: data.demandLevel === "High" ? "+30%" : "+10%",
          environmentLabel: "Startup, Big Tech"
        };

        // Create learning path preview (mock data for now)
        const learningPathPreview: LearningPathPreviewItem[] = [
          { emoji: "📚", title: "Programming Basics" },
          { emoji: "💻", title: "Project Practice" },
          { emoji: "🚀", title: "Advanced Skills" },
          { emoji: "🎯", title: "Real-world Application" }
        ];

        const mappedData: CareerDetailProps = {
          name: data.name,
          category: data.category || "Information Technology",
          difficulty: data.difficulty || (data.demandLevel === "High" ? 4 : 3),
          averageSalaryLabel: `${data.minSalary} - ${data.maxSalary} million/month`,
          shortDescription: data.description?.substring(0, 150) + "..." || "No description available",
          tags: data.tags && data.tags.length > 0 ? data.tags : ["IT", "Technology", "Development"],
          iconUrl: data.iconUrl || undefined,
          overview: data.description || "No detailed description available",
          minSalary: data.minSalary * 1000000,
          maxSalary: data.maxSalary * 1000000,
          currency: "VND",
          salaryUnit: "month",
          responsibilities: responsibilitiesArray.length > 0 ? responsibilitiesArray : ["No specific responsibilities listed"],
          requiredSkills: data.requiredSkills && data.requiredSkills.length > 0 ? data.requiredSkills : ["Programming", "Analysis", "Problem Solving"],
          relatedSubjects: data.relatedSubjects && data.relatedSubjects.length > 0 ? data.relatedSubjects : ["Math", "Physics", "English"],
          outlook: outlook,
          demandLevel: demandLevelMap[data.demandLevel] || "medium",
          learningPathId: data.id,
          learningPathPreview: learningPathPreview,
          // primaryLabel: "Start Learning Now",
          accent: "blue",
          // onPrimaryAction: () => {
            // alert("Start learning: " + data.name);
            // 👉 BỔ SUNG THÊM 2 DÒNG NÀY VÀO ĐÂY ĐỂ TRUYỀN XUỐNG COMPONENT CON:
          primaryLabel: isCreatingPath ? "Đang khởi tạo..." : "Start Learning Now",
          onPrimaryAction: handleStartLearning,
          
        };

        setCareerData(mappedData);
      } catch (err) {
        console.error("Error fetching career detail:", err);
        const errorMessage = err instanceof Error ? err.message : "Failed to load career details";
        setError(errorMessage);
      } finally {
        setLoading(false);
      }
    };

    fetchCareerDetail();
  }, [params.id]);
  // nghiệp vụ chuẩn viết riêng hàm xử lý bấm nút
  const handleStartLearning= async() => {
    // get id Career 
     const careerId = params.id ? parseInt(params.id as string) : 0;
     // --- DEBUG LOG ---
     console.log("DEBUG: Preparing to start learning path. Full params:", params, "Parsed careerId:", careerId);
     if(!careerId) return "Error khong the lay id user";
     try{
      //call API đăng kí lộ trình 
      const res= await startLearningPath(careerId);
      // debug xem lỗi 
      console.log("API TRẢ VỀ TOÀN BỘ LÀ:", JSON.stringify(res, null, 2));
      // Lấy ID trả về từ API (backend giờ trả về camelCase)
      const newPathId = res?.learningPathId;

      if(newPathId)
      {
        // chuyển sang trang lộ trình học cho user coi 
        router.push(`/learningpath/${newPathId}`);
      }else
      {
        alert("Có lỗi xảy ra , không lấy được Id learningPath");
      }
     }
     catch(error)
     {
      console.error("Error when start learning :",error);
     }
  };

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <div className="mb-4 text-2xl font-bold">Loading...</div>
          <div className="text-gray-500">Please wait a moment</div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <div className="mb-4 text-2xl font-bold text-red-500">Error</div>
          <div className="text-gray-500">{error}</div>
        </div>
      </div>
    );
  }

  if (!careerData) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <div className="mb-4 text-2xl font-bold">Data not found</div>
          <div className="text-gray-500">Please try again later</div>
        </div>
      </div>
    );
  }
  return (
    <div className="flex justify-center w-full px-4 py-8 sm:px-6 lg:px-8 bg-grey-100 min-h-screen">
      <div className="w-full max-w-5xl">
        <CareerDetail {...careerData} onPrimaryAction={handleStartLearning} primaryLabel={isCreatingPath? "Đang khởi tạo...": "Start Learning Now"} />
      </div>
    </div>
  );
}
