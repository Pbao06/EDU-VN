"use client";

import React, { useState, useMemo } from "react";
import {
  Server,
  Code,
  Megaphone,
  Palette,
  Database,
  Heart,
  GraduationCap,
  Settings,
  TrendingUp,
  Brush,
} from "lucide-react";

import Navbar from "@/components/shared/Navbar";
import Hero from "@/components/shared/Hero";
import CareerFilter from "@/components/shared/CareerFilter";
import CareerGrid from "@/components/shared/CareerGrid";
import Footer from "@/components/shared/Footer";

// Mock careers data matching the styling and domain of EDU VN
const MOCK_CAREERS = [
  {
    id: "1",
    name: "Lập trình viên Backend",
    description: "Xây dựng hệ thống máy chủ, cơ sở dữ liệu và các API mạnh mẽ để hỗ trợ vận hành ứng dụng mượt mà.",
    salary: "25 - 45 triệu",
    salaryMin: 25,
    icon: Server,
    demand: "high" as const,
    accent: "blue" as const,
    difficulty: "Thử thách cao" as const,
    category: "Công nghệ thông tin",
  },
  {
    id: "2",
    name: "Lập trình viên Frontend",
    description: "Tạo nên giao diện người dùng tương tác, trực quan và tối ưu hóa trải nghiệm lướt web, di động.",
    salary: "20 - 35 triệu",
    salaryMin: 20,
    icon: Code,
    demand: "high" as const,
    accent: "yellow" as const,
    difficulty: "Trung bình" as const,
    category: "Công nghệ thông tin",
  },
  {
    id: "3",
    name: "Chuyên viên Marketing số",
    description: "Lên kế hoạch, quản lý chiến dịch quảng cáo mạng xã hội, tối ưu SEO để kết nối khách hàng với doanh nghiệp.",
    salary: "15 - 30 triệu",
    salaryMin: 15,
    icon: Megaphone,
    demand: "high" as const,
    accent: "orange" as const,
    difficulty: "Trung bình" as const,
    category: "Kinh doanh & Marketing",
  },
  {
    id: "4",
    name: "Nhà thiết kế UI/UX",
    description: "Nghiên cứu nhu cầu khách hàng, phác thảo trải nghiệm người dùng và vẽ nên giao diện ứng dụng số hiện đại.",
    salary: "18 - 32 triệu",
    salaryMin: 18,
    icon: Palette,
    demand: "medium" as const,
    accent: "pink" as const,
    difficulty: "Trung bình" as const,
    category: "Thiết kế & Sáng tạo",
  },
  {
    id: "5",
    name: "Chuyên viên Phân tích Dữ liệu",
    description: "Khai phá dữ liệu lớn, vẽ các dashboard trực quan để cung cấp các góc nhìn phân tích hữu ích cho doanh nghiệp.",
    salary: "22 - 40 triệu",
    salaryMin: 22,
    icon: Database,
    demand: "high" as const,
    accent: "green" as const,
    difficulty: "Thử thách cao" as const,
    category: "Công nghệ thông tin",
  },
  {
    id: "6",
    name: "Bác sĩ Đa khoa",
    description: "Khám bệnh, chẩn đoán triệu chứng, kê đơn và đồng hành chăm sóc sức khỏe lâu dài cho người bệnh.",
    salary: "30 - 60 triệu",
    salaryMin: 30,
    icon: Heart,
    demand: "high" as const,
    accent: "blue" as const,
    difficulty: "Thử thách cao" as const,
    category: "Y tế & Sức khỏe",
  },
  {
    id: "7",
    name: "Giáo viên Tiếng Anh",
    description: "Truyền đạt kiến thức ngôn ngữ toàn diện, thiết kế bài giảng hấp dẫn và nâng bước hội nhập quốc tế.",
    salary: "12 - 25 triệu",
    salaryMin: 12,
    icon: GraduationCap,
    demand: "medium" as const,
    accent: "yellow" as const,
    difficulty: "Trung bình" as const,
    category: "Giáo dục",
  },
  {
    id: "8",
    name: "Kỹ sư Cơ khí",
    description: "Thiết kế, chế tạo, kiểm thử lắp ráp và tối ưu quy trình vận hành các máy móc, động cơ công nghiệp.",
    salary: "15 - 28 triệu",
    salaryMin: 15,
    icon: Settings,
    demand: "medium" as const,
    accent: "green" as const,
    difficulty: "Thử thách cao" as const,
    category: "Kỹ thuật",
  },
  {
    id: "9",
    name: "Cố vấn Tài chính",
    description: "Xây dựng chiến lược phân bổ nguồn vốn, quản trị rủi ro đầu tư và hoạch định kế hoạch hưu trí cho khách hàng.",
    salary: "20 - 50 triệu",
    salaryMin: 20,
    icon: TrendingUp,
    demand: "medium" as const,
    accent: "orange" as const,
    difficulty: "Thử thách cao" as const,
    category: "Kinh doanh & Marketing",
  },
  {
    id: "10",
    name: "Nhà sáng tạo Nội dung",
    description: "Sản xuất video ngắn, thiết kế hình ảnh độc đáo và viết kịch bản quảng bá trên TikTok, Youtube, Facebook.",
    salary: "8 - 18 triệu",
    salaryMin: 8,
    icon: Brush,
    demand: "high" as const,
    accent: "pink" as const,
    difficulty: "Dễ tiếp cận" as const,
    category: "Thiết kế & Sáng tạo",
  },
];

export default function Home() {
  // State for filtering
  const [search, setSearch] = useState("");
  const [field, setField] = useState("Tất cả lĩnh vực");
  const [salary, setSalary] = useState("Mọi mức lương");
  const [difficulty, setDifficulty] = useState("Mọi độ khó");
  const [sort, setSort] = useState("Phổ biến nhất");

  // Filtering & Sorting memoized logic
  const filteredCareers = useMemo(() => {
    return MOCK_CAREERS.filter((career) => {
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

      // 3. Difficulty Filter
      if (difficulty !== "Mọi độ khó" && career.difficulty !== difficulty) {
        return false;
      }

      // 4. Salary Filter
      if (salary !== "Mọi mức lương") {
        const val = career.salaryMin;
        if (salary === "Dưới 10 triệu" && val >= 10) return false;
        if (salary === "10 - 20 triệu" && (val < 10 || val > 20)) return false;
        if (salary === "20 - 35 triệu" && (val < 20 || val > 35)) return false;
        if (salary === "Trên 35 triệu" && val <= 35) return false;
      }

      return true;
    }).sort((a, b) => {
      // 5. Sorting Options
      if (sort === "Lương cao nhất") {
        return b.salaryMin - a.salaryMin;
      }
      if (sort === "A - Z") {
        return a.name.localeCompare(b.name, "vi");
      }
      // "Phổ biến nhất" & "Mới cập nhật": Default order
      return 0;
    });
  }, [search, field, salary, difficulty, sort]);

  // Handle click on a career card
  const handleCareerClick = (career: any) => {
    alert(`Bạn đã chọn xem chi tiết ngành: ${career.name}\nMức lương: ${career.salary}\nLĩnh vực: ${career.category}`);
  };

  return (
    <div className="flex min-h-screen w-full flex-col bg-[#F3F4F6] text-black">
      {/* 1. Navbar */}
      <Navbar logoHref="#" quizHref="#quiz" />

      {/* 2. Hero Section */}
      <Hero quizHref="#quiz" />

      {/* 3. Main Content: Search, Filter, and Career Grid */}
      <main id="careers" className="mx-auto w-full max-w-6xl px-4 py-16 sm:px-6">
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
        <div className="mt-6">
          <CareerGrid
            careers={filteredCareers}
            title={`Kết quả tìm kiếm`}
            subtitle={`Hiển thị ${filteredCareers.length} ngành nghề phù hợp với tiêu chí của bạn`}
            showSearch={false} // Disable inner search to use unified CareerFilter
            showFilters={false} // Disable inner filter tags to use unified CareerFilter
            onCareerClick={handleCareerClick}
          />
        </div>
      </main>

      {/* 6. Footer */}
      <Footer quizHref="#quiz" />
    </div>
  );
}
