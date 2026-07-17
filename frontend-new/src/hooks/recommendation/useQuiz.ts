import { useState } from 'react';
import { quizService } from '../../services/quizService';

export const useQuiz = () => {
  const [loading, setLoading] = useState(false);

  const getAvailableQuizzes = async (fieldId: number) => {
    setLoading(true);
    try {
      return await quizService.getAvailableQuizzes(fieldId);
    } finally {
      setLoading(false);
    }
  };

  const getQuizQuestions = async (id: number) => {
    setLoading(true);
    try {
      return await quizService.getQuizQuestions(id);
    } finally {
      setLoading(false);
    }
  };

  const submitQuiz = async (id: number, data: any) => {
    setLoading(true);
    try {
      return await quizService.submitQuiz(id, data);
    } finally {
      setLoading(false);
    }
  };

  const getQuizResult = async (id: number) => {
    setLoading(true);
    try {
      return await quizService.getQuizResult(id);
    } finally {
      setLoading(false);
    }
  };

  return { getAvailableQuizzes, getQuizQuestions, submitQuiz, getQuizResult, loading };
};
