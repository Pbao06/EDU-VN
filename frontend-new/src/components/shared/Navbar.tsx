"use client";
import React, { useEffect, useRef, useState } from "react";
import { Menu, X, Sparkles, Compass, User, LogOut, ChevronDown } from "lucide-react";
import { useAuth } from "@/hooks/auth/userAuth";

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
  { label: "Ngành nghề", href: "/home#careers" },
  { label: "Lộ trình học", href: "/home#learning-paths" },
  { label: "Về EDU VN", href: "/home#about" },
];

interface ShadowLinkProps {
  href?: string;
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

/**
 * UserMenu — Neo-Brutalism dropdown
 * Trigger = user badge. Dropdown reveals "Trang cá nhân" (Profile) + "Đăng xuất".
 * Animated open/close (scale + fade + slight slide), closes on outside click / Escape.
 */
interface UserMenuProps {
  fullName?: string;
  profileHref?: string;
  onLogout: () => void;
}

function UserMenu({ fullName, profileHref = "/profile", onLogout }: UserMenuProps) {
  const [open, setOpen] = useState(false);
  const [hover, setHover] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    function handleEscape(e: KeyboardEvent) {
      if (e.key === "Escape") setOpen(false);
    }
    document.addEventListener("mousedown", handleClickOutside);
    document.addEventListener("keydown", handleEscape);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
      document.removeEventListener("keydown", handleEscape);
    };
  }, []);

  return (
    <div className="relative" ref={containerRef}>
      <button
        type="button"
        onClick={() => setOpen((v) => !v)}
        onMouseEnter={() => setHover(true)}
        onMouseLeave={() => setHover(false)}
        aria-haspopup="menu"
        aria-expanded={open}
        className="flex items-center gap-2 rounded-2xl border-2 border-black bg-white px-4 py-2 text-sm font-extrabold text-black transition-transform duration-150 focus:outline-none focus:ring-2 focus:ring-blue-600"
        style={{
          boxShadow: open ? HARD_SHADOW_PRESSED : hover ? HARD_SHADOW_HOVER : HARD_SHADOW,
          transform: !open && hover ? "translate(-2px, -2px)" : "translate(0px, 0px)",
        }}
      >
        <User className="h-4 w-4 text-blue-600" />
        {fullName}
        <ChevronDown
          className={`h-4 w-4 transition-transform duration-200 ${open ? "rotate-180" : "rotate-0"}`}
          strokeWidth={2.5}
        />
      </button>

      {/* Dropdown panel */}
      <div
        role="menu"
        className={`absolute right-0 top-[calc(100%+10px)] z-20 w-52 origin-top-right rounded-2xl border-2 border-black bg-white p-1.5 transition-all duration-200 ease-out ${
          open
            ? "pointer-events-auto translate-y-0 scale-100 opacity-100"
            : "pointer-events-none -translate-y-1 scale-95 opacity-0"
        }`}
        style={{ boxShadow: HARD_SHADOW }}
      >
        <a
          href={profileHref}
          role="menuitem"
          onClick={() => setOpen(false)}
          className="flex items-center gap-2 rounded-xl px-3 py-2.5 text-sm font-bold text-black transition-colors hover:bg-amber-300/40 focus:outline-none focus:ring-2 focus:ring-blue-600"
        >
          <User className="h-4 w-4 text-blue-600" />
          Trang cá nhân
        </a>
        <div className="my-1 h-0.5 w-full bg-black/10" />
        <button
          type="button"
          role="menuitem"
          onClick={() => {
            setOpen(false);
            onLogout();
          }}
          className="flex w-full items-center gap-2 rounded-xl px-3 py-2.5 text-left text-sm font-bold text-black transition-colors hover:bg-amber-300/40 focus:outline-none focus:ring-2 focus:ring-blue-600"
        >
          <LogOut className="h-4 w-4" />
          Đăng xuất
        </button>
      </div>
    </div>
  );
}

interface NavbarProps {
  logoHref?: string;
  navItems?: NavItem[];
  browseCareersHref?: string;
  quizHref?: string;
  profileHref?: string;
}

export default function Navbar({
  logoHref = "/home",
  navItems = DEFAULT_NAV_ITEMS,
  browseCareersHref = "#careers",
  quizHref = "#quiz",
  profileHref = "/profile",
}: NavbarProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [logoHover, setLogoHover] = useState(false);
  const { user, isAuthenticated, logout } = useAuth();

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
            {isAuthenticated ? (
              <UserMenu fullName={user?.fullName} profileHref={profileHref} onLogout={logout} />
            ) : (
              <>
                <ShadowLink href="/register">Đăng kí</ShadowLink>
                <ShadowLink href="/login" variant="primary">
                  <Sparkles className="h-4 w-4" strokeWidth={2.5} />
                  Đăng Nhập
                </ShadowLink>
              </>
            )}
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
            {isAuthenticated ? (
              <>
                <div className="flex w-full items-center justify-center gap-2 rounded-2xl border-2 border-black bg-white px-3 py-3 text-base font-extrabold shadow-[3px_3px_0_0_#111111]">
                  <User className="h-5 w-5 text-blue-600" />
                  {user?.fullName}
                </div>
                <ShadowLink
                  href={profileHref}
                  variant="outline"
                  className="w-full"
                  onClick={() => setIsOpen(false)}
                >
                  <User className="h-5 w-5" />
                  Trang cá nhân
                </ShadowLink>
                <ShadowLink
                  onClick={() => {
                    logout();
                    setIsOpen(false);
                  }}
                  variant="outline"
                  className="w-full"
                >
                  <LogOut className="h-5 w-5" />
                  Đăng xuất
                </ShadowLink>
              </>
            ) : (
              <>
                <ShadowLink href="/register" className="w-full" onClick={() => setIsOpen(false)}>
                  Đăng kí
                </ShadowLink>
                <ShadowLink
                  href="/login"
                  variant="primary"
                  className="w-full"
                  onClick={() => setIsOpen(false)}
                >
                  <Sparkles className="h-4 w-4" strokeWidth={2.5} />
                  Đăng nhập
                </ShadowLink>
              </>
            )}
          </div>
        </nav>
      </div>
    </header>
  );
}
