'use client';
import { useEffect, useMemo, useState, type ReactNode } from "react";
import React from "react"; // Đảm bảo đã import React
import {
  ArrowLeft,
  ArrowRight,
  Check,
  Flag,
  List,
  Menu,
  Sparkles,
  Star,
  X,
} from "lucide-react";

export interface PracticeOption {
  id: string;
  label: ReactNode;
}

export interface PracticeQuestion {
  id: string;
  prompt: ReactNode;
  code?: string;
  options: PracticeOption[];
  correctOptionId: string;
  explanation?: ReactNode;
}

export type AnswerMap = Record<string, string>; // questionId -> optionId

export interface TopicPracticeProps {
  topicName: string;
  subjectName?: string;
  questions: PracticeQuestion[];

  initialAnswers?: AnswerMap;
  initialIndex?: number;

  onBack?: () => void;
  onSubmit?: (result: {
    answers: AnswerMap;
    correctCount: number;
    total: number;
  }) => void;

  className?: string;
}

type QState = "correct" | "wrong" | "current" | "empty";

export function TopicPractice({
  topicName,
  subjectName,
  questions,
  initialAnswers = {},
  initialIndex = 0,
  onBack,
  onSubmit,
  className = "",
}: TopicPracticeProps) {
  const [index, setIndex] = useState(
    Math.min(Math.max(0, initialIndex), Math.max(0, questions.length - 1)),
  );
  const [answers, setAnswers] = useState<AnswerMap>(initialAnswers);
  const [checked, setChecked] = useState<Record<string, boolean>>({});
  const [mobileNavOpen, setMobileNavOpen] = useState(false);

  const total = questions.length;
  const current = questions[index];

  const answeredCount = useMemo(
    () => Object.keys(answers).filter((k) => answers[k]).length,
    [answers],
  );
  const correctCount = useMemo(
    () =>
      questions.reduce(
        (n, q) => n + (answers[q.id] === q.correctOptionId ? 1 : 0),
        0,
      ),
    [answers, questions],
  );
  const progress = total === 0 ? 0 : Math.round((answeredCount / total) * 100);

  useEffect(() => {
    setMobileNavOpen(false);
  }, [index]);

  function stateOf(qi: number): QState {
    const q = questions[qi];
    if (qi === index) return "current";
    if (!checked[q.id]) return "empty";
    const a = answers[q.id];
    if (!a) return "empty";
    return a === q.correctOptionId ? "correct" : "wrong";
  }

  function pick(optionId: string) {
    if (!current) return;
    setAnswers((prev) => ({ ...prev, [current.id]: optionId }));
  }

  function submitCurrent() {
    if (!current) return;
    setChecked((prev) => ({ ...prev, [current.id]: true }));
  }

  function goPrev() {
    setIndex((i) => Math.max(0, i - 1));
  }
  function goNext() {
    setIndex((i) => Math.min(total - 1, i + 1));
  }

  function finish() {
    const allChecked: Record<string, boolean> = {};
    questions.forEach((q) => (allChecked[q.id] = true));
    setChecked(allChecked);
    onSubmit?.({ answers, correctCount, total });
  }

  const isLast = index === total - 1;
  const currentChecked = current ? !!checked[current.id] : false;
  const currentAnswer = current ? answers[current.id] : undefined;

  return (
    <div className={`w-full max-w-6xl ${className}`}>
      {/* Top bar */}
      <div className="mb-5 flex items-center gap-3">
        {onBack && (
          <button
            type="button"
            onClick={onBack}
            className="inline-flex items-center gap-2 rounded-full border-[2.5px] border-black bg-white px-3.5 py-1.5 text-xs font-extrabold uppercase tracking-wider text-black shadow-[3px_3px_0_0_#000] transition-transform hover:-translate-y-0.5"
          >
            <ArrowLeft className="h-4 w-4" strokeWidth={2.5} />
            Back
          </button>
        )}
        <button
          type="button"
          onClick={() => setMobileNavOpen(true)}
          className="ml-auto inline-flex items-center gap-2 rounded-full border-[2.5px] border-black bg-[#FFD84D] px-3.5 py-1.5 text-xs font-extrabold uppercase tracking-wider text-black shadow-[3px_3px_0_0_#000] lg:hidden"
          aria-label="Open question list"
        >
          <Menu className="h-4 w-4" strokeWidth={2.5} />
          {index + 1}/{total}
        </button>
      </div>

      <div className="grid gap-6 lg:grid-cols-[260px_minmax(0,1fr)]">
        {/* ============ SIDEBAR (desktop) ============ */}
        <aside className="hidden lg:block">
          <SidebarPanel
            questions={questions}
            index={index}
            stateOf={stateOf}
            onJump={setIndex}
            correctCount={correctCount}
            answeredCount={answeredCount}
            total={total}
          />
        </aside>

        {/* ============ MAIN ============ */}
        <main className="min-w-0">
          {/* header */}
          <section className="rounded-[24px] border-[2.5px] border-black bg-white p-5 shadow-[8px_8px_0_0_#000] sm:p-6">
            <div className="flex items-start justify-between gap-3">
              <div className="min-w-0">
                {subjectName && (
                  <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1 text-[11px] font-extrabold uppercase tracking-widest text-black shadow-[3px_3px_0_0_#000]">
                    ✦ {subjectName}
                  </span>
                )}
                <h1 className="mt-2 truncate text-2xl font-extrabold tracking-tight text-black sm:text-3xl">
                  {topicName}
                </h1>
              </div>
              <span className="hidden shrink-0 rounded-2xl border-[2.5px] border-black bg-[#FFD84D] px-3 py-2 text-sm font-extrabold shadow-[3px_3px_0_0_#000] sm:inline-flex">
                {index + 1} / {total}
              </span>
            </div>

            <div className="mt-4">
              <div className="mb-1.5 flex items-center justify-between">
                <span className="text-[11px] font-extrabold uppercase tracking-widest text-black/70">
                  Progress
                </span>
                <span className="text-sm font-extrabold text-black">
                  {progress}%
                </span>
              </div>
              <div className="relative h-3.5 overflow-hidden rounded-full border-[2.5px] border-black bg-white shadow-[3px_3px_0_0_#000]">
                <div
                  className="h-full bg-[#4D7CFF] transition-[width] duration-500"
                  style={{ width: `${progress}%` }}
                />
              </div>
            </div>
          </section>

          {/* question card */}
          {current && (
            <section className="relative mt-6 overflow-hidden rounded-[28px] border-[2.5px] border-black bg-white p-5 shadow-[10px_10px_0_0_#000] sm:p-7">
              <span
                aria-hidden
                className="absolute -right-8 -top-8 h-28 w-28 rounded-full border-[2.5px] border-black bg-[#DCE6FF]"
              />
              <span
                aria-hidden
                className="absolute right-4 top-4 hidden h-9 w-9 rotate-12 items-center justify-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[3px_3px_0_0_#000] sm:flex"
              >
                <Sparkles className="h-4 w-4" strokeWidth={2.5} />
              </span>

              <div className="relative">
                <div className="flex items-center gap-2">
                  <span className="inline-flex items-center gap-1.5 rounded-full border-[2.5px] border-black bg-white px-3 py-1 text-[11px] font-extrabold uppercase tracking-widest text-black shadow-[3px_3px_0_0_#000]">
                    <Star
                      className="h-3 w-3 fill-black"
                      strokeWidth={0}
                    />
                    Question {index + 1}
                  </span>
                </div>

                <h2 className="mt-3 text-lg font-extrabold leading-snug text-black sm:text-xl">
                  {current.prompt}
                </h2>

                {current.code && (
                  <pre className="mt-4 overflow-x-auto rounded-2xl border-[2.5px] border-black bg-[#0F172A] p-4 text-sm font-mono leading-relaxed text-[#E2E8F0] shadow-[4px_4px_0_0_#000]">
                    <code>{current.code}</code>
                  </pre>
                )}

                {/* options */}
                <ul className="mt-5 grid gap-3">
                  {current.options.map((opt) => {
                    const selected = currentAnswer === opt.id;
                    const isCorrect = opt.id === current.correctOptionId;
                    const showResult = currentChecked;
                    const bg =
                      showResult && isCorrect
                        ? "#DAF7E1"
                        : showResult && selected && !isCorrect
                          ? "#FFD9D9"
                          : selected
                            ? "#FFF6C9"
                            : "#FFFFFF";
                    return (
                      <li key={opt.id}>
                        <button
                          type="button"
                          onClick={() => !showResult && pick(opt.id)}
                          disabled={showResult}
                          className={`flex w-full items-center gap-3 rounded-2xl border-[2.5px] border-black px-4 py-3 text-left shadow-[4px_4px_0_0_#000] transition-all ${
                            showResult
                              ? "cursor-default"
                              : "hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000]"
                          }`}
                          style={{ backgroundColor: bg }}
                        >
                          <span
                            className={`grid h-7 w-7 shrink-0 place-items-center rounded-lg border-[2.5px] border-black text-xs font-extrabold ${
                              selected ? "bg-[#4D7CFF] text-white" : "bg-white"
                            }`}
                          >
                            {String.fromCharCode(
                              65 + current.options.indexOf(opt),
                            )}
                          </span>
                          <span className="min-w-0 flex-1 text-sm font-bold text-black sm:text-base">
                            {opt.label}
                          </span>
                          {showResult && isCorrect && (
                            <Check
                              className="h-5 w-5 shrink-0 text-black"
                              strokeWidth={3}
                            />
                          )}
                          {showResult && selected && !isCorrect && (
                            <X
                              className="h-5 w-5 shrink-0 text-black"
                              strokeWidth={3}
                            />
                          )}
                        </button>
                      </li>
                    );
                  })}
                </ul>

                {currentChecked && current.explanation && (
                  <div className="mt-4 rounded-2xl border-[2.5px] border-black bg-[#F5F5F5] p-4 shadow-[4px_4px_0_0_#000]">
                    <div className="mb-1 text-[11px] font-extrabold uppercase tracking-widest text-black/70">
                      Giải thích
                    </div>
                    <div className="text-sm text-black">
                      {current.explanation}
                    </div>
                  </div>
                )}

                {/* actions */}
                <div className="mt-6 flex flex-wrap items-center gap-3">
                  <button
                    type="button"
                    onClick={goPrev}
                    disabled={index === 0}
                    className="inline-flex h-11 items-center gap-2 rounded-2xl border-[2.5px] border-black bg-white px-4 text-sm font-extrabold uppercase tracking-wide text-black shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_#000] disabled:cursor-not-allowed disabled:opacity-40 disabled:hover:translate-y-0 disabled:hover:shadow-[4px_4px_0_0_#000]"
                  >
                    <ArrowLeft className="h-4 w-4" strokeWidth={2.5} />
                    Previous
                  </button>

                  {!currentChecked ? (
                    <button
                      type="button"
                      onClick={submitCurrent}
                      disabled={!currentAnswer}
                      className="inline-flex h-11 items-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] px-5 text-sm font-extrabold uppercase tracking-wide text-white shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000] disabled:cursor-not-allowed disabled:opacity-50 disabled:hover:translate-y-0 disabled:hover:shadow-[4px_4px_0_0_#000]"
                    >
                      <Check className="h-4 w-4" strokeWidth={3} />
                      Submit
                    </button>
                  ) : isLast ? (
                    <button
                      type="button"
                      onClick={finish}
                      className="inline-flex h-11 items-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#FF6B2C] px-5 text-sm font-extrabold uppercase tracking-wide text-white shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000]"
                    >
                      <Flag className="h-4 w-4" strokeWidth={2.5} />
                      Finish
                    </button>
                  ) : (
                    <button
                      type="button"
                      onClick={goNext}
                      className="inline-flex h-11 items-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] px-5 text-sm font-extrabold uppercase tracking-wide text-white shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000]"
                    >
                      Next
                      <ArrowRight className="h-4 w-4" strokeWidth={2.5} />
                    </button>
                  )}

                  <div className="ml-auto text-xs font-extrabold uppercase tracking-wider text-black/60">
                    {correctCount}/{total} correct
                  </div>
                </div>
              </div>
            </section>
          )}
        </main>
      </div>

      {/* Mobile sidebar drawer */}
      {mobileNavOpen && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <div
            className="absolute inset-0 bg-black/50"
            onClick={() => setMobileNavOpen(false)}
          />
          <div className="absolute inset-y-0 left-0 w-[85%] max-w-sm overflow-y-auto border-r-[2.5px] border-black bg-white p-4 shadow-[10px_0_0_0_#000]">
            <div className="mb-3 flex items-center justify-between">
              <span className="inline-flex items-center gap-2 text-sm font-extrabold uppercase tracking-wider">
                <List className="h-4 w-4" strokeWidth={2.5} />
                Questions
              </span>
              <button
                type="button"
                onClick={() => setMobileNavOpen(false)}
                className="grid h-9 w-9 place-items-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[2px_2px_0_0_#000]"
                aria-label="Close"
              >
                <X className="h-4 w-4" strokeWidth={2.5} />
              </button>
            </div>
            <SidebarPanel
              questions={questions}
              index={index}
              stateOf={stateOf}
              onJump={setIndex}
              correctCount={correctCount}
              answeredCount={answeredCount}
              total={total}
            />
          </div>
        </div>
      )}
    </div>
  );
}

