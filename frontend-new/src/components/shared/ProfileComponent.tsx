"use client";
import React from "react";
import type {useprofile,editprofile } from "@/types/Profile/userProfile";
import {
  Trophy,
  BookOpen,
  ListChecks,
  ArrowRight,
  Pencil,
  KeyRound,
} from "lucide-react";
import { useProfile } from "@/hooks/learning/useProfile";
import { userProfile } from "@/services/profileService";

/**
 * Profile — EDU VN
 * Same design language as Hero: Neo-Brutalism, hard shadows, thick black borders,
 * flat orange/blue/amber accents, extrabold type.
 * Layout is grid + divider driven — NOT a stack of repeated cards.
 * Header (Avatar + Personal Information) has no card boxes — pure grid/flex/divider.
 */

const HARD_SHADOW = "4px 4px 0 0 #111111";
const HARD_SHADOW_HOVER = "6px 6px 0 0 #111111";

const personalInfo = [
  { label: "Full Name", value: "Nguyễn An" },
  { label: "Email", value: "an.nguyen@email.com" },
  { label: "Main Goal", value: "Trở thành Backend Developer" },
  { label: "Field of Interest", value: "Software Engineering" },
  { label: "User Type", value: "Learner" },
  { label: "Role", value: "Student" },
];


export interface LearningPath {
  id: string;           // nên có id để làm key + gọi API theo path
  name: string;
  progress: number;      // 0-100
  subject: string;
  action: "Continue" | "Start";
}
export interface Achievement {
  completedTopics: number;
  completedSubjects: number;
  totalLearningPaths: number;
}

export interface ProfileProps {
  personalInfo:useprofile;
  learningPath: LearningPath[];
  achievement: Achievement;
  onEditProfile: () => void;
  onChangePassword: () => void;
  onContinuePath: (pathId: string) => void;
}
// const learningPaths = [
//   {
//     name: "Backend Developer",
//     progress: 72,
//     subject: "ASP.NET Core",
//     action: "Continue",
//   },
//   {
//     name: "Frontend Developer",
//     progress: 18,
//     subject: "JavaScript",
//     action: "Continue",
//   },
//   {
//     name: "AI Engineer",
//     progress: 0,
//     subject: "—",
//     action: "Start",
//   },
// ];



