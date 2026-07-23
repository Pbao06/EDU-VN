"use client";
import React, { useState, useMemo, useEffect} from "react";
import { Sparkles } from "lucide-react";
import { useRouter } from "next/navigation";
import { useCareer } from "@/hooks/recommendation/useCareer";
import Hero from "@/components/shared/Hero";
import CareerFilter from "@/components/shared/CareerFilter";
import CareerGrid from"@/components/shared/CareerGrid";
import { DemandLevel } from"@/components/shared/CareerCard";

export default function Home() {
   const router = useRouter();
   const { getListCareerPublic } = useCareer();
      const [realCareers, setRealCareers] = useState<any[]>([]);
      const [loading, setLoading] = useState(true);

      // Load data thật từ API public
      useEffect(() => {
        const fetchData = async () => {
          try {
            const data = await getListCareerPublic(); // Lấy tất cả ngành nghề (public)
            const formatted = data.map((item: any) => {
              // Convert demandLevel từ API (High/Medium/Low) sang lowercase cho frontend
              const demandLevelMap: Record<string, DemandLevel> = {
                "High": "high",
                "Medium": "medium", 
                "Low": "low",
                "high": "high",
                "medium": "medium",
                "low": "low"
              };
              
              return {
                id: item.id?.toString() || item._id?.toString() || "unknown",
                name: item.name,
                description: item.shortDescription || item.description,
                salary: item.salary ? `${item.salary} triệu` : "Liên hệ",
                icon: Sparkles, // Placeholder
                demand: demandLevelMap[item.demandLevel] || "high" as DemandLevel,
                category: "Công nghệ thông tin", // Có thể thêm field category từ API nếu có
                accent: "blue" as const
              };
            });
            setRealCareers(formatted);
          } catch (err) {
            console.error(err);
          } finally {
            setLoading(false);
          }
      };
        fetchData();
      }, []);
  // State for filtering
  const [search, setSearch] = useState("");
  const [field, setField] = useState("Tất cả lĩnh vực");
  const [salary, setSalary] = useState("Mọi mức lương");
  const [difficulty, setDifficulty] = useState("Mọi độ khó");
  const [sort, setSort] = useState("Phổ biến nhất");

  // Filtering & Sorting memoized logic
  const filteredCareers = useMemo(() => {
    return realCareers.filter((career) => {
      // 1. Text Search
      if (search.trim() !== "") {
        const query = search.toLowerCase();
        const matchName = career.name.toLowerCase().includes(query);
        const matchDesc = career.description.toLowerCase().includes(query);
        if (!matchName && !matchDesc) return false;
      }

      // 2. Category/Field Filter
      if (field !== "Tất cả lĩnh vực" && career.category !== field) {
        return false;
      }

      return true;
    });
  }, [realCareers, search, field]);

  // Handle click on a career card
  const handleCareerClick = (career: any) => {
    console.log("Career clicked:", career);
    console.log("Career ID:", career.id);
    console.log("Navigating to:", `/careerdetail/${career.id}`);
    router.push(`/careerdetail/${career.id}`);
  };

  return (
    <>
      {/* 2. Hero Section */}
      <Hero quizHref="#quiz" />

      {/* 3. Main Content: Search, Filter, and Career Grid */}
      <div id="careers" className="mx-auto w-full max-w-6xl px-4 py-16 sm:px-6  min-h-screen">
        <div className="mb-12 flex flex-col gap-8">
          {/* Header of Content Section */}
          <div className="text-center sm:text-left">
            <h2 className="text-3xl font-extrabold tracking-tight text-black sm:text-4xl">
              Danh mục ngành nghề nổi bật
            </h2>
            <p className="mt-2 text-base text-zinc-600 sm:text-lg">
              Sử dụng các bộ lọc dưới đây để tìm kiếm và khám phá ngành nghề phù hợp với bạn nhất.
            </p>
          </div>

          {/* 3. CareerFilter */}
          <CareerFilter
            onSearchChange={setSearch}
            onFieldChange={setField}
            onSalaryChange={setSalary}
            onDifficultyChange={setDifficulty}
            onSortChange={setSort}
          />
        </div>

        {/* 4 & 5. CareerGrid (renders CareerCard internally) */}
        <div className="mt-6  rounded-2xl p-6">
          <CareerGrid
            careers={filteredCareers}
            title={`Kết quả tìm kiếm`}
            subtitle={`Hiển thị ${filteredCareers.length} ngành nghề phù hợp với tiêu chí của bạn`}
            showSearch={false} // Disable inner search to use unified CareerFilter
            showFilters={false} // Disable inner filter tags to use unified CareerFilter
            onCareerClick={handleCareerClick}
          />
        </div>
      </div>
    </>
  );
}
