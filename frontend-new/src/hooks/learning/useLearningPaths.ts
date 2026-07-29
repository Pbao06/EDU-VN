import { useState, useEffect,useCallback } from 'react';
import { learningPathService } from '../../services/learningPathService';
import { LearningPathDto,LearningPathDetailDto,CreateLearningPathDto ,CreateLearningPathResponseDto} from '../../types/Learning/learning-path';

export const useLearningPaths = () => {
  const [paths, setPaths] = useState<LearningPathDto[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchPaths =useCallback (async () => { // usecallBack là đảm bảo cho không tự gọi lại hàm( vì useEffect thg này nó auto gọi API để load trang)
    setLoading(true);
    try {
      const data = await learningPathService.getUserLearningPaths();// gọi API lấy data 
      setPaths(data); // bỏ đồ vào hộp đựng data 
    } finally {
      setLoading(false);
    }
  },[]); // có thay đổi gì thì nó phụ thuộc vào [] này nè , nhma nó rỗng nên sẽ ko thay đổi 
  
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
