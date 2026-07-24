"use client"
import { useState, type FormEvent } from "react";
import { Compass, Check, Rocket, ChevronDown, Loader2 } from "lucide-react";

export interface CareerQuizFormData {
  fullName: string;
  userType: string;
  mainGoal: string;
  field: string;
}

export interface CareerQuizCardProps {
  title: string;
  description: string;
  benefits?: string[];
  userTypeOptions?: string[];
  mainGoalOptions?: string[];
  fieldOptions?: string[];
  onSubmit: (data: CareerQuizFormData) => void;
  loading?: boolean;
  className?: string;
}

const defaultBenefits = [
  "Cá nhân hóa lộ trình học",
  "Đánh giá nghề nghiệp bằng AI",
  "Learning Path rõ ràng",
];

const defaultUserTypeOptions = [
  "Học sinh",
  "Sinh viên",
  "Người đi làm",
  "Phụ huynh",
  "Khác",
];

const defaultMainGoalOptions = [
  "Tìm hiểu nghề nghiệp",
  "Xây dựng lộ trình học",
  "Chuyển ngành",
  "Khám phá bản thân",
];

const defaultFieldOptions = [
  "Công nghệ thông tin",
  "Kinh doanh / Marketing",
  "Y tế",
  "Kỹ thuật",
  "Nghệ thuật / Sáng tạo",
  "Khoa học xã hội",
  "Khác",
];

function EduVnLogo() {
  return (
    <a
      href="/home"
      className="group inline-flex items-center gap-3 focus:outline-none"
      aria-label="EDU VN - Trang chủ"
    >
      <span className="relative">
        <span
          aria-hidden
          className="absolute -right-1.5 -top-1.5 z-10 text-edu-yellow"
        >
          <svg
            viewBox="0 0 24 24"
            fill="currentColor"
            className="h-4 w-4 drop-shadow-[1px_1px_0_var(--edu-black)]"
          >
            <path d="M12 2l2.4 6.4L21 9.2l-5 4.3L17.6 20 12 16.7 6.4 20 8 13.5 3 9.2l6.6-.8L12 2z" />
          </svg>
        </span>
        <span className="flex h-12 w-12 items-center justify-center rounded-2xl border-[2.5px] border-edu-black bg-edu-blue text-edu-white shadow-hard-sm transition-transform duration-200 group-hover:-rotate-6 group-active:translate-x-[2px] group-active:translate-y-[2px] group-active:shadow-[2px_2px_0_0_var(--edu-black)]">
          <Compass className="h-6 w-6" strokeWidth={2.5} />
        </span>
      </span>
      <span className="text-2xl font-extrabold tracking-tight text-edu-black">
        EDU<span className="text-edu-blue">VN</span>
      </span>
    </a>
  );
}

function StarSvg({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="currentColor"
      className={className}
      aria-hidden="true"
    >
      <path d="M12 2l2.4 6.4L21 9.2l-5 4.3L17.6 20 12 16.7 6.4 20 8 13.5 3 9.2l6.6-.8L12 2z" />
    </svg>
  );
}

function HeroIllustration() {
  return (
    <div className="relative mt-8 hidden h-44 w-44 sm:block">
      <span className="absolute -right-2 -top-3 z-20 text-edu-yellow">
        <StarSvg className="h-8 w-8 drop-shadow-[2px_2px_0_var(--edu-black)]" />
      </span>
      <span className="absolute -bottom-3 -left-3 z-10 h-14 w-14 rounded-full border-[2.5px] border-edu-black bg-edu-green shadow-hard-sm" />
      <span className="absolute -bottom-4 right-4 z-10 h-12 w-12 rotate-12 rounded-2xl border-[2.5px] border-edu-black bg-edu-orange shadow-hard-sm" />
      <span className="absolute inset-0 z-10 flex items-center justify-center rounded-card border-[2.5px] border-edu-black bg-edu-blue shadow-hard-md">
        <Rocket className="h-20 w-20 text-edu-white" strokeWidth={2} />
      </span>
    </div>
  );
}

function BenefitItem({ text }: { text: string }) {
  return (
    <li className="flex items-center gap-2 text-sm font-semibold text-edu-black/80 sm:text-base">
      <span className="flex h-5 w-5 shrink-0 items-center justify-center rounded-full border-[1.5px] border-edu-black bg-edu-green text-edu-black">
        <Check className="h-3 w-3" strokeWidth={3} />
      </span>
      {text}
    </li>
  );
}

function InputField({
  label,
  value,
  onChange,
  placeholder,
  type = "text",
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  placeholder: string;
  type?: string;
}) {
  return (
    <label className="flex flex-col gap-1.5">
      <span className="text-xs font-extrabold uppercase tracking-wider text-edu-black/60">
        {label}
      </span>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="w-full rounded-2xl border-[2.5px] border-edu-black bg-edu-white px-4 py-3 text-sm font-bold text-edu-black placeholder:text-edu-black/30 shadow-[3px_3px_0_0_var(--edu-black)] outline-none transition-transform focus:ring-2 focus:ring-edu-blue active:translate-x-[1px] active:translate-y-[1px] active:shadow-[1px_1px_0_0_var(--edu-black)]"
      />
    </label>
  );
}

function SelectField({
  label,
  value,
  options,
  onChange,
  placeholder,
}: {
  label: string;
  value: string;
  options: string[];
  onChange: (value: string) => void;
  placeholder: string;
}) {
  return (
    <label className="flex flex-col gap-1.5">
      <span className="text-xs font-extrabold uppercase tracking-wider text-edu-black/60">
        {label}
      </span>
      <div className="relative">
        <select
          value={value}
          onChange={(e) => onChange(e.target.value)}
          className="w-full appearance-none rounded-2xl border-[2.5px] border-edu-black bg-edu-white px-4 py-3 pr-10 text-sm font-bold text-edu-black shadow-[3px_3px_0_0_var(--edu-black)] outline-none transition-transform focus:ring-2 focus:ring-edu-blue active:translate-x-[1px] active:translate-y-[1px] active:shadow-[1px_1px_0_0_var(--edu-black)]"
        >
          <option value="" disabled>
            {placeholder}
          </option>
          {options.map((opt) => (
            <option key={opt} value={opt}>
              {opt}
            </option>
          ))}
        </select>
        <ChevronDown className="pointer-events-none absolute right-3 top-1/2 h-5 w-5 -translate-y-1/2 text-edu-black" />
      </div>
    </label>
  );
}

export function CareerQuizCard({
  title,
  description,
  benefits = defaultBenefits,
  userTypeOptions = defaultUserTypeOptions,
  mainGoalOptions = defaultMainGoalOptions,
  fieldOptions = defaultFieldOptions,
  onSubmit,
  loading = false,
  className = "",
}: CareerQuizCardProps) {
  const [form, setForm] = useState<CareerQuizFormData>({
    fullName: "",
    userType: "",
    mainGoal: "",
    field: "",
  });
  const [error, setError] = useState<string | null>(null);

  const updateField = <K extends keyof CareerQuizFormData>(
    key: K,
    value: CareerQuizFormData[K]
  ) => {
    setForm((prev) => ({ ...prev, [key]: value }));
    if (error) setError(null);
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    if (!form.fullName.trim()) {
      setError("Vui lòng nhập họ tên.");
      return;
    }
    if (!form.userType || !form.mainGoal || !form.field) {
      setError("Vui lòng chọn đầy đủ thông tin.");
      return;
    }
    setError(null);
    onSubmit(form);
  };

  return (
    <div
      className={`relative flex min-h-screen items-center justify-center overflow-hidden bg-edu-white px-4 py-12 sm:px-6 lg:px-8 ${className}`}
    >
      {/* Decorative background */}
      <span
        aria-hidden
        className="pointer-events-none absolute -left-16 -top-16 h-56 w-56 rounded-full border-[2.5px] border-edu-black bg-edu-yellow opacity-90 shadow-hard-lg"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute -right-20 top-40 h-40 w-40 rotate-12 rounded-card border-[2.5px] border-edu-black bg-edu-orange shadow-hard-lg"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute bottom-10 left-10 hidden h-24 w-24 rotate-[18deg] rounded-2xl border-[2.5px] border-edu-black bg-edu-green shadow-hard-md md:block"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute inset-0 opacity-[0.10]"
        style={{
          backgroundImage:
            "radial-gradient(circle, var(--edu-black) 1.2px, transparent 1.2px)",
          backgroundSize: "24px 24px",
        }}
      />

      <div className="relative z-10 w-full max-w-6xl">
        <div className="grid grid-cols-1 items-stretch overflow-hidden rounded-card border-[2.5px] border-edu-black bg-edu-white shadow-hard-lg lg:grid-cols-[7fr_3fr]">
          {/* Left column: 70% */}
          <div className="flex flex-col justify-between gap-8 p-8 sm:p-10 lg:p-12">
            <div>
              <EduVnLogo />
              <h1 className="mt-8 text-3xl font-extrabold leading-tight tracking-tight text-edu-black sm:text-4xl lg:text-5xl">
                {title}
              </h1>
              <p className="mt-4 max-w-xl text-base leading-relaxed text-edu-black/70 sm:text-lg">
                {description}
              </p>
              <ul className="mt-6 flex flex-col gap-3">
                {benefits.map((b) => (
                  <BenefitItem key={b} text={b} />
                ))}
              </ul>
            </div>
            <HeroIllustration />
          </div>

          {/* Right column: 30% */}
          <div className="flex flex-col border-t-[2.5px] border-edu-black bg-edu-yellow/20 p-6 sm:p-8 lg:border-t-0 lg:border-l-[2.5px]">
            <div className="relative flex h-full flex-col gap-5 rounded-card border-[2.5px] border-edu-black bg-edu-white p-6 shadow-hard-md">
              <span
                aria-hidden
                className="absolute -top-4 left-6 inline-flex items-center gap-1 rounded-full border-[2.5px] border-edu-black bg-edu-yellow px-3 py-1 text-[11px] font-extrabold uppercase tracking-wider text-edu-black shadow-hard-sm"
              >
                <StarSvg className="h-3 w-3" /> Bắt đầu ngay
              </span>

              <div className="pt-2">
                <h2 className="text-xl font-extrabold text-edu-black">
                  Thông tin của bạn
                </h2>
                <p className="mt-1 text-sm font-semibold text-edu-black/60">
                  Điền nhanh để nhận đề xuất phù hợp.
                </p>
              </div>

              <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <InputField
                  label="Họ và tên"
                  value={form.fullName}
                  onChange={(v) => updateField("fullName", v)}
                  placeholder="Nguyễn Văn A"
                />
                <SelectField
                  label="Bạn là"
                  value={form.userType}
                  options={userTypeOptions}
                  onChange={(v) => updateField("userType", v)}
                  placeholder="Chọn đối tượng"
                />
                <SelectField
                  label="Mục tiêu chính"
                  value={form.mainGoal}
                  options={mainGoalOptions}
                  onChange={(v) => updateField("mainGoal", v)}
                  placeholder="Chọn mục tiêu"
                />
                <SelectField
                  label="Lĩnh vực quan tâm"
                  value={form.field}
                  options={fieldOptions}
                  onChange={(v) => updateField("field", v)}
                  placeholder="Chọn lĩnh vực"
                />

                {error && (
                  <div className="rounded-xl border-[2px] border-edu-black bg-destructive/15 px-3 py-2 text-sm font-bold text-destructive">
                    {error}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={loading}
                  className="mt-2 flex w-full items-center justify-center gap-2 rounded-full border-[2.5px] border-edu-black bg-edu-blue px-6 py-3 text-sm font-extrabold uppercase tracking-wider text-edu-white shadow-[4px_4px_0_0_var(--edu-black)] transition-transform hover:-translate-y-0.5 hover:shadow-[6px_6px_0_0_var(--edu-black)] active:translate-y-0 active:shadow-[2px_2px_0_0_var(--edu-black)] disabled:cursor-not-allowed disabled:opacity-60"
                >
                  {loading ? (
                    <Loader2 className="h-5 w-5 animate-spin" />
                  ) : (
                    <>
                      Bắt đầu
                      <Rocket className="h-4 w-4" />
                    </>
                  )}
                </button>
              </form>

              <p className="mt-auto text-center text-xs font-bold text-edu-black/40">
                Chỉ mất khoảng 5 phút
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default CareerQuizCard;
