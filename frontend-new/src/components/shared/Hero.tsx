import React, { useState } from "react";
import { Sparkles, ArrowRight, Star } from "lucide-react";

/**
 * Hero — EDU VN
 * Goal: drive users to the Career Quiz. Neo-Brutalism, playful, energetic.
 * Pure React + Tailwind core utilities only (no next/link, no next/image, no cn helper).
 * Decorations are absolutely positioned, subtle (low opacity / small scale),
 * and never sit on top of the headline or CTA.
 */

const HARD_SHADOW = "4px 4px 0 0 #111111";
const HARD_SHADOW_HOVER = "6px 6px 0 0 #111111";
const HARD_SHADOW_PRESSED = "2px 2px 0 0 #111111";

interface QuizButtonProps {
  href?: string;
}

function QuizButton({ href = "#quiz" }: QuizButtonProps) {
  const [hover, setHover] = useState(false);
  const [active, setActive] = useState(false);

  return (
    <a
      href={href}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => {
        setHover(false);
        setActive(false);
      }}
      onMouseDown={() => setActive(true)}
      onMouseUp={() => setActive(false)}
      className="inline-flex items-center justify-center gap-2 rounded-2xl border-2 border-black bg-orange-400 px-7 py-4 text-base font-extrabold text-black transition-transform duration-150 sm:text-lg"
      style={{
        boxShadow: active ? HARD_SHADOW_PRESSED : hover ? HARD_SHADOW_HOVER : HARD_SHADOW,
        transform: active
          ? "translate(0px, 0px)"
          : hover
          ? "translate(-3px, -3px)"
          : "translate(0px, 0px)",
      }}
    >
      <Sparkles className="h-5 w-5" strokeWidth={2.5} />
      Start Career Quiz
      <ArrowRight className="h-5 w-5" strokeWidth={2.5} />
    </a>
  );
}

/** A gentle dotted curve, drawn with plain SVG — no external assets. */
interface DottedCurveProps {
  className?: string;
}

function DottedCurve({ className = "" }: DottedCurveProps) {
  return (
    <svg
      className={className}
      width="140"
      height="70"
      viewBox="0 0 140 70"
      fill="none"
      aria-hidden="true"
    >
      <path
        d="M2 60C30 10 90 10 138 45"
        stroke="#111111"
        strokeWidth="3"
        strokeLinecap="round"
        strokeDasharray="1 14"
        opacity="0.35"
      />
    </svg>
  );
}

interface HeroProps {
  quizHref?: string;
}

export default function Hero({ quizHref = "#quiz" }: HeroProps) {
  return (
    <section className="relative w-full overflow-hidden bg-white font-sans">
      {/* Decorative layer — subtle, kept behind and around the content, never over it */}
      <div className="pointer-events-none absolute inset-0" aria-hidden="true">
        {/* Yellow star, top-left */}
        <span className="absolute left-[6%] top-[14%] flex h-12 w-12 rotate-[-8deg] items-center justify-center rounded-2xl border-2 border-black bg-amber-300 opacity-90 sm:h-14 sm:w-14">
          <Star className="h-6 w-6 text-black" strokeWidth={2} fill="#111111" />
        </span>

        {/* Blue abstract blob, top-right */}
        <span className="absolute right-[8%] top-[8%] h-16 w-16 rotate-12 rounded-3xl border-2 border-black bg-blue-600 opacity-80 sm:h-20 sm:w-20" />

        {/* Orange dot, right side */}
        <span className="absolute right-[18%] top-[46%] h-5 w-5 rounded-full border-2 border-black bg-orange-400 opacity-80" />

        {/* Small dot, left side */}
        <span className="absolute left-[14%] bottom-[22%] h-4 w-4 rounded-full border-2 border-black bg-blue-600 opacity-60" />

        {/* Dotted curves */}
        <DottedCurve className="absolute left-[2%] bottom-[8%] hidden sm:block" />
        <DottedCurve className="absolute right-[4%] bottom-[4%] hidden rotate-180 sm:block" />

        {/* Tiny sparkle near headline, kept clear of text */}
        <Sparkles
          className="absolute left-[46%] top-[6%] h-5 w-5 rotate-12 text-amber-300 opacity-90 sm:h-6 sm:w-6"
          fill="#FCD34D"
          strokeWidth={1.5}
        />
      </div>

      {/* Content */}
      <div className="relative mx-auto flex max-w-4xl flex-col items-center px-4 py-20 text-center sm:px-6 sm:py-28">
        <span
          className="mb-6 inline-flex items-center gap-2 rounded-2xl border-2 border-black bg-white px-4 py-1.5 text-xs font-extrabold uppercase tracking-wide text-black sm:text-sm"
          style={{ boxShadow: HARD_SHADOW }}
        >
          <Sparkles className="h-4 w-4 text-blue-600" strokeWidth={2.5} />
          Định hướng nghề nghiệp cùng AI
        </span>

        <h1 className="text-4xl font-extrabold leading-[1.1] tracking-tight text-black sm:text-6xl">
          Bạn hợp ngành nào?
          <br />
          Để <span className="text-blue-600">EDU VN</span> giúp bạn tìm ra.
        </h1>

        <p className="mt-6 max-w-xl text-base font-semibold leading-relaxed text-black/70 sm:text-lg">
          Làm bài Career Quiz 5 phút, nhận gợi ý ngành nghề và lộ trình học cá
          nhân hóa dựa trên chính con người bạn — không phải một bài trắc
          nghiệm chung chung nào khác.
        </p>

        <div className="mt-9">
          <QuizButton href={quizHref} />
        </div>

        <p className="mt-4 text-sm font-semibold text-black/60">
          Or explore all careers below.
        </p>
      </div>
    </section>
  );
}