export default function Profile({
  personalInfo,learningPath,achievement,onEditProfile,onChangePassword,onContinuePath
}: ProfileProps) 
{
  return (
    <div className="mx-auto w-full max-w-6xl px-4 py-12 font-sans text-black sm:px-6 lg:px-8">
      {/* Profile Header: Avatar (left) + Personal Information (right) — no card boxes */}
      <div className="grid grid-cols-1 gap-10 border-b-2 border-black pb-10 lg:grid-cols-[220px_1fr] lg:gap-14">
        {/* Avatar — just the identity anchor, no card */}
        <div className="flex flex-row items-center gap-4 lg:flex-col lg:items-start lg:gap-0">
          <div
            className="flex h-24 w-24 shrink-0 items-center justify-center rounded-2xl border-2 border-black bg-amber-300 text-3xl font-extrabold"
            style={{ boxShadow: HARD_SHADOW }}
          >
            Pbao
          </div>
          <div className="lg:mt-5">
            <span className="inline-flex w-fit items-center rounded-xl border-2 border-black bg-blue-600 px-3 py-1 text-xs font-extrabold uppercase tracking-wide text-white">
              User
            </span>
          </div>
        </div>

        {/* Personal Information — grid + divider, no per-field card */}
        <div>
          <h2 className="text-xs font-extrabold uppercase tracking-wide text-black/50">
            Personal Information
          </h2>

          <div className="mt-4 grid grid-cols-1 gap-x-10 gap-y-4 sm:grid-cols-2">
              <div className="flex items-baseline justify-between gap-4 border-b border-black/10 pb-3 sm:justify-start sm:gap-6" >
                <dt className="shrink-0 text-xs font-extrabold uppercase tracking-wide text-black/50 sm:w-36">
                  Full Name
                </dt>
                <dd className="truncate text-right text-sm font-extrabold sm:text-left">
                  {personalInfo.fullName}
                </dd>
              </div>
               
               <div className="flex items-baseline justify-between gap-4 border-b border-black/10 pb-3 sm:justify-start sm:gap-6" >
                <dt className="shrink-0 text-xs font-extrabold uppercase tracking-wide text-black/50 sm:w-36">
                  Email
                </dt>
                <dd className="truncate text-right text-sm font-extrabold sm:text-left">
                  {personalInfo.email}
                </dd>
              </div>
               <div className="flex items-baseline justify-between gap-4 border-b border-black/10 pb-3 sm:justify-start sm:gap-6" >
                <dt className="shrink-0 text-xs font-extrabold uppercase tracking-wide text-black/50 sm:w-36">
                  User Type
                </dt>
                <dd className="truncate text-right text-sm font-extrabold sm:text-left">
                  {personalInfo.useType}
                </dd>
              </div>
               <div className="flex items-baseline justify-between gap-4 border-b border-black/10 pb-3 sm:justify-start sm:gap-6" >
                <dt className="shrink-0 text-xs font-extrabold uppercase tracking-wide text-black/50 sm:w-36">
                  Main Goal
                </dt>
                <dd className="truncate text-right text-sm font-extrabold sm:text-left">
                  {personalInfo.mainGoal}
                </dd>
              </div>
               <div className="flex items-baseline justify-between gap-4 border-b border-black/10 pb-3 sm:justify-start sm:gap-6" >
                <dt className="shrink-0 text-xs font-extrabold uppercase tracking-wide text-black/50 sm:w-36">
                  Role
                </dt>
                <dd className="truncate text-right text-sm font-extrabold sm:text-left">
                  {personalInfo.role}
                </dd>
              </div>
               <div className="flex items-baseline justify-between gap-4 border-b border-black/10 pb-3 sm:justify-start sm:gap-6" >
                <dt className="shrink-0 text-xs font-extrabold uppercase tracking-wide text-black/50 sm:w-36">
                  Field
                </dt>
                <dd className="truncate text-right text-sm font-extrabold sm:text-left">
                  {personalInfo.fieldName}
                </dd>
              </div>
               {/* <div className="flex items-baseline justify-between gap-4 border-b border-black/10 pb-3 sm:justify-start sm:gap-6" >
                <dt className="shrink-0 text-xs font-extrabold uppercase tracking-wide text-black/50 sm:w-36">
                  Update At:
                </dt>
                <dd className="truncate text-right text-sm font-extrabold sm:text-left">
                  {personalInfo.updateAt}
                </dd>
              </div> */}
          </div>
          <div className="mt-7 flex flex-col gap-3 sm:flex-row">
            <button
              type="button"
              className="inline-flex items-center justify-center gap-2 rounded-2xl border-2 border-black bg-white px-5 py-2.5 text-sm font-extrabold text-black transition-transform duration-150 hover:bg-orange-100"
              style={{ boxShadow: HARD_SHADOW }}
              onMouseEnter={(e) => {
                e.currentTarget.style.boxShadow = HARD_SHADOW_HOVER;
                e.currentTarget.style.transform = "translate(-3px, -3px)";
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.boxShadow = HARD_SHADOW;
                e.currentTarget.style.transform = "translate(0px, 0px)";
              }}
            >
              <Pencil className="h-4 w-4" strokeWidth={2.5} />
              Edit Profile
            </button>

            <button
              type="button"
              className="inline-flex items-center justify-center gap-2 rounded-2xl border-2 border-black bg-white px-5 py-2.5 text-sm font-extrabold text-black transition-transform duration-150 hover:bg-blue-50"
              style={{ boxShadow: HARD_SHADOW }}
              onMouseEnter={(e) => {
                e.currentTarget.style.boxShadow = HARD_SHADOW_HOVER;
                e.currentTarget.style.transform = "translate(-3px, -3px)";
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.boxShadow = HARD_SHADOW;
                e.currentTarget.style.transform = "translate(0px, 0px)";
              }}
            >
              <KeyRound className="h-4 w-4" strokeWidth={2.5} />
              Change Password
            </button>
          </div>
        </div>
      </div>

      {/* My Learning Paths — file-explorer style table, divider-based, no cards */}
      <div className="border-b-2 border-black py-10">
        <h2 className="text-lg font-extrabold uppercase tracking-wide">
          My Learning Paths
        </h2>

        <div className="mt-6 overflow-x-auto">
          <table className="w-full min-w-[560px] border-collapse text-left">
            <thead>
              <tr className="border-b-2 border-black text-xs font-extrabold uppercase tracking-wide text-black/60">
                <th className="pb-3 pr-4">Learning Path</th>
                <th className="pb-3 pr-4">Progress</th>
                <th className="pb-3 pr-4">Current Subject</th>
                <th className="pb-3 text-right">Action</th>
              </tr>
            </thead>
            <tbody>
              {learningPath.map((path) => (
                <tr
                  key={path.id}
                  className="border-b border-black/15 text-sm sm:text-base"
                >
                  <td className="py-4 pr-4 font-extrabold">{path.name}</td>
                  <td className="py-4 pr-4">
                    <div className="flex items-center gap-3">
                      <span className="w-10 shrink-0 font-extrabold">
                        {path.progress}%
                      </span>
                      <div className="h-2.5 w-28 rounded-full border-2 border-black bg-white">
                        <div
                          className="h-full rounded-full bg-orange-400"
                          style={{ width: `${path.progress}%` }}
                        />
                      </div>
                    </div>
                  </td>
                  <td className="py-4 pr-4 font-semibold text-black/70">
                    {path.subject}
                  </td>
                  <td className="py-4 text-right">
                    <button
                      type="button"
                      className="inline-flex items-center gap-1.5 rounded-xl border-2 border-black bg-white px-4 py-1.5 text-xs font-extrabold uppercase tracking-wide hover:bg-amber-100 sm:text-sm"
                      onClick={() => onContinuePath(path.id)} 
                    >
                      {path.action}
                      <ArrowRight className="h-3.5 w-3.5" strokeWidth={2.5} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Achievement — small grid, icon + stat */}
      <div className="pt-10">
        <h2 className="text-lg font-extrabold uppercase tracking-wide">
          Achievement
        </h2>
        <div className="mt-5 grid grid-cols-1 gap-6 sm:grid-cols-3">
          <div className="flex items-center gap-3 border-b border-black/10 pb-4 sm:border-b-0 sm:pb-0">
            <ListChecks className="h-6 w-6 text-orange-400" strokeWidth={2.5} />
            <div>
              <p className="text-xl font-extrabold">35</p>
              <p className="text-xs font-semibold uppercase tracking-wide text-black/50">
                Completed Topics
              </p>
            </div>
          </div>
          <div className="flex items-center gap-3 border-b border-black/10 pb-4 sm:border-b-0 sm:pb-0">
            <BookOpen className="h-6 w-6 text-blue-600" strokeWidth={2.5} />
            <div>
              <p className="text-xl font-extrabold">8</p>
              <p className="text-xs font-semibold uppercase tracking-wide text-black/50">
                Completed Subjects
              </p>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <Trophy className="h-6 w-6 text-amber-300" strokeWidth={2.5} />
            <div>
              <p className="text-xl font-extrabold">3</p>
              <p className="text-xs font-semibold uppercase tracking-wide text-black/50">
                Learning Paths
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}