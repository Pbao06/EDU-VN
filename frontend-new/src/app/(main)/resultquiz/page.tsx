"use client";

import RecommendationQuiz from '@/components/shared/QuizResult'
import { useEffect, useState } from 'react';

const ResultQuizPage = () => {
  const [results, setResults] = useState<any[] | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    try {
      const raw = sessionStorage.getItem('lastQuizResult');
      if (raw) {
        const parsed = JSON.parse(raw);
        const mapped = (parsed.careers || []).map((c: any) => ({
          id: String(c.careerId),
          name: c.careerName,
          score: Math.round(c.matchPercentage),
          description: c.description,
        }));
        setResults(mapped);
      } else {
        setResults([]);
      }
    } catch (e) {
      console.error('Failed to parse quiz result from sessionStorage', e);
      setResults([]);
    } finally {
      setLoading(false);
    }
  }, []);

  const handleHome = () => {
    window.location.href = '/home';
  };

  const handleSelect = (item: any) => {
    window.location.href = `/careerdetail/${item.id}`;
  };

  if (loading) return <div className="p-8 text-center">Đang tải kết quả…</div>;

  if (!results || results.length === 0) {
    return <div className="p-8 text-center">Không có kết quả nào để hiển thị.</div>;
  }

  return (
    <RecommendationQuiz
      results={results}
      onHome={handleHome}
      onSelect={handleSelect}
    />
  );
};

export default ResultQuizPage;
