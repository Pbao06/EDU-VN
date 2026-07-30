'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import CareerQuiz, { type QuizQuestion } from '@/components/shared/Quiz';
import { useQuiz } from '@/hooks/recommendation/useQuiz';


const transformApiQuestions = (quiz: any): QuizQuestion[] => {
  return (
    quiz?.questions?.map((question: any) => ({
      id: String(question.id),
      question: question.content || question.question || 'Câu hỏi',
      hint: question.hint,
      choices: (question.answers || []).map((answer: any, index: number) => ({
        id: String(answer.id),
        label: answer.content || answer.label || `Đáp án ${index + 1}`,
      })),
    })) || []
  );
};

const transformAnswers = (answers: Record<string, string>) => {
  return Object.fromEntries(
    Object.entries(answers).map(([questionId, answerId]) => [Number(questionId), Number(answerId)]),
  );
};

const CareerQuizPage = () => {
  const [questions, setQuestions] = useState<QuizQuestion[]>([]);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { getUserQuiz, submitQuiz, loading } = useQuiz();
  const router = useRouter();

  useEffect(() => {
    const loadQuiz = async () => {
      try {
        const quiz = await getUserQuiz(); // gọi api từ hook để nạp data lên 
        setQuestions(transformApiQuestions(quiz));
      } catch (err) {
        console.error('Lỗi khi load quiz:', err);
        //catch error backend 
        // Tạm thời ép cứng: cứ load quiz lỗi là tống cổ về onboarding cho lẹ =)))
        router.push('/onboarding');
        //const errorMessage= err?.message || JSON.stringify(err);
        setError('Không thể tải quiz. Vui lòng thử lại.');
      }
    };

    loadQuiz();
  }, [getUserQuiz]);

  const handleFinish = async (answers: Record<string, string>) => { // hàm submit form gửi data về 
    setSubmitting(true);
    setError(null);
    try {
      const payload = { answers: transformAnswers(answers) };
      const result = await submitQuiz(payload); // gọi hook submit nè 
      console.log('Quiz submit result:', result);
      // Save result to sessionStorage so result page can read it
      try {
        sessionStorage.setItem('lastQuizResult', JSON.stringify(result));
      } catch (e) {
        console.warn('Could not save quiz result to sessionStorage', e);
      }
      // Redirect to result page
      router.push('/resultquiz');
    } catch (err) {
      console.error('Lỗi khi submit quiz:', err);
      setError('Gửi đáp án thất bại. Vui lòng thử lại.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading && questions.length === 0) {
    return (
      <div className="mx-auto mt-20 max-w-xl rounded-3xl border border-black/10 bg-white p-8 text-center shadow-[8px_8px_0_0_rgba(0,0,0,0.12)]">
        <p className="text-lg font-bold text-black">Đang tải quiz...</p>
        <p className="mt-2 text-sm text-black/60">Vui lòng đợi trong giây lát.</p>
      </div>
    );
  }

  return (
    <>
      <CareerQuiz
        questions={questions}
        onFinish={handleFinish}
        submitting={submitting}
      />
      {error && (
        <div className="mt-6 rounded-xl border border-red-500 bg-red-50 p-4 text-sm font-medium text-red-700">
          {error}
        </div>
      )}
    </>
  );
};

export default CareerQuizPage;

