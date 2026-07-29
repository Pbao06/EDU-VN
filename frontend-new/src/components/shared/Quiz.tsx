import { useEffect, useMemo, useState, type ReactNode } from "react";
export interface QuizChoice {
  id: string;
  label: ReactNode;
  emoji?: string;
}
export interface QuizQuestion {
  id: string;
  question: ReactNode;
  hint?: ReactNode;
  choices: QuizChoice[];
}
export interface CareerQuizProps {
  questions: QuizQuestion[];
  /** Tiêu đề nhỏ phía trên card */
  eyebrow?: string;
  initialIndex?: number;
  initialAnswers?: Record<string, string>;
  onChange?: (answers: Record<string, string>) => void;
  onFinish?: (answers: Record<string, string>) => void;
  submitting?: boolean;
  className?: string;
}
const CHIPS = ["#4D7CFF", "#FFD84D", "#FF6B2C", "#8AD6A0", "#C9B6FF"];
export function CareerQuiz({
  questions,
  eyebrow = "🧠 Đánh giá nghề nghiệp",
  initialIndex = 0,
  initialAnswers = {},
  onChange,
  onFinish,
  submitting = false,
  className = "",
}: CareerQuizProps) {
  const total = questions.length;
  const [index, setIndex] = useState(
    Math.min(Math.max(0, initialIndex), Math.max(0, total - 1)),
  );
  const [answers, setAnswers] =
    useState<Record<string, string>>(initialAnswers);
  const [enter, setEnter] = useState(true);
  const current = questions[index];
  const picked = current ? answers[current.id] : undefined;
  const isLast = index === total - 1;
  const progress = useMemo(
    () => (total === 0 ? 0 : Math.round(((index + 1) / total) * 100)),
    [index, total],
  );
  useEffect(() => {
    setEnter(false);
    const t = window.setTimeout(() => setEnter(true), 20);
    return () => window.clearTimeout(t);
  }, [index]);
  function choose(choiceId: string) {
    if (!current) return;
    const next = { ...answers, [current.id]: choiceId };
    setAnswers(next);
    onChange?.(next);
  }
  function advance() {
    if (!picked) return;
    if (isLast) {
      onFinish?.(answers);
      return;
    }
    setIndex((i) => Math.min(total - 1, i + 1));
  }
  if (!current) return null;
  return (
    <div
      className={`relative mx-auto w-full max-w-[700px] px-4 py-10 ${className}`}
    >
      {/* decorations */}
      <span
        aria-hidden
        className="pointer-events-none absolute -left-6 top-16 hidden h-16 w-16 rotate-12 rounded-2xl border-[2.5px] border-black bg-[#FFD84D] shadow-[5px_5px_0_0_#000] sm:block"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute -right-4 bottom-24 hidden h-12 w-12 rounded-full border-[2.5px] border-black bg-[#FF6B2C] shadow-[5px_5px_0_0_#000] sm:block"
      />
      {/* ==== header above card ==== */}
      <div className="mb-6 text-center">
        <span className="inline-flex items-center gap-2 rounded-full border-[2.5px] border-black bg-white px-4 py-1.5 text-[12px] font-extrabold uppercase tracking-widest text-black shadow-[4px_4px_0_0_#000]">
          {eyebrow}
        </span>
        <div className="mt-4 flex items-center justify-center gap-2 text-sm font-extrabold uppercase tracking-widest text-black/70">
          Câu
          <span className="rounded-lg border-[2.5px] border-black bg-[#FFD84D] px-2 py-0.5 text-black shadow-[3px_3px_0_0_#000]">
            {index + 1}
          </span>
          / {total}
        </div>
        {/* segmented progress */}
        <div className="mx-auto mt-4 flex max-w-md gap-1.5">
          {questions.map((q, i) => (
            <span
              key={q.id}
              className="h-3 flex-1 rounded-full border-[2px] border-black transition-colors duration-300"
              style={{
                backgroundColor:
                  i < index ? "#4D7CFF" : i === index ? "#FFD84D" : "#FFFFFF",
              }}
            />
          ))}
        </div>
        <div className="mt-2 text-[11px] font-extrabold uppercase tracking-widest text-black/50">
          {progress}% hoàn thành
        </div>
      </div>
      {/* ==== quiz card ==== */}
      <section
        key={current.id}
        className={`relative overflow-hidden rounded-[32px] border-[3px] border-black bg-white p-6 shadow-[12px_12px_0_0_#000] transition-all duration-300 sm:p-9 ${
          enter ? "translate-y-0 opacity-100" : "translate-y-3 opacity-0"
        }`}
      >
        <span
          aria-hidden
          className="absolute -right-10 -top-10 h-32 w-32 rounded-full border-[3px] border-black bg-[#DCE6FF]"
        />
        <div className="relative">
          <h2 className="text-balance text-2xl font-extrabold leading-tight tracking-tight text-black sm:text-[32px]">
            {current.question}
          </h2>
          {current.hint && (
            <p className="mt-2 text-sm font-bold text-black/55">
              {current.hint}
            </p>
          )}
          <ul className="mt-7 grid gap-3.5">
            {current.choices.map((c, i) => {
              const active = picked === c.id;
              return (
                <li key={c.id}>
                  <button
                    type="button"
                    onClick={() => choose(c.id)}
                    aria-pressed={active}
                    className={`cursor-pointer group flex w-full items-center gap-4 rounded-2xl border-[2.5px] border-black px-4 py-4 text-left transition-all ${
                      active
                        ? "translate-x-[2px] translate-y-[2px] bg-[#DCE6FF] shadow-[3px_3px_0_0_#000]"
                        : "bg-white shadow-[6px_6px_0_0_#000] hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000]"
                    }`}
                  >
                    <span
                      className="grid h-11 w-11 shrink-0 place-items-center rounded-xl border-[2.5px] border-black text-base font-extrabold shadow-[3px_3px_0_0_#000]"
                      style={{
                        backgroundColor: active
                          ? CHIPS[i % CHIPS.length]
                          : "#FFFFFF",
                      }}
                    >
                      {c.emoji ?? String.fromCharCode(65 + i)}
                    </span>
                    <span className="min-w-0 flex-1 text-base font-extrabold text-black sm:text-lg">
                      {c.label}
                    </span>
                    <span
                      className={`grid h-6 w-6 shrink-0 place-items-center rounded-full border-[2.5px] border-black transition-colors ${
                        active ? "bg-[#4D7CFF]" : "bg-white"
                      }`}
                    >
                      {active && (
                        <svg
                          viewBox="0 0 24 24"
                          className="h-3.5 w-3.5"
                          fill="none"
                          stroke="#fff"
                          strokeWidth="4"
                          strokeLinecap="round"
                          strokeLinejoin="round"
                        >
                          <path d="M4 12.5 9.5 18 20 6.5" />
                        </svg>
                      )}
                    </span>
                  </button>
                </li>
              );
            })}
          </ul>
          <div className="mt-8 flex items-center gap-3">
            {index > 0 && (
              <button
                type="button"
                onClick={() => setIndex((i) => Math.max(0, i - 1))}
                className="cursor-pointer inline-flex h-12 items-center rounded-2xl border-[2.5px] border-black bg-white px-4 text-sm font-extrabold uppercase tracking-wide text-black shadow-[5px_5px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[7px_7px_0_0_#000]"
              >
                ← Quay lại
              </button>
            )}
            <button
              type="button"
              onClick={advance}
              disabled={!picked || submitting}
              className={`cursor-pointer inline-flex h-12 flex-1 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black px-6 text-sm font-extrabold uppercase tracking-widest text-white shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000] disabled:cursor-not-allowed disabled:opacity-40 disabled:hover:translate-y-0 disabled:hover:shadow-[6px_6px_0_0_#000] ${
                isLast ? "bg-[#FF6B2C]" : "bg-[#4D7CFF]"
              }`}
            >
              {submitting
                ? "Đang xử lý…"
                : isLast
                  ? "Xem kết quả 🎉"
                  : "Tiếp tục →"}
            </button>
          </div>
        </div>
      </section>
      <p className="mt-5 text-center text-xs font-bold text-black/45">
        Không có câu trả lời đúng hay sai — chọn điều bạn thấy giống mình nhất.
      </p>
    </div>
  );
}
export default CareerQuiz;