function SidebarPanel({
  questions,
  index,
  stateOf,
  onJump,
  correctCount,
  answeredCount,
  total,
}: {
  questions: PracticeQuestion[];
  index: number;
  stateOf: (i: number) => QState;
  onJump: (i: number) => void;
  correctCount: number;
  answeredCount: number;
  total: number;
}) {
  return (
    <div className="rounded-[24px] border-[2.5px] border-black bg-white p-4 shadow-[8px_8px_0_0_#000]">
      <div className="mb-3 flex items-center gap-2">
        <span className="grid h-8 w-8 place-items-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] shadow-[2px_2px_0_0_#000]">
          <List className="h-4 w-4" strokeWidth={2.5} />
        </span>
        <span className="text-sm font-extrabold uppercase tracking-wider text-black">
          Questions
        </span>
        <span className="ml-auto rounded-full border-[2px] border-black bg-white px-2 py-0.5 text-[11px] font-extrabold shadow-[2px_2px_0_0_#000]">
          {answeredCount}/{total}
        </span>
      </div>

      <ul className="grid grid-cols-5 gap-2 lg:grid-cols-4">
        {questions.map((q, i) => {
          const s = stateOf(i);
          const bg =
            s === "current"
              ? "#FFD84D"
              : s === "correct"
                ? "#DAF7E1"
                : s === "wrong"
                  ? "#FFD9D9"
                  : "#FFFFFF";
          return (
            <li key={q.id}>
              <button
                type="button"
                onClick={() => onJump(i)}
                aria-label={`Go to question ${i + 1}`}
                className="relative flex h-10 w-full items-center justify-center rounded-xl border-[2.5px] border-black text-sm font-extrabold shadow-[2px_2px_0_0_#000] transition-transform hover:-translate-y-0.5"
                style={{ backgroundColor: bg }}
              >
                {i + 1}
                {s === "correct" && (
                  <Check
                    className="absolute -right-1 -top-1 h-3.5 w-3.5 rounded-full border-[1.5px] border-black bg-[#22C55E] p-[1px] text-white"
                    strokeWidth={3}
                  />
                )}
                {s === "wrong" && (
                  <X
                    className="absolute -right-1 -top-1 h-3.5 w-3.5 rounded-full border-[1.5px] border-black bg-[#EF4444] p-[1px] text-white"
                    strokeWidth={3}
                  />
                )}
                {s === "current" && (
                  <Star
                    className="absolute -right-1 -top-1 h-3.5 w-3.5 rounded-full border-[1.5px] border-black bg-white fill-black p-[1px]"
                    strokeWidth={0}
                  />
                )}
              </button>
            </li>
          );
        })}
      </ul>

      <div className="mt-4 space-y-1.5 text-[11px] font-bold text-black/70">
        <LegendRow color="#DAF7E1" label="Đúng" />
        <LegendRow color="#FFD9D9" label="Sai" />
        <LegendRow color="#FFD84D" label="Câu hiện tại" />
        <LegendRow color="#FFFFFF" label="Chưa làm" />
      </div>

      <div className="mt-4 rounded-2xl border-[2.5px] border-black bg-[#DCE6FF] p-3 text-center shadow-[3px_3px_0_0_#000]">
        <div className="text-[11px] font-extrabold uppercase tracking-widest text-black/70">
          Correct
        </div>
        <div className="text-2xl font-extrabold text-black">
          {correctCount}
          <span className="text-black/50">/{total}</span>
        </div>
      </div>
    </div>
  );
}

function LegendRow({ color, label }: { color: string; label: string }) {
  return (
    <div className="flex items-center gap-2">
      <span
        className="h-4 w-4 rounded-md border-[2px] border-black"
        style={{ backgroundColor: color }}
      />
      {label}
    </div>
  );
}

export default TopicPractice;
