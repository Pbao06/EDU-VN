import { useState } from 'react';
import { topicService } from '../../services/topicService';

export const useTopic = () => {
  const [loading, setLoading] = useState(false);

  const getTopicDetail = async (topicId: number) => {
    setLoading(true);
    try {
      return await topicService.getTopicDetail(topicId);
    } finally {
      setLoading(false);
    }
  };

  const submitTopic = async (data: any) => {
    setLoading(true);
    try {
      return await topicService.submitTopic(data);
    } finally {
      setLoading(false);
    }
  };

  return { getTopicDetail, submitTopic, loading };
};
