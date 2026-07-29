"use client";
import React, { useEffect, useRef, useState } from "react";
import { Search, ChevronDown, SlidersHorizontal, Check } from "lucide-react";

/**
 * CareerFilter — EDU VN
 * Purpose: let users browse the careers list manually (homepage goal #2).
 * Reads as one unified filter bar. Neo-Brutalism, playful, responsive.
 * Pure React + Tailwind core utilities only. No API calls — all state is local;
 * parent can hook in via the on*Change callbacks.
 *
 * Dropdowns are custom (button + animated panel), matching the Navbar
 * UserMenu style — no native <select> so the panel can carry the same
 * hard-shadow / rounded-2xl / fade-scale language as the rest of the app.
 */

const HARD_SHADOW = "3px 3px 0 0 #111111";
const HARD_SHADOW_HOVER = "5px 5px 0 0 #111111";
const HARD_SHADOW_PRESSED = "1px 1px 0 0 #111111";
const HARD_SHADOW_FOCUS = "4px 4px 0 0 #111111";

const DEFAULT_FIELDS = [
  "Tất cả lĩnh vực",
  "Công nghệ thông tin",
  "Kinh doanh & Marketing",
  "Thiết kế & Sáng tạo",
  "Y tế & Sức khỏe",
  "Giáo dục",
  "Kỹ thuật",
];

const DEFAULT_SALARY_RANGES = [
  "Mọi mức lương",
  "Dưới 10 triệu",
  "10 - 20 triệu",
  "20 - 35 triệu",
  "Trên 35 triệu",
];

const DEFAULT_DIFFICULTIES = ["Mọi độ khó", "Dễ tiếp cận", "Trung bình", "Thử thách cao"];

const DEFAULT_SORT_OPTIONS = [
  "Phổ biến nhất",
  "Lương cao nhất",
  "Mới cập nhật",
  "A - Z",
];

/**
 * FilterDropdown — custom trigger + animated panel.
 * Same visual language as the Navbar UserMenu dropdown:
 * hard shadow, border-2 border-black, rounded-2xl panel,
 * scale + fade + slight slide transition, outside-click / Escape to close.
 */
interface FilterDropdownProps {
  label: string;
  icon: React.ReactNode;
  options: string[];
  value: string;
  onChange: (value: string) => void;
  align?: "left" | "right";
}

function FilterDropdown({ label, icon, options, value, onChange, align = "left" }: FilterDropdownProps) {
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
    <div className="relative min-w-[9.5rem] flex-1 sm:min-w-[10.5rem]" ref={containerRef}>
      <button
        type="button"
        onClick={() => setOpen((v) => !v)}
        onMouseEnter={() => setHover(true)}
        onMouseLeave={() => setHover(false)}
        aria-haspopup="listbox"
        aria-expanded={open}
        aria-label={label}
        className="flex w-full items-center gap-2 rounded-xl border-2 border-black bg-white px-3 py-2.5 text-left transition-transform duration-150 focus:outline-none focus:ring-2 focus:ring-blue-600"
        style={{
          boxShadow: open ? HARD_SHADOW_PRESSED : hover ? HARD_SHADOW_HOVER : HARD_SHADOW,
          transform: !open && hover ? "translate(-1px, -1px)" : "translate(0px, 0px)",
        }}
      >
        {icon}
        <span className="flex-1 truncate text-sm font-bold text-black">{value}</span>
        <ChevronDown
          className={`h-4 w-4 shrink-0 text-black/60 transition-transform duration-200 ${
            open ? "rotate-180" : "rotate-0"
          }`}
          strokeWidth={2.5}
        />
      </button>

      {/* Dropdown panel */}
      <div
        role="listbox"
        aria-label={label}
        className={`absolute top-[calc(100%+10px)] z-20 w-full min-w-[13rem] origin-top rounded-2xl border-2 border-black bg-white p-1.5 transition-all duration-200 ease-out ${
          align === "right" ? "right-0 origin-top-right" : "left-0 origin-top-left"
        } ${
          open
            ? "pointer-events-auto translate-y-0 scale-100 opacity-100"
            : "pointer-events-none -translate-y-1 scale-95 opacity-0"
        }`}
        style={{ boxShadow: HARD_SHADOW }}
      >
        {options.map((opt) => {
          const selected = opt === value;
          return (
            <button
              key={opt}
              type="button"
              role="option"
              aria-selected={selected}
              onClick={() => {
                onChange(opt);
                setOpen(false);
              }}
              className={`flex w-full items-center gap-2 rounded-xl px-3 py-2.5 text-left text-sm font-bold transition-colors focus:outline-none focus:ring-2 focus:ring-blue-600 ${
                selected ? "bg-amber-300/50 text-black" : "text-black hover:bg-amber-300/40"
              }`}
            >
              <Check
                className={`h-4 w-4 shrink-0 text-blue-600 ${selected ? "opacity-100" : "opacity-0"}`}
                strokeWidth={3}
              />
              <span className="truncate">{opt}</span>
            </button>
          );
        })}
      </div>
    </div>
  );
}

