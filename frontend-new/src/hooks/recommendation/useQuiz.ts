import { useCallback, useState } from 'react';
import { quizService } from '../../services/quizService';

export const useQuiz = () => {
  const [loading, setLoading] = useState(false);

  const getUserQuiz = useCallback(async () => {
    setLoading(true);
    try {
      return await quizService.getUserQuiz();
    } finally {
      setLoading(false);
    }
  }, []);

  const submitQuiz = useCallback(async (data: any) => {
    setLoading(true);
    try {
      return await quizService.submitQuiz(data);
    } finally {
      setLoading(false);
    }
  }, []);

  const getQuizResult = useCallback(async (id: number) => {
    setLoading(true);
    try {
      return await quizService.getQuizResult(id);
    } finally {
      setLoading(false);
    }
  }, []);

  return { getUserQuiz, submitQuiz, getQuizResult, loading };
};
