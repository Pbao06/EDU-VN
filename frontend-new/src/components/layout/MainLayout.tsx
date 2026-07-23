import React from "react";
import Navbar from "@/components/shared/Navbar";
import Footer from "@/components/shared/Footer";

interface MainLayoutProps {
  children: React.ReactNode;
}

export default function MainLayout({ children }: MainLayoutProps) {
  return (
    <div className="flex flex-col min-h-screen ">
      <Navbar />
      <main className="flex-grow w-full bg-[#FFF8E7]">
        {children}
      </main>
      <Footer />
    </div>
  );
}
//bg-[#F5F7FA] transition-colors duration-300
//bg-[#FFF8E7]
//bg-slate-200