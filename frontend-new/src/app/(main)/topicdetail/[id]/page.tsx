"use client";
import { useEffect, useState } from "react";
import { TopicPractice, PracticeQuestion, AnswerMap } from "@/components/shared/TopicPractice";
import { useTopic } from "@/hooks/learning/useTopic";
import { useRouter } from "next/navigation"; 

// Define a type for the expected API response structure for better type safety
interface ApiTopicData {
  name: string;
  subjectId: number;
  questions: {
    id: number;
    content: string;
    explanation: string | null;
    answers: {
      id: number;
      content: string;
      isCorrect: boolean;
    }[];
  }[];
}

const TopicDetailPage = ({ params }: { params: { id: string } }) => {
  const { getTopicDetail,submitTopic, loading } = useTopic();
  const topicIdParam = params.id;
  const [topicData, setTopicData] = useState<{ name: string; subjectId: number; questions: PracticeQuestion[] } | null>(null);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter(); // 👈 THÊM

   // 👇 MỚI: tách riêng state cho việc submit, không dùng chung `loading` của fetch
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);


  useEffect(() => {
    if (!topicIdParam) {
        setError("Không có ID chủ đề.");
        return;
    }

    const topicId = parseInt(topicIdParam, 10);
    if (isNaN(topicId)) {
      setError("ID chủ đề không hợp lệ.");
      return;
    }

    const fetchAndSetTopicData = async () => {
      try {
        setError(null);
        const data: ApiTopicData = await getTopicDetail(topicId);
        
        console.log("DEBUG: API Topic detail response:", data);

        // Check if data exists
        if (!data) {
          throw new Error("Không nhận được dữ liệu từ máy chủ.");
        }

        // Map the API data to the structure required by the TopicPractice component
        // Use optional chaining and fallback to empty array to prevent crashes
        const mappedQuestions: PracticeQuestion[] = (data.questions || []).map(q => {
          const correctOption = (q.answers || []).find(a => a.isCorrect);
          if (!correctOption) {
            // Throw an error if a question has no correct answer, which is a data integrity issue.
            throw new Error(`Câu hỏi ID ${q.id} không có đáp án đúng.`);
          }
          return {
            id: q.id.toString(),
            prompt: q.content,
            explanation: q.explanation || "",
            options: (q.answers || []).map(a => ({
              id: a.id.toString(),
              label: a.content,
            })),
            correctOptionId: correctOption.id.toString(),
          };
        });

        setTopicData({
          name: data.name,
          subjectId: data.subjectId,
          questions: mappedQuestions,
        });

      } catch (err: any) {
        console.error("Failed to fetch topic data:", err);
        setError(err.message || "Không thể tải dữ liệu chủ đề. Vui lòng thử lại.");
      }
    };

    fetchAndSetTopicData();
  }, [topicIdParam]); // Dependency array ensures this runs when the id changes


  const handleSubmit=async (result:{
    answers:AnswerMap;
    correctCount:number;
    total:number;
  })=>{
    if(!topicData) return;
    setIsSubmitting(true);
    setSubmitError(null);

    try{
       // Convert answers: {questionId(string): optionId(string)} -> {number: number}
      const answersForApi: Record<number, number> = Object.fromEntries(
        Object.entries(result.answers).map(([questionId, optionId]) => [
          Number(questionId),
          Number(optionId),
        ])
      );

      const topicId = parseInt(topicIdParam, 10);

      const response = await submitTopic({
        topicId,
        answers: answersForApi,
      });

      console.log("DEBUG: Kết quả submit từ server:", response);

      // TODO: hiển thị kết quả chính thức (response.score, response.isTopicCompleted...)
      // Ví dụ: điều hướng qua trang kết quả hoặc show modal
       // 👇 THÊM: điều hướng về trang Subject cha sau khi submit thành công
      router.push(`/subjectdetail/${topicData.subjectId}`);

    }catch(err:any)
    {
       console.error("Submit thất bại:", err);
        setSubmitError(err.message || "Nộp bài thất bại, vui lòng thử lại.");
    }finally{
      setIsSubmitting(false);
    }


  }

  if (loading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <p className="text-lg font-semibold">Đang tải...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex h-64 items-center justify-center rounded-lg bg-red-100 p-4 text-red-700">
        <p>
          <strong>Lỗi:</strong> {error}
        </p>
      </div>
    );
  }

  if (!topicData) {
    return null; // Or some other placeholder if no data is available
  }

  return (
    <div className="p-4">
      <TopicPractice 
      topicName={topicData.name} 
      questions={topicData.questions} 
       onSubmit={handleSubmit}
        isSubmitting={isSubmitting}
        submitError={submitError}
      />
    </div>
  );
};

export default TopicDetailPage;
