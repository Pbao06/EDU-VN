'use client';
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from "react";

export type AlertOptions = {
  title: string;
  message: ReactNode;
  confirmLabel?: string;
  cancelLabel?: string;
  onConfirm?: () => void | Promise<void>;
  onCancel?: () => void;
  tone?: "default" | "danger" | "success";
};

type AlertContextValue = {
  showAlert: (options: AlertOptions) => void;
  hideAlert: () => void;
};

const AlertContext = createContext<AlertContextValue | null>(null);

export function useAlert() {
  const ctx = useContext(AlertContext);
  if (!ctx) {
    throw new Error("useAlert must be used within an <AlertProvider>");
  }
  return ctx;
}

const toneStyles: Record<
  NonNullable<AlertOptions["tone"]>,
  { badge: string; confirm: string; accent: string; emoji: string }
> = {
  default: {
    badge: "bg-[#FFD84D]",
    confirm: "bg-[#2C6BFF] text-white hover:bg-[#1f57e0]",
    accent: "bg-[#2C6BFF]",
    emoji: "✦",
  },
  danger: {
    badge: "bg-[#FF7A59]",
    confirm: "bg-[#FF4D4D] text-white hover:bg-[#e63f3f]",
    accent: "bg-[#FF4D4D]",
    emoji: "！",
  },
  success: {
    badge: "bg-[#8FE388]",
    confirm: "bg-[#22A06B] text-white hover:bg-[#1c8a5a]",
    accent: "bg-[#22A06B]",
    emoji: "✓",
  },
};

export function AlertProvider({ children }: { children: ReactNode }) {
  const [options, setOptions] = useState<AlertOptions | null>(null);
  const [open, setOpen] = useState(false);
  const [busy, setBusy] = useState(false);
  const closeTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  const hideAlert = useCallback(() => {
    setOpen(false);
    if (closeTimer.current) clearTimeout(closeTimer.current);
    closeTimer.current = setTimeout(() => setOptions(null), 180);
  }, []);

  const showAlert = useCallback((next: AlertOptions) => {
    if (closeTimer.current) clearTimeout(closeTimer.current);
    setOptions(next);
    setOpen(true);
    setBusy(false);
  }, []);

  useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape" && !busy) {
        options?.onCancel?.();
        hideAlert();
      }
    };
    window.addEventListener("keydown", onKey);
    const prev = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    return () => {
      window.removeEventListener("keydown", onKey);
      document.body.style.overflow = prev;
    };
  }, [open, busy, options, hideAlert]);

  useEffect(() => {
    return () => {
      if (closeTimer.current) clearTimeout(closeTimer.current);
    };
  }, []);

  const value = useMemo(() => ({ showAlert, hideAlert }), [showAlert, hideAlert]);

  const tone = toneStyles[options?.tone ?? "default"];

  const handleConfirm = async () => {
    if (!options) return;
    try {
      setBusy(true);
      await options.onConfirm?.();
    } finally {
      setBusy(false);
      hideAlert();
    }
  };

  const handleCancel = () => {
    if (busy) return;
    options?.onCancel?.();
    hideAlert();
  };

  return (
    <AlertContext.Provider value={value}>
      {children}

      {options && (
        <div
          aria-hidden={!open}
          className={[
            "fixed inset-0 z-[1000] flex items-center justify-center px-4 transition-opacity duration-150",
            open ? "opacity-100" : "pointer-events-none opacity-0",
          ].join(" ")}
          role="presentation"
        >
          {/* Backdrop */}
          <button
            type="button"
            aria-label="Đóng hộp thoại"
            onClick={handleCancel}
            className="absolute inset-0 bg-black/40 backdrop-blur-[2px]"
          />

          {/* Dialog */}
          <div
            role="alertdialog"
            aria-modal="true"
            aria-labelledby="global-alert-title"
            aria-describedby="global-alert-message"
            className={[
              "relative w-full max-w-md rounded-[28px] border-[2.5px] border-black bg-white",
              "shadow-[8px_8px_0_0_#000] transition-all duration-200",
              open ? "translate-y-0 scale-100" : "translate-y-3 scale-[0.98]",
            ].join(" ")}
          >
            {/* Decorative sticker */}
            <div
              className={[
                "absolute -top-4 left-6 inline-flex items-center gap-1.5 rounded-full",
                "border-[2.5px] border-black px-3 py-1 text-xs font-black uppercase tracking-wide",
                "shadow-[3px_3px_0_0_#000]",
                tone.badge,
              ].join(" ")}
            >
              <span>{tone.emoji}</span>
              <span>EDU VN</span>
            </div>

            {/* Close (X) */}
            <button
              type="button"
              onClick={handleCancel}
              disabled={busy}
              aria-label="Đóng"
              className="absolute -right-3 -top-3 grid h-9 w-9 place-items-center rounded-full border-[2.5px] border-black bg-white text-lg font-black shadow-[3px_3px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:translate-x-0.5 active:translate-x-0 active:translate-y-0 disabled:opacity-50"
            >
              ✕
            </button>

            <div className="px-6 pb-6 pt-8 sm:px-8 sm:pb-8 sm:pt-10">
              <div className="flex items-start gap-4">
                <div
                  className={[
                    "hidden h-12 w-12 shrink-0 place-items-center rounded-2xl border-[2.5px] border-black text-xl font-black text-white shadow-[3px_3px_0_0_#000] sm:grid",
                    tone.accent,
                  ].join(" ")}
                  aria-hidden
                >
                  {tone.emoji}
                </div>
                <div className="min-w-0 flex-1">
                  <h2
                    id="global-alert-title"
                    className="text-2xl font-black leading-tight tracking-tight text-black"
                  >
                    {options.title}
                  </h2>
                  <div
                    id="global-alert-message"
                    className="mt-2 text-[15px] leading-relaxed text-black/70"
                  >
                    {options.message}
                  </div>
                </div>
              </div>

              <div className="mt-7 flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
                <button
                  type="button"
                  onClick={handleCancel}
                  disabled={busy}
                  className="inline-flex h-12 items-center justify-center rounded-2xl border-[2.5px] border-black bg-white px-5 text-sm font-bold text-black shadow-[4px_4px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:translate-x-0.5 hover:shadow-[6px_6px_0_0_#000] active:translate-x-0 active:translate-y-0 active:shadow-[2px_2px_0_0_#000] disabled:opacity-50"
                >
                  {options.cancelLabel ?? "Hủy"}
                </button>
                <button
                  type="button"
                  onClick={handleConfirm}
                  disabled={busy}
                  className={[
                    "inline-flex h-12 items-center justify-center gap-2 rounded-2xl border-[2.5px] border-black px-6 text-sm font-black shadow-[4px_4px_0_0_#000] transition-transform hover:-translate-y-0.5 hover:translate-x-0.5 hover:shadow-[6px_6px_0_0_#000] active:translate-x-0 active:translate-y-0 active:shadow-[2px_2px_0_0_#000] disabled:opacity-70",
                    tone.confirm,
                  ].join(" ")}
                >
                  {busy && (
                    <span
                      className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white"
                      aria-hidden
                    />
                  )}
                  {options.confirmLabel ?? "Xác nhận"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </AlertContext.Provider>
  );
}
