"use client";
import type { LucideIcon } from "lucide-react";
import { TrendingUp, ArrowUpRight } from "lucide-react";

export type DemandLevel = "low" | "medium" | "high";

export interface CareerCardProps {
  name: string;
  description: string;
  salary: string;
  icon: LucideIcon;
  demand: DemandLevel;
  accent?: "yellow" | "orange" | "blue" | "pink" | "green";
  onClick?: () => void;
  className?: string;
}

const accentMap: Record<NonNullable<CareerCardProps["accent"]>, string> = {
  yellow: "bg-[#FFD84D]",
  orange: "bg-[#FF8A3D]",
  blue: "bg-[#4D7CFF]",
  pink: "bg-[#FF7AB6]",
  green: "bg-[#7BE495]",
};

const demandConfig: Record<
  DemandLevel,
  { label: string; dots: number; bg: string; text: string }
> = {
  low: { label: "Nhu cầu thấp", dots: 1, bg: "bg-white", text: "text-black" },
  medium: {
    label: "Nhu cầu trung bình",
    dots: 2,
    bg: "bg-[#FFD84D]",
    text: "text-black",
  },
  high: { label: "Nhu cầu cao", dots: 3, bg: "bg-[#7BE495]", text: "text-black" },
};

export function CareerCard({
  name,
  description,
  salary,
  icon: Icon,
  demand,
  accent = "yellow",
  onClick,
  className = "",
}: CareerCardProps) {
  const d = demandConfig[demand];

  return (
    <button
      type="button"
      onClick={onClick}
      className={`group relative flex w-full flex-col gap-5 rounded-[28px] border-[2.5px] border-black bg-white p-6 text-left shadow-[6px_6px_0_0_#000] transition-all duration-200 hover:-translate-x-0.5 hover:-translate-y-0.5 hover:shadow-[10px_10px_0_0_#000] focus:outline-none focus-visible:ring-4 focus-visible:ring-[#4D7CFF]/40 active:translate-x-0 active:translate-y-0 active:shadow-[3px_3px_0_0_#000] ${className}`}
    >
      {/* decorative star */}
      <span
        aria-hidden
        className="absolute -right-2 -top-2 hidden h-6 w-6 rotate-12 items-center justify-center text-black sm:flex"
      >
        <svg viewBox="0 0 24 24" fill="currentColor" className="h-full w-full">
          <path d="M12 2l2.4 6.4L21 9.2l-5 4.3L17.6 20 12 16.7 6.4 20 8 13.5 3 9.2l6.6-.8L12 2z" />
        </svg>
      </span>

      {/* header row: icon + demand */}
      <div className="flex items-start justify-between gap-3">
        <div
          className={`flex h-14 w-14 shrink-0 items-center justify-center rounded-2xl border-[2.5px] border-black ${accentMap[accent]} shadow-[3px_3px_0_0_#000]`}
        >
          <Icon className="h-7 w-7 text-black" strokeWidth={2.5} />
        </div>

        <div
          className={`flex items-center gap-1.5 rounded-full border-[2px] border-black px-3 py-1 ${d.bg} ${d.text}`}
        >
          <span className="flex items-center gap-0.5">
            {Array.from({ length: 3 }).map((_, i) => (
              <span
                key={i}
                className={`h-1.5 w-1.5 rounded-full border border-black ${
                  i < d.dots ? "bg-black" : "bg-white"
                }`}
              />
            ))}
          </span>
          <span className="text-[11px] font-bold uppercase tracking-wide">
            {d.label}
          </span>
        </div>
      </div>

      {/* title + description */}
      <div className="flex flex-col gap-2">
        <h3 className="text-xl font-extrabold leading-tight tracking-tight text-black sm:text-2xl">
          {name}
        </h3>
        <p className="line-clamp-2 text-sm leading-relaxed text-black/70">
          {description}
        </p>
      </div>

      {/* footer: salary + cta */}
      <div className="mt-auto flex items-end justify-between gap-3 pt-2">
        <div className="flex flex-col">
          <span className="text-[11px] font-bold uppercase tracking-wider text-black/50">
            Mức lương
          </span>
          <span className="flex items-center gap-1 text-base font-extrabold text-black">
            <TrendingUp className="h-4 w-4" strokeWidth={3} />
            {salary}
          </span>
        </div>

        <span className="flex h-10 w-10 items-center justify-center rounded-full border-[2.5px] border-black bg-[#4D7CFF] text-white shadow-[3px_3px_0_0_#000] transition-transform duration-200 group-hover:rotate-45">
          <ArrowUpRight className="h-5 w-5" strokeWidth={3} />
        </span>
      </div>
    </button>
  );
}

export default CareerCard;
