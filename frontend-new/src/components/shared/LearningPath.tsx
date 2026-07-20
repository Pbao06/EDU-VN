"use client";

import React, { useEffect, useRef, useState, type ReactNode } from "react";
import { useLearningPaths } from "@/hooks/learning/useLearningPaths";
import { LearningPathDetailDto } from "@/types/Learning/learning-path";
import {useParams} from "next/navigation";

/* ─────────────────────────────────────────────
   DATA (DỮ LIỆU CŨ - ĐÃ COMMENT)
───────────────────────────────────────────── */
// const SUBJECTS: Subject[] = [
//   {
//     id: "csharp",
//     code: "C#",
//     fullName: "Lập trình C#",
//     topicCount: 12,
//     completedTopics: 10,
//     progress: 80,
//     status: "in-progress",
//     description:
//       "Nền tảng lập trình hướng đối tượng với C# — ngôn ngữ chính trong hệ sinh thái .NET",
//     cardBg: "#EFF6FF",
//     accentHex: "#4D7CFF",
//     iconBg: "#4D7CFF",
//     progressGradient: "linear-gradient(90deg, #1d4ed8 0%, #60a5fa 100%)",
//     icon: (
//       <svg
//         width="22"
//         height="22"
//         viewBox="0 0 24 24"
//         fill="none"
//         stroke="white"
//         strokeWidth="2.5"
//         strokeLinecap="round"
//         strokeLinejoin="round"
//       >
//         <polyline points="16 18 22 12 16 6" />
//         <polyline points="8 6 2 12 8 18" />
//       </svg>
//     ),
//   },
//   {
//     id: "oop",
//     code: "OOP",
//     fullName: "Lập trình hướng đối tượng",
//     topicCount: 10,
//     completedTopics: 2,
//     progress: 20,
//     status: "in-progress",
//     description:
//       "Các nguyên lý OOP cốt lõi: Encapsulation, Inheritance, Polymorphism, Abstraction",
//     cardBg: "#FFF7ED",
//     accentHex: "#FF8A3D",
//     iconBg: "#FF8A3D",
//     progressGradient: "linear-gradient(90deg, #c2410c 0%, #fb923c 100%)",
//     icon: (
//       <svg
//         width="22"
//         height="22"
//         viewBox="0 0 24 24"
//         fill="none"
//         stroke="white"
//         strokeWidth="2.5"
//         strokeLinecap="round"
//         strokeLinejoin="round"
//       >
//         <circle cx="12" cy="12" r="3" />
//         <path d="M12 1v4M12 19v4M4.22 4.22l2.83 2.83M16.95 16.95l2.83 2.83M1 12h4M19 12h4M4.22 19.78l2.83-2.83M16.95 7.05l2.83-2.83" />
//       </svg>
//     ),
//   },
//   {
//     id: "sql",
//     code: "SQL",
//     fullName: "Cơ sở dữ liệu SQL",
//     topicCount: 15,
//     completedTopics: 0,
//     progress: 0,
//     status: "not-started",
//     description:
//       "Thiết kế database, viết query, JOIN, index và tối ưu truy vấn SQL Server / PostgreSQL",
//     cardBg: "#FAFAFA",
//     accentHex: "#71717a",
//     iconBg: "#52525b",
//     progressGradient: "linear-gradient(90deg, #52525b 0%, #a1a1aa 100%)",
//     icon: (
//       <svg
//         width="22"
//         height="22"
//         viewBox="0 0 24 24"
//         fill="none"
//         stroke="white"
//         strokeWidth="2.5"
//         strokeLinecap="round"
//         strokeLinejoin="round"
//       >
//         <ellipse cx="12" cy="5" rx="9" ry="3" />
//         <path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3" />
//         <path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5" />
//       </svg>
//     ),
//   },
// ];

/* ─────────────────────────────────────────────
   HELPERS
───────────────────────────────────────────── */
function StarRating({ rating, max = 5 }: { rating: number; max?: number }) {
  return (
    <span className="flex items-center gap-0.5">
      {Array.from({ length: max }, (_, i) => (
        <svg
          key={i}
          width="14"
          height="14"
          viewBox="0 0 24 24"
          fill={i < rating ? "#FFD84D" : "transparent"}
          stroke="#000"
          strokeWidth="2.5"
        >
          <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2" />
        </svg>
      ))}
    </span>
  );
}

/* ─────────────────────────────────────────────
   SECTION WRAPPER  (matches CareerDetail Section)
───────────────────────────────────────────── */
function Section({
  icon,
  title,
  children,
}: {
  icon: ReactNode;
  title: string;
  children: ReactNode;
}) {
  return (
    <section>
      <div className="mb-4 flex items-center gap-3">
        <span className="flex h-8 w-8 items-center justify-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[3px_3px_0_0_#000]">
          {icon}
        </span>
        <h2 className="text-lg font-extrabold uppercase tracking-wider text-black sm:text-xl">
          {title}
        </h2>
      </div>
      {children}
    </section>
  );
}

/* ─────────────────────────────────────────────
   ANIMATED PROGRESS BAR
───────────────────────────────────────────── */
function AnimatedProgressBar({
  progress,
  gradient,
  delay = 200,
  height = "h-5",
}: {
  progress: number;
  gradient: string;
  delay?: number;
  height?: string;
}) {
  const [width, setWidth] = useState(0);
  const [triggered, setTriggered] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting && !triggered) {
            setTriggered(true);
            const t = setTimeout(() => setWidth(progress), delay);
            return () => clearTimeout(t);
          }
        });
      },
      { threshold: 0.3 }
    );
    if (ref.current) observer.observe(ref.current);
    return () => observer.disconnect();
  }, [triggered, progress, delay]);

  return (
    <div
      ref={ref}
      className={`relative ${height} w-full overflow-hidden rounded-full border-[2.5px] border-black bg-[#E4E4E7]`}
      role="progressbar"
      aria-valuenow={progress}
      aria-valuemin={0}
      aria-valuemax={100}
    >
      {progress > 0 && (
        <div
          className="h-full rounded-full border-r-[2.5px] border-black relative overflow-hidden"
          style={{
            width: `${width}%`,
            background: gradient,
            transition: "width 1.3s cubic-bezier(0.4,0,0.2,1)",
          }}
        >
          <div className="absolute top-0 left-0 right-0 h-1/2 rounded-t-full bg-white opacity-25" />
        </div>
      )}
    </div>
  );
}

/* ─────────────────────────────────────────────
   SUBJECT CARD
───────────────────────────────────────────── */
function SubjectCard({ subject }: { subject: any }) {
  const [hovered, setHovered] = useState(false);

  const statusMap = {
    "not-started": { label: "Chưa bắt đầu", bg: "#F4F4F5", text: "#52525b", dot: "#A1A1AA" },
    "in-progress": { label: "Đang học", bg: "#DCFCE7", text: "#16a34a", dot: "#22C55E" },
    completed: { label: "Hoàn thành", bg: "#DBEAFE", text: "#1d4ed8", dot: "#3B82F6" },
  } as const;
  const st = statusMap[subject.status as keyof typeof statusMap];

  return (
    <div
      className="relative flex flex-col overflow-hidden rounded-[28px] border-[2.5px] border-black cursor-pointer"
      style={{
        backgroundColor: subject.cardBg,
        boxShadow: hovered ? "8px 8px 0 0 #000" : "5px 5px 0 0 #000",
        transform: hovered ? "translate(-2px,-2px)" : "translate(0,0)",
        transition: "transform 0.15s ease, box-shadow 0.15s ease",
      }}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      {/* top stripe */}
      <div
        className="h-1.5 w-full"
        style={{ background: subject.progressGradient }}
      />

      <div className="flex flex-col flex-1 p-5 sm:p-6 gap-4">
        {/* Header */}
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-center gap-3">
            <div
              className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl border-[2.5px] border-black shadow-[3px_3px_0_0_#000]"
              style={{ backgroundColor: subject.iconBg }}
            >
              {subject.icon}
            </div>
            <div>
              <h3 className="text-2xl font-extrabold leading-none tracking-tight text-black">
                {subject.code}
              </h3>
              <p className="mt-0.5 text-xs font-semibold text-black/60 leading-tight">
                {subject.fullName}
              </p>
            </div>
          </div>
          {/* status badge */}
          <span
            className="inline-flex shrink-0 items-center gap-1.5 rounded-full border-[2px] border-black px-2.5 py-1 text-[11px] font-extrabold shadow-[2px_2px_0_0_#000]"
            style={{ backgroundColor: st.bg, color: st.text }}
          >
            <span
              className="h-1.5 w-1.5 rounded-full"
              style={{ backgroundColor: st.dot }}
            />
            {st.label}
          </span>
        </div>

        {/* Description */}
        <p className="text-sm leading-relaxed text-black/70 line-clamp-2">
          {subject.description}
        </p>

        {/* Topic count badges */}
        <div className="flex flex-wrap gap-2">
          <span className="inline-flex items-center gap-1.5 rounded-full border-[2px] border-black bg-white px-3 py-1 text-xs font-bold text-black shadow-[2px_2px_0_0_#000]">
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
              <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" />
              <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z" />
            </svg>
            {subject.topicCount} chủ đề
          </span>
          {subject.progress > 0 && (
            <span className="inline-flex items-center gap-1.5 rounded-full border-[2px] border-black bg-[#F0FDF4] px-3 py-1 text-xs font-bold text-[#16a34a] shadow-[2px_2px_0_0_#000]">
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="20 6 9 17 4 12" />
              </svg>
              {subject.completedTopics}/{subject.topicCount} xong
            </span>
          )}
        </div>

        {/* Progress */}
        <div>
          <div className="mb-2 flex items-center justify-between">
            <span className="text-xs font-semibold text-black/60">Tiến độ</span>
            <span
              className="text-sm font-extrabold"
              style={{ color: subject.progress === 0 ? "#A1A1AA" : subject.accentHex }}
            >
              {subject.progress}%
            </span>
          </div>
          <AnimatedProgressBar
            progress={subject.progress}
            gradient={subject.progressGradient}
            delay={300}
            height="h-4"
          />
        </div>

        {/* CTA */}
        <button
          type="button"
          className="mt-auto inline-flex w-full items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black py-3 text-sm font-extrabold uppercase tracking-wide shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000]"
          style={{
            backgroundColor: subject.progress === 0 ? "#F4F4F5" : subject.accentHex,
            color: subject.progress === 0 ? "#52525b" : "white",
          }}
        >
          {subject.progress === 0 ? (
            <>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                <polygon points="5 3 19 12 5 21 5 3" />
              </svg>
              Bắt đầu học
            </>
          ) : (
            <>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="11" cy="11" r="8" />
                <line x1="21" y1="21" x2="16.65" y2="16.65" />
              </svg>
              Xem chi tiết
            </>
          )}
        </button>
      </div>
    </div>
  );
}

/* ─────────────────────────────────────────────
   PAGE
───────────────────────────────────────────── */
export default function LearningPathPage() {

  const params= useParams(); // get Id ừ url
  const id=params.id ? Number(params.id) : null;
  // call hook here 
  const {paths,loading,getDetail}=useLearningPaths();
  // tạo state để lưu dữ liệu cần render ra 
  const [data,setData]= useState<any>(null);
  // logic để lấy 
  useEffect(()=>{
    if(id)
    {
      // gọi getDetail bằng Id thay vì url 
      getDetail(id).then((res)=> setData(res));
    }
  },[id]);
  // xử lý loading nếu đang tải thì loading 
  if(loading || !data) return <div className='p-20 text-center'>Đang tải lộ trình...</div>
  return (
    <div className="min-h-screen bg-white">
      {/* Subtle dot pattern background */}
      <div
        className="pointer-events-none fixed inset-0 opacity-[0.04]"
        style={{
          backgroundImage: "radial-gradient(circle, #000 1.5px, transparent 1.5px)",
          backgroundSize: "20px 20px",
        }}
      />

      <main className="relative z-10 mx-auto max-w-5xl space-y-8 px-4 py-10 sm:px-6 sm:py-14 lg:px-8">

        {/* ── BREADCRUMB ── */}
        <nav aria-label="Breadcrumb">
          <ol className="flex flex-wrap items-center gap-2 text-sm font-semibold text-black/50">
            <li>
              <a href="/" className="hover:text-black transition-colors">Trang chủ</a>
            </li>
            <li>/</li>
            <li>
              <span>Lộ trình học</span>
            </li>
            <li>/</li>
            <li>
              <span className="font-extrabold text-black">Backend Developer</span>
            </li>
          </ol>
        </nav>

        {/* ══════════════════════════════════════
            1. HERO
        ══════════════════════════════════════ */}
        <section className="relative overflow-hidden rounded-[32px] border-[2.5px] border-black bg-white p-6 shadow-[10px_10px_0_0_#000] sm:p-9">
          {/* decorative circle */}
          <span
            aria-hidden
            className="absolute -right-10 -top-10 h-40 w-40 rounded-full border-[2.5px] border-black bg-[#EFF6FF]"
          />
          {/* sparkle badge */}
          <span
            aria-hidden
            className="absolute right-6 top-6 hidden h-10 w-10 rotate-12 items-center justify-center rounded-2xl border-[2.5px] border-black bg-[#FFD84D] shadow-[4px_4px_0_0_#000] sm:flex"
          >
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#000" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <path d="M12 2l2.4 7.4H22l-6.2 4.5 2.4 7.4L12 17l-6.2 4.3 2.4-7.4L2 9.4h7.6L12 2z" />
            </svg>
          </span>

          <div className="relative flex flex-col gap-6 sm:flex-row sm:items-start sm:gap-7">
            {/* Icon */}
            <div className="relative shrink-0">
              <span
                aria-hidden
                className="absolute -left-2 -top-2 h-full w-full rounded-[28px] border-[2.5px] border-black bg-[#DCE6FF]"
              />
              <div
                className="relative flex h-24 w-24 items-center justify-center overflow-hidden rounded-[28px] border-[2.5px] border-black bg-[#4D7CFF] shadow-[6px_6px_0_0_#000] sm:h-28 sm:w-28"
              >
                <svg width="44" height="44" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M20 14.66V20a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h5.34" />
                  <polygon points="18 2 22 6 12 16 8 16 8 12 18 2" />
                </svg>
              </div>
            </div>

            {/* Title block */}
            <div className="flex min-w-0 flex-1 flex-col gap-3">
              <div className="flex flex-wrap items-center gap-2">
                <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1 text-[11px] font-extrabold uppercase tracking-widest text-black shadow-[3px_3px_0_0_#000]">
                  ✦ Lộ trình học
                </span>
                <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-[#DCE6FF] px-3 py-1 text-[11px] font-extrabold uppercase tracking-widest text-black shadow-[3px_3px_0_0_#000]">
                  Công nghệ
                </span>
              </div>

              <h1 className="text-3xl font-extrabold leading-tight tracking-tight text-black sm:text-5xl">
                {data.title}
              </h1>

              {/* meta row */}
              <div className="flex flex-wrap items-center gap-2">
                <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1.5 shadow-[3px_3px_0_0_#000]">
                  <span className="text-[11px] font-extrabold uppercase tracking-wider text-black/70">
                    Độ khó
                  </span>
                  <StarRating rating={4} />
                </span>
                <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-[#FFF6C9] px-3 py-1.5 shadow-[3px_3px_0_0_#000]">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#000" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                    <circle cx="12" cy="12" r="10" />
                    <polyline points="12 6 12 12 16 14" />
                  </svg>
                  <span className="text-xs font-extrabold uppercase tracking-wider text-black">
                    4 tháng
                  </span>
                </span>
                <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-[#F0FDF4] px-3 py-1.5 shadow-[3px_3px_0_0_#000]">
                  <span className="text-xs font-extrabold uppercase tracking-wider text-[#16a34a]">
                    {data.totalSubjects} chủ đề
                  </span>
                </span>
              </div>

              <p className="text-sm leading-relaxed text-black/70 sm:text-base">
                {data.title}
              </p>

              {/* tags */}
              <ul className="mt-1 flex flex-wrap gap-2">
                {["C#", ".NET", "OOP", "SQL", "Backend"].map((t) => (
                  <li
                    key={t}
                    className="rounded-full border-[2px] border-black bg-white px-2.5 py-1 text-xs font-bold text-black shadow-[2px_2px_0_0_#000]"
                  >
                    #{t}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </section>

        {/* ══════════════════════════════════════
            2. TIẾN ĐỘ TỔNG THỂ
        ══════════════════════════════════════ */}
        <Section
          icon={
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#000" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <polyline points="22 12 18 12 15 21 9 3 6 12 2 12" />
            </svg>
          }
          title="Tiến Độ Tổng Thể"
        >
          <div className="rounded-[28px] border-[2.5px] border-black bg-white p-5 shadow-[8px_8px_0_0_#000] sm:p-7">
            {/* header */}
            <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="text-sm font-semibold text-black/60">{data.title} · Đang học</p>
                <p className="mt-1 text-base font-extrabold text-black">
                  Hoàn thành {data.completedSubjects} / {data.totalSubjects} môn
                </p>
              </div>
              <div className="inline-flex items-baseline gap-1 self-start rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] px-5 py-3 shadow-[4px_4px_0_0_#000]">
                <span className="text-4xl font-extrabold leading-none text-white">{data.overallProgress}</span>
                <span className="text-xl font-bold text-white/80">%</span>
              </div>
            </div>

            {/* progress bar */}
            <AnimatedProgressBar
              progress={data.overallProgress}
              gradient="linear-gradient(90deg, #4D7CFF 0%, #60a5fa 100%)"
              delay={200}
              height="h-7"
            />

            {/* stats */}
            <div className="mt-6 grid grid-cols-2 gap-3 sm:grid-cols-4">
              {[
                { value: "0/3", label: "Môn hoàn thành", bg: "#EFF6FF", color: "#4D7CFF" },
                { value: `${data.completedSubjects}/${data.totalSubjects}`, label: "Môn học", bg: "#F0FDF4", color: "#16a34a" },
                { value: "~1.8 tháng", label: "Đã học", bg: "#FFF7ED", color: "#FF8A3D" },
                { value: "~2.2 tháng", label: "Còn lại", bg: "#FFFBEB", color: "#d97706" },
              ].map((s, i) => (
                <div
                  key={i}
                  className="rounded-2xl border-[2.5px] border-black p-3.5 shadow-[3px_3px_0_0_#000]"
                  style={{ backgroundColor: s.bg }}
                >
                  <div className="text-lg font-extrabold leading-tight" style={{ color: s.color }}>
                    {s.value}
                  </div>
                  <div className="mt-0.5 text-xs font-semibold text-black/60 leading-tight">
                    {s.label}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </Section>

        {/* ══════════════════════════════════════
            3. CÁC MÔN HỌC
        ══════════════════════════════════════ */}
        <Section
          icon={
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#000" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" />
              <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z" />
            </svg>
          }
          title="Các Môn Học"
        >
          {/* sub-header */}
          <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <p className="text-sm font-semibold text-black/60">
              {data.totalSubjects} môn học trong lộ trình · Hoàn thành tất cả để đạt chứng chỉ
            </p>
          </div>

          {/* grid */}
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3 sm:gap-6">
            {data.subjects.map((s: any) => (
              <SubjectCard key={s.id} subject={{
                id: s.id.toString(),
                code: s.code,
                fullName: s.name,
                topicCount: s.totalTopics,
                completedTopics: s.completedTopics,
                progress: s.subjectProgress,
                status: s.isCompleted ? "completed" : (s.isInProgress ? "in-progress" : "not-started"),
                description: s.description,
                cardBg: "#FAFAFA",
                accentHex: "#71717a",
                iconBg: "#52525b",
                progressGradient: "linear-gradient(90deg, #52525b 0%, #a1a1aa 100%)",
                icon: (
                  <svg
                    width="22"
                    height="22"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="white"
                    strokeWidth="2.5"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  >
                    <ellipse cx="12" cy="5" rx="9" ry="3" />
                    <path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3" />
                    <path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5" />
                  </svg>
                ),
              }} />
            ))}
          </div>

          {/* encouragement banner */}
          <div className="mt-6 flex flex-col gap-4 rounded-2xl border-[2.5px] border-black bg-[#FFFBEB] p-5 shadow-[4px_4px_0_0_#000] sm:flex-row sm:items-center">
            <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[2px_2px_0_0_#000]">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#000" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
              </svg>
            </span>
            <div className="flex-1">
              <p className="text-sm font-extrabold text-black">
                💪 Tiếp tục phát huy! Bạn đã hoàn thành{" "}
                <span className="text-[#4D7CFF]">{data.overallProgress}%</span> lộ trình.
              </p>
            </div>
            <svg
              className="hidden sm:block animate-spin"
              style={{ animationDuration: "8s" }}
              width="28"
              height="28"
              viewBox="0 0 32 32"
              fill="none"
            >
              <path
                d="M16 2l2.5 8.5H27l-7 5 2.5 8.5L16 19l-6.5 5L12 15.5l-7-5h8.5L16 2z"
                fill="#FFD84D"
                stroke="#000"
                strokeWidth="1.5"
              />
            </svg>
          </div>
        </Section>

        {/* ── FOOTER ── */}
        <p className="pb-6 text-center text-sm font-semibold text-black/40">
          Cập nhật lần cuối: 20/07/2026 · EduVN © 2026
        </p>
      </main>
    </div>
  );
}
