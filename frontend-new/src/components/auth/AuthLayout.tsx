import type { ReactNode } from "react";
import { Compass } from "lucide-react";

export interface AuthLayoutProps {
  title: string;
  description?: string;
  children: ReactNode;
  footer?: ReactNode;
  className?: string;
}

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
          className="absolute -right-1.5 -top-1.5 z-10 text-[#FFD84D]"
        >
          <svg viewBox="0 0 24 24" fill="currentColor" className="h-4 w-4 drop-shadow-[1px_1px_0_#000]">
            <path d="M12 2l2.4 6.4L21 9.2l-5 4.3L17.6 20 12 16.7 6.4 20 8 13.5 3 9.2l6.6-.8L12 2z" />
          </svg>
        </span>
        <span className="flex h-12 w-12 items-center justify-center rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] text-white shadow-[4px_4px_0_0_#000] transition-transform duration-200 group-hover:-rotate-6 group-active:translate-x-[2px] group-active:translate-y-[2px] group-active:shadow-[2px_2px_0_0_#000]">
          <Compass className="h-6 w-6" strokeWidth={2.5} />
        </span>
      </span>
      <span className="text-2xl font-extrabold tracking-tight text-black">
        EDU<span className="text-[#4D7CFF]">VN</span>
      </span>
    </a>
  );
}

export function AuthLayout({
  title,
  description,
  children,
  footer,
  className = "",
}: AuthLayoutProps) {
  return (
    <div
      className={`relative min-h-screen w-full overflow-hidden bg-white ${className}`}
    >
      <span
        aria-hidden
        className="pointer-events-none absolute -left-16 -top-16 h-56 w-56 rounded-full border-[2.5px] border-black bg-[#FFD84D] opacity-90 shadow-[8px_8px_0_0_#000]"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute -right-20 top-40 h-40 w-40 rotate-12 rounded-[32px] border-[2.5px] border-black bg-[#FF8A3D] shadow-[8px_8px_0_0_#000]"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute bottom-10 left-10 hidden h-24 w-24 rotate-[18deg] rounded-2xl border-[2.5px] border-black bg-[#7BE495] shadow-[6px_6px_0_0_#000] md:block"
      />
      <span
        aria-hidden
        className="pointer-events-none absolute inset-0 opacity-[0.12]"
        style={{
          backgroundImage:
            "radial-gradient(circle, #000 1.2px, transparent 1.2px)",
          backgroundSize: "24px 24px",
        }}
      />

      <header className="relative z-10 flex items-center justify-between px-6 py-6 sm:px-10">
        <EduVnLogo />
        <a
          href="/home"
          className="hidden rounded-full border-[2.5px] border-black bg-white px-4 py-2 text-xs font-extrabold uppercase tracking-wider text-black shadow-[3px_3px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:shadow-[5px_5px_0_0_#000] active:translate-y-0 active:shadow-[2px_2px_0_0_#000] sm:inline-flex"
        >
          ← Về trang chủ
        </a>
      </header>

      <main className="relative z-10 flex min-h-[calc(100vh-88px)] items-center justify-center px-6 pb-16 pt-4 sm:px-10">
        <div className="w-full max-w-md rounded-[28px] border-[2.5px] border-black bg-white p-8 shadow-[8px_8px_0_0_#000] sm:p-10">
          <div className="mb-7 flex flex-col gap-2 text-center">
            <h1 className="text-3xl font-extrabold tracking-tight text-black">
              {title}
            </h1>
            {/* {description && (
              <p className="text-sm font-semibold text-black/60">
                {description}
              </p>
            )} */}
          </div>
          {children}
          {footer && <div className="mt-6">{footer}</div>}
        </div>
      </main>
    </div>
  );
}

// import type { ReactNode } from "react";
// import { Compass } from "lucide-react";

// export interface AuthLayoutProps {
//   title: string;
//   description?: string;
//   children: ReactNode;
//   footer?: ReactNode;
//   className?: string;
// }

// function EduVnLogo() {
//   return (
//     <a
//       href="/home"
//       className="group inline-flex items-center gap-3 focus:outline-none"
//       aria-label="EDU VN - Trang chủ"
//     >
//       <span className="relative">
//         <span
//           aria-hidden
//           className="absolute -right-1.5 -top-1.5 z-10 text-[#FFD84D]"
//         >
//           <svg viewBox="0 0 24 24" fill="currentColor" className="h-4 w-4 drop-shadow-[1px_1px_0_#000]">
//             <path d="M12 2l2.4 6.4L21 9.2l-5 4.3L17.6 20 12 16.7 6.4 20 8 13.5 3 9.2l6.6-.8L12 2z" />
//           </svg>
//         </span>
//         <span className="flex h-12 w-12 items-center justify-center rounded-2xl border-[2.5px] border-black bg-[#4D7CFF] text-white shadow-[4px_4px_0_0_#000] transition-transform duration-200 group-hover:-rotate-6 group-active:translate-x-[2px] group-active:translate-y-[2px] group-active:shadow-[2px_2px_0_0_#000]">
//           <Compass className="h-6 w-6" strokeWidth={2.5} />
//         </span>
//       </span>
//       <span className="text-2xl font-extrabold tracking-tight text-black">
//         EDU<span className="text-[#4D7CFF]">VN</span>
//       </span>
//     </a>
//   );
// }

// export function AuthLayout({
//   title,
//   description,
//   children,
//   footer,
//   className = "",
// }: AuthLayoutProps) {
//   return (
//     <div
//       className={`relative min-h-screen w-full overflow-hidden bg-white ${className}`}
//     >
//       <span
//         aria-hidden
//         className="pointer-events-none absolute -left-16 -top-16 h-56 w-56 rounded-full border-[2.5px] border-black bg-[#FFD84D] opacity-90 shadow-[8px_8px_0_0_#000]"
//       />
//       <span
//         aria-hidden
//         className="pointer-events-none absolute -right-20 top-40 h-40 w-40 rotate-12 rounded-[32px] border-[2.5px] border-black bg-[#FF8A3D] shadow-[8px_8px_0_0_#000]"
//       />
//       <span
//         aria-hidden
//         className="pointer-events-none absolute bottom-10 left-10 hidden h-24 w-24 rotate-[18deg] rounded-2xl border-[2.5px] border-black bg-[#7BE495] shadow-[6px_6px_0_0_#000] md:block"
//       />
//       <span
//         aria-hidden
//         className="pointer-events-none absolute inset-0 opacity-[0.12]"
//         style={{
//           backgroundImage:
//             "radial-gradient(circle, #000 1.2px, transparent 1.2px)",
//           backgroundSize: "24px 24px",
//         }}
//       />

//       <header className="relative z-10 flex items-center justify-between px-6 py-6 sm:px-10">
//         <EduVnLogo />
//         <a
//           href="/home"
//           className="hidden rounded-full border-[2.5px] border-black bg-white px-4 py-2 text-xs font-extrabold uppercase tracking-wider text-black shadow-[3px_3px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:shadow-[5px_5px_0_0_#000] active:translate-y-0 active:shadow-[2px_2px_0_0_#000] sm:inline-flex"
//         >
//           ← Về trang chủ
//         </a>
//       </header>

//       <main className="relative z-10 flex min-h-[calc(100vh-88px)] items-center justify-center px-6 pb-16 pt-4 sm:px-10">
//         {children}
//       </main>
//     </div>
//   );
// }
