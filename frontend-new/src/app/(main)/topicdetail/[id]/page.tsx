
"use client";

import { useEffect, useState } from "react";
import { TopicPractice, PracticeQuestion } from "@/components/shared/TopicPractice";
import { useTopic } from "@/hooks/learning/useTopic";

// Define a type for the expected API response structure for better type safety
interface ApiTopicData {
  name: string;
  learningQuestions: {
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
  const { getTopicDetail, loading } = useTopic();
  const [topicData, setTopicData] = useState<{ name: string; questions: PracticeQuestion[] } | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const topicId = parseInt(params.id, 10);
    if (isNaN(topicId)) {
      setError("ID chủ đề không hợp lệ.");
      return;
    }

    const fetchAndSetTopicData = async () => {
      try {
        setError(null);
        const data: ApiTopicData = await getTopicDetail(topicId);

        // Map the API data to the structure required by the TopicPractice component
        const mappedQuestions: PracticeQuestion[] = data.learningQuestions.map(q => {
          const correctOption = q.answers.find(a => a.isCorrect);
          if (!correctOption) {
            // Throw an error if a question has no correct answer, which is a data integrity issue.
            throw new Error(`Câu hỏi ID ${q.id} không có đáp án đúng.`);
          }
          return {
            id: q.id.toString(),
            prompt: q.content,
            explanation: q.explanation,
            options: q.answers.map(a => ({
              id: a.id.toString(),
              label: a.content,
            })),
            correctOptionId: correctOption.id.toString(),
          };
        });

        setTopicData({
          name: data.name,
          questions: mappedQuestions,
        });

      } catch (err: any) {
        console.error("Failed to fetch topic data:", err);
        setError(err.message || "Không thể tải dữ liệu chủ đề. Vui lòng thử lại.");
      }
    };

    fetchAndSetTopicData();
  }, [params.id]); // Dependency array ensures this runs when the id changes

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
      <TopicPractice topicName={topicData.name} questions={topicData.questions} />
    </div>
  );
};

export default TopicDetailPage;

