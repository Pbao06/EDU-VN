"use client";
import { useMemo, useState } from "react";
import { Search, SlidersHorizontal, Sparkles } from "lucide-react";
import { CareerCard, type CareerCardProps, type DemandLevel } from "./CareerCard";

export interface CareerItem extends Omit<CareerCardProps, "onClick" | "className"> {
  id: string;
  category?: string;
}

export interface CareerGridProps {
  careers: CareerItem[];
  title?: string;
  subtitle?: string;
  categories?: string[];
  showSearch?: boolean;
  showFilters?: boolean;
  onCareerClick?: (career: CareerItem) => void;
  className?: string;
  emptyLabel?: string;
}

type DemandFilter = "all" | DemandLevel;

const demandFilters: { value: DemandFilter; label: string; bg: string }[] = [
  { value: "all", label: "Tất cả", bg: "bg-white" },
  { value: "high", label: "Nhu cầu cao", bg: "bg-[#7BE495]" },
  { value: "medium", label: "Trung bình", bg: "bg-[#FFD84D]" },
  { value: "low", label: "Nhu cầu thấp", bg: "bg-white" },
];

export function CareerGrid({
  careers,
  title = "Khám phá nghề nghiệp",
  subtitle = "Chọn một nghề để xem lộ trình học tập chi tiết",
  categories,
  showSearch = true,
  showFilters = true,
  onCareerClick,
  className = "",
  emptyLabel = "Không tìm thấy nghề nghiệp phù hợp",
}: CareerGridProps) {
  const [query, setQuery] = useState("");
  const [demand, setDemand] = useState<DemandFilter>("all");
  const [category, setCategory] = useState<string>("all");

  const derivedCategories = useMemo(() => {
    if (categories && categories.length) return categories;
    const set = new Set<string>();
    careers.forEach((c) => c.category && set.add(c.category));
    return Array.from(set);
  }, [categories, careers]);

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    return careers.filter((c) => {
      if (demand !== "all" && c.demand !== demand) return false;
      if (category !== "all" && c.category !== category) return false;
      if (!q) return true;
      return (
        c.name.toLowerCase().includes(q) ||
        c.description.toLowerCase().includes(q)
      );
    });
  }, [careers, query, demand, category]);

  return (
    <section className={`w-full ${className}`}>
      {/* Header */}
      <div className="relative mb-8 flex flex-col gap-3 sm:mb-10">
        <span className="inline-flex w-fit items-center gap-1.5 rounded-full border-[2.5px] border-black bg-[#FFD84D] px-3 py-1 text-[11px] font-extrabold uppercase tracking-wider text-black shadow-[3px_3px_0_0_#000]">
          <Sparkles className="h-3.5 w-3.5" strokeWidth={3} />
          {filtered.length} nghề nghiệp
        </span>
        <h2 className="text-3xl font-extrabold tracking-tight text-black sm:text-4xl md:text-5xl">
          {title}
        </h2>
        {subtitle && (
          <p className="max-w-2xl text-base text-black/70 sm:text-lg">{subtitle}</p>
        )}
      </div>

      {/* Toolbar */}
      {(showSearch || showFilters) && (
        <div className="mb-8 flex flex-col gap-4">
          {showSearch && (
            <div className="relative">
              <Search
                className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black"
                strokeWidth={2.5}
              />
              <input
                type="text"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Tìm nghề nghiệp bạn quan tâm..."
                className="w-full rounded-2xl border-[2.5px] border-black bg-white py-3.5 pl-12 pr-4 text-base font-semibold text-black placeholder:text-black/40 shadow-[5px_5px_0_0_#000] transition-shadow focus:outline-none focus:shadow-[7px_7px_0_0_#000]"
              />
            </div>
          )}

          {showFilters && (
            <div className="flex flex-col gap-3">
              <div className="flex flex-wrap items-center gap-2">
                <span className="flex items-center gap-1.5 text-xs font-extrabold uppercase tracking-wider text-black/60">
                  <SlidersHorizontal className="h-3.5 w-3.5" strokeWidth={3} />
                  Nhu cầu
                </span>
                {demandFilters.map((f) => {
                  const active = demand === f.value;
                  return (
                    <button
                      key={f.value}
                      type="button"
                      onClick={() => setDemand(f.value)}
                      className={`rounded-full border-[2.5px] border-black px-4 py-1.5 text-xs font-extrabold uppercase tracking-wide transition-all ${
                        active
                          ? `${f.bg} text-black shadow-[3px_3px_0_0_#000] -translate-y-0.5`
                          : "bg-white text-black/60 hover:text-black hover:shadow-[3px_3px_0_0_#000] hover:-translate-y-0.5"
                      }`}
                    >
                      {f.label}
                    </button>
                  );
                })}
              </div>

              {derivedCategories.length > 0 && (
                <div className="flex flex-wrap items-center gap-2">
                  <span className="text-xs font-extrabold uppercase tracking-wider text-black/60">
                    Lĩnh vực
                  </span>
                  <button
                    type="button"
                    onClick={() => setCategory("all")}
                    className={`rounded-full border-[2.5px] border-black px-4 py-1.5 text-xs font-extrabold uppercase tracking-wide transition-all ${
                      category === "all"
                        ? "bg-[#4D7CFF] text-white shadow-[3px_3px_0_0_#000] -translate-y-0.5"
                        : "bg-white text-black/60 hover:text-black hover:shadow-[3px_3px_0_0_#000] hover:-translate-y-0.5"
                    }`}
                  >
                    Tất cả
                  </button>
                  {derivedCategories.map((cat) => {
                    const active = category === cat;
                    return (
                      <button
                        key={cat}
                        type="button"
                        onClick={() => setCategory(cat)}
                        className={`rounded-full border-[2.5px] border-black px-4 py-1.5 text-xs font-extrabold uppercase tracking-wide transition-all ${
                          active
                            ? "bg-[#FF8A3D] text-black shadow-[3px_3px_0_0_#000] -translate-y-0.5"
                            : "bg-white text-black/60 hover:text-black hover:shadow-[3px_3px_0_0_#000] hover:-translate-y-0.5"
                        }`}
                      >
                        {cat}
                      </button>
                    );
                  })}
                </div>
              )}
            </div>
          )}
        </div>
      )}

      {/* Grid */}
      {filtered.length === 0 ? (
        <div className="flex flex-col items-center justify-center gap-3 rounded-[28px] border-[2.5px] border-dashed border-black bg-white p-12 text-center">
          <div className="flex h-14 w-14 items-center justify-center rounded-2xl border-[2.5px] border-black bg-[#FFD84D] shadow-[3px_3px_0_0_#000]">
            <Search className="h-7 w-7 text-black" strokeWidth={2.5} />
          </div>
          <p className="text-base font-bold text-black">{emptyLabel}</p>
          <p className="text-sm text-black/60">Thử điều chỉnh bộ lọc hoặc từ khoá khác</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          {filtered.map((career) => {
            const { id, category: _cat, ...rest } = career;
            return (
              <CareerCard
                key={id}
                {...rest}
                onClick={() => onCareerClick?.(career)}
              />
            );
          })}
        </div>
      )}
    </section>
  );
}

export default CareerGrid;
