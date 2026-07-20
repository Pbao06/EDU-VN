import type { ReactNode } from "react";
import  Link  from "next/link";
import {
  Check,
  TrendingUp,
  Wallet,
  Sparkles,
  ArrowRight,
  Star,
  Briefcase,
  GraduationCap,
  Rocket,
  BookOpen,
  Zap,
  Globe2,
} from "lucide-react";

export type DemandLevel = "low" | "medium" | "high";

export interface CareerOutlook {
  demandLabel: string; // e.g. "Rất cao"
  demandStars?: number; // 1..5
  growthLabel: string; // e.g. "+30%" or "Xu hướng tăng"
  environmentLabel: string; // e.g. "Startup, Big Tech" or "Remote Supported"
}

export interface LearningPathPreviewItem {
  emoji?: string;
  title: string;
}

export interface CareerDetailProps {
  // Hero
  name: string;
  category: string;
  difficulty: number; // 1..5
  averageSalaryLabel: string; // e.g. "20 triệu/tháng"
  shortDescription: string;
  tags: string[];
  iconUrl?: string;

  // Overview
  overview: string;

  // Salary
  minSalary: number;
  maxSalary: number;
  currency?: string;
  salaryUnit?: string;

  // Responsibilities
  responsibilities: string[];

  // Skills
  requiredSkills: string[];

  // Subjects
  relatedSubjects: string[];

  // Outlook
  outlook: CareerOutlook;
  demandLevel?: DemandLevel;

  // Learning Path Preview
  learningPathPreview: LearningPathPreviewItem[];
  onViewFullRoadmap?: () => void;

  learningPathId?: number;

  // CTA
  onPrimaryAction?: () => void;
  onSecondaryAction?: () => void;
  primaryLabel?: string;
  secondaryLabel?: string;

  accent?: "yellow" | "orange" | "blue" | "green";
  className?: string;
}

const ACCENTS: Record<
  NonNullable<CareerDetailProps["accent"]>,
  { bg: string; soft: string }
> = {
  yellow: { bg: "#FFD84D", soft: "#FFF6C9" },
  orange: { bg: "#FF8A3D", soft: "#FFE1CC" },
  blue: { bg: "#4D7CFF", soft: "#DCE6FF" },
  green: { bg: "#7BE495", soft: "#DAF7E1" },
};

function formatSalary(v: number, currency: string) {
  if (v >= 1_000_000)
    return `${(v / 1_000_000).toFixed(v % 1_000_000 === 0 ? 0 : 1)}tr ${currency}`;
  if (v >= 1_000) return `${(v / 1_000).toFixed(0)}k ${currency}`;
  return `${v.toLocaleString("vi-VN")} ${currency}`;
}

export function CareerDetail({
  name,
  category,
  difficulty,
  averageSalaryLabel,
  shortDescription,
  tags,
  iconUrl,
  overview,
  minSalary,
  maxSalary,
  currency = "VND",
  salaryUnit = "tháng",
  responsibilities,
  requiredSkills,
  relatedSubjects,
  outlook,
  learningPathPreview,
  onViewFullRoadmap,
  learningPathId,
  onPrimaryAction,
  onSecondaryAction,
  primaryLabel = "Bắt đầu học ngay",
  secondaryLabel = "Làm bài Quiz định hướng",
  accent = "blue",
  className = "",
}: CareerDetailProps) {
  const a = ACCENTS[accent];

  return (
    <article className={`w-full max-w-5xl space-y-8 ${className}`}>
      {/* ============ 1. HERO ============ */}
      <section className="relative overflow-hidden rounded-[32px] border-[2.5px] border-black bg-white p-6 shadow-[10px_10px_0_0_#000] sm:p-9">
        {/* decorative shapes */}
        <span
          aria-hidden
          className="absolute -right-10 -top-10 h-40 w-40 rounded-full border-[2.5px] border-black"
          style={{ backgroundColor: a.soft }}
        />
        <span
          aria-hidden
          className="absolute -right-4 top-6 hidden h-10 w-10 rotate-12 items-center justify-center rounded-2xl border-[2.5px] border-black bg-[#FFD84D] shadow-[4px_4px_0_0_#000] sm:flex"
        >
          <Sparkles className="h-5 w-5 text-black" strokeWidth={2.5} />
        </span>

        <div className="relative flex flex-col gap-6 sm:flex-row sm:items-start sm:gap-7">
          {/* Icon */}
          {iconUrl && (
            <div className="relative shrink-0">
              <span
                aria-hidden
                className="absolute -left-2 -top-2 h-full w-full rounded-[28px] border-[2.5px] border-black"
                style={{ backgroundColor: a.soft }}
              />
              <div
                className="relative flex h-24 w-24 items-center justify-center overflow-hidden rounded-[28px] border-[2.5px] border-black shadow-[6px_6px_0_0_#000] sm:h-28 sm:w-28"
                style={{ backgroundColor: a.bg }}
              >
                <img
                  src={iconUrl}
                  alt={name}
                  className="h-14 w-14 object-contain sm:h-16 sm:w-16"
                  loading="lazy"
                />
              </div>
            </div>
          )}

          {/* Title block */}
          <div className="flex min-w-0 flex-1 flex-col gap-3">
            <div className="flex flex-wrap items-center gap-2">
              <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1 text-[11px] font-extrabold uppercase tracking-widest text-black shadow-[3px_3px_0_0_#000]">
                ✦ Career profile
              </span>
              <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-[#DCE6FF] px-3 py-1 text-[11px] font-extrabold uppercase tracking-widest text-black shadow-[3px_3px_0_0_#000]">
                {category}
              </span>
            </div>

            <h1 className="text-3xl font-extrabold leading-tight tracking-tight text-black sm:text-5xl">
              {name}
            </h1>

            {/* meta row: difficulty + salary */}
            <div className="flex flex-wrap items-center gap-2">
              <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1.5 shadow-[3px_3px_0_0_#000]">
                <span className="text-[11px] font-extrabold uppercase tracking-wider text-black/70">
                  Difficulty
                </span>
                <span className="flex items-center gap-0.5">
                  {[1, 2, 3, 4, 5].map((i) => (
                    <Star
                      key={i}
                      className="h-3.5 w-3.5"
                      strokeWidth={2.5}
                      fill={i <= difficulty ? "#FFD84D" : "transparent"}
                      color="#000"
                    />
                  ))}
                </span>
              </span>

              <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-[#FFF6C9] px-3 py-1.5 shadow-[3px_3px_0_0_#000]">
                <Wallet className="h-4 w-4 text-black" strokeWidth={2.5} />
                <span className="text-xs font-extrabold uppercase tracking-wider text-black">
                  Avg {averageSalaryLabel}
                </span>
              </span>
            </div>

            <p className="text-sm leading-relaxed text-black/70 sm:text-base">
              {shortDescription}
            </p>

            {/* tags */}
            {tags.length > 0 && (
              <ul className="mt-1 flex flex-wrap gap-2">
                {tags.map((t) => (
                  <li
                    key={t}
                    className="rounded-full border-[2px] border-black bg-white px-2.5 py-1 text-xs font-bold text-black shadow-[2px_2px_0_0_#000]"
                  >
                    #{t}
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      </section>

      {/* ============ 2. OVERVIEW ============ */}
      <Section
        icon={<Briefcase className="h-4 w-4" strokeWidth={2.5} />}
        title="Overview"
      >
        <div className="rounded-2xl border-[2.5px] border-black bg-white p-5 shadow-[6px_6px_0_0_#000]">
          <p className="whitespace-pre-line text-sm leading-relaxed text-black/80 sm:text-base">
            {overview}
          </p>
        </div>
      </Section>

      {/* ============ 3. SALARY ============ */}
      <Section
        icon={<Wallet className="h-4 w-4" strokeWidth={2.5} />}
        title="Reference Salary"
      >
        <div className="grid gap-4 sm:grid-cols-2">
          <SalaryTile
            label="Minimum Salary"
            value={formatSalary(minSalary, currency)}
            unit={`/ ${salaryUnit}`}
            bg="#DCE6FF"
          />
          <SalaryTile
            label="Maximum Salary"
            value={formatSalary(maxSalary, currency)}
            unit={`/ ${salaryUnit}`}
            bg="#FFE1CC"
            highlight
          />
        </div>
      </Section>

      {/* ============ 4. RESPONSIBILITIES ============ */}
      <Section
        icon={<Check className="h-4 w-4" strokeWidth={3} />}
        title="Main Responsibilities"
      >
        <ul className="grid gap-3 sm:grid-cols-2">
          {responsibilities.map((item, i) => (
            <li
              key={i}
              className="flex items-start gap-3 rounded-2xl border-[2.5px] border-black bg-white p-4 shadow-[4px_4px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_#000]"
            >
              <span
                className="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-xl border-[2.5px] border-black bg-[#7BE495] shadow-[2px_2px_0_0_#000]"
                aria-hidden
              >
                <Check className="h-4 w-4 text-black" strokeWidth={3.5} />
              </span>
              <span className="text-sm font-semibold leading-relaxed text-black/80">
                {item}
              </span>
            </li>
          ))}
        </ul>
      </Section>

      {/* ============ 5. REQUIRED SKILLS (chips) ============ */}
      <Section
        icon={<Zap className="h-4 w-4" strokeWidth={2.5} />}
        title="Required Skills"
      >
        <div className="flex flex-wrap gap-2">
          {requiredSkills.map((s) => (
            <span
              key={s}
              className="inline-flex items-center rounded-full border-[2px] border-black bg-white px-3 py-1.5 text-sm font-bold text-black shadow-[2px_2px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:shadow-[3px_3px_0_0_#000]"
            >
              {s}
            </span>
          ))}
        </div>
      </Section>

      {/* ============ 6. RELATED SUBJECTS ============ */}
      <Section
        icon={<GraduationCap className="h-4 w-4" strokeWidth={2.5} />}
        title="Related Subjects"
      >
        <ul className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
          {relatedSubjects.map((s) => (
            <li
              key={s}
              className="flex items-center gap-3 rounded-2xl border-[2.5px] border-black bg-white p-4 shadow-[4px_4px_0_0_#000]"
            >
              <span
                className="flex h-8 w-8 shrink-0 items-center justify-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[2px_2px_0_0_#000]"
                aria-hidden
              >
                <BookOpen className="h-4 w-4 text-black" strokeWidth={2.5} />
              </span>
              <span className="text-sm font-bold text-black">{s}</span>
            </li>
          ))}
        </ul>
      </Section>

      {/* ============ 7. CAREER OUTLOOK ============ */}
      <Section
        icon={<TrendingUp className="h-4 w-4" strokeWidth={2.5} />}
        title="Career Outlook"
      >
        <div className="grid gap-4 sm:grid-cols-3">
          <OutlookCard
            bg="#DCE6FF"
            label="Demand"
            value={outlook.demandLabel}
            extra={
              typeof outlook.demandStars === "number" && (
                <span className="flex items-center gap-0.5">
                  {[1, 2, 3, 4, 5].map((i) => (
                    <Star
                      key={i}
                      className="h-4 w-4"
                      strokeWidth={2.5}
                      fill={i <= (outlook.demandStars ?? 0) ? "#FF8A3D" : "transparent"}
                      color="#000"
                    />
                  ))}
                </span>
              )
            }
          />
          <OutlookCard
            bg="#DAF7E1"
            label="Growth"
            value={outlook.growthLabel}
            extra={
              <TrendingUp className="h-5 w-5 text-black" strokeWidth={2.5} />
            }
          />
          <OutlookCard
            bg="#FFE1CC"
            label="Environment"
            value={outlook.environmentLabel}
            extra={<Globe2 className="h-5 w-5 text-black" strokeWidth={2.5} />}
          />
        </div>
      </Section>

      {/* ============ 8. LEARNING PATH PREVIEW ============ */}
      <Section
        icon={<Rocket className="h-4 w-4" strokeWidth={2.5} />}
        title="Learning Path Preview"
      >
        <div className="rounded-[28px] border-[2.5px] border-black bg-white p-5 shadow-[8px_8px_0_0_#000] sm:p-6">
          <ol className="space-y-3">
            {learningPathPreview.map((item, i) => (
              <li
                key={`${item.title}-${i}`}
                className="flex items-center gap-4 rounded-2xl border-[2.5px] border-black bg-[#FFF6C9] p-4 shadow-[4px_4px_0_0_#000]"
              >
                <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl border-[2.5px] border-black bg-white text-sm font-extrabold shadow-[2px_2px_0_0_#000]">
                  {i + 1}
                </span>
                <span className="text-xl" aria-hidden>
                  {item.emoji ?? "📘"}
                </span>
                <span className="flex-1 text-sm font-bold text-black sm:text-base">
                  {item.title}
                </span>
              </li>
            ))}
          </ol>

          {onViewFullRoadmap && (
            <div className="mt-5 flex justify-end">
              <button
                type="button"
                onClick={onViewFullRoadmap}
                className="inline-flex items-center gap-2 rounded-2xl border-[2.5px] border-black bg-white px-4 py-2.5 text-sm font-extrabold uppercase tracking-wide text-black shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:bg-[#FFD84D] hover:shadow-[6px_6px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000]"
              >
                View Full Roadmap
                <ArrowRight className="h-4 w-4" strokeWidth={2.5} />
              </button>
            </div>
          )}
        </div>
      </Section>

      {/* ============ 9. CTA ============ */}
      {(onPrimaryAction || onSecondaryAction) && (
        <section className={`flex gap-3 ${onSecondaryAction ? 'flex-col sm:flex-row' : 'justify-center'}`}>
          {onPrimaryAction && (
            <Link
              href={learningPathId ? `/learningpath/${learningPathId}` : "/learningpath"}
              className={`inline-flex h-14 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] text-base font-extrabold uppercase tracking-wide text-white shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[3px_3px_0_0_#000] ${onSecondaryAction ? 'flex-1' : 'px-8'}`}
            >
              {primaryLabel}
              <ArrowRight className="h-5 w-5" strokeWidth={2.5} />
            </Link>
          )}
          {onSecondaryAction && (
            <button
              type="button"
              onClick={onSecondaryAction}
              className="inline-flex h-14 flex-1 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#FFD84D] text-base font-extrabold uppercase tracking-wide text-black shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[3px_3px_0_0_#000]"
            >
              {secondaryLabel}
            </button>
          )}
        </section>
      )}
    </article>
  );
}

/* ---------- helpers ---------- */

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

function SalaryTile({
  label,
  value,
  unit,
  bg,
  highlight = false,
}: {
  label: string;
  value: string;
  unit: string;
  bg: string;
  highlight?: boolean;
}) {
  return (
    <div
      className="relative rounded-2xl border-[2.5px] border-black p-5 shadow-[6px_6px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000]"
      style={{ backgroundColor: bg }}
    >
      <div className="flex items-center justify-between">
        <span className="text-[11px] font-extrabold uppercase tracking-widest text-black/70">
          {label}
        </span>
        {highlight && (
          <span className="rounded-full border-[2px] border-black bg-white px-2 py-0.5 text-[10px] font-extrabold uppercase tracking-wider text-black">
            Top
          </span>
        )}
      </div>
      <div className="mt-2 flex items-baseline gap-1.5">
        <span className="text-3xl font-extrabold tracking-tight text-black sm:text-4xl">
          {value}
        </span>
        <span className="text-xs font-bold text-black/60">{unit}</span>
      </div>
    </div>
  );
}

function OutlookCard({
  bg,
  label,
  value,
  extra,
}: {
  bg: string;
  label: string;
  value: string;
  extra?: ReactNode;
}) {
  return (
    <div
      className="rounded-2xl border-[2.5px] border-black p-5 shadow-[6px_6px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000]"
      style={{ backgroundColor: bg }}
    >
      <div className="flex items-center justify-between">
        <span className="text-[11px] font-extrabold uppercase tracking-widest text-black/70">
          {label}
        </span>
        {extra}
      </div>
      <div className="mt-2 text-2xl font-extrabold tracking-tight text-black sm:text-3xl">
        {value}
      </div>
    </div>
  );
}

export default CareerDetail;
