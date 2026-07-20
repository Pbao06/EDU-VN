import { useState, useEffect } from 'react';
import { learningPathService } from '../../services/learningPathService';
import { LearningPathDto,LearningPathDetailDto } from '../../types/Learning/learning-path';

export const useLearningPaths = () => {
  const [paths, setPaths] = useState<LearningPathDto[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchPaths = async () => {
    setLoading(true);
    try {
      const data = await learningPathService.getUserLearningPaths();
      setPaths(data);
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    fetchPaths();
  }, []);
  
  const getDetail= async (id:number):Promise<LearningPathDetailDto>=>{
    setLoading(true);
    try{
        return await learningPathService.getLearningPathDetail(id);
    }
  finally {
    setLoading(false);
  };
}


  return { paths, loading,getDetail, refetch: fetchPaths };
};
