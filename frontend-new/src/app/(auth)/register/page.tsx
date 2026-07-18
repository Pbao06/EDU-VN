"use client";
import { useState, type FormEvent, type ReactNode } from "react";
import { Eye, EyeOff, Mail, Lock, User, Loader2 } from "lucide-react";
import { AuthLayout } from "@/components/auth/AuthLayout";

export interface RegisterCardProps {
  onSubmit?: (data: {
    fullName: string;
    email: string;
    password: string;
    acceptTerms: boolean;
  }) => void | Promise<void>;
  onGoogleClick?: () => void;
  onFacebookClick?: () => void;
  onSignIn?: () => void;
  loading?: boolean;
  error?: string | null;
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

type Strength = {
  score: 0 | 1 | 2 | 3 | 4;
  label: string;
  color: string;
};

function evaluateStrength(pw: string): Strength {
  // Độ đa dạng ký tự: 0-4 (thường, hoa, số, đặc biệt)
  let variety = 0;
  if (/[a-z]/.test(pw)) variety++;
  if (/[A-Z]/.test(pw)) variety++;
  if (/\d/.test(pw)) variety++;
  if (/[^A-Za-z0-9]/.test(pw)) variety++;

  // Điểm cộng theo độ dài: 0-2 (>=8 ký tự, >=12 ký tự)
  let lengthBonus = 0;
  if (pw.length >= 8) lengthBonus++;
  if (pw.length >= 12) lengthBonus++;

  const raw = pw.length === 0 ? 0 : variety + lengthBonus; // 0-6

  let score: Strength["score"];
  if (raw <= 0) score = 0;
  else if (raw <= 2) score = 1;
  else if (raw === 3) score = 2;
  else if (raw === 4) score = 3;
  else score = 4;

  const map: Strength[] = [
    { score: 0, label: "Chưa nhập", color: "#E5E5E5" },
    { score: 1, label: "Yếu", color: "#FF6B6B" },
    { score: 2, label: "Trung bình", color: "#FF8A3D" },
    { score: 3, label: "Khá", color: "#FFD84D" },
    { score: 4, label: "Mạnh", color: "#7BE495" },
  ];
  return map[score] as Strength;
}

function RegisterCard({
  onSubmit,
  onGoogleClick,
  onFacebookClick,
  onSignIn,
  loading = false,
  error = null,
  className = "",
}: RegisterCardProps) {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const strength = evaluateStrength(password);
  const mismatch = confirm.length > 0 && confirm !== password;

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (loading || mismatch || !acceptTerms) return;
    await onSubmit?.({ fullName, email, password, acceptTerms });
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

      <Field label="Họ và tên" htmlFor="reg-name">
        <div className="relative">
          <User className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
          <input
            id="reg-name"
            type="text"
            required
            autoComplete="name"
            placeholder="Nguyễn Văn A"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            className="h-14 w-full rounded-2xl border-[2.5px] border-black bg-white pl-12 pr-4 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] focus:border-[#4D7CFF]"
          />
        </div>
      </Field>

      <Field label="Email" htmlFor="reg-email">
        <div className="relative">
          <Mail className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
          <input
            id="reg-email"
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

      <Field label="Mật khẩu" htmlFor="reg-password">
        <div className="relative">
          <Lock className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
          <input
            id="reg-password"
            type={showPassword ? "text" : "password"}
            required
            autoComplete="new-password"
            placeholder="Ít nhất 8 ký tự"
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

        {/* Strength meter — 1 thanh liền, tinh gọn hơn thay vì 4 khối vuông rời */}
        <div className="mt-1 flex items-center gap-2">
          <div className="h-2.5 flex-1 overflow-hidden rounded-full border-[2px] border-black bg-white">
            <div
              className="h-full rounded-full transition-all duration-300"
              style={{
                width: `${strength.score * 25}%`,
                backgroundColor: strength.color,
              }}
            />
          </div>
          <span className="w-20 text-right text-[11px] font-extrabold uppercase tracking-wider text-black/70">
            {strength.label}
          </span>
        </div>
      </Field>

      <Field label="Nhập lại mật khẩu" htmlFor="reg-confirm">
        <div className="relative">
          <Lock className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
          <input
            id="reg-confirm"
            type={showConfirm ? "text" : "password"}
            required
            autoComplete="new-password"
            placeholder="Xác nhận mật khẩu"
            value={confirm}
            onChange={(e) => setConfirm(e.target.value)}
            className={`h-14 w-full rounded-2xl border-[2.5px] bg-white pl-12 pr-14 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] ${
              mismatch ? "border-[#FF6B6B] focus:border-[#FF6B6B]" : "border-black focus:border-[#4D7CFF]"
            }`}
          />
          <button
            type="button"
            onClick={() => setShowConfirm((v) => !v)}
            aria-label={showConfirm ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
            className="absolute right-3 top-1/2 flex h-9 w-9 -translate-y-1/2 items-center justify-center rounded-xl border-[2px] border-black bg-[#FFD84D] text-black shadow-[2px_2px_0_0_#000] transition-transform hover:-translate-y-[calc(50%+2px)] active:translate-y-[calc(-50%+1px)] active:shadow-[1px_1px_0_0_#000]"
          >
            {showConfirm ? <EyeOff className="h-4 w-4" strokeWidth={2.5} /> : <Eye className="h-4 w-4" strokeWidth={2.5} />}
          </button>
        </div>
        {mismatch && (
          <p className="text-xs font-bold text-[#D93636]">Mật khẩu nhập lại không khớp</p>
        )}
      </Field>

      <label className="group flex cursor-pointer items-start gap-2.5 select-none">
        <span className="relative mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-lg border-[2.5px] border-black bg-white shadow-[2px_2px_0_0_#000] transition-all group-hover:-translate-y-0.5 group-hover:shadow-[3px_3px_0_0_#000]">
          <input
            type="checkbox"
            checked={acceptTerms}
            onChange={(e) => setAcceptTerms(e.target.checked)}
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
          <span aria-hidden className="absolute inset-0 -z-10 rounded-lg bg-[#7BE495] opacity-0 transition-opacity peer-checked:opacity-100" />
        </span>
        <span className="text-sm font-semibold leading-relaxed text-black/80">
          Tôi đồng ý với{" "}
          <a href="#" className="font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black">
            Điều khoản
          </a>{" "}
          và{" "}
          <a href="#" className="font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black">
            Chính sách bảo mật
          </a>{" "}
          của EDU VN.
        </span>
      </label>

      <button
        type="submit"
        disabled={loading || mismatch || !acceptTerms}
        className="mt-1 inline-flex h-14 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] text-base font-extrabold uppercase tracking-wide text-white shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[3px_3px_0_0_#000] disabled:cursor-not-allowed disabled:opacity-70 disabled:hover:translate-y-0 disabled:hover:shadow-[6px_6px_0_0_#000]"
      >
        {loading ? (
          <>
            <Loader2 className="h-5 w-5 animate-spin" strokeWidth={2.5} />
            Đang tạo tài khoản...
          </>
        ) : (
          <>Tạo tài khoản →</>
        )}
      </button>

      <div className="relative my-1 flex items-center gap-3">
        <span className="h-[2.5px] flex-1 bg-black/15" />
        <span className="text-xs font-extrabold uppercase tracking-widest text-black/50">hoặc đăng ký với</span>
        <span className="h-[2.5px] flex-1 bg-black/15" />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <SocialButton onClick={onGoogleClick} icon={<GoogleIcon />} label="Google" />
        <SocialButton onClick={onFacebookClick} icon={<FacebookIcon />} label="Facebook" />
      </div>

      <p className="mt-2 text-center text-sm font-semibold text-black/70">
        Đã có tài khoản?{" "}
        <a href="/login" className="font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black">
          Đăng nhập
        </a>
      </p>
    </form>
  );
}

function Field({ label, htmlFor, children }: { label: string; htmlFor: string; children: ReactNode }) {
  return (
    <div className="flex flex-col gap-2">
      <label htmlFor={htmlFor} className="text-xs font-extrabold uppercase tracking-widest text-black">
        {label}
      </label>
      {children}
    </div>
  );
}

function SocialButton({ onClick, icon, label }: { onClick?: () => void; icon: ReactNode; label: string }) {
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

export default function RegisterPage() {
  return (
    <AuthLayout
      title="Đăng ký"
      description="Tham gia EDU VN và bắt đầu hành trình của bạn ngay hôm nay!"
    >
      <RegisterCard />
    </AuthLayout>
  );
}

// "use client";
// import { useState, type FormEvent, type ReactNode } from "react";
// import { Eye, EyeOff, Mail, Lock, User, Loader2 } from "lucide-react";
// import { AuthLayout } from "@/components/auth/AuthLayout";

// export interface RegisterCardProps {
//   onSubmit?: (data: {
//     fullName: string;
//     email: string;
//     password: string;
//     acceptTerms: boolean;
//   }) => void | Promise<void>;
//   onGoogleClick?: () => void;
//   onFacebookClick?: () => void;
//   onSignIn?: () => void;
//   loading?: boolean;
//   error?: string | null;
//   className?: string;
// }

// function GoogleIcon() {
//   return (
//     <svg viewBox="0 0 24 24" className="h-5 w-5" aria-hidden>
//       <path fill="#EA4335" d="M12 10.2v3.9h5.5c-.24 1.4-1.7 4.1-5.5 4.1-3.3 0-6-2.7-6-6.1s2.7-6.1 6-6.1c1.9 0 3.1.8 3.8 1.5l2.6-2.5C16.8 3.4 14.6 2.4 12 2.4 6.7 2.4 2.4 6.7 2.4 12s4.3 9.6 9.6 9.6c5.5 0 9.2-3.9 9.2-9.4 0-.6-.1-1.1-.2-1.6H12z"/>
//     </svg>
//   );
// }

// function FacebookIcon() {
//   return (
//     <svg viewBox="0 0 24 24" className="h-5 w-5" aria-hidden>
//       <path fill="#1877F2" d="M22 12a10 10 0 10-11.6 9.9v-7H8v-2.9h2.4V9.8c0-2.4 1.4-3.7 3.6-3.7 1 0 2.1.2 2.1.2v2.3h-1.2c-1.2 0-1.5.7-1.5 1.5v1.8h2.6l-.4 2.9h-2.2v7A10 10 0 0022 12z"/>
//     </svg>
//   );
// }

// type Strength = {
//   score: 0 | 1 | 2 | 3 | 4;
//   label: string;
//   color: string;
// };

// function evaluateStrength(pw: string): Strength {
//   let score = 0;
//   if (pw.length >= 8) score++;
//   if (/[A-Z]/.test(pw) && /[a-z]/.test(pw)) score++;
//   if (/\d/.test(pw)) score++;
//   if (/[^A-Za-z0-9]/.test(pw)) score++;
//   const map: Strength[] = [
//     { score: 0, label: "Chưa nhập", color: "#E5E5E5" },
//     { score: 1, label: "Yếu", color: "#FF6B6B" },
//     { score: 2, label: "Trung bình", color: "#FF8A3D" },
//     { score: 3, label: "Khá", color: "#FFD84D" },
//     { score: 4, label: "Mạnh", color: "#7BE495" },
//   ];
//   return map[score] as Strength;
// }

// function RegisterCard({
//   onSubmit,
//   onGoogleClick,
//   onFacebookClick,
//   onSignIn,
//   loading = false,
//   error = null,
//   className = "",
// }: RegisterCardProps) {
//   const [fullName, setFullName] = useState("");
//   const [email, setEmail] = useState("");
//   const [password, setPassword] = useState("");
//   const [confirm, setConfirm] = useState("");
//   const [acceptTerms, setAcceptTerms] = useState(false);
//   const [showPassword, setShowPassword] = useState(false);
//   const [showConfirm, setShowConfirm] = useState(false);

//   const strength = evaluateStrength(password);
//   const mismatch = confirm.length > 0 && confirm !== password;

//   const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
//     e.preventDefault();
//     if (loading || mismatch || !acceptTerms) return;
//     await onSubmit?.({ fullName, email, password, acceptTerms });
//   };

//   return (
//     <form onSubmit={handleSubmit} className={`flex flex-col gap-5 ${className}`} noValidate>
//       {error && (
//         <div
//           role="alert"
//           className="rounded-2xl border-[2.5px] border-black bg-[#FFB4B4] px-4 py-3 text-sm font-bold text-black shadow-[4px_4px_0_0_#000]"
//         >
//           {error}
//         </div>
//       )}

//       <Field label="Họ và tên" htmlFor="reg-name">
//         <div className="relative">
//           <User className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
//           <input
//             id="reg-name"
//             type="text"
//             required
//             autoComplete="name"
//             placeholder="Nguyễn Văn A"
//             value={fullName}
//             onChange={(e) => setFullName(e.target.value)}
//             className="h-14 w-full rounded-2xl border-[2.5px] border-black bg-white pl-12 pr-4 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] focus:border-[#4D7CFF]"
//           />
//         </div>
//       </Field>

//       <Field label="Email" htmlFor="reg-email">
//         <div className="relative">
//           <Mail className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
//           <input
//             id="reg-email"
//             type="email"
//             required
//             autoComplete="email"
//             placeholder="you@eduvn.com"
//             value={email}
//             onChange={(e) => setEmail(e.target.value)}
//             className="h-14 w-full rounded-2xl border-[2.5px] border-black bg-white pl-12 pr-4 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] focus:border-[#4D7CFF]"
//           />
//         </div>
//       </Field>

//       <Field label="Mật khẩu" htmlFor="reg-password">
//         <div className="relative">
//           <Lock className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
//           <input
//             id="reg-password"
//             type={showPassword ? "text" : "password"}
//             required
//             autoComplete="new-password"
//             placeholder="Ít nhất 8 ký tự"
//             value={password}
//             onChange={(e) => setPassword(e.target.value)}
//             className="h-14 w-full rounded-2xl border-[2.5px] border-black bg-white pl-12 pr-14 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] focus:border-[#4D7CFF]"
//           />
//           <button
//             type="button"
//             onClick={() => setShowPassword((v) => !v)}
//             aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
//             className="absolute right-3 top-1/2 flex h-9 w-9 -translate-y-1/2 items-center justify-center rounded-xl border-[2px] border-black bg-[#FFD84D] text-black shadow-[2px_2px_0_0_#000] transition-transform hover:-translate-y-[calc(50%+2px)] active:translate-y-[calc(-50%+1px)] active:shadow-[1px_1px_0_0_#000]"
//           >
//             {showPassword ? <EyeOff className="h-4 w-4" strokeWidth={2.5} /> : <Eye className="h-4 w-4" strokeWidth={2.5} />}
//           </button>
//         </div>

//         {/* Strength meter — 1 thanh liền, tinh gọn hơn thay vì 4 khối vuông rời */}
//         <div className="mt-1 flex items-center gap-2">
//           <div className="h-2.5 flex-1 overflow-hidden rounded-full border-[2px] border-black bg-white">
//             <div
//               className="h-full rounded-full transition-all duration-300"
//               style={{
//                 width: `${strength.score * 25}%`,
//                 backgroundColor: strength.color,
//               }}
//             />
//           </div>
//           <span className="w-20 text-right text-[11px] font-extrabold uppercase tracking-wider text-black/70">
//             {strength.label}
//           </span>
//         </div>
//       </Field>

//       <Field label="Nhập lại mật khẩu" htmlFor="reg-confirm">
//         <div className="relative">
//           <Lock className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-black/50" strokeWidth={2.5} />
//           <input
//             id="reg-confirm"
//             type={showConfirm ? "text" : "password"}
//             required
//             autoComplete="new-password"
//             placeholder="Xác nhận mật khẩu"
//             value={confirm}
//             onChange={(e) => setConfirm(e.target.value)}
//             className={`h-14 w-full rounded-2xl border-[2.5px] bg-white pl-12 pr-14 text-base font-semibold text-black placeholder:font-medium placeholder:text-black/35 shadow-[4px_4px_0_0_#000] outline-none transition-all focus:-translate-y-0.5 focus:shadow-[6px_6px_0_0_#000] ${
//               mismatch ? "border-[#FF6B6B] focus:border-[#FF6B6B]" : "border-black focus:border-[#4D7CFF]"
//             }`}
//           />
//           <button
//             type="button"
//             onClick={() => setShowConfirm((v) => !v)}
//             aria-label={showConfirm ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
//             className="absolute right-3 top-1/2 flex h-9 w-9 -translate-y-1/2 items-center justify-center rounded-xl border-[2px] border-black bg-[#FFD84D] text-black shadow-[2px_2px_0_0_#000] transition-transform hover:-translate-y-[calc(50%+2px)] active:translate-y-[calc(-50%+1px)] active:shadow-[1px_1px_0_0_#000]"
//           >
//             {showConfirm ? <EyeOff className="h-4 w-4" strokeWidth={2.5} /> : <Eye className="h-4 w-4" strokeWidth={2.5} />}
//           </button>
//         </div>
//         {mismatch && (
//           <p className="text-xs font-bold text-[#D93636]">Mật khẩu nhập lại không khớp</p>
//         )}
//       </Field>

//       <label className="group flex cursor-pointer items-start gap-2.5 select-none">
//         <span className="relative mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-lg border-[2.5px] border-black bg-white shadow-[2px_2px_0_0_#000] transition-all group-hover:-translate-y-0.5 group-hover:shadow-[3px_3px_0_0_#000]">
//           <input
//             type="checkbox"
//             checked={acceptTerms}
//             onChange={(e) => setAcceptTerms(e.target.checked)}
//             className="peer absolute inset-0 cursor-pointer opacity-0"
//           />
//           <svg
//             viewBox="0 0 24 24"
//             className="h-4 w-4 scale-0 text-black transition-transform peer-checked:scale-100"
//             fill="none"
//             stroke="currentColor"
//             strokeWidth="4"
//             strokeLinecap="round"
//             strokeLinejoin="round"
//             aria-hidden
//           >
//             <polyline points="4 12 10 18 20 6" />
//           </svg>
//           <span aria-hidden className="absolute inset-0 -z-10 rounded-lg bg-[#7BE495] opacity-0 transition-opacity peer-checked:opacity-100" />
//         </span>
//         <span className="text-sm font-semibold leading-relaxed text-black/80">
//           Tôi đồng ý với{" "}
//           <a href="#" className="font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black">
//             Điều khoản
//           </a>{" "}
//           và{" "}
//           <a href="#" className="font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black">
//             Chính sách bảo mật
//           </a>{" "}
//           của EDU VN.
//         </span>
//       </label>

//       <button
//         type="submit"
//         disabled={loading || mismatch || !acceptTerms}
//         className="mt-1 inline-flex h-14 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] text-base font-extrabold uppercase tracking-wide text-white shadow-[6px_6px_0_0_#000] transition-all hover:-translate-y-0.5 hover:shadow-[8px_8px_0_0_#000] active:translate-x-[2px] active:translate-y-[2px] active:shadow-[3px_3px_0_0_#000] disabled:cursor-not-allowed disabled:opacity-70 disabled:hover:translate-y-0 disabled:hover:shadow-[6px_6px_0_0_#000]"
//       >
//         {loading ? (
//           <>
//             <Loader2 className="h-5 w-5 animate-spin" strokeWidth={2.5} />
//             Đang tạo tài khoản...
//           </>
//         ) : (
//           <>Tạo tài khoản →</>
//         )}
//       </button>

//       <div className="relative my-1 flex items-center gap-3">
//         <span className="h-[2.5px] flex-1 bg-black/15" />
//         <span className="text-xs font-extrabold uppercase tracking-widest text-black/50">hoặc đăng ký với</span>
//         <span className="h-[2.5px] flex-1 bg-black/15" />
//       </div>

//       <div className="grid grid-cols-2 gap-3">
//         <SocialButton onClick={onGoogleClick} icon={<GoogleIcon />} label="Google" />
//         <SocialButton onClick={onFacebookClick} icon={<FacebookIcon />} label="Facebook" />
//       </div>

//       <p className="mt-2 text-center text-sm font-semibold text-black/70">
//         Đã có tài khoản?{" "}
//         <a href="/login" className="font-extrabold text-[#4D7CFF] underline decoration-[2.5px] underline-offset-4 hover:text-black">
//           Đăng nhập
//         </a>
//       </p>
//     </form>
//   );
// }

// function Field({ label, htmlFor, children }: { label: string; htmlFor: string; children: ReactNode }) {
//   return (
//     <div className="flex flex-col gap-2">
//       <label htmlFor={htmlFor} className="text-xs font-extrabold uppercase tracking-widest text-black">
//         {label}
//       </label>
//       {children}
//     </div>
//   );
// }

// function SocialButton({ onClick, icon, label }: { onClick?: () => void; icon: ReactNode; label: string }) {
//   return (
//     <button
//       type="button"
//       onClick={onClick}
//       className="inline-flex h-12 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black bg-white text-sm font-extrabold text-black shadow-[4px_4px_0_0_#000] transition-all hover:-translate-y-0.5 hover:bg-[#F5F5F5] hover:shadow-[6px_6px_0_0_#000] active:translate-x-[1px] active:translate-y-[1px] active:shadow-[2px_2px_0_0_#000]"
//     >
//       {icon}
//       {label}
//     </button>
//   );
// }

// export default function RegisterPage() {
//   return (
//     <AuthLayout
//       title="Đăng ký"
//       description="Tham gia EDU VN và bắt đầu hành trình của bạn ngay hôm nay!"
//     >
//       <RegisterCard />
//     </AuthLayout>
//   );
// }

