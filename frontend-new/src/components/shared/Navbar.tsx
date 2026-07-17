import React, { useState } from "react";
import { Menu, X, Sparkles, Compass } from "lucide-react";

/**
 * Navbar — EDU VN
 * Neo-Brutalism / playful startup style.
 * Pure React + Tailwind core utilities only (no next/link, no next/image, no cn helper).
 *
 * Tokens:
 *  - Primary Blue   -> bg-blue-600
 *  - Accent Yellow  -> bg-amber-300
 *  - Accent Orange  -> bg-orange-400
 *  - Ink (border)   -> black
 *
 * Hard shadow + exact brand hex are applied via inline style since
 * artifacts don't run a Tailwind JIT compiler (no arbitrary bracket values).
 */

const HARD_SHADOW = "3px 3px 0 0 #111111";
const HARD_SHADOW_HOVER = "5px 5px 0 0 #111111";
const HARD_SHADOW_PRESSED = "1px 1px 0 0 #111111";

interface NavItem {
  label: string;
  href: string;
}

const DEFAULT_NAV_ITEMS: NavItem[] = [
  { label: "Ngành nghề", href: "#careers" },
  { label: "Lộ trình học", href: "#learning-paths" },
  { label: "Về EDU VN", href: "#about" },
];

interface ShadowLinkProps {
  href: string;
  children: React.ReactNode;
  className?: string;
  variant?: "primary" | "outline" | "outline-mobile";
  onClick?: () => void;
}

function ShadowLink({ href, children, className = "", variant = "outline", onClick }: ShadowLinkProps) {
  const [hover, setHover] = useState(false);
  const [active, setActive] = useState(false);

  const base =
    variant === "primary"
      ? "bg-orange-400"
      : variant === "outline-mobile"
      ? "bg-white w-full text-center"
      : "bg-white";

  return (
    <a
      href={href}
      onClick={onClick}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => {
        setHover(false);
        setActive(false);
      }}
      onMouseDown={() => setActive(true)}
      onMouseUp={() => setActive(false)}
      className={`inline-flex items-center justify-center gap-2 rounded-2xl border-2 border-black px-4 py-2 text-sm font-extrabold text-black transition-transform duration-150 ${base} ${className}`}
      style={{
        boxShadow: active ? HARD_SHADOW_PRESSED : hover ? HARD_SHADOW_HOVER : HARD_SHADOW,
        transform: active
          ? "translate(0px, 0px)"
          : hover
          ? "translate(-2px, -2px)"
          : "translate(0px, 0px)",
      }}
    >
      {children}
    </a>
  );
}

interface NavbarProps {
  logoHref?: string;
  navItems?: NavItem[];
  browseCareersHref?: string;
  quizHref?: string;
}

export default function Navbar({
  logoHref = "#",
  navItems = DEFAULT_NAV_ITEMS,
  browseCareersHref = "#careers",
  quizHref = "#quiz",
}: NavbarProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [logoHover, setLogoHover] = useState(false);

  return (
    <header className="w-full bg-white font-sans">
      <div className="mx-auto max-w-6xl px-4 sm:px-6">
        <div className="flex h-20 items-center justify-between">
          {/* Logo / Brand */}
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

          {/* Desktop nav links */}
          <nav className="hidden items-center gap-1 lg:flex">
            {navItems.map((item) => (
              <a
                key={item.href}
                href={item.href}
                className="rounded-xl px-4 py-2 text-sm font-bold text-black/80 transition-colors hover:bg-amber-300/40 hover:text-black focus:outline-none focus:ring-2 focus:ring-blue-600"
              >
                {item.label}
              </a>
            ))}
          </nav>

          {/* Desktop actions */}
          <div className="hidden items-center gap-3 lg:flex">
            <ShadowLink href={browseCareersHref}>Khám phá ngành nghề</ShadowLink>
            <ShadowLink href={quizHref} variant="primary">
              <Sparkles className="h-4 w-4" strokeWidth={2.5} />
              Làm bài Quiz
            </ShadowLink>
          </div>

          {/* Mobile toggle */}
          <button
            type="button"
            onClick={() => setIsOpen((v) => !v)}
            aria-expanded={isOpen}
            aria-label={isOpen ? "Đóng menu" : "Mở menu"}
            className="flex h-11 w-11 items-center justify-center rounded-2xl border-2 border-black bg-white transition-transform duration-150 focus:outline-none focus:ring-2 focus:ring-blue-600 lg:hidden"
            style={{ boxShadow: HARD_SHADOW }}
          >
            {isOpen ? <X className="h-5 w-5" strokeWidth={2.5} /> : <Menu className="h-5 w-5" strokeWidth={2.5} />}
          </button>
        </div>
      </div>

      <div className="h-0.5 w-full bg-black" />

      {/* Mobile panel */}
      <div
        className={`overflow-hidden border-b-2 border-black bg-white transition-all duration-300 ease-in-out lg:hidden ${
          isOpen ? "max-h-96 opacity-100" : "max-h-0 opacity-0"
        }`}
      >
        <nav className="flex flex-col gap-2 px-4 pb-5 pt-2 sm:px-6">
          {navItems.map((item) => (
            <a
              key={item.href}
              href={item.href}
              onClick={() => setIsOpen(false)}
              className="rounded-xl px-3 py-3 text-base font-bold text-black hover:bg-amber-300/40 focus:outline-none focus:ring-2 focus:ring-blue-600"
            >
              {item.label}
            </a>
          ))}
          <div className="mt-2 flex flex-col gap-3">
            <ShadowLink href={browseCareersHref} className="w-full" onClick={() => setIsOpen(false)}>
              Khám phá ngành nghề
            </ShadowLink>
            <ShadowLink
              href={quizHref}
              variant="primary"
              className="w-full"
              onClick={() => setIsOpen(false)}
            >
              <Sparkles className="h-4 w-4" strokeWidth={2.5} />
              Làm bài Quiz
            </ShadowLink>
          </div>
        </nav>
      </div>
    </header>
  );
}
