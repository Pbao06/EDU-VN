"use client";
import { useState, type FormEvent, type ReactNode } from "react";
import { Eye, EyeOff, Mail, Lock, Loader2 } from "lucide-react";
import { AuthLayout } from "@/components/auth/AuthLayout";
import { useAuth } from "@/hooks/auth/userAuth"; 
export interface LoginCardProps {
  onSubmit?: (data: { email: string; password: string; remember: boolean }) => void | Promise<void>;
  onGoogleClick?: () => void;
  onFacebookClick?: () => void;
  onForgotPassword?: () => void;
  onSignUp?: () => void;
  loading?: boolean;
  error?: string | null;
  defaultEmail?: string;
  className?: string;
}

function GoogleIcon() {
  return (
    <svg viewBox="0 0 24 24" className="h-5 w-5" aria-hidden>
      <path fill="#EA4335" d="M12 10.2v3.9h5.5c-.24 1.4-1.7 4.1-5.5 4.1-3.3 0-6-2.7-6-6.1s2.7-6.1 6-6.1c1.9 0 3.1.8 3.8 1.5l2.6-2.5C16.8 3.4 14.6 2.4 12 2.4 6.7 2.4 2.4 6.7 2.4 12s4.3 9.6 9.6 9.6c5.5 0 9.2-3.9 9.2-9.4 0-.6-.1-1.1-.2-1.6H12z"/>
    </svg>
  );
}

function FacebookIcon() {
  return (
    <svg viewBox="0 0 24 24" className="h-5 w-5" aria-hidden>
      <path fill="#1877F2" d="M22 12a10 10 0 10-11.6 9.9v-7H8v-2.9h2.4V9.8c0-2.4 1.4-3.7 3.6-3.7 1 0 2.1.2 2.1.2v2.3h-1.2c-1.2 0-1.5.7-1.5 1.5v1.8h2.6l-.4 2.9h-2.2v7A10 10 0 0022 12z"/>
    </svg>
  );
}

function LoginCard({
  onSubmit,
  onGoogleClick,
  onFacebookClick,
  onForgotPassword,
  onSignUp,
  loading = false,
  error = null,
  defaultEmail = "",
  className = "",
}: LoginCardProps) {
  const [email, setEmail] = useState(defaultEmail);
  const [password, setPassword] = useState("");
  const [remember, setRemember] = useState(true);
  const [showPassword, setShowPassword] = useState(false);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (loading) return;
    await onSubmit?.({ email, password, remember });
  };

  return (
    <form onSubmit={handleSubmit} className={`flex flex-col gap-5 ${className}`} noValidate>
      {error && (
        <div
          role="alert"
          className="rounded-2xl border-[2.5px] border-black bg-[#FFB4B4] px-4 py-3 text-sm font-bold text-black shadow-[4px_4px_0_0_#000]"
        >
          {error}
        </div>
      )}

      <Field label="Email" htmlFor="login-email">
        <div className="relative">
          <Mail
            className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50"
            strokeWidth={2.5}
          />
          <input
            id="login-email"
            type="email"
            required
            autoComplete="email"
            placeholder="you@eduvn.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="h-14 w-full rounded-2xl border-[2.5px] border-black bg-white pl-12 pr-4 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] focus:border-[#4D7CFF]"
          />
        </div>
      </Field>

      <Field label="Mật khẩu" htmlFor="login-password">
        <div className="relative">
          <Lock
            className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50"
            strokeWidth={2.5}
          />
          <input
            id="login-password"
            type={showPassword ? "text" : "password"}
            required
            autoComplete="current-password"
            placeholder="••••••••"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="h-14 w-full rounded-2xl border-[2.5px] border-black bg-white pl-12 pr-14 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] focus:border-[#4D7CFF]"
          />
          <button
            type="button"
            onClick={() => setShowPassword((v) => !v)}
            aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
            className="absolute right-3 top-1/2 flex h-9 w-9 -translate-y-1/2 items-center justify-center rounded-xl border-[2px] border-black bg-[#FFD84D] text-black shadow-[2px_2px_0_0_#000] transition-transform hover:-translate-y-[calc(50%+2px)] active:translate-y-[calc(-50%+1px)] active:shadow-[1px_1px_0_0_#000]"
          >
            {showPassword ? <EyeOff className="h-4 w-4" strokeWidth={2.5} /> : <Eye className="h-4 w-4" strokeWidth={2.5} />}
          </button>
        </div>
      </Field>

      <div className="flex items-center justify-between gap-3">
        <label className="group flex cursor-pointer items-center gap-2.5 select-none">
          <span className="relative flex h-6 w-6 items-center justify-center rounded-lg border-[2.5px] border-black bg-white shadow-[2px_2px_0_0_#000] transition-all group-hover:-translate-y-0.5 group-hover:shadow-[3px_3px_0_0_#000]">
            <input
              type="checkbox"
              checked={remember}
              onChange={(e) => setRemember(e.target.checked)}
              className="peer absolute inset-0 cursor-pointer opacity-0"
            />
            <svg
              viewBox="0 0 24 24"
              className="h-4 w-4 scale-0 text-black transition-transform peer-checked:scale-100"
              fill="none"
              stroke="currentColor"
              strokeWidth="4"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden
            >
              <polyline points="4 12 10 18 20 6" />
            </svg>
            <span
              aria-hidden
              className="absolute inset-0 -z-10 rounded-lg bg-[#7BE495] opacity-0 transition-opacity peer-checked:opacity-100"
            />
          </span>
          <span className="text-sm font-bold text-black">Ghi nhớ tôi</span>
        </label>

        <button
          type="button"
          className="text-sm font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black"
        >
          Quên mật khẩu?
        </button>
      </div>

      <button
        type="submit"
        disabled={loading}
        className="mt-1 inline-flex h-14 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] text-base font-extrabold uppercase tracking-wide text-white shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[3px_3px_0_0_#000] disabled:cursor-not-allowed disabled:opacity-70 disabled:hover:translate-y-0 disabled:hover:shadow-[6px_6px_0_0_#000]"
      >
        {loading ? (
          <>
            <Loader2 className="h-5 w-5 animate-spin" strokeWidth={2.5} />
            Đang đăng nhập...
          </>
        ) : (
          <>Đăng nhập →</>
        )}
      </button>

      <div className="relative my-1 flex items-center gap-3">
        <span className="h-[2.5px] flex-1 bg-black/15" />
        <span className="text-xs font-extrabold uppercase tracking-widest text-black/50">
          hoặc
        </span>
        <span className="h-[2.5px] flex-1 bg-black/15" />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <SocialButton onClick={onGoogleClick} icon={<GoogleIcon />} label="Google" />
        <SocialButton onClick={onFacebookClick} icon={<FacebookIcon />} label="Facebook" />
      </div>

      <p className="mt-2 text-center text-sm font-semibold text-black/70">
        Chưa có tài khoản?{" "}
        <a href="/register" className="font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black">
          Đăng ký ngay
        </a>
      </p>
    </form>
  );
}

function Field({
  label,
  htmlFor,
  children,
}: {
  label: string;
  htmlFor: string;
  children: ReactNode;
}) {
  return (
    <div className="flex flex-col gap-2">
      <label
        htmlFor={htmlFor}
        className="text-xs font-extrabold uppercase tracking-widest text-black"
      >
        {label}
      </label>
      {children}
    </div>
  );
}

function SocialButton({
  onClick,
  icon,
  label,
}: {
  onClick?: () => void;
  icon: ReactNode;
  label: string;
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="inline-flex h-12 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-white text-sm font-extrabold text-black shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:bg-[#F5F5F5] hover:shadow-[6px_6px_0_0_#000] active:translate-x-[1px] active:translate-y-[1px] active:shadow-[2px_2px_0_0_#000]"
    >
      {icon}
      {label}
    </button>
  );
}

// Đây là Page Component chính mà Next.js cần
export default function LoginPage() {
  const { login, loading, error } = useAuth();

  const handleLoginSubmit = async (data: { email: string; password: string; remember: boolean }) => {
    try {
      await login({
        email: data.email,
        password: data.password,
      });
    } catch (err) {
      // Lỗi đã được useAuth lưu trữ và hook tự cập nhật state 'error'
    }
  };

  return (
    <AuthLayout
      title="Đăng nhập"
      description="Chào mừng bạn quay trở lại với EDU VN!"
    >
      <LoginCard 
        onSubmit={handleLoginSubmit} 
        loading={loading} 
        error={error} 
      />
    </AuthLayout>
  );
}
