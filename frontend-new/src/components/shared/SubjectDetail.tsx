import { useEffect, useRef, useState, type ReactNode } from "react";
import {
  ArrowLeft,
  ArrowRight,
  BookOpen,
  Check,
  ChevronRight,
  Clock,
  HelpCircle,
  Lock,
  Play,
  Sparkles,
  Zap,
} from "lucide-react";

export type SubjectDifficulty = "beginner" | "intermediate" | "advanced";
export type TopicStatus = "completed" | "in_progress" | "locked" | "available";

export interface SubjectTopic {
  id: string;
  title: string;
  description?: string;
  emoji?: string;
  questions?: number;
  minutes?: number;
  status: TopicStatus;
}

export interface SubjectDetailProps {
  name: string;
  description: string;
  difficulty: SubjectDifficulty;
  topicsCount: number;
  hours: number;
  progress: number; // 0..100
  topics: SubjectTopic[];

  onBack?: () => void;
  onContinue?: () => void;
  onTopicClick?: (topic: SubjectTopic) => void;

  continueLabel?: string;
  className?: string;
}

const DIFFICULTY_META: Record<
  SubjectDifficulty,
  { label: string; bg: string; dot: string }
> = {
  beginner: { label: "Beginner", bg: "#DAF7E1", dot: "#22C55E" },
  intermediate: { label: "Intermediate", bg: "#FFF6C9", dot: "#F5C518" },
  advanced: { label: "Advanced", bg: "#FFE1CC", dot: "#FF6B2C" },
};

const STATUS_META: Record<
  TopicStatus,
  { label: string; bg: string; icon: ReactNode; locked?: boolean }
> = {
  completed: {
    label: "Completed",
    bg: "#DAF7E1",
    icon: <Check className="h-3.5 w-3.5" strokeWidth={3} />,
  },
  in_progress: {
    label: "In Progress",
    bg: "#FFF6C9",
    icon: <Play className="h-3 w-3 fill-black" strokeWidth={0} />,
  },
  available: {
    label: "Available",
    bg: "#DCE6FF",
    icon: <Zap className="h-3.5 w-3.5" strokeWidth={2.5} />,
  },
  locked: {
    label: "Locked",
    bg: "#E9E9E9",
    icon: <Lock className="h-3.5 w-3.5" strokeWidth={2.5} />,
    locked: true,
  },
};

