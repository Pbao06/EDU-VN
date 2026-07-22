"use client";
import React, { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { useSubject } from "@/hooks/learning/useSubject";
import SubjectDetail, { 
  SubjectDetailProps, 
  SubjectTopic,
  SubjectDifficulty,
  TopicStatus 
} from "@/components/shared/SubjectDetail";

export default function SubjectDetailPage() {
  console.log("SubjectDetailPage đã được load!"); 
  const params = useParams();
  const router = useRouter();
  const { getSubjectDetail, loading } = useSubject();
  const [subjectData, setSubjectData] = useState<SubjectDetailProps | null>(null);
  const [error, setError] = useState<string | null>(null);

  // Lấy subjectId từ URL params
  const rawSubjectId = params?.id;
  const subjectId = rawSubjectId ? parseInt(Array.isArray(rawSubjectId) ? rawSubjectId[0] : rawSubjectId, 10) : 0;
  


  useEffect(() => {
    const fetchSubjectDetail = async () => {
      if (!subjectId) {
        setError("Invalid subject ID");
        return;
      }
      
      try {
        const data = await getSubjectDetail(subjectId);
        // debug 
        console.log("API trả về lấy chi tiết môn là : ", data);
        
        // Map API data to SubjectDetail props
        const difficultyMap: Record<string, SubjectDifficulty> = {
          "beginner": "beginner",
          "intermediate": "intermediate",
          "advanced": "advanced",
          "Beginner": "beginner",
          "Intermediate": "intermediate",
          "Advanced": "advanced"
        };

        const statusMap: Record<string, TopicStatus> = {
          "completed": "completed",
          "in_progress": "in_progress",
          "locked": "locked",
          "available": "available",
          "Completed": "completed",
          "In Progress": "in_progress",
          "Locked": "locked",
          "Available": "available"
        };

        // Map topics array
        const mappedTopics: SubjectTopic[] = (data.topics || []).map((topic: any) => ({
          id: topic.id || topic._id || String(Math.random()),
          title: topic.title || topic.name || "Unknown Topic",
          description: topic.description,
          emoji: topic.emoji,
          questions: topic.questions,
          minutes: topic.minutes,
          status: statusMap[topic.status] || "available"
        }));

        const mappedData: SubjectDetailProps = {
          name: data.name || data.title || "Unknown Subject",
          description: data.description || "No description available",
          difficulty: difficultyMap[data.difficulty] || "beginner",
          topicsCount: data.topicsCount || mappedTopics.length,
          hours: data.hours || data.duration || 0,
          progress: data.progress || 0,
          topics: mappedTopics,
          onBack: () => router.back(),
          onContinue: () => {
            console.log("Continue learning:", data.name);
          },
          onTopicClick: (topic: SubjectTopic) => {
            console.log("Topic clicked:", topic);
          }
        };

        setSubjectData(mappedData);
      } catch (err) {
        console.error("Error fetching subject detail:", err);
        const errorMessage = err instanceof Error ? err.message : "Failed to load subject details";
        setError(errorMessage);
      }
    };

    fetchSubjectDetail();
  }, [subjectId]);

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

  if (!subjectData) {
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
    <div className="flex justify-center w-full px-4 py-8 sm:px-6 lg:px-8 min-h-screen">
      <div className="w-full max-w-5xl">
        <SubjectDetail {...subjectData} />
      </div>
    </div>
  );
}
