import { useState } from 'react';
import { subjectService } from '../../services/subjectService';

export const useSubject = () => {
  const [loading, setLoading] = useState(false);

  const getSubjectDetail = async ( subjectId: number) => {
    setLoading(true);
    try {
      return await subjectService.getSubjectDetail(subjectId);
    } finally {
      setLoading(false);
    }
  };

  return { getSubjectDetail, loading };
};
