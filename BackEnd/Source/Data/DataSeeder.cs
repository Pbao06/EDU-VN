using Microsoft.EntityFrameworkCore;
using Source.Models;
using Source.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Source.Data
{
    public static class DataSeeder
    {
       

        public static async Task SeedIdentityAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("Seeding Identity for admin account...");

            // 1. Seed Roles
            string[] rolenames = { "Admin", "User" };
            foreach (var role in rolenames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Admin User
            var email = "phangia223@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(email);

            if (adminUser == null)
            {
                var newadmin = new User
                {
                    UserName = email,
                    Email = email,
                    FullName = "Pbao Admin", // Bỏ khoảng trắng thừa ở đầu
                    EmailConfirmed = true,
                    IsOnboardingCompleted = true,
                };

                var createacc = await userManager.CreateAsync(newadmin, "Phangiabao@65");

                if (createacc.Succeeded)
                {
                    await userManager.AddToRoleAsync(newadmin, "Admin");
                    Console.WriteLine("Admin user created successfully.");
                }
                else
                {
                    // Sửa lỗi cú pháp string.Join tại đây
                    var errors = string.Join(", ", createacc.Errors.Select(e => e.Description));
                    Console.WriteLine($"Failed to create Admin user: {errors}");
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }

        public static async Task SeedDataAsync(ApplicationDbContext context)
        {
            // Check if data already exists
            if (await context.Fields.AnyAsync())
            {
                Console.WriteLine("Database already seeded.");
                return;
            }

            Console.WriteLine("Starting data seeding...");

            // 1. Seed Fields
            var fields = await SeedFieldsAsync(context);

            // 2. Seed Careers
            var careers = await SeedCareersAsync(context, fields);

            // 3. Seed Subjects
            var subjects = await SeedSubjectsAsync(context);

            // 4. Seed CareerSubjects (Many-to-Many)
            await SeedCareerSubjectsAsync(context, careers, subjects);

            // 5. Seed Quizzes
            var quizzes = await SeedQuizzesAsync(context, fields);

            // 6. Seed Topics
            var topics = await SeedTopicsAsync(context, subjects);

            // 7. Seed Questions
            var questions = await SeedQuestionsAsync(context, quizzes);

            // 8. Seed Answers
            var answers = await SeedAnswersAsync(context, questions);

            // 9. Seed AnswerCareerWeights (QUAN TRỌNG NHẤT)
            await SeedAnswerCareerWeightsAsync(context, answers, careers);

            Console.WriteLine("Data seeding completed successfully!");
        }

        private static async Task<List<Field>> SeedFieldsAsync(ApplicationDbContext context)
        {
            Console.WriteLine("Seeding Fields...");
            var fields = new List<Field>
            {
                new Field
                {
                    Code = "CNTT",
                    Name = "CNTT - Công Nghệ Thông Tin",
                    Description = "Lĩnh vực công nghệ thông tin, lập trình, AI, data science"
                },
                new Field
                {
                    Code = "MARKETING",
                    Name = "Marketing - Tiếp Thị",
                    Description = "Digital marketing, branding, content marketing"
                },
                new Field
                {
                    Code = "KINH_TE",
                    Name = "Kinh Tế - Tài Chính",
                    Description = "Tài chính, ngân hàng, đầu tư, kế toán"
                },
                new Field
                {
                    Code = "Y_TE",
                    Name = "Y Tế - Sức Khỏe",
                    Description = "Y học, dược, chăm sóc sức khỏe"
                },
                new Field
                {
                    Code = "GIAO_DUC",
                    Name = "Giáo Dục",
                    Description = "Giảng dạy, đào tạo, phát triển giáo dục"
                }
            };

            await context.Fields.AddRangeAsync(fields);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {fields.Count} Fields.");
            return fields;
        }

        private static async Task<List<Career>> SeedCareersAsync(ApplicationDbContext context, List<Field> fields)
        {
            Console.WriteLine("Seeding Careers...");
            var cnttField = fields.First(f => f.Code == "CNTT");
            var marketingField = fields.First(f => f.Code == "MARKETING");
            var kinhTeField = fields.First(f => f.Code == "KINH_TE");

            var careers = new List<Career>
            {
                // CNTT Careers
                new Career
                {
                    Code = "FRONTEND_DEV",
                    FieldId = cnttField.Id,
                    Name = "Lập Trình Viên Frontend",
                    Description = "Phát triển giao diện web/app, tạo trải nghiệm người dùng",
                    Responsibilities = "Thiết kế và phát triển UI/UX, làm việc với HTML/CSS/JavaScript, tích hợp API",
                    MinSalary = 15,
                    MaxSalary = 35,
                    DemandLevel = "High",
                    IconUrl = "/icons/frontend.png",
                    PopularityScore = 90
                },
                new Career
                {
                    Code = "BACKEND_DEV",
                    FieldId = cnttField.Id,
                    Name = "Lập Trình Viên Backend",
                    Description = "Phát triển server, API, database, xử lý logic phía sau",
                    Responsibilities = "Thiết kế kiến trúc server, phát triển API, quản lý database, tối ưu performance",
                    MinSalary = 18,
                    MaxSalary = 40,
                    DemandLevel = "High",
                    IconUrl = "/icons/backend.png",
                    PopularityScore = 85
                },
                new Career
                {
                    Code = "DATA_SCIENTIST",
                    FieldId = cnttField.Id,
                    Name = "Data Scientist",
                    Description = "Phân tích dữ liệu, xây dựng model machine learning, AI",
                    Responsibilities = "Thu thập và làm sạch dữ liệu, xây dựng model ML, phân tích insight, visual data",
                    MinSalary = 25,
                    MaxSalary = 50,
                    DemandLevel = "VeryHigh",
                    IconUrl = "/icons/datascience.png",
                    PopularityScore = 80
                },
                new Career
                {
                    Code = "DEVOPS_ENGINEER",
                    FieldId = cnttField.Id,
                    Name = "DevOps Engineer",
                    Description = "Quản lý infrastructure, CI/CD, cloud deployment",
                    Responsibilities = "Setup server, CI/CD pipeline, quản lý cloud, monitoring, automation",
                    MinSalary = 20,
                    MaxSalary = 45,
                    DemandLevel = "High",
                    IconUrl = "/icons/devops.png",
                    PopularityScore = 75
                },
                new Career
                {
                    Code = "MOBILE_DEV",
                    FieldId = cnttField.Id,
                    Name = "Mobile Developer",
                    Description = "Phát triển ứng dụng mobile (iOS/Android)",
                    Responsibilities = "Phát triển app mobile, tích hợp API, tối ưu performance, test trên nhiều device",
                    MinSalary = 16,
                    MaxSalary = 38,
                    DemandLevel = "High",
                    IconUrl = "/icons/mobile.png",
                    PopularityScore = 82
                },
                // Marketing Careers
                new Career
                {
                    Code = "DIGITAL_MARKETING",
                    FieldId = marketingField.Id,
                    Name = "Digital Marketing Specialist",
                    Description = "Quản lý chiến dịch digital marketing, SEO, SEM",
                    Responsibilities = "Quản lý chiến dịch online, SEO/SEM, analytics, optimization",
                    MinSalary = 12,
                    MaxSalary = 30,
                    DemandLevel = "High",
                    IconUrl = "/icons/digitalmarketing.png",
                    PopularityScore = 88
                },
                new Career
                {
                    Code = "CONTENT_CREATOR",
                    FieldId = marketingField.Id,
                    Name = "Content Creator",
                    Description = "Tạo nội dung sáng tạo cho social media, website",
                    Responsibilities = "Viết content, tạo video, quản lý social media, xây dựng community",
                    MinSalary = 10,
                    MaxSalary = 25,
                    DemandLevel = "Medium",
                    IconUrl = "/icons/content.png",
                    PopularityScore = 85
                },
                // Kinh Tế Careers
                new Career
                {
                    Code = "FINANCIAL_ANALYST",
                    FieldId = kinhTeField.Id,
                    Name = "Financial Analyst",
                    Description = "Phân tích tài chính, đầu tư, báo cáo tài chính",
                    Responsibilities = "Phân tích dữ liệu tài chính, đánh giá đầu tư, lập báo cáo, tư vấn chiến lược",
                    MinSalary = 12,
                    MaxSalary = 35,
                    DemandLevel = "High",
                    IconUrl = "/icons/finance.png",
                    PopularityScore = 80
                },
                new Career
                {
                    Code = "ACCOUNTANT",
                    FieldId = kinhTeField.Id,
                    Name = "Accountant",
                    Description = "Quản lý sổ sách, báo cáo thuế, kế toán",
                    Responsibilities = "Ghi chép giao dịch, lập báo cáo tài chính, quản lý thuế, kiểm toán",
                    MinSalary = 12,
                    MaxSalary = 25,
                    DemandLevel = "Medium",
                    IconUrl = "/icons/accountant.png",
                    PopularityScore = 75
                }
            };

            await context.Careers.AddRangeAsync(careers);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {careers.Count} Careers.");
            return careers;
        }

        private static async Task<List<Subject>> SeedSubjectsAsync(ApplicationDbContext context)
        {
            Console.WriteLine("Seeding Subjects...");
            var subjects = new List<Subject>
            {
                // CNTT Subjects
                new Subject
                {
                    Code = "LAP_TRINH_CO_BAN",
                    Name = "Lập Trình Cơ Bản",
                    Description = "Kiến thức lập trình nền tảng, thuật toán cơ bản",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "CAU_TRUC_DU_LIEU",
                    Name = "Cấu Trúc Dữ Liệu & Giải Thuật",
                    Description = "Data structures, algorithms, complexity analysis",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "CO_SO_DU_LIEU",
                    Name = "Cơ Sở Dữ Liệu",
                    Description = "SQL, NoSQL, database design, optimization",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "WEB_DEVELOPMENT",
                    Name = "Web Development",
                    Description = "HTML, CSS, JavaScript, React/Angular/Vue",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "OOP",
                    Name = "Lập Trình Hướng Đối Tượng",
                    Description = "OOP principles, design patterns, SOLID",
                    Type = "Core"
                },
                // Marketing Subjects
                new Subject
                {
                    Code = "DIGITAL_MARKETING_FUND",
                    Name = "Digital Marketing Fundamentals",
                    Description = "SEO, SEM, Social Media Marketing",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "CONTENT_MARKETING",
                    Name = "Content Marketing",
                    Description = "Content strategy, copywriting, storytelling",
                    Type = "Specialized"
                },
                // Kinh Tế Subjects
                new Subject
                {
                    Code = "FINANCIAL_ACCOUNTING",
                    Name = "Financial Accounting",
                    Description = "Accounting principles, financial statements",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "INVESTMENT_ANALYSIS",
                    Name = "Investment Analysis",
                    Description = "Stock market, portfolio management, risk analysis",
                    Type = "Specialized"
                }
            };

            await context.Subjects.AddRangeAsync(subjects);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {subjects.Count} Subjects.");
            return subjects;
        }

        private static async Task SeedCareerSubjectsAsync(ApplicationDbContext context, List<Career> careers, List<Subject> subjects)
        {
            Console.WriteLine("Seeding CareerSubjects...");
            
            var frontendDev = careers.First(c => c.Code == "FRONTEND_DEV");
            var backendDev = careers.First(c => c.Code == "BACKEND_DEV");
            var dataScientist = careers.First(c => c.Code == "DATA_SCIENTIST");
            var devOps = careers.First(c => c.Code == "DEVOPS_ENGINEER");
            var mobileDev = careers.First(c => c.Code == "MOBILE_DEV");
            var digitalMarketing = careers.First(c => c.Code == "DIGITAL_MARKETING");
            var contentCreator = careers.First(c => c.Code == "CONTENT_CREATOR");
            var financialAnalyst = careers.First(c => c.Code == "FINANCIAL_ANALYST");
            var accountant = careers.First(c => c.Code == "ACCOUNTANT");

            var lapTrinhCoBan = subjects.First(s => s.Code == "LAP_TRINH_CO_BAN");
            var cauTrucDuLieu = subjects.First(s => s.Code == "CAU_TRUC_DU_LIEU");
            var coSoDuLieu = subjects.First(s => s.Code == "CO_SO_DU_LIEU");
            var webDev = subjects.First(s => s.Code == "WEB_DEVELOPMENT");
            var oop = subjects.First(s => s.Code == "OOP");
            var digitalMarketingFund = subjects.First(s => s.Code == "DIGITAL_MARKETING_FUND");
            var contentMarketing = subjects.First(s => s.Code == "CONTENT_MARKETING");
            var financialAccounting = subjects.First(s => s.Code == "FINANCIAL_ACCOUNTING");
            var investmentAnalysis = subjects.First(s => s.Code == "INVESTMENT_ANALYSIS");

            var careerSubjects = new List<CareerSubject>
            {
                // Frontend Developer Subjects
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để hiểu logic code" },
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = webDev.Id, Priority = 1, Reason = "Core skill cho frontend" },
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = oop.Id, Priority = 2, Reason = "Giúp tổ chức code tốt hơn" },
                // Backend Developer Subjects
                new CareerSubject { CareerId = backendDev.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để hiểu lập trình" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = cauTrucDuLieu.Id, Priority = 1, Reason = "Quan trọng cho performance" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = coSoDuLieu.Id, Priority = 1, Reason = "Core skill cho backend" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = oop.Id, Priority = 2, Reason = "Giúp thiết kế architecture tốt" },
                // Data Scientist Subjects
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để viết code ML" },
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = cauTrucDuLieu.Id, Priority = 1, Reason = "Quan trọng cho thuật toán ML" },
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = coSoDuLieu.Id, Priority = 1, Reason = "Cần thiết để xử lý dữ liệu" },
                // DevOps Subjects
                new CareerSubject { CareerId = devOps.Id, SubjectId = lapTrinhCoBan.Id, Priority = 2, Reason = "Cần thiết để viết script" },
                new CareerSubject { CareerId = devOps.Id, SubjectId = coSoDuLieu.Id, Priority = 1, Reason = "Quan trọng để quản lý data" },
                // Mobile Developer Subjects
                new CareerSubject { CareerId = mobileDev.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để hiểu lập trình" },
                new CareerSubject { CareerId = mobileDev.Id, SubjectId = webDev.Id, Priority = 1, Reason = "Giúp phát triển cross-platform" },
                new CareerSubject { CareerId = mobileDev.Id, SubjectId = oop.Id, Priority = 2, Reason = "Giúp tổ chức code tốt hơn" },
                // Digital Marketing Subjects
                new CareerSubject { CareerId = digitalMarketing.Id, SubjectId = digitalMarketingFund.Id, Priority = 1, Reason = "Core skill cho digital marketing" },
                new CareerSubject { CareerId = digitalMarketing.Id, SubjectId = contentMarketing.Id, Priority = 2, Reason = "Hỗ trợ chiến dịch marketing" },
                // Content Creator Subjects
                new CareerSubject { CareerId = contentCreator.Id, SubjectId = contentMarketing.Id, Priority = 1, Reason = "Core skill cho content creator" },
                // Financial Analyst Subjects
                new CareerSubject { CareerId = financialAnalyst.Id, SubjectId = financialAccounting.Id, Priority = 1, Reason = "Cần thiết để hiểu báo cáo tài chính" },
                new CareerSubject { CareerId = financialAnalyst.Id, SubjectId = investmentAnalysis.Id, Priority = 1, Reason = "Core skill cho financial analyst" },
                // Accountant Subjects
                new CareerSubject { CareerId = accountant.Id, SubjectId = financialAccounting.Id, Priority = 1, Reason = "Core skill cho accountant" }
            };

            await context.CareerSubjects.AddRangeAsync(careerSubjects);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {careerSubjects.Count} CareerSubjects.");
        }

        private static async Task<List<Quiz>> SeedQuizzesAsync(ApplicationDbContext context, List<Field> fields)
        {
            Console.WriteLine("Seeding Quizzes...");
            var cnttField = fields.First(f => f.Code == "CNTT");
            var marketingField = fields.First(f => f.Code == "MARKETING");
            var kinhTeField = fields.First(f => f.Code == "KINH_TE");

            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    Code = "QUIZ_CNTT",
                    Title = "Trắc Nghiệm Định Hướng Nghề CNTT",
                    Description = "Khám phá nghề nghiệp phù hợp với bạn trong lĩnh vực CNTT",
                    FieldId = cnttField.Id,
                   
                    DurationMinutes = 15
                },
                new Quiz
                {
                    Code = "QUIZ_MARKETING",
                    Title = "Trắc Nghiệm Định Hướng Nghề Marketing",
                    Description = "Khám phá nghề nghiệp phù hợp với bạn trong lĩnh vực Marketing",
                    FieldId = marketingField.Id,
                    
                    DurationMinutes = 15
                },
                new Quiz
                {
                    Code = "QUIZ_KINH_TE",
                    Title = "Trắc Nghiệm Định Hướng Nghề Kinh Tế",
                    Description = "Khám phá nghề nghiệp phù hợp với bạn trong lĩnh vực Kinh tế - Tài chính",
                    FieldId = kinhTeField.Id,
                  
                    DurationMinutes = 15
                }
            };

            await context.Quizzes.AddRangeAsync(quizzes);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {quizzes.Count} Quizzes.");
            return quizzes;
        }

        private static async Task<List<Topic>> SeedTopicsAsync(ApplicationDbContext context, List<Subject> subjects)
        {
            Console.WriteLine("Seeding Topics...");
            
            var lapTrinhCoBan = subjects.First(s => s.Code == "LAP_TRINH_CO_BAN");
            var cauTrucDuLieu = subjects.First(s => s.Code == "CAU_TRUC_DU_LIEU");
            var coSoDuLieu = subjects.First(s => s.Code == "CO_SO_DU_LIEU");
            var webDev = subjects.First(s => s.Code == "WEB_DEVELOPMENT");

            var topics = new List<Topic>
            {
                // Topics cho Lập Trình Cơ Bản
                new Topic { SubjectId = lapTrinhCoBan.Id, Name = "Biến & Kiểu Dữ Liệu", Description = "Hiểu về biến, kiểu dữ liệu cơ bản", DifficultyLevel = 1 },
                new Topic { SubjectId = lapTrinhCoBan.Id, Name = "Cấu Trúc Điều Khiển", Description = "If/else, switch, loops", DifficultyLevel = 1 },
                new Topic { SubjectId = lapTrinhCoBan.Id, Name = "Hàm & Procedure", Description = "Định nghĩa và gọi hàm", DifficultyLevel = 2 },
                // Topics cho Cấu Trúc Dữ Liệu
                new Topic { SubjectId = cauTrucDuLieu.Id, Name = "Array & List", Description = "Mảng và danh sách", DifficultyLevel = 2 },
                new Topic { SubjectId = cauTrucDuLieu.Id, Name = "Stack & Queue", Description = "Ngăn xếp và hàng đợi", DifficultyLevel = 3 },
                new Topic { SubjectId = cauTrucDuLieu.Id, Name = "Tree & Graph", Description = "Cây và đồ thị", DifficultyLevel = 4 },
                // Topics cho Cơ Sở Dữ Liệu
                new Topic { SubjectId = coSoDuLieu.Id, Name = "SQL Cơ Bản", Description = "SELECT, INSERT, UPDATE, DELETE", DifficultyLevel = 1 },
                new Topic { SubjectId = coSoDuLieu.Id, Name = "Database Design", Description = "ERD, normalization", DifficultyLevel = 3 },
                // Topics cho Web Development
                new Topic { SubjectId = webDev.Id, Name = "HTML & CSS", Description = "Cấu trúc và style web", DifficultyLevel = 1 },
                new Topic { SubjectId = webDev.Id, Name = "JavaScript Cơ Bản", Description = "Variables, functions, DOM", DifficultyLevel = 2 },
                new Topic { SubjectId = webDev.Id, Name = "React Framework", Description = "Components, state, props", DifficultyLevel = 3 }
            };

            await context.Topics.AddRangeAsync(topics);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {topics.Count} Topics.");
            return topics;
        }

        private static async Task<List<RecommendationQuestion>> SeedQuestionsAsync(ApplicationDbContext context, List<Quiz> quizzes)
        {
            Console.WriteLine("Seeding RecommendationQuestions...");
            var cnttQuiz = quizzes.First(q => q.Code == "QUIZ_CNTT");

            var questions = new List<RecommendationQuestion>
            {
                // Questions cho Quiz CNTT
                new RecommendationQuestion
                {
                    Content = "Bạn thích làm việc với giao diện người dùng hay xử lý logic phía sau?",
                    QuizId = cnttQuiz.Id
                },
                new RecommendationQuestion
                {
                    Content = "Bạn quan tâm hơn đến dữ liệu và phân tích thống kê?",
                    QuizId = cnttQuiz.Id
                },
                new RecommendationQuestion
                {
                    Content = "Bạn thích làm việc với infrastructure, server và deployment?",
                    QuizId = cnttQuiz.Id
                },
                new RecommendationQuestion
                {
                    Content = "Bạn thích phát triển ứng dụng cho mobile devices?",
                    QuizId = cnttQuiz.Id
                },
                new RecommendationQuestion
                {
                    Content = "Bạn hứng thú với machine learning và AI?",
                    QuizId = cnttQuiz.Id
                }
            };

            await context.RecommendationQuestions.AddRangeAsync(questions);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {questions.Count} RecommendationQuestions.");
            return questions;
        }

        private static async Task<List<RecommendationAnswer>> SeedAnswersAsync(ApplicationDbContext context, List<RecommendationQuestion> questions)
        {
            Console.WriteLine("Seeding RecommendationAnswers...");
            
            var question1 = questions[0];
            var question2 = questions[1];
            var question3 = questions[2];
            var question4 = questions[3];
            var question5 = questions[4];

            var answers = new List<RecommendationAnswer>
            {
                // Answers cho Question 1 (Frontend vs Backend)
                new RecommendationAnswer { Content = "Thích thiết kế giao diện đẹp, tương tác người dùng", RecommendationQuestionId = question1.Id },
                new RecommendationAnswer { Content = "Thích xử lý logic, thuật toán, database", RecommendationQuestionId = question1.Id },
                new RecommendationAnswer { Content = "Thích cả hai", RecommendationQuestionId = question1.Id },

                // Answers cho Question 2 (Data Science)
                new RecommendationAnswer { Content = "Rất quan tâm, thích tìm kiếm pattern từ dữ liệu", RecommendationQuestionId = question2.Id },
                new RecommendationAnswer { Content = "Hơi quan tâm nhưng không phải ưu tiên", RecommendationQuestionId = question2.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = question2.Id },

                // Answers cho Question 3 (DevOps)
                new RecommendationAnswer { Content = "Thích setup server, CI/CD, cloud", RecommendationQuestionId = question3.Id },
                new RecommendationAnswer { Content = "Thích nhưng không chuyên sâu", RecommendationQuestionId = question3.Id },
                new RecommendationAnswer { Content = "Không thích, muốn tập trung vào code", RecommendationQuestionId = question3.Id },

                // Answers cho Question 4 (Mobile)
                new RecommendationAnswer { Content = "Rất thích, muốn build app cho iOS/Android", RecommendationQuestionId = question4.Id },
                new RecommendationAnswer { Content = "Hơi thích nhưng không chuyên sâu", RecommendationQuestionId = question4.Id },
                new RecommendationAnswer { Content = "Thích web hơn", RecommendationQuestionId = question4.Id },

                // Answers cho Question 5 (AI/ML)
                new RecommendationAnswer { Content = "Rất hứng thú, muốn học AI/ML", RecommendationQuestionId = question5.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú nhưng không chuyên sâu", RecommendationQuestionId = question5.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = question5.Id }
            };

            await context.RecommendationAnswers.AddRangeAsync(answers);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {answers.Count} RecommendationAnswers.");
            return answers;
        }

        private static async Task SeedAnswerCareerWeightsAsync(ApplicationDbContext context, List<RecommendationAnswer> answers, List<Career> careers)
        {
            Console.WriteLine("Seeding AnswerCareerWeights (QUAN TRỌNG NHẤT)...");
            
            var frontendDev = careers.First(c => c.Code == "FRONTEND_DEV");
            var backendDev = careers.First(c => c.Code == "BACKEND_DEV");
            var dataScientist = careers.First(c => c.Code == "DATA_SCIENTIST");
            var devOps = careers.First(c => c.Code == "DEVOPS_ENGINEER");
            var mobileDev = careers.First(c => c.Code == "MOBILE_DEV");

            var answerCareerWeights = new List<AnswerCareerWeight>
            {
                // Question 1: Frontend vs Backend vs Both
                // Answer 1: Thích giao diện → Frontend++, Backend--, Mobile+
                new AnswerCareerWeight { RecommendationAnswerId = answers[0].Id, CareerId = frontendDev.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[0].Id, CareerId = backendDev.Id, Weight = -2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[0].Id, CareerId = mobileDev.Id, Weight = 3 },

                // Answer 2: Thích logic → Frontend--, Backend++, Data Science+
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = frontendDev.Id, Weight = -2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = backendDev.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = dataScientist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = devOps.Id, Weight = 1 },

                // Answer 3: Thích cả hai → Frontend+, Backend+, Mobile+
                new AnswerCareerWeight { RecommendationAnswerId = answers[2].Id, CareerId = frontendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[2].Id, CareerId = backendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[2].Id, CareerId = mobileDev.Id, Weight = 2 },

                // Question 2: Data Science interest
                // Answer 4: Rất quan tâm dữ liệu → Data Science++, Backend+, Frontend-
                new AnswerCareerWeight { RecommendationAnswerId = answers[3].Id, CareerId = dataScientist.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[3].Id, CareerId = backendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[3].Id, CareerId = frontendDev.Id, Weight = -1 },

                // Answer 5: Hơi quan tâm → Data Science+, Backend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[4].Id, CareerId = dataScientist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[4].Id, CareerId = backendDev.Id, Weight = 1 },

                // Answer 6: Không quan tâm → Data Science--, Frontend+, Mobile+
                new AnswerCareerWeight { RecommendationAnswerId = answers[5].Id, CareerId = dataScientist.Id, Weight = -3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[5].Id, CareerId = frontendDev.Id, Weight = 1 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[5].Id, CareerId = mobileDev.Id, Weight = 1 },

                // Question 3: DevOps interest
                // Answer 7: Thích infrastructure → DevOps++, Backend++, Frontend--
                new AnswerCareerWeight { RecommendationAnswerId = answers[6].Id, CareerId = devOps.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[6].Id, CareerId = backendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[6].Id, CareerId = frontendDev.Id, Weight = -2 },

                // Answer 8: Thích nhưng không chuyên → DevOps+, Backend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[7].Id, CareerId = devOps.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[7].Id, CareerId = backendDev.Id, Weight = 1 },

                // Answer 9: Không thích → DevOps--, Frontend++, Mobile++
                new AnswerCareerWeight { RecommendationAnswerId = answers[8].Id, CareerId = devOps.Id, Weight = -3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[8].Id, CareerId = frontendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[8].Id, CareerId = mobileDev.Id, Weight = 1 },

                // Question 4: Mobile interest
                // Answer 10: Rất thích mobile → Mobile++, Frontend+, Backend-
                new AnswerCareerWeight { RecommendationAnswerId = answers[9].Id, CareerId = mobileDev.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[9].Id, CareerId = frontendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[9].Id, CareerId = backendDev.Id, Weight = -1 },

                // Answer 11: Hơi thích → Mobile+, Frontend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[10].Id, CareerId = mobileDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[10].Id, CareerId = frontendDev.Id, Weight = 1 },

                // Answer 12: Thích web hơn → Mobile--, Frontend++, Backend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[11].Id, CareerId = mobileDev.Id, Weight = -3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[11].Id, CareerId = frontendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[11].Id, CareerId = backendDev.Id, Weight = 1 },

                // Question 5: AI/ML interest
                // Answer 13: Rất hứng thú AI → Data Science++, Backend++, DevOps+
                new AnswerCareerWeight { RecommendationAnswerId = answers[12].Id, CareerId = dataScientist.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[12].Id, CareerId = backendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[12].Id, CareerId = devOps.Id, Weight = 1 },

                // Answer 14: Hơi hứng thú → Data Science+, Backend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[13].Id, CareerId = dataScientist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[13].Id, CareerId = backendDev.Id, Weight = 1 },

                // Answer 15: Không hứng thú → Data Science--, Frontend++, Mobile++
                new AnswerCareerWeight { RecommendationAnswerId = answers[14].Id, CareerId = dataScientist.Id, Weight = -3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[14].Id, CareerId = frontendDev.Id, Weight = 1 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[14].Id, CareerId = mobileDev.Id, Weight = 1 }
            };

            await context.AnswerCareerWeights.AddRangeAsync(answerCareerWeights);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {answerCareerWeights.Count} AnswerCareerWeights.");
        }
 
       }
   
 }