export function SubjectDetail({
  name,
  description,
  difficulty,
  topicsCount,
  hours,
  progress,
  topics,
  onBack,
  onContinue,
  onTopicClick,
  continueLabel = "Continue Learning",
  className = "",
}: SubjectDetailProps) {
  const clamped = Math.max(0, Math.min(100, progress));
  const diff = DIFFICULTY_META[difficulty];

  // sticky compact header when the big hero scrolls past
  const sentinelRef = useRef<HTMLDivElement>(null);
  const [compact, setCompact] = useState(false);

  useEffect(() => {
    const el = sentinelRef.current;
    if (!el || typeof IntersectionObserver === "undefined") return;
    const io = new IntersectionObserver(
      ([entry]) => setCompact(!entry.isIntersecting),
      { rootMargin: "-8px 0px 0px 0px", threshold: 0 },
    );
    io.observe(el);
    return () => io.disconnect();
  }, []);

  return (
    <div className={`w-full max-w-5xl ${className}`}>
      {/* ============ COMPACT STICKY HEADER ============ */}
      {/* <div
        className={`sticky top-0 z-30 -mx-4 px-4 pt-3 pb-3 transition-all duration-200 sm:-mx-6 sm:px-6 ${
          compact
            ? "pointer-events-auto opacity-100 translate-y-0"
            : "pointer-events-none -translate-y-2 opacity-0"
        }`}
      >
        <div className="flex items-center gap-3 rounded-2xl border-[2.5px] border-black bg-white px-4 py-2.5 shadow-[6px_6px_0_0_#000]">
          {onBack && (
            <button
              type="button"
              onClick={onBack}
              className=" cursor-pointer flex h-8 w-8 shrink-0 items-center justify-center rounded-lg border-[2px] border-black bg-[#FFD84D] shadow-[2px_2px_0_0_#000] transition-transform hover:-translate-y-0.5"
              aria-label="Back"
            >
              <ArrowLeft className="h-4 w-4" strokeWidth={2.5} />
            </button>
          )}
          <div className="flex min-w-0 flex-1 items-center gap-3">
            <span className="truncate text-sm font-extrabold text-black sm:text-base">
              {name}
            </span>
            <div className="relative hidden h-2.5 flex-1 overflow-hidden rounded-full border-[2px] border-black bg-white sm:block">
              <div
                className="h-full bg-[#4D7CFF] transition-[width] duration-500"
                style={{ width: `${clamped}%` }}
              />
            </div>
          </div>
          <span className="shrink-0 rounded-full border-[2px] border-black bg-[#4D7CFF] px-2.5 py-0.5 text-xs font-extrabold text-white shadow-[2px_2px_0_0_#000]">
            {clamped}%
          </span>
        </div>
      </div> */}

      {/* ============ HERO ============ */}
      <div ref={sentinelRef} />

      {onBack && (
        <button
          type="button"
          onClick={onBack}
          className="cursor-pointer mb-4 inline-flex items-center gap-2 rounded-full border-[2.5px] border-black bg-white px-3.5 py-1.5 text-xs font-extrabold uppercase tracking-wider text-black shadow-[3px_3px_0_0_#000] transition-transform hover:-translate-y-0.5"
        >
          <ArrowLeft className="h-4 w-4" strokeWidth={2.5} />
          Back
        </button>
      )}

      <section className="relative overflow-hidden rounded-[28px] border-[2.5px] border-black bg-white p-5 shadow-[10px_10px_0_0_#000] sm:p-7">
        <span
          aria-hidden
          className="absolute -right-8 -top-8 h-32 w-32 rounded-full border-[2.5px] border-black bg-[#DCE6FF]"
        />
        <span
          aria-hidden
          className="absolute right-4 top-4 hidden h-9 w-9 rotate-12 items-center justify-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[3px_3px_0_0_#000] sm:flex"
        >
          <Sparkles className="h-4 w-4" strokeWidth={2.5} />
        </span>

        <div className="relative">
          <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1 text-[11px] font-extrabold uppercase tracking-widest text-black shadow-[3px_3px_0_0_#000]">
            ✦ Subject
          </span>

          <h1 className="mt-3 text-3xl font-extrabold leading-tight tracking-tight text-black sm:text-4xl">
            {name}
          </h1>
          <p className="mt-2 max-w-2xl text-sm leading-relaxed text-black/70 sm:text-base">
            {description}
          </p>

          {/* meta chips */}
          <div className="mt-4 flex flex-wrap items-center gap-2">
            <span
              className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black px-3 py-1.5 shadow-[3px_3px_0_0_#000]"
              style={{ backgroundColor: diff.bg }}
            >
              <span
                className="h-2.5 w-2.5 rounded-full border-[1.5px] border-black"
                style={{ backgroundColor: diff.dot }}
              />
              <span className="text-xs font-extrabold uppercase tracking-wider text-black">
                {diff.label}
              </span>
            </span>
            <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1.5 shadow-[3px_3px_0_0_#000]">
              <BookOpen className="h-3.5 w-3.5" strokeWidth={2.5} />
              <span className="text-xs font-extrabold uppercase tracking-wider text-black">
                {topicsCount} Topics
              </span>
            </span>
            <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1.5 shadow-[3px_3px_0_0_#000]">
              <Clock className="h-3.5 w-3.5" strokeWidth={2.5} />
              <span className="text-xs font-extrabold uppercase tracking-wider text-black">
                {hours} Hours
              </span>
            </span>
          </div>

          {/* progress */}
          <div className="mt-5">
            <div className="mb-1.5 flex items-center justify-between">
              <span className="text-[11px] font-extrabold uppercase tracking-widest text-black/70">
                Progress
              </span>
              <span className="text-sm font-extrabold text-black">
                {clamped}%
              </span>
            </div>
            <div className="relative h-4 overflow-hidden rounded-full border-[2.5px] border-black bg-white shadow-[3px_3px_0_0_#000]">
              <div
                className="h-full bg-[#4D7CFF] transition-[width] duration-700"
                style={{ width: `${clamped}%` }}
              />
            </div>
          </div>

          {/* {onContinue && (
            <button
              type="button"
              onClick={onContinue}
              className="mt-5 inline-flex h-12 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] px-6 text-sm font-extrabold uppercase tracking-wide text-white shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[3px_3px_0_0_#000]"
            >
              {continueLabel}
              <ArrowRight className="h-4 w-4" strokeWidth={2.5} />
            </button>
          )} */}
        </div>
      </section>

      {/* ============ TOPICS ============ */}
      <section className="mt-8">
        <div className="mb-4 flex items-center gap-3">
          <span className="flex h-8 w-8 items-center justify-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[3px_3px_0_0_#000]">
            <BookOpen className="h-4 w-4" strokeWidth={2.5} />
          </span>
          <h2 className="text-lg font-extrabold uppercase tracking-wider text-black sm:text-xl">
            Topics
          </h2>
          <span className="ml-auto rounded-full border-[2px] border-black bg-white px-2.5 py-0.5 text-xs font-extrabold text-black shadow-[2px_2px_0_0_#000]">
            {topics.length}
          </span>
        </div>

        <ul className="grid gap-4 sm:grid-cols-2">
          {topics.map((t, i) => (
            <TopicCard
              key={t.id}
              index={i + 1}
              topic={t}
              onClick={() => {
                if (t.status === "locked") return;
                onTopicClick?.(t);
              }}
            />
          ))}
        </ul>
      </section>
    </div>
  );
}

function TopicCard({
  topic,
  index,
  onClick,
}: {
  topic: SubjectTopic;
  index: number;
  onClick: () => void;
}) {
  const meta = STATUS_META[topic.status];
  const locked = meta.locked;

  return (
    <li>
      <button
        type="button"
        onClick={onClick}
        disabled={locked}
        aria-label={`${topic.title} — ${meta.label}`}
        className={` cursor-pointer group relative flex w-full items-start gap-4 rounded-[24px] border-[2.5px] border-black p-4 text-left shadow-[6px_6px_0_0_#000] transition-all sm:p-5 ${
          locked
            ? "cursor-not-allowed bg-[#F5F5F5] opacity-80"
            : "bg-white hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[3px_3px_0_0_#000]"
        }`}
      >
        <div className="relative shrink-0">
          <span
            className="flex h-12 w-12 items-center justify-center rounded-2xl border-[2.5px] border-black text-2xl shadow-[3px_3px_0_0_#000]"
            style={{ backgroundColor: locked ? "#E9E9E9" : "#FFF6C9" }}
            aria-hidden
          >
            {topic.emoji ?? "📘"}
          </span>
          <span className="absolute -bottom-1.5 -right-1.5 flex h-6 w-6 items-center justify-center rounded-lg border-[2px] border-black bg-white text-[10px] font-extrabold shadow-[2px_2px_0_0_#000]">
            {index}
          </span>
        </div>

        <div className="min-w-0 flex-1">
          <h3 className="truncate text-base font-extrabold text-black sm:text-lg">
            {topic.title}
          </h3>
          {topic.description && (
            <p className="mt-0.5 line-clamp-2 text-sm text-black/60">
              {topic.description}
            </p>
          )}

          <div className="mt-3 flex flex-wrap items-center gap-2">
            {!locked && typeof topic.questions === "number" && (
              <span className="inline-flex items-center gap-1 rounded-full border-[2px] border-black bg-white px-2.5 py-0.5 text-[11px] font-bold text-black shadow-[2px_2px_0_0_#000]">
                <HelpCircle className="h-3 w-3" strokeWidth={2.5} />
                {topic.questions} Questions
              </span>
            )}
            {!locked && typeof topic.minutes === "number" && (
              <span className="inline-flex items-center gap-1 rounded-full border-[2px] border-black bg-white px-2.5 py-0.5 text-[11px] font-bold text-black shadow-[2px_2px_0_0_#000]">
                <Clock className="h-3 w-3" strokeWidth={2.5} />
                {topic.minutes} min
              </span>
            )}
            <span
              className="ml-auto inline-flex items-center gap-1 rounded-full border-[2px] border-black px-2.5 py-0.5 text-[11px] font-extrabold uppercase tracking-wider text-black shadow-[2px_2px_0_0_#000]"
              style={{ backgroundColor: meta.bg }}
            >
              {meta.icon}
              {meta.label}
            </span>
          </div>
        </div>

        <ChevronRight
          className={`mt-1 h-5 w-5 shrink-0 transition-transform ${
            locked ? "text-black/30" : "text-black group-hover:translate-x-1"
          }`}
          strokeWidth={2.5}
        />
      </button>
    </li>
  );
}

export default SubjectDetail;
