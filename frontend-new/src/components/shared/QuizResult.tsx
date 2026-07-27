import type { ReactNode } from "react";
export interface RecommendationItem {
  id: string;
  name: string;
  /** 0 - 100 */
  score: number;
  emoji?: string;
  description?: ReactNode;
}
export interface RecommendationQuizProps {
  title?: string;
  description?: ReactNode;
  /** Danh sách nghề đã có score; component tự sắp xếp giảm dần */
  results: RecommendationItem[];
  onSelect?: (item: RecommendationItem) => void;
  onHome?: () => void;
  className?: string;
}
const MEDALS = ["🥇", "🥈", "🥉"];
const TOP_COLORS = ["#FFD84D", "#DCE6FF", "#FFC7A8"];
function ScoreRing({ score, color }: { score: number; color: string }) {
  return (
    <div className="relative mx-auto mt-5 h-24 w-24">
      <div
        className="grid h-24 w-24 place-items-center rounded-full border-[3px] border-black shadow-[5px_5px_0_0_#000]"
        style={{
          background: `conic-gradient(${color} ${score * 3.6}deg, #FFFFFF 0deg)`,
        }}
      >
        <span className="grid h-16 w-16 place-items-center rounded-full border-[2.5px] border-black bg-white text-lg font-extrabold text-black">
          {score}%
        </span>
      </div>
    </div>
  );
}
export function RecommendationQuiz({
  title = "Kết quả đánh giá của bạn",
  description = "Dựa trên câu trả lời, đây là các ngành phù hợp nhất với bạn.",
  results,
  onSelect,
  onHome,
  className = "",
}: RecommendationQuizProps) {
  const sorted = [...results].sort((a, b) => b.score - a.score);
  const top3 = sorted.slice(0, 3);
  const rest = sorted.slice(3);
  return (
    <div
      className={`relative min-h-screen w-full overflow-hidden bg-[linear-gradient(180deg,#EEF3FF_0%,#FFFFFF_45%,#FFF7E0_100%)] ${className}`}
    >
      {/* decorations */}
      <span
        aria-hidden
        className="pointer-events-none absolute left-6 top-32 hidden h-14 w-14 rotate-12 rounded-2xl border-[2.5px] border-black bg-[#FFD84D] shadow-[5px_5px_0_0_#000] lg:block"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute right-10 top-56 hidden h-10 w-10 rounded-full border-[2.5px] border-black bg-[#FF6B2C] shadow-[4px_4px_0_0_#000] lg:block"
      />
      <div className="relative mx-auto w-full max-w-6xl px-4 py-10 sm:px-6 sm:py-14">
        {/* home button */}
        <div className="flex justify-end">
          <button
            type="button"
            onClick={onHome}
            className="inline-flex h-11 items-center gap-2 rounded-2xl border-[2.5px] border-black bg-white px-4 text-xs font-extrabold uppercase tracking-widest text-black shadow-[5px_5px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[7px_7px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000]"
          >
            🏠 Về trang chủ
          </button>
        </div>
        {/* heading */}
        <header className="mt-4 text-center">
          <span className="inline-flex items-center gap-2 rounded-full border-[2.5px] border-black bg-white px-4 py-1.5 text-[12px] font-extrabold uppercase tracking-widest text-black shadow-[4px_4px_0_0_#000]">
            🎉 Hoàn thành bài đánh giá
          </span>
          <h1 className="mx-auto mt-5 max-w-3xl text-balance text-3xl font-extrabold leading-tight tracking-tight text-black sm:text-5xl">
            {title}
          </h1>
          <p className="mx-auto mt-4 max-w-xl text-base font-bold text-black/60">
            {description}
          </p>
        </header>
        <div className="my-10 h-[3px] w-full rounded-full bg-black/10" />
        {/* top 3 */}
        <h2 className="text-xl font-extrabold uppercase tracking-wide text-black sm:text-2xl">
          Top 3 ngành nghề phù hợp
        </h2>
        <div className="mt-6 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {top3.map((item, i) => (
            <article
              key={item.id}
              className={`relative overflow-hidden rounded-[28px] border-[3px] border-black bg-white p-6 text-center shadow-[10px_10px_0_0_#000] transition-all hover:-translate-y-1 hover:shadow-[13px_13px_0_0_#000] ${
                i === 0 ? "lg:-translate-y-3" : ""
              }`}
            >
              <span
                aria-hidden
                className="absolute -right-8 -top-8 h-24 w-24 rounded-full border-[3px] border-black"
                style={{ backgroundColor: TOP_COLORS[i] }}
              />
              <div className="relative">
                <span className="inline-grid h-14 w-14 place-items-center rounded-2xl border-[2.5px] border-black bg-white text-2xl shadow-[4px_4px_0_0_#000]">
                  {item.emoji ?? MEDALS[i]}
                </span>
                <ScoreRing score={item.score} color={TOP_COLORS[i]} />
                {item.description && (
                  <p className="mt-3 min-h-[52px] text-sm font-semibold leading-6 text-black/70">
                    {item.description}
                  </p>
                )}
                <h3 className="mt-3 text-xl font-extrabold leading-tight text-black sm:text-2xl">
                  {item.name}
                </h3>
                <button
                  type="button"
                  onClick={() => onSelect?.(item)}
                  className="mt-6 inline-flex h-12 w-full items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] px-5 text-sm font-extrabold uppercase tracking-widest text-white shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[2px_2px_0_0_#000]"
                >
                  Chi tiết →
                </button>
              </div>
            </article>
          ))}
        </div>
        {rest.length > 0 && (
          <>
            <div className="my-10 h-[3px] w-full rounded-full bg-black/10" />
            <h2 className="text-xl font-extrabold uppercase tracking-wide text-black sm:text-2xl">
              Các nghề khác phù hợp
            </h2>
            <section className="mt-6 overflow-hidden rounded-[28px] border-[3px] border-black bg-white shadow-[10px_10px_0_0_#000]">
              <ul className="divide-y-[2.5px] divide-black">
                {rest.map((item, i) => (
                  <li key={item.id}>
                    <button
                      type="button"
                      onClick={() => onSelect?.(item)}
                      className="group flex w-full items-center gap-4 px-5 py-4 text-left transition-colors hover:bg-[#F2F6FF] sm:px-7 sm:py-5"
                    >
                      <span className="grid h-10 w-10 shrink-0 place-items-center rounded-xl border-[2.5px] border-black bg-[#FFD84D] text-sm font-extrabold text-black shadow-[3px_3px_0_0_#000]">
                        {item.emoji ?? i + 4}
                      </span>
                      <span className="min-w-0 flex-1">
                        <span className="block truncate text-base font-extrabold text-black sm:text-lg">
                          {item.name}
                        </span>
                        <span className="mt-2 block h-2.5 w-full max-w-xs overflow-hidden rounded-full border-[2px] border-black bg-white">
                          <span
                            className="block h-full bg-[#4D7CFF]"
                            style={{ width: `${item.score}%` }}
                          />
                        </span>
                      </span>
                      <span className="shrink-0 rounded-lg border-[2.5px] border-black bg-white px-2.5 py-1 text-sm font-extrabold text-black shadow-[3px_3px_0_0_#000]">
                        {item.score}%
                      </span>
                      <span className="grid h-9 w-9 shrink-0 place-items-center rounded-full border-[2.5px] border-black bg-white text-black shadow-[3px_3px_0_0_#000] transition-transform group-hover:translate-x-1">
                        ›
                      </span>
                    </button>
                  </li>
                ))}
              </ul>
            </section>
          </>
        )}
        <p className="mt-10 text-center text-xs font-bold text-black/45">
          Kết quả mang tính tham khảo — hãy khám phá thêm lộ trình học của từng
          nghề nhé.
        </p>
      </div>
    </div>
  );
}
export default RecommendationQuiz;