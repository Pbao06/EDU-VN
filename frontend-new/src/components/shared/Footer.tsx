import React, { useState } from "react";
import { Compass, Sparkles, Facebook, Instagram, Youtube, Mail, ArrowUpRight } from "lucide-react";

/**
 * Footer — EDU VN
 * Neo-Brutalism / playful startup style. Matches Navbar tokens.
 * Pure React + Tailwind core utilities only (no next/link, no next/image, no cn helper).
 */

const HARD_SHADOW = "3px 3px 0 0 #111111";
const HARD_SHADOW_HOVER = "5px 5px 0 0 #111111";
const HARD_SHADOW_PRESSED = "1px 1px 0 0 #111111";

const FOOTER_COLUMNS = [
  {
    title: "Khám phá",
    links: [
      { label: "Ngành nghề", href: "#careers" },
      { label: "Lộ trình học", href: "#learning-paths" },
      { label: "Career Quiz", href: "#quiz" },
    ],
  },
  {
    title: "Về EDU VN",
    links: [
      { label: "Câu chuyện của chúng mình", href: "#about" },
      { label: "Đội ngũ", href: "#team" },
      { label: "Tuyển dụng", href: "#careers-internal" },
    ],
  },
  {
    title: "Hỗ trợ",
    links: [
      { label: "Câu hỏi thường gặp", href: "#faq" },
      { label: "Liên hệ", href: "#contact" },
      { label: "Điều khoản sử dụng", href: "#terms" },
    ],
  },
];

const SOCIAL_LINKS = [
  { label: "Facebook", href: "#", Icon: Facebook },
  { label: "Instagram", href: "#", Icon: Instagram },
  { label: "Youtube", href: "#", Icon: Youtube },
];

function IconBadge({ href, label, Icon }) {
  const [hover, setHover] = useState(false);
  return (
    <a
      href={href}
      aria-label={label}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
      className="flex h-11 w-11 items-center justify-center rounded-2xl border-2 border-black bg-white text-black transition-transform duration-150 focus:outline-none focus:ring-2 focus:ring-blue-600"
      style={{
        boxShadow: hover ? HARD_SHADOW_HOVER : HARD_SHADOW,
        transform: hover ? "translate(-2px, -2px)" : "translate(0px, 0px)",
      }}
    >
      <Icon className="h-5 w-5" strokeWidth={2.25} />
    </a>
  );
}

function QuizCta({ href = "#quiz" }) {
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
      className="inline-flex items-center justify-center gap-2 rounded-2xl border-2 border-black bg-orange-400 px-5 py-3 text-sm font-extrabold text-black transition-transform duration-150"
      style={{
        boxShadow: active ? HARD_SHADOW_PRESSED : hover ? HARD_SHADOW_HOVER : HARD_SHADOW,
        transform: active
          ? "translate(0px, 0px)"
          : hover
          ? "translate(-2px, -2px)"
          : "translate(0px, 0px)",
      }}
    >
      <Sparkles className="h-4 w-4" strokeWidth={2.5} />
      Làm bài Quiz ngay
      <ArrowUpRight className="h-4 w-4" strokeWidth={2.5} />
    </a>
  );
}

export default function Footer({
  logoHref = "#",
  columns = FOOTER_COLUMNS,
  socialLinks = SOCIAL_LINKS,
  quizHref = "#quiz",
  contactEmail = "hello@eduvn.io",
}) {
  const [logoHover, setLogoHover] = useState(false);

  return (
    <footer className="w-full border-t-2 border-black bg-white font-sans">
      {/* CTA strip — reinforces the homepage's #1 goal even at the bottom of the page */}
      <div className="mx-auto max-w-6xl px-4 pt-10 sm:px-6">
        <div
          className="flex flex-col items-start justify-between gap-5 rounded-3xl border-2 border-black bg-amber-300/60 p-6 sm:flex-row sm:items-center sm:p-8"
          style={{ boxShadow: HARD_SHADOW }}
        >
          <div>
            <p className="text-lg font-extrabold text-black sm:text-xl">
              Chưa biết mình hợp ngành nào?
            </p>
            <p className="mt-1 text-sm font-semibold text-black/70">
              Làm bài Career Quiz 5 phút để nhận gợi ý ngành nghề phù hợp với bạn.
            </p>
          </div>
          <QuizCta href={quizHref} />
        </div>
      </div>

      {/* Main footer content */}
      <div className="mx-auto max-w-6xl px-4 py-12 sm:px-6">
        <div className="grid grid-cols-1 gap-10 sm:grid-cols-2 lg:grid-cols-[1.3fr_1fr_1fr_1fr]">
          {/* Brand column */}
          <div>
            <a
              href={logoHref}
              onMouseEnter={() => setLogoHover(true)}
              onMouseLeave={() => setLogoHover(false)}
              className="flex items-center gap-2 rounded-2xl focus:outline-none focus:ring-2 focus:ring-blue-600"
            >
              <span
                className="relative flex h-11 w-11 items-center justify-center rounded-2xl border-2 border-black bg-blue-600 transition-transform duration-150"
                style={{
                  boxShadow: logoHover ? HARD_SHADOW_HOVER : HARD_SHADOW,
                  transform: logoHover ? "translate(-2px, -2px)" : "translate(0px, 0px)",
                }}
              >
                <Compass className="h-6 w-6 text-white" strokeWidth={2.5} />
                <Sparkles
                  className="absolute -right-2 -top-2 h-4 w-4 rotate-12 text-amber-300"
                  fill="#FCD34D"
                  strokeWidth={1.5}
                />
              </span>
              <span className="text-xl font-extrabold tracking-tight text-black">
                EDU<span className="text-blue-600">VN</span>
              </span>
            </a>
            <p className="mt-4 max-w-xs text-sm font-semibold leading-relaxed text-black/70">
              Nền tảng định hướng nghề nghiệp và học tập cá nhân hóa, giúp bạn tìm ra
              con đường phù hợp với chính mình.
            </p>

            <div className="mt-5 flex items-center gap-3">
              {socialLinks.map((s) => (
                <IconBadge key={s.label} href={s.href} label={s.label} Icon={s.Icon} />
              ))}
            </div>
          </div>

          {/* Link columns */}
          {columns.map((col) => (
            <div key={col.title}>
              <p className="text-sm font-extrabold uppercase tracking-wide text-black">
                {col.title}
              </p>
              <ul className="mt-4 flex flex-col gap-3">
                {col.links.map((link) => (
                  <li key={link.href}>
                    <a
                      href={link.href}
                      className="text-sm font-semibold text-black/70 transition-colors hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-600 rounded"
                    >
                      {link.label}
                    </a>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>
      </div>

      {/* Bottom bar */}
      <div className="border-t-2 border-black">
        <div className="mx-auto flex max-w-6xl flex-col-reverse items-center justify-between gap-3 px-4 py-5 text-sm font-semibold text-black/70 sm:flex-row sm:px-6">
          <p>© {new Date().getFullYear()} EDU VN. Đồng hành cùng bạn chọn đúng ngành.</p>
          <a
            href={`mailto:${contactEmail}`}
            className="flex items-center gap-1.5 text-black hover:text-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-600 rounded"
          >
            <Mail className="h-4 w-4" strokeWidth={2.25} />
            {contactEmail}
          </a>
        </div>
      </div>
    </footer>
  );
}