interface CareerFilterProps {
  fieldOptions?: string[];
  salaryOptions?: string[];
  difficultyOptions?: string[];
  sortOptions?: string[];
  onSearchChange?: (value: string) => void;
  onFieldChange?: (value: string) => void;
  onSalaryChange?: (value: string) => void;
  onDifficultyChange?: (value: string) => void;
  onSortChange?: (value: string) => void;
}

export default function CareerFilter({
  fieldOptions = DEFAULT_FIELDS,
  salaryOptions = DEFAULT_SALARY_RANGES,
  difficultyOptions = DEFAULT_DIFFICULTIES,
  sortOptions = DEFAULT_SORT_OPTIONS,
  onSearchChange,
  onFieldChange,
  onSalaryChange,
  onDifficultyChange,
  onSortChange,
}: CareerFilterProps) {
  const [search, setSearch] = useState("");
  const [field, setField] = useState(fieldOptions[0]);
  const [salary, setSalary] = useState(salaryOptions[0]);
  const [difficulty, setDifficulty] = useState(difficultyOptions[0]);
  const [sort, setSort] = useState(sortOptions[0]);
  const [searchFocused, setSearchFocused] = useState(false);

  const handleSearch = (value: string) => {
    setSearch(value);
    onSearchChange?.(value);
  };

  return (
    <div
      className="w-full rounded-3xl border-2 border-black bg-white p-3 sm:p-4"
      style={{ boxShadow: HARD_SHADOW }}
    >
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center">
        {/* Search input */}
        <div
          className="flex flex-1 items-center gap-2 rounded-2xl border-2 border-black bg-amber-300/30 px-4 py-3 transition-shadow duration-150"
          style={{ boxShadow: searchFocused ? HARD_SHADOW_FOCUS : "none" }}
        >
          <Search className="h-5 w-5 shrink-0 text-black" strokeWidth={2.5} />
          <input
            type="text"
            value={search}
            onChange={(e) => handleSearch(e.target.value)}
            onFocus={() => setSearchFocused(true)}
            onBlur={() => setSearchFocused(false)}
            placeholder="Tìm ngành nghề, ví dụ: Thiết kế UX/UI..."
            className="w-full bg-transparent text-sm font-bold text-black placeholder:font-semibold placeholder:text-black/40 focus:outline-none"
          />
        </div>

        {/* Divider for large screens */}
        <div className="hidden h-10 w-0.5 shrink-0 bg-black/10 lg:block" />

        {/* Filter dropdowns */}
        <div className="flex flex-wrap gap-2 sm:gap-3">
          <FilterDropdown
            label="Lĩnh vực"
            icon={<SlidersHorizontal className="h-4 w-4 shrink-0 text-blue-600" strokeWidth={2.5} />}
            options={fieldOptions}
            value={field}
            onChange={(v) => {
              setField(v);
              onFieldChange?.(v);
            }}
          />
          <FilterDropdown
            label="Mức lương"
            icon={<span className="text-sm font-extrabold text-blue-600">₫</span>}
            options={salaryOptions}
            value={salary}
            onChange={(v) => {
              setSalary(v);
              onSalaryChange?.(v);
            }}
          />
          <FilterDropdown
            label="Độ khó"
            icon={<span className="text-sm font-extrabold text-blue-600">◆</span>}
            options={difficultyOptions}
            value={difficulty}
            onChange={(v) => {
              setDifficulty(v);
              onDifficultyChange?.(v);
            }}
          />
        </div>

        {/* Sort dropdown, visually set apart on the right */}
        <div className="lg:ml-auto">
          <FilterDropdown
            label="Sắp xếp"
            icon={<span className="text-sm font-extrabold text-orange-500">↕</span>}
            options={sortOptions}
            value={sort}
            onChange={(v) => {
              setSort(v);
              onSortChange?.(v);
            }}
            align="right"
          />
        </div>
      </div>
    </div>
  );
}
// "use client";
// import React, { useState } from "react";
// import { Search, ChevronDown, SlidersHorizontal } from "lucide-react";

// /**
//  * CareerFilter — EDU VN
//  * Purpose: let users browse the careers list manually (homepage goal #2).
//  * Reads as one unified filter bar. Neo-Brutalism, playful, responsive.
//  * Pure React + Tailwind core utilities only. No API calls — all state is local;
//  * parent can hook in via the on*Change callbacks.
//  */

// const HARD_SHADOW = "3px 3px 0 0 #111111";
// const HARD_SHADOW_FOCUS = "4px 4px 0 0 #111111";

// const DEFAULT_FIELDS = [
//   "Tất cả lĩnh vực",
//   "Công nghệ thông tin",
//   "Kinh doanh & Marketing",
//   "Thiết kế & Sáng tạo",
//   "Y tế & Sức khỏe",
//   "Giáo dục",
//   "Kỹ thuật",
// ];

// const DEFAULT_SALARY_RANGES = [
//   "Mọi mức lương",
//   "Dưới 10 triệu",
//   "10 - 20 triệu",
//   "20 - 35 triệu",
//   "Trên 35 triệu",
// ];

// const DEFAULT_DIFFICULTIES = ["Mọi độ khó", "Dễ tiếp cận", "Trung bình", "Thử thách cao"];

// const DEFAULT_SORT_OPTIONS = [
//   "Phổ biến nhất",
//   "Lương cao nhất",
//   "Mới cập nhật",
//   "A - Z",
// ];

// interface FilterSelectProps {
//   label: string;
//   icon: React.ReactNode;
//   options: string[];
//   value: string;
//   onChange: (value: string) => void;
// }

// function FilterSelect({ label, icon, options, value, onChange }: FilterSelectProps) {
//   const [focused, setFocused] = useState(false);

//   return (
//     <div className="relative flex min-w-[9.5rem] flex-1 items-center gap-2 rounded-xl border-2 border-black bg-white px-3 py-2.5 transition-shadow duration-150 sm:min-w-[10.5rem]">
//       {icon}
//       <span className="sr-only">{label}</span>
//       <select
//         value={value}
//         onChange={(e) => onChange(e.target.value)}
//         onFocus={() => setFocused(true)}
//         onBlur={() => setFocused(false)}
//         aria-label={label}
//         className="w-full cursor-pointer appearance-none bg-transparent pr-5 text-sm font-bold text-black focus:outline-none"
//         style={{ boxShadow: focused ? HARD_SHADOW_FOCUS : "none" }}
//       >
//         {options.map((opt) => (
//           <option key={opt} value={opt}>
//             {opt}
//           </option>
//         ))}
//       </select>
//       <ChevronDown
//         className="pointer-events-none absolute right-3 h-4 w-4 text-black/60"
//         strokeWidth={2.5}
//       />
//     </div>
//   );
// }

// interface CareerFilterProps {
//   fieldOptions?: string[];
//   salaryOptions?: string[];
//   difficultyOptions?: string[];
//   sortOptions?: string[];
//   onSearchChange?: (value: string) => void;
//   onFieldChange?: (value: string) => void;
//   onSalaryChange?: (value: string) => void;
//   onDifficultyChange?: (value: string) => void;
//   onSortChange?: (value: string) => void;
// }

// export default function CareerFilter({
//   fieldOptions = DEFAULT_FIELDS,
//   salaryOptions = DEFAULT_SALARY_RANGES,
//   difficultyOptions = DEFAULT_DIFFICULTIES,
//   sortOptions = DEFAULT_SORT_OPTIONS,
//   onSearchChange,
//   onFieldChange,
//   onSalaryChange,
//   onDifficultyChange,
//   onSortChange,
// }: CareerFilterProps) {
//   const [search, setSearch] = useState("");
//   const [field, setField] = useState(fieldOptions[0]);
//   const [salary, setSalary] = useState(salaryOptions[0]);
//   const [difficulty, setDifficulty] = useState(difficultyOptions[0]);
//   const [sort, setSort] = useState(sortOptions[0]);
//   const [searchFocused, setSearchFocused] = useState(false);

//   const handleSearch = (value: string) => {
//     setSearch(value);
//     onSearchChange?.(value);
//   };

//   return (
//     <div
//       className="w-full rounded-3xl border-2 border-black bg-white p-3 sm:p-4"
//       style={{ boxShadow: HARD_SHADOW }}
//     >
//       <div className="flex flex-col gap-3 lg:flex-row lg:items-center">
//         {/* Search input */}
//         <div
//           className="flex flex-1 items-center gap-2 rounded-2xl border-2 border-black bg-amber-300/30 px-4 py-3 transition-shadow duration-150"
//           style={{ boxShadow: searchFocused ? HARD_SHADOW_FOCUS : "none" }}
//         >
//           <Search className="h-5 w-5 shrink-0 text-black" strokeWidth={2.5} />
//           <input
//             type="text"
//             value={search}
//             onChange={(e) => handleSearch(e.target.value)}
//             onFocus={() => setSearchFocused(true)}
//             onBlur={() => setSearchFocused(false)}
//             placeholder="Tìm ngành nghề, ví dụ: Thiết kế UX/UI..."
//             className="w-full bg-transparent text-sm font-bold text-black placeholder:font-semibold placeholder:text-black/40 focus:outline-none"
//           />
//         </div>

//         {/* Divider for large screens */}
//         <div className="hidden h-10 w-0.5 shrink-0 bg-black/10 lg:block" />

//         {/* Filter selects */}
//         <div className="flex flex-wrap gap-2 sm:gap-3">
//           <FilterSelect
//             label="Lĩnh vực"
//             icon={<SlidersHorizontal className="h-4 w-4 shrink-0 text-blue-600" strokeWidth={2.5} />}
//             options={fieldOptions}
//             value={field}
//             onChange={(v) => {
//               setField(v);
//               onFieldChange?.(v);
//             }}
//           />
//           <FilterSelect
//             label="Mức lương"
//             icon={<span className="text-sm font-extrabold text-blue-600">₫</span>}
//             options={salaryOptions}
//             value={salary}
//             onChange={(v) => {
//               setSalary(v);
//               onSalaryChange?.(v);
//             }}
//           />
//           <FilterSelect
//             label="Độ khó"
//             icon={<span className="text-sm font-extrabold text-blue-600">◆</span>}
//             options={difficultyOptions}
//             value={difficulty}
//             onChange={(v) => {
//               setDifficulty(v);
//               onDifficultyChange?.(v);
//             }}
//           />
//         </div>

//         {/* Sort dropdown, visually set apart on the right */}
//         <div className="lg:ml-auto">
//           <FilterSelect
//             label="Sắp xếp"
//             icon={<span className="text-sm font-extrabold text-orange-500">↕</span>}
//             options={sortOptions}
//             value={sort}
//             onChange={(v) => {
//               setSort(v);
//               onSortChange?.(v);
//             }}
//           />
//         </div>
//       </div>
//     </div>
//   );
// }
