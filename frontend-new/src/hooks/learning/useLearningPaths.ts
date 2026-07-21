import { useState, useEffect,useCallback } from 'react';
import { learningPathService } from '../../services/learningPathService';
import { LearningPathDto,LearningPathDetailDto,CreateLearningPathDto ,CreateLearningPathResponseDto} from '../../types/Learning/learning-path';

export const useLearningPaths = () => {
  const [paths, setPaths] = useState<LearningPathDto[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchPaths =useCallback (async () => {
    setLoading(true);
    try {
      const data = await learningPathService.getUserLearningPaths();
      setPaths(data);
    } finally {
      setLoading(false);
    }
  },[]);
  
  const getDetail= useCallback(async (id:number):Promise<LearningPathDetailDto>=>{
    setLoading(true);
    try{
        return await learningPathService.getLearningPathDetail(id);
    }
  finally {
    setLoading(false);
  };
  },[]);


  const startLearningPath=useCallback(async (careerId:number, title?:string):Promise<CreateLearningPathResponseDto>=>{
    setLoading(true);
    try{
      return await learningPathService.startLearningPath(careerId,title);
    }
    finally{
      setLoading(false);
    }
  },[]);
  return { paths,startLearningPath,loading,getDetail, refetch: fetchPaths };
};
