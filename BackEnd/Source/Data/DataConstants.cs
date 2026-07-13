namespace Source.Data
{
    /// <summary>
    /// Constants class chứa các ID cố định cho dữ liệu seeding
    /// Giúp code dễ đọc, dễ maintain và tránh bug khi thay đổi text
    /// </summary>
    public static class DataConstants
    {
        #region Fields (Lĩnh vực)
        public static class Fields
        {
            public const int CNTT = 1;                    // Công Nghệ Thông Tin
            public const int Marketing = 2;               // Marketing - Tiếp Thị
            public const int KinhTe = 3;                  // Kinh Tế - Tài Chính
            public const int YTe = 4;                     // Y Tế - Sức Khỏe
            public const int GiaoDuc = 5;                 // Giáo Dục
        }
        #endregion

        #region Careers (Nghề nghiệp)
        public static class Careers
        {
            // CNTT Careers (ID: 10-19)
            public const int FrontendDeveloper = 10;     // Lập Trình Viên Frontend
            public const int BackendDeveloper = 11;       // Lập Trình Viên Backend
            public const int DataScientist = 12;          // Data Scientist
            public const int DevOpsEngineer = 13;          // DevOps Engineer
            public const int MobileDeveloper = 14;        // Mobile Developer

            // Marketing Careers (ID: 20-29)
            public const int DigitalMarketing = 20;      // Digital Marketing Specialist
            public const int ContentCreator = 21;        // Content Creator

            // Kinh Tế Careers (ID: 30-39)
            public const int FinancialAnalyst = 30;      // Financial Analyst
            public const int Accountant = 31;             // Accountant
        }
        #endregion

        #region Subjects (Môn học)
        public static class Subjects
        {
            // CNTT Subjects (ID: 100-109)
            public const int LapTrinhCoBan = 100;         // Lập Trình Cơ Bản
            public const int CauTrucDuLieu = 101;         // Cấu Trúc Dữ Liệu & Giải Thuật
            public const int CoSoDuLieu = 102;            // Cơ Sở Dữ Liệu
            public const int WebDevelopment = 103;         // Web Development
            public const int OOP = 104;                    // Lập Trình Hướng Đối Tượng

            // Marketing Subjects (ID: 110-119)
            public const int DigitalMarketingFund = 110;  // Digital Marketing Fundamentals
            public const int ContentMarketing = 111;       // Content Marketing

            // Kinh Tế Subjects (ID: 120-129)
            public const int FinancialAccounting = 120;   // Financial Accounting
            public const int InvestmentAnalysis = 121;    // Investment Analysis
        }
        #endregion

        #region Topics (Chủ đề học tập)
        public static class Topics
        {
            // Topics cho Lập Trình Cơ Bản (ID: 200-209)
            public const int BienKieuDuLieu = 200;        // Biến & Kiểu Dữ Liệu
            public const int CauTrucDieuKhien = 201;      // Cấu Trúc Điều Khiển
            public const int HamProcedure = 202;          // Hàm & Procedure

            // Topics cho Cấu Trúc Dữ Liệu (ID: 210-219)
            public const int ArrayList = 210;             // Array & List
            public const int StackQueue = 211;             // Stack & Queue
            public const int TreeGraph = 212;             // Tree & Graph

            // Topics cho Cơ Sở Dữ Liệu (ID: 220-229)
            public const int SQLCoBan = 220;              // SQL Cơ Bản
            public const int DatabaseDesign = 221;        // Database Design

            // Topics cho Web Development (ID: 230-239)
            public const int HTMLCSS = 230;               // HTML & CSS
            public const int JavaScriptCoBan = 231;       // JavaScript Cơ Bản
            public const int ReactFramework = 232;         // React Framework
        }
        #endregion

        #region Quizzes (Bài trắc nghiệm)
        public static class Quizzes
        {
            public const int QuizCNTT = 1;                // Quiz định hướng nghề CNTT
            public const int QuizMarketing = 2;            // Quiz định hướng nghề Marketing
            public const int QuizKinhTe = 3;              // Quiz định hướng nghề Kinh Tế
        }
        #endregion

        #region Questions (Câu hỏi Career Quiz)
        public static class Questions
        {
            // Questions cho Quiz CNTT (ID: 1000-1009)
            public const int Q1_FrontendBackend = 1000;   // Thích giao diện hay logic?
            public const int Q2_DataInterest = 1001;      // Quan tâm dữ liệu?
            public const int Q3_DevOpsInterest = 1002;     // Thích infrastructure?
            public const int Q4_MobileInterest = 1003;     // Thích mobile?
            public const int Q5_AIInterest = 1004;         // Hứng thú AI/ML?
        }
        #endregion

        #region Answers (Đáp án Career Quiz)
        public static class Answers
        {
            // Answers cho Q1 (ID: 2000-2002)
            public const int A1_ThichGiaoDien = 2000;     // Thích giao diện
            public const int A1_ThichLogic = 2001;         // Thích logic
            public const int A1_ThichCaHai = 2002;         // Thích cả hai

            // Answers cho Q2 (ID: 2003-2005)
            public const int A2_RatQuanTam = 2003;        // Rất quan tâm dữ liệu
            public const int A2_HoiQuanTam = 2004;         // Hơi quan tâm
            public const int A2_KhongQuanTam = 2005;       // Không quan tâm

            // Answers cho Q3 (ID: 2006-2008)
            public const int A3_ThichInfra = 2006;         // Thích infrastructure
            public const int A3_ThichKhongChuyen = 2007;  // Thích nhưng không chuyên
            public const int A3_KhongThich = 2008;        // Không thích

            // Answers cho Q4 (ID: 2009-2011)
            public const int A4_RatThichMobile = 2009;     // Rất thích mobile
            public const int A4_HoiThich = 2010;          // Hơi thích
            public const int A4_ThichWeb = 2011;           // Thích web hơn

            // Answers cho Q5 (ID: 2012-2014)
            public const int A5_RatHungThu = 2012;         // Rất hứng thú AI
            public const int A5_HoiHungThu = 2013;         // Hơi hứng thú
            public const int A5_KhongHungThu = 2014;       // Không hứng thú
        }
        #endregion
    }
}