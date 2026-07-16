import { useState } from 'react';
import { subjectService } from '../../services/subjectService';

export const useSubject = () => {
  const [loading, setLoading] = useState(false);

  const getSubjectDetail = async (learningPathId: number, subjectId: number) => {
    setLoading(true);
    try {
      return await subjectService.getSubjectDetail(learningPathId, subjectId);
    } finally {
      setLoading(false);
    }
  };

  return { getSubjectDetail, loading };
};
