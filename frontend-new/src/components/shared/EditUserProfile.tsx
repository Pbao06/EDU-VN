"use client";
import React, { useState, useEffect } from "react";
import { Pencil } from "lucide-react";

// Khớp với các trường bạn muốn cho phép user sửa
export type EditProfileFormData = {
  fullName: string;
  useType: string;
  mainGoal: string;
  fieldId: number;
};

export type FieldOption = {
  id: number;
  name: string;
};

export type MainGoalOption = {
  value: string;
  label: string;
};

interface EditProfileModalProps {
  isOpen: boolean;
  initialData: EditProfileFormData;
  onClose: () => void;
  onSave: (data: EditProfileFormData) => Promise<void> | void;
  useTypeOptions?: string[];
  fieldOptions?: FieldOption[];
  mainGoalOptions?: MainGoalOption[];
}

const HARD_SHADOW = "4px 4px 0 0 #111111";
const HARD_SHADOW_HOVER = "6px 6px 0 0 #111111";

export function EditProfileModal({
  isOpen,
  initialData,
  onClose,
  onSave,
  useTypeOptions = ["HighSchoolStudent", "University", "Working"],
  fieldOptions,
  mainGoalOptions = [
    { value: "UniversityExam", label: "Thi đại học" },
    { value: "ImproveGrades", label: "Cải thiện điểm số" },
    { value: "NewSkill", label: "Học kỹ năng mới" },
    { value: "InterviewPrep", label: "Chuẩn bị phỏng vấn" },
  ],
}: EditProfileModalProps) {
  const [formData, setFormData] = useState<EditProfileFormData>(initialData);
  const [busy, setBusy] = useState(false);

  // Sync lại data khi mở modal (phòng trường hợp data bên ngoài thay đổi)
  useEffect(() => {
    setFormData(initialData);
  }, [initialData, isOpen]);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setBusy(true);
      await onSave(formData);
      onClose();
    } catch (error) {
      console.error("Lỗi cập nhật profile:", error);
    } finally {
      setBusy(false);
    }
  };

  return (
    <div
      aria-hidden={!isOpen}
      className="fixed inset-0 z-[1000] flex items-center justify-center px-4 font-sans text-black"
      role="presentation"
    >
      {/* Backdrop */}
      <button
        type="button"
        aria-label="Close"
        onClick={onClose}
        disabled={busy}
        className="absolute inset-0 bg-black/40 backdrop-blur-sm"
      />

      {/* Modal Content */}
      <div
        role="dialog"
        aria-modal="true"
        className="relative flex max-h-[90vh] w-full max-w-lg flex-col rounded-2xl border-2 border-black bg-white"
        style={{ boxShadow: HARD_SHADOW }}
      >
        {/* Header sticker */}
        <div
          className="absolute -top-4 left-6 inline-flex items-center gap-1.5 rounded-xl border-2 border-black bg-amber-300 px-3 py-1 text-xs font-extrabold uppercase tracking-wide"
          style={{ boxShadow: "3px 3px 0 0 #111111" }}
        >
          <Pencil className="h-3.5 w-3.5" strokeWidth={3} />
          <span>Edit Profile</span>
        </div>

        {/* Close button */}
        <button
          type="button"
          onClick={onClose}
          disabled={busy}
          className="absolute -right-3 -top-3 flex h-8 w-8 items-center justify-center rounded-full border-2 border-black bg-white text-lg font-extrabold transition-transform active:translate-y-0.5"
          style={{ boxShadow: "3px 3px 0 0 #111111" }}
        >
          ✕
        </button>

        <div className="overflow-y-auto px-6 pb-6 pt-10 sm:px-8">
          <h2 className="text-2xl font-extrabold tracking-tight">Cập nhật thông tin</h2>
          <p className="mt-1 text-sm font-bold text-black/60">
            Chỉnh sửa các thông tin hiển thị trên hồ sơ của bạn.
          </p>

          <form onSubmit={handleSubmit} className="mt-6 space-y-5">
            {/* Full Name */}
            <div>
              <label className="mb-1.5 block text-xs font-extrabold uppercase tracking-wide text-black/60">
                Full Name
              </label>
              <input
                type="text"
                required
                value={formData.fullName}
                onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                className="w-full rounded-xl border-2 border-black bg-white px-4 py-3 text-sm font-extrabold outline-none transition-transform focus:-translate-y-0.5"
                style={{ boxShadow: "3px 3px 0 0 #111111" }}
              />
            </div>

            {/* User Type */}
            <div>
              <label className="mb-1.5 block text-xs font-extrabold uppercase tracking-wide text-black/60">
                User Type
              </label>
              <select
                value={formData.useType}
                onChange={(e) => setFormData({ ...formData, useType: e.target.value })}
                className="w-full cursor-pointer appearance-none rounded-xl border-2 border-black bg-white px-4 py-3 text-sm font-extrabold outline-none transition-transform focus:-translate-y-0.5"
                style={{ boxShadow: "3px 3px 0 0 #111111" }}
              >
                {useTypeOptions.map((opt) => (
                  <option key={opt} value={opt}>
                    {opt}
                  </option>
                ))}
              </select>
            </div>

            {/* Field of Interest */}
            <div>
              <label className="mb-1.5 block text-xs font-extrabold uppercase tracking-wide text-black/60">
                Field of Interest
              </label>
              <select
                value={formData.fieldId}
                onChange={(e) => setFormData({ ...formData, fieldId: Number(e.target.value) })}
                className="w-full cursor-pointer appearance-none rounded-xl border-2 border-black bg-white px-4 py-3 text-sm font-extrabold outline-none transition-transform focus:-translate-y-0.5"
                style={{ boxShadow: "3px 3px 0 0 #111111" }}
              >
                {fieldOptions?.map((opt) => (
                  <option key={opt.id} value={opt.id}>
                    {opt.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Main Goal */}
            <div>
              <label className="mb-1.5 block text-xs font-extrabold uppercase tracking-wide text-black/60">
                Main Goal
              </label>
              <select
                value={formData.mainGoal}
                onChange={(e) => setFormData({ ...formData, mainGoal: e.target.value })}
                className="w-full cursor-pointer appearance-none rounded-xl border-2 border-black bg-white px-4 py-3 text-sm font-extrabold outline-none transition-transform focus:-translate-y-0.5"
                style={{ boxShadow: "3px 3px 0 0 #111111" }}
              >
                {mainGoalOptions.map((opt) => (
                  <option key={opt.value} value={opt.value}>
                    {opt.label}
                  </option>
                ))}
              </select>
            </div>

            {/* Action Buttons */}
            <div className="mt-8 flex flex-col-reverse gap-3 pt-2 sm:flex-row sm:justify-end">
              <button
                type="button"
                onClick={onClose}
                disabled={busy}
                className="inline-flex h-11 items-center justify-center rounded-xl border-2 border-black bg-white px-5 text-sm font-extrabold transition-transform duration-150 hover:bg-gray-100 disabled:opacity-50"
                style={{ boxShadow: HARD_SHADOW }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.boxShadow = HARD_SHADOW_HOVER;
                  e.currentTarget.style.transform = "translate(-2px, -2px)";
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.boxShadow = HARD_SHADOW;
                  e.currentTarget.style.transform = "translate(0px, 0px)";
                }}
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={busy}
                className="inline-flex h-11 items-center justify-center gap-2 rounded-xl border-2 border-black bg-blue-600 px-6 text-sm font-extrabold text-white transition-transform duration-150 hover:bg-blue-700 disabled:opacity-70"
                style={{ boxShadow: HARD_SHADOW }}
                onMouseEnter={(e) => {
                  if (busy) return;
                  e.currentTarget.style.boxShadow = HARD_SHADOW_HOVER;
                  e.currentTarget.style.transform = "translate(-2px, -2px)";
                }}
                onMouseLeave={(e) => {
                  if (busy) return;
                  e.currentTarget.style.boxShadow = HARD_SHADOW;
                  e.currentTarget.style.transform = "translate(0px, 0px)";
                }}
              >
                {busy && (
                  <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />
                )}
                Save Changes
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}