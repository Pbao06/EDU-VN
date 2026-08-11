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

            // 7. Seed Questions (Recommendation)
            var questions = await SeedQuestionsAsync(context, quizzes);

            // 8. Seed Answers (Recommendation)
            var answers = await SeedAnswersAsync(context, questions);

            // 9. Seed AnswerCareerWeights (QUAN TRỌNG NHẤT)
            await SeedAnswerCareerWeightsAsync(context, answers, careers);

            // 10. Seed Learning Questions (knowledge test for subjects)
            var learningQuestions = await SeedLearningQuestionsAsync(context, topics);

            // 11. Seed Learning Answers (knowledge test answers)
            await SeedLearningAnswersAsync(context, learningQuestions);

            Console.WriteLine("Data seeding completed successfully!");
        }

        private static async Task<List<Field>> SeedFieldsAsync(ApplicationDbContext context)
        {
            Console.WriteLine("Seeding Fields...");
            var seedFields = new List<Field>
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

            // Only insert Fields that do not already exist (detect by business key: Code)
            var seedCodes = seedFields.Select(f => f.Code).ToList();
            var existingCodes = await context.Fields
                .Where(f => seedCodes.Contains(f.Code))
                .Select(f => f.Code)
                .ToListAsync();

            var newFields = seedFields.Where(f => !existingCodes.Contains(f.Code)).ToList();
            if (newFields.Count > 0)
            {
                await context.Fields.AddRangeAsync(newFields);
                await context.SaveChangesAsync();
            }

            // Return ALL Fields (existing + newly inserted) so downstream methods use real DB IDs
            var allFields = await context.Fields
                .Where(f => seedCodes.Contains(f.Code))
                .OrderBy(f => f.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newFields.Count} new Fields. Total Fields in DB: {allFields.Count}.");
            return allFields;
        }

        private static async Task<List<Career>> SeedCareersAsync(ApplicationDbContext context, List<Field> fields)
        {
            Console.WriteLine("Seeding Careers...");
            var cnttField = fields.First(f => f.Code == "CNTT");
            var marketingField = fields.First(f => f.Code == "MARKETING");
            var kinhTeField = fields.First(f => f.Code == "KINH_TE");
            var yTeField = fields.First(f => f.Code == "Y_TE");
            var giaoDucField = fields.First(f => f.Code == "GIAO_DUC");

            var seedCareers = new List<Career>
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
                new Career
                {
                    Code = "AI_ENGINEER",
                    FieldId = cnttField.Id,
                    Name = "AI Engineer",
                    Description = "Xây dựng và triển khai các hệ thống trí tuệ nhân tạo, model machine learning",
                    Responsibilities = "Phát triển model AI/ML, xử lý dữ liệu lớn, triển khai mô hình vào sản phẩm, tối ưu hiệu suất model",
                    MinSalary = 25,
                    MaxSalary = 55,
                    DemandLevel = "VeryHigh",
                    IconUrl = "/icons/aiengineer.png",
                    PopularityScore = 92
                },
                new Career
                {
                    Code = "CYBERSECURITY_ENGINEER",
                    FieldId = cnttField.Id,
                    Name = "Kỹ Sư An Ninh Mạng",
                    Description = "Bảo vệ hệ thống, mạng và dữ liệu khỏi các mối đe dọa tấn công mạng",
                    Responsibilities = "Phân tích và phòng chống tấn công mạng, kiểm tra bảo mật, quản lý firewall và IDS/IPS, ứng phó sự cố",
                    MinSalary = 22,
                    MaxSalary = 50,
                    DemandLevel = "High",
                    IconUrl = "/icons/cybersecurity.png",
                    PopularityScore = 88
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
                new Career
                {
                    Code = "BRAND_MANAGER",
                    FieldId = marketingField.Id,
                    Name = "Brand Manager",
                    Description = "Xây dựng và phát triển chiến lược thương hiệu, định vị sản phẩm trên thị trường",
                    Responsibilities = "Xây dựng chiến lược thương hiệu, quản lý brand identity, phân tích thị trường, phối hợp chiến dịch truyền thông",
                    MinSalary = 15,
                    MaxSalary = 35,
                    DemandLevel = "High",
                    IconUrl = "/icons/brandmanager.png",
                    PopularityScore = 80
                },
                new Career
                {
                    Code = "SOCIAL_MEDIA_SPECIALIST",
                    FieldId = marketingField.Id,
                    Name = "Chuyên Viên Social Media",
                    Description = "Quản lý và phát triển kênh mạng xã hội, tạo nội dung tương tác",
                    Responsibilities = "Quản lý fanpage/kênh mạng xã hội, lên kế hoạch nội dung, phân tích hiệu quả, tương tác cộng đồng",
                    MinSalary = 10,
                    MaxSalary = 22,
                    DemandLevel = "Medium",
                    IconUrl = "/icons/socialmedia.png",
                    PopularityScore = 84
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
                },
                new Career
                {
                    Code = "INVESTMENT_ANALYST",
                    FieldId = kinhTeField.Id,
                    Name = "Chuyên Viên Phân Tích Đầu Tư",
                    Description = "Phân tích và đánh giá cơ hội đầu tư, quản lý danh mục tài sản",
                    Responsibilities = "Phân tích thị trường chứng khoán, đánh giá cổ phiếu, quản lý danh mục đầu tư, lập báo cáo phân tích",
                    MinSalary = 15,
                    MaxSalary = 40,
                    DemandLevel = "High",
                    IconUrl = "/icons/investmentanalyst.png",
                    PopularityScore = 82
                },
                new Career
                {
                    Code = "BANKING_SPECIALIST",
                    FieldId = kinhTeField.Id,
                    Name = "Chuyên Viên Ngân Hàng",
                    Description = "Xử lý nghiệp vụ tín dụng, quản lý quan hệ khách hàng ngân hàng",
                    Responsibilities = "Thẩm định hồ sơ tín dụng, quản lý quan hệ khách hàng, phân tích rủi ro, tư vấn sản phẩm ngân hàng",
                    MinSalary = 12,
                    MaxSalary = 30,
                    DemandLevel = "High",
                    IconUrl = "/icons/banking.png",
                    PopularityScore = 78
                },
                // Y Tế Careers
                new Career
                {
                    Code = "PHARMACIST",
                    FieldId = yTeField.Id,
                    Name = "Dược Sĩ",
                    Description = "Tư vấn và cấp phát thuốc, đảm bảo sử dụng thuốc an toàn và hiệu quả",
                    Responsibilities = "Kiểm tra và cấp phát thuốc, tư vấn sử dụng thuốc, kiểm soát chất lượng dược phẩm, theo dõi tương tác thuốc",
                    MinSalary = 12,
                    MaxSalary = 28,
                    DemandLevel = "High",
                    IconUrl = "/icons/pharmacist.png",
                    PopularityScore = 80
                },
                new Career
                {
                    Code = "MEDICAL_LAB_TECHNICIAN",
                    FieldId = yTeField.Id,
                    Name = "Kỹ Thuật Viên Xét Nghiệm",
                    Description = "Thực hiện các xét nghiệm y khoa hỗ trợ chẩn đoán và điều trị bệnh",
                    Responsibilities = "Lấy và xử lý mẫu bệnh phẩm, thực hiện xét nghiệm, kiểm soát chất lượng phòng lab, báo cáo kết quả",
                    MinSalary = 10,
                    MaxSalary = 22,
                    DemandLevel = "Medium",
                    IconUrl = "/icons/medlab.png",
                    PopularityScore = 76
                },
                // Giáo Dục Careers
                new Career
                {
                    Code = "EDUCATIONAL_CONSULTANT",
                    FieldId = giaoDucField.Id,
                    Name = "Chuyên Viên Tư Vấn Giáo Dục",
                    Description = "Tư vấn lộ trình học tập, hướng nghiệp và phát triển cá nhân cho học viên",
                    Responsibilities = "Tư vấn lộ trình học tập, đánh giá năng lực học viên, hỗ trợ chọn trường/ngành, xây dựng kế hoạch phát triển",
                    MinSalary = 10,
                    MaxSalary = 25,
                    DemandLevel = "Medium",
                    IconUrl = "/icons/educonsultant.png",
                    PopularityScore = 74
                },
                new Career
                {
                    Code = "INSTRUCTIONAL_DESIGNER",
                    FieldId = giaoDucField.Id,
                    Name = "Chuyên Viên Thiết Kế Giảng Dạy",
                    Description = "Thiết kế chương trình, giáo trình và nội dung đào tạo hiệu quả",
                    Responsibilities = "Thiết kế chương trình đào tạo, xây dựng giáo trình, phát triển nội dung e-learning, đánh giá hiệu quả đào tạo",
                    MinSalary = 12,
                    MaxSalary = 28,
                    DemandLevel = "Medium",
                    IconUrl = "/icons/instructionaldesign.png",
                    PopularityScore = 72
                }
            };

            // Only insert Careers that do not already exist (detect by business key: Code)
            var seedCodes = seedCareers.Select(c => c.Code).ToList();
            var existingCodes = await context.Careers
                .Where(c => seedCodes.Contains(c.Code))
                .Select(c => c.Code)
                .ToListAsync();

            var newCareers = seedCareers.Where(c => !existingCodes.Contains(c.Code)).ToList();
            if (newCareers.Count > 0)
            {
                await context.Careers.AddRangeAsync(newCareers);
                await context.SaveChangesAsync();
            }

            // Return ALL Careers (existing + newly inserted) so downstream methods use real DB IDs
            var allCareers = await context.Careers
                .Where(c => seedCodes.Contains(c.Code))
                .OrderBy(c => c.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newCareers.Count} new Careers. Total Careers in DB: {allCareers.Count}.");
            return allCareers;
        }

        private static async Task<List<Subject>> SeedSubjectsAsync(ApplicationDbContext context)
        {
            Console.WriteLine("Seeding Subjects...");
            var seedSubjects = new List<Subject>
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
                new Subject
                {
                    Code = "HTML_CSS",
                    Name = "HTML & CSS",
                    Description = "Cấu trúc trang web và styling",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "JAVASCRIPT",
                    Name = "JavaScript",
                    Description = "Ngôn ngữ lập trình cho frontend, DOM manipulation",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "REACT",
                    Name = "React",
                    Description = "Thư viện UI hiện đại, components, hooks",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "C_SHARP_FUNDAMENTALS",
                    Name = "C# Fundamentals",
                    Description = "Cú pháp C#, biến, kiểu dữ liệu, cấu trúc điều khiển",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "OOP_C_SHARP",
                    Name = "OOP với C#",
                    Description = "Lập trình hướng đối tượng trong C#: class, inheritance, polymorphism",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "SQL_DATABASE",
                    Name = "SQL & Database",
                    Description = "Truy vấn SQL, thiết kế cơ sở dữ liệu, JOIN, GROUP BY",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "ASPNET_CORE",
                    Name = "ASP.NET Core",
                    Description = "Xây dựng Web API với ASP.NET Core, routing, DI, Middleware",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "EF_CORE_LINQ",
                    Name = "Entity Framework Core & LINQ",
                    Description = "Truy cập dữ liệu với EF Core, LINQ queries, migrations",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "MACHINE_LEARNING",
                    Name = "Machine Learning",
                    Description = "Học máy, thuật toán ML, đánh giá model",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "AI_FUNDAMENTALS",
                    Name = "Trí Tuệ Nhân Tạo Cơ Bản",
                    Description = "Giới thiệu AI, neural network, deep learning",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "PYTHON_ML",
                    Name = "Python cho ML",
                    Description = "Lập trình Python, NumPy, Pandas, scikit-learn",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "NETWORK_SECURITY",
                    Name = "An Ninh Mạng",
                    Description = "Bảo mật mạng, firewall, IDS/IPS, mã hóa",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "CYBERSECURITY_FUNDAMENTALS",
                    Name = "Bảo Mật Thông Tin Cơ Bản",
                    Description = "Kiến thức nền tảng về bảo mật, mối đe dọa, phòng chống",
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
                new Subject
                {
                    Code = "BRAND_MANAGEMENT",
                    Name = "Quản Trị Thương Hiệu",
                    Description = "Chiến lược thương hiệu, brand identity, định vị",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "MARKETING_STRATEGY",
                    Name = "Chiến Lược Marketing",
                    Description = "Lập kế hoạch marketing, phân tích thị trường, 4P",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "SOCIAL_MEDIA_STRATEGY",
                    Name = "Chiến Lược Social Media",
                    Description = "Quản lý kênh mạng xã hội, lên kế hoạch nội dung, tương tác",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "CONTENT_STRATEGY",
                    Name = "Chiến Lược Nội Dung",
                    Description = "Xây dựng kế hoạch nội dung, đa kênh, đo lường hiệu quả",
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
                },
                new Subject
                {
                    Code = "FINANCIAL_MODELING",
                    Name = "Mô Hình Tài Chính",
                    Description = "Xây dựng mô hình tài chính, định giá doanh nghiệp",
                    Type = "Specialized"
                },
                new Subject
                {
                    Code = "BANKING_FUNDAMENTALS",
                    Name = "Nghiệp Vụ Ngân Hàng",
                    Description = "Tín dụng, thanh toán, quản trị rủi ro ngân hàng",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "FINANCIAL_REGULATION",
                    Name = "Luật & Quy Định Tài Chính",
                    Description = "Quy định ngân hàng, luật chứng khoán, tuân thủ",
                    Type = "Specialized"
                },
                // Y Tế Subjects
                new Subject
                {
                    Code = "PHARMACOLOGY",
                    Name = "Dược Lý Học",
                    Description = "Cơ chế tác dụng của thuốc, dược động học, tương tác thuốc",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "PHARMACEUTICAL_SCIENCE",
                    Name = "Khoa Học Dược",
                    Description = "Bào chế thuốc, kiểm soát chất lượng, dược lâm sàng",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "MEDICAL_LAB_SCIENCE",
                    Name = "Kỹ Thuật Xét Nghiệm",
                    Description = "Kỹ thuật xét nghiệm huyết học, sinh hóa, vi sinh",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "CLINICAL_DIAGNOSTICS",
                    Name = "Chẩn Đoán Lâm Sàng",
                    Description = "Giải thích kết quả xét nghiệm, quy trình chẩn đoán",
                    Type = "Specialized"
                },
                // Giáo Dục Subjects
                new Subject
                {
                    Code = "EDUCATIONAL_CONSULTING",
                    Name = "Tư Vấn Giáo Dục",
                    Description = "Tư vấn lộ trình học tập, đánh giá năng lực",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "CAREER_COUNSELING",
                    Name = "Tư Vấn Hướng Nghiệp",
                    Description = "Định hướng nghề nghiệp, phát triển cá nhân",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "INSTRUCTIONAL_DESIGN",
                    Name = "Thiết Kế Giảng Dạy",
                    Description = "Mô hình thiết kế giảng dạy, xây dựng giáo trình",
                    Type = "Core"
                },
                new Subject
                {
                    Code = "E_LEARNING_DEVELOPMENT",
                    Name = "Phát Triển E-Learning",
                    Description = "Xây dựng khóa học online, công cụ e-learning, LMS",
                    Type = "Specialized"
                }
            };

            // Only insert Subjects that do not already exist (detect by business key: Code)
            var seedCodes = seedSubjects.Select(s => s.Code).ToList();
            var existingCodes = await context.Subjects
                .Where(s => seedCodes.Contains(s.Code))
                .Select(s => s.Code)
                .ToListAsync();

            var newSubjects = seedSubjects.Where(s => !existingCodes.Contains(s.Code)).ToList();
            if (newSubjects.Count > 0)
            {
                await context.Subjects.AddRangeAsync(newSubjects);
                await context.SaveChangesAsync();
            }

            // Return ALL Subjects (existing + newly inserted) so downstream methods use real DB IDs
            var allSubjects = await context.Subjects
                .Where(s => seedCodes.Contains(s.Code))
                .OrderBy(s => s.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newSubjects.Count} new Subjects. Total Subjects in DB: {allSubjects.Count}.");
            return allSubjects;
        }

        private static async Task SeedCareerSubjectsAsync(ApplicationDbContext context, List<Career> careers, List<Subject> subjects)
        {
            Console.WriteLine("Seeding CareerSubjects...");
            
            var frontendDev = careers.First(c => c.Code == "FRONTEND_DEV");
            var backendDev = careers.First(c => c.Code == "BACKEND_DEV");
            var dataScientist = careers.First(c => c.Code == "DATA_SCIENTIST");
            var devOps = careers.First(c => c.Code == "DEVOPS_ENGINEER");
            var mobileDev = careers.First(c => c.Code == "MOBILE_DEV");
            var aiEngineer = careers.First(c => c.Code == "AI_ENGINEER");
            var cyberSecurity = careers.First(c => c.Code == "CYBERSECURITY_ENGINEER");
            var digitalMarketing = careers.First(c => c.Code == "DIGITAL_MARKETING");
            var contentCreator = careers.First(c => c.Code == "CONTENT_CREATOR");
            var brandManager = careers.First(c => c.Code == "BRAND_MANAGER");
            var socialMedia = careers.First(c => c.Code == "SOCIAL_MEDIA_SPECIALIST");
            var financialAnalyst = careers.First(c => c.Code == "FINANCIAL_ANALYST");
            var accountant = careers.First(c => c.Code == "ACCOUNTANT");
            var investmentAnalyst = careers.First(c => c.Code == "INVESTMENT_ANALYST");
            var bankingSpecialist = careers.First(c => c.Code == "BANKING_SPECIALIST");
            var pharmacist = careers.First(c => c.Code == "PHARMACIST");
            var medLabTech = careers.First(c => c.Code == "MEDICAL_LAB_TECHNICIAN");
            var eduConsultant = careers.First(c => c.Code == "EDUCATIONAL_CONSULTANT");
            var instructionalDesigner = careers.First(c => c.Code == "INSTRUCTIONAL_DESIGNER");

            var lapTrinhCoBan = subjects.First(s => s.Code == "LAP_TRINH_CO_BAN");
            var cauTrucDuLieu = subjects.First(s => s.Code == "CAU_TRUC_DU_LIEU");
            var coSoDuLieu = subjects.First(s => s.Code == "CO_SO_DU_LIEU");
            var webDev = subjects.First(s => s.Code == "WEB_DEVELOPMENT");
            var oop = subjects.First(s => s.Code == "OOP");
            var htmlCss = subjects.First(s => s.Code == "HTML_CSS");
            var javaScript = subjects.First(s => s.Code == "JAVASCRIPT");
            var react = subjects.First(s => s.Code == "REACT");
            var cSharpFund = subjects.First(s => s.Code == "C_SHARP_FUNDAMENTALS");
            var oopCSharp = subjects.First(s => s.Code == "OOP_C_SHARP");
            var sqlDb = subjects.First(s => s.Code == "SQL_DATABASE");
            var aspNetCore = subjects.First(s => s.Code == "ASPNET_CORE");
            var efCoreLinq = subjects.First(s => s.Code == "EF_CORE_LINQ");
            var machineLearning = subjects.First(s => s.Code == "MACHINE_LEARNING");
            var aiFund = subjects.First(s => s.Code == "AI_FUNDAMENTALS");
            var pythonMl = subjects.First(s => s.Code == "PYTHON_ML");
            var networkSecurity = subjects.First(s => s.Code == "NETWORK_SECURITY");
            var cyberFund = subjects.First(s => s.Code == "CYBERSECURITY_FUNDAMENTALS");
            var digitalMarketingFund = subjects.First(s => s.Code == "DIGITAL_MARKETING_FUND");
            var contentMarketing = subjects.First(s => s.Code == "CONTENT_MARKETING");
            var brandManagement = subjects.First(s => s.Code == "BRAND_MANAGEMENT");
            var marketingStrategy = subjects.First(s => s.Code == "MARKETING_STRATEGY");
            var socialMediaStrategy = subjects.First(s => s.Code == "SOCIAL_MEDIA_STRATEGY");
            var contentStrategy = subjects.First(s => s.Code == "CONTENT_STRATEGY");
            var financialAccounting = subjects.First(s => s.Code == "FINANCIAL_ACCOUNTING");
            var investmentAnalysis = subjects.First(s => s.Code == "INVESTMENT_ANALYSIS");
            var financialModeling = subjects.First(s => s.Code == "FINANCIAL_MODELING");
            var bankingFund = subjects.First(s => s.Code == "BANKING_FUNDAMENTALS");
            var financialRegulation = subjects.First(s => s.Code == "FINANCIAL_REGULATION");
            var pharmacology = subjects.First(s => s.Code == "PHARMACOLOGY");
            var pharmScience = subjects.First(s => s.Code == "PHARMACEUTICAL_SCIENCE");
            var medLabScience = subjects.First(s => s.Code == "MEDICAL_LAB_SCIENCE");
            var clinicalDx = subjects.First(s => s.Code == "CLINICAL_DIAGNOSTICS");
            var eduConsulting = subjects.First(s => s.Code == "EDUCATIONAL_CONSULTING");
            var careerCounseling = subjects.First(s => s.Code == "CAREER_COUNSELING");
            var instructionalDesign = subjects.First(s => s.Code == "INSTRUCTIONAL_DESIGN");
            var eLearning = subjects.First(s => s.Code == "E_LEARNING_DEVELOPMENT");

            var seedCareerSubjects = new List<CareerSubject>
            {
                // Frontend Developer Subjects
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để hiểu logic code" },
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = webDev.Id, Priority = 1, Reason = "Core skill cho frontend" },
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = oop.Id, Priority = 2, Reason = "Giúp tổ chức code tốt hơn" },
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = htmlCss.Id, Priority = 1, Reason = "Nền tảng cấu trúc và style" },
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = javaScript.Id, Priority = 1, Reason = "Ngôn ngữ cốt lõi của frontend" },
                new CareerSubject { CareerId = frontendDev.Id, SubjectId = react.Id, Priority = 2, Reason = "Framework UI phổ biến nhất" },
                // Backend Developer Subjects
                new CareerSubject { CareerId = backendDev.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để hiểu lập trình" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = cauTrucDuLieu.Id, Priority = 1, Reason = "Quan trọng cho performance" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = coSoDuLieu.Id, Priority = 1, Reason = "Core skill cho backend" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = oop.Id, Priority = 2, Reason = "Giúp thiết kế architecture tốt" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = cSharpFund.Id, Priority = 1, Reason = "Ngôn ngữ backend chính" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = oopCSharp.Id, Priority = 1, Reason = "Kiến trúc hướng đối tượng C#" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = sqlDb.Id, Priority = 1, Reason = "Quản lý và truy vấn dữ liệu" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = aspNetCore.Id, Priority = 2, Reason = "Xây dựng Web API" },
                new CareerSubject { CareerId = backendDev.Id, SubjectId = efCoreLinq.Id, Priority = 2, Reason = "Truy cập dữ liệu EF Core" },
                // Data Scientist Subjects
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để viết code ML" },
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = cauTrucDuLieu.Id, Priority = 1, Reason = "Quan trọng cho thuật toán ML" },
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = coSoDuLieu.Id, Priority = 1, Reason = "Cần thiết để xử lý dữ liệu" },
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = machineLearning.Id, Priority = 1, Reason = "Core skill cho data science" },
                new CareerSubject { CareerId = dataScientist.Id, SubjectId = pythonMl.Id, Priority = 1, Reason = "Công cụ chính để phân tích dữ liệu" },
                // DevOps Subjects
                new CareerSubject { CareerId = devOps.Id, SubjectId = lapTrinhCoBan.Id, Priority = 2, Reason = "Cần thiết để viết script" },
                new CareerSubject { CareerId = devOps.Id, SubjectId = coSoDuLieu.Id, Priority = 1, Reason = "Quan trọng để quản lý data" },
                new CareerSubject { CareerId = devOps.Id, SubjectId = networkSecurity.Id, Priority = 2, Reason = "Hiểu hạ tầng mạng và bảo mật" },
                // Mobile Developer Subjects
                new CareerSubject { CareerId = mobileDev.Id, SubjectId = lapTrinhCoBan.Id, Priority = 1, Reason = "Cần thiết để hiểu lập trình" },
                new CareerSubject { CareerId = mobileDev.Id, SubjectId = webDev.Id, Priority = 1, Reason = "Giúp phát triển cross-platform" },
                new CareerSubject { CareerId = mobileDev.Id, SubjectId = oop.Id, Priority = 2, Reason = "Giúp tổ chức code tốt hơn" },
                // AI Engineer Subjects
                new CareerSubject { CareerId = aiEngineer.Id, SubjectId = machineLearning.Id, Priority = 1, Reason = "Core skill cho AI engineer" },
                new CareerSubject { CareerId = aiEngineer.Id, SubjectId = aiFund.Id, Priority = 1, Reason = "Nền tảng trí tuệ nhân tạo" },
                new CareerSubject { CareerId = aiEngineer.Id, SubjectId = pythonMl.Id, Priority = 1, Reason = "Công cụ chính để phát triển model" },
                new CareerSubject { CareerId = aiEngineer.Id, SubjectId = cauTrucDuLieu.Id, Priority = 2, Reason = "Hiểu thuật toán và độ phức tạp" },
                // Cybersecurity Engineer Subjects
                new CareerSubject { CareerId = cyberSecurity.Id, SubjectId = networkSecurity.Id, Priority = 1, Reason = "Core skill cho bảo mật mạng" },
                new CareerSubject { CareerId = cyberSecurity.Id, SubjectId = cyberFund.Id, Priority = 1, Reason = "Nền tảng bảo mật thông tin" },
                new CareerSubject { CareerId = cyberSecurity.Id, SubjectId = lapTrinhCoBan.Id, Priority = 2, Reason = "Đọc và phân tích mã độc" },
                // Digital Marketing Subjects
                new CareerSubject { CareerId = digitalMarketing.Id, SubjectId = digitalMarketingFund.Id, Priority = 1, Reason = "Core skill cho digital marketing" },
                new CareerSubject { CareerId = digitalMarketing.Id, SubjectId = contentMarketing.Id, Priority = 2, Reason = "Hỗ trợ chiến dịch marketing" },
                new CareerSubject { CareerId = digitalMarketing.Id, SubjectId = marketingStrategy.Id, Priority = 2, Reason = "Lập kế hoạch chiến lược" },
                // Content Creator Subjects
                new CareerSubject { CareerId = contentCreator.Id, SubjectId = contentMarketing.Id, Priority = 1, Reason = "Core skill cho content creator" },
                new CareerSubject { CareerId = contentCreator.Id, SubjectId = contentStrategy.Id, Priority = 2, Reason = "Xây dựng kế hoạch nội dung" },
                // Brand Manager Subjects
                new CareerSubject { CareerId = brandManager.Id, SubjectId = brandManagement.Id, Priority = 1, Reason = "Core skill cho brand manager" },
                new CareerSubject { CareerId = brandManager.Id, SubjectId = marketingStrategy.Id, Priority = 1, Reason = "Hoạch định chiến lược thương hiệu" },
                new CareerSubject { CareerId = brandManager.Id, SubjectId = contentMarketing.Id, Priority = 2, Reason = "Truyền thông thương hiệu" },
                // Social Media Specialist Subjects
                new CareerSubject { CareerId = socialMedia.Id, SubjectId = socialMediaStrategy.Id, Priority = 1, Reason = "Core skill cho social media" },
                new CareerSubject { CareerId = socialMedia.Id, SubjectId = contentStrategy.Id, Priority = 1, Reason = "Lên kế hoạch nội dung" },
                new CareerSubject { CareerId = socialMedia.Id, SubjectId = digitalMarketingFund.Id, Priority = 2, Reason = "Hiểu nền tảng digital marketing" },
                // Financial Analyst Subjects
                new CareerSubject { CareerId = financialAnalyst.Id, SubjectId = financialAccounting.Id, Priority = 1, Reason = "Cần thiết để hiểu báo cáo tài chính" },
                new CareerSubject { CareerId = financialAnalyst.Id, SubjectId = investmentAnalysis.Id, Priority = 1, Reason = "Core skill cho financial analyst" },
                new CareerSubject { CareerId = financialAnalyst.Id, SubjectId = financialModeling.Id, Priority = 2, Reason = "Xây dựng mô hình tài chính" },
                // Accountant Subjects
                new CareerSubject { CareerId = accountant.Id, SubjectId = financialAccounting.Id, Priority = 1, Reason = "Core skill cho accountant" },
                new CareerSubject { CareerId = accountant.Id, SubjectId = financialRegulation.Id, Priority = 2, Reason = "Hiểu quy định kế toán thuế" },
                // Investment Analyst Subjects
                new CareerSubject { CareerId = investmentAnalyst.Id, SubjectId = investmentAnalysis.Id, Priority = 1, Reason = "Core skill phân tích đầu tư" },
                new CareerSubject { CareerId = investmentAnalyst.Id, SubjectId = financialModeling.Id, Priority = 1, Reason = "Định giá và mô hình hóa" },
                new CareerSubject { CareerId = investmentAnalyst.Id, SubjectId = financialAccounting.Id, Priority = 2, Reason = "Đọc báo cáo tài chính" },
                // Banking Specialist Subjects
                new CareerSubject { CareerId = bankingSpecialist.Id, SubjectId = bankingFund.Id, Priority = 1, Reason = "Nghiệp vụ ngân hàng cốt lõi" },
                new CareerSubject { CareerId = bankingSpecialist.Id, SubjectId = financialRegulation.Id, Priority = 1, Reason = "Tuân thủ quy định ngân hàng" },
                new CareerSubject { CareerId = bankingSpecialist.Id, SubjectId = financialAccounting.Id, Priority = 2, Reason = "Hiểu sổ sách tài chính ngân hàng" },
                // Pharmacist Subjects
                new CareerSubject { CareerId = pharmacist.Id, SubjectId = pharmacology.Id, Priority = 1, Reason = "Core skill cho dược sĩ" },
                new CareerSubject { CareerId = pharmacist.Id, SubjectId = pharmScience.Id, Priority = 1, Reason = "Kiến thức khoa học dược" },
                // Medical Lab Technician Subjects
                new CareerSubject { CareerId = medLabTech.Id, SubjectId = medLabScience.Id, Priority = 1, Reason = "Kỹ thuật xét nghiệm cốt lõi" },
                new CareerSubject { CareerId = medLabTech.Id, SubjectId = clinicalDx.Id, Priority = 1, Reason = "Giải thích kết quả chẩn đoán" },
                // Educational Consultant Subjects
                new CareerSubject { CareerId = eduConsultant.Id, SubjectId = eduConsulting.Id, Priority = 1, Reason = "Tư vấn giáo dục cốt lõi" },
                new CareerSubject { CareerId = eduConsultant.Id, SubjectId = careerCounseling.Id, Priority = 1, Reason = "Định hướng nghề nghiệp" },
                // Instructional Designer Subjects
                new CareerSubject { CareerId = instructionalDesigner.Id, SubjectId = instructionalDesign.Id, Priority = 1, Reason = "Thiết kế giảng dạy cốt lõi" },
                new CareerSubject { CareerId = instructionalDesigner.Id, SubjectId = eLearning.Id, Priority = 1, Reason = "Phát triển nội dung e-learning" }
            };

            // Only insert relationships that do not already exist (detect by CareerId + SubjectId)
            var careerIds = seedCareerSubjects.Select(cs => cs.CareerId).Distinct().ToList();
            var subjectIds = seedCareerSubjects.Select(cs => cs.SubjectId).Distinct().ToList();
            var existingRelations = await context.CareerSubjects
                .Where(cs => careerIds.Contains(cs.CareerId) && subjectIds.Contains(cs.SubjectId))
                .Select(cs => new { cs.CareerId, cs.SubjectId })
                .ToListAsync();

            var newCareerSubjects = seedCareerSubjects
                .Where(cs => !existingRelations.Any(e => e.CareerId == cs.CareerId && e.SubjectId == cs.SubjectId))
                .ToList();

            if (newCareerSubjects.Count > 0)
            {
                await context.CareerSubjects.AddRangeAsync(newCareerSubjects);
                await context.SaveChangesAsync();
            }

            Console.WriteLine($"Seeded {newCareerSubjects.Count} new CareerSubjects.");
        }

        private static async Task<List<Quiz>> SeedQuizzesAsync(ApplicationDbContext context, List<Field> fields)
        {
            Console.WriteLine("Seeding Quizzes...");
            var cnttField = fields.First(f => f.Code == "CNTT");
            var marketingField = fields.First(f => f.Code == "MARKETING");
            var kinhTeField = fields.First(f => f.Code == "KINH_TE");
            var yTeField = fields.First(f => f.Code == "Y_TE");
            var giaoDucField = fields.First(f => f.Code == "GIAO_DUC");

            var seedQuizzes = new List<Quiz>
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
                },
                new Quiz
                {
                    Code = "QUIZ_Y_TE",
                    Title = "Trắc Nghiệm Định Hướng Nghề Y Tế",
                    Description = "Khám phá nghề nghiệp phù hợp với bạn trong lĩnh vực Y tế - Sức khỏe",
                    FieldId = yTeField.Id,
                    DurationMinutes = 15
                },
                new Quiz
                {
                    Code = "QUIZ_GIAO_DUC",
                    Title = "Trắc Nghiệm Định Hướng Nghề Giáo Dục",
                    Description = "Khám phá nghề nghiệp phù hợp với bạn trong lĩnh vực Giáo dục",
                    FieldId = giaoDucField.Id,
                    DurationMinutes = 15
                }
            };

            // Only insert Quizzes that do not already exist (detect by business key: Code)
            var seedCodes = seedQuizzes.Select(q => q.Code).ToList();
            var existingCodes = await context.Quizzes
                .Where(q => seedCodes.Contains(q.Code))
                .Select(q => q.Code)
                .ToListAsync();

            var newQuizzes = seedQuizzes.Where(q => !existingCodes.Contains(q.Code)).ToList();
            if (newQuizzes.Count > 0)
            {
                await context.Quizzes.AddRangeAsync(newQuizzes);
                await context.SaveChangesAsync();
            }

            // Return ALL Quizzes (existing + newly inserted) so downstream methods use real DB IDs
            var allQuizzes = await context.Quizzes
                .Where(q => seedCodes.Contains(q.Code))
                .OrderBy(q => q.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newQuizzes.Count} new Quizzes. Total Quizzes in DB: {allQuizzes.Count}.");
            return allQuizzes;
        }

        private static async Task<List<Topic>> SeedTopicsAsync(ApplicationDbContext context, List<Subject> subjects)
        {
            Console.WriteLine("Seeding Topics...");
            
            var lapTrinhCoBan = subjects.First(s => s.Code == "LAP_TRINH_CO_BAN");
            var cauTrucDuLieu = subjects.First(s => s.Code == "CAU_TRUC_DU_LIEU");
            var coSoDuLieu = subjects.First(s => s.Code == "CO_SO_DU_LIEU");
            var webDev = subjects.First(s => s.Code == "WEB_DEVELOPMENT");
            var htmlCss = subjects.First(s => s.Code == "HTML_CSS");
            var javaScript = subjects.First(s => s.Code == "JAVASCRIPT");
            var react = subjects.First(s => s.Code == "REACT");
            var cSharpFund = subjects.First(s => s.Code == "C_SHARP_FUNDAMENTALS");
            var oopCSharp = subjects.First(s => s.Code == "OOP_C_SHARP");
            var sqlDb = subjects.First(s => s.Code == "SQL_DATABASE");
            var aspNetCore = subjects.First(s => s.Code == "ASPNET_CORE");
            var efCoreLinq = subjects.First(s => s.Code == "EF_CORE_LINQ");
            var machineLearning = subjects.First(s => s.Code == "MACHINE_LEARNING");
            var aiFund = subjects.First(s => s.Code == "AI_FUNDAMENTALS");
            var pythonMl = subjects.First(s => s.Code == "PYTHON_ML");
            var networkSecurity = subjects.First(s => s.Code == "NETWORK_SECURITY");
            var cyberFund = subjects.First(s => s.Code == "CYBERSECURITY_FUNDAMENTALS");
            var brandManagement = subjects.First(s => s.Code == "BRAND_MANAGEMENT");
            var marketingStrategy = subjects.First(s => s.Code == "MARKETING_STRATEGY");
            var socialMediaStrategy = subjects.First(s => s.Code == "SOCIAL_MEDIA_STRATEGY");
            var contentStrategy = subjects.First(s => s.Code == "CONTENT_STRATEGY");
            var financialModeling = subjects.First(s => s.Code == "FINANCIAL_MODELING");
            var bankingFund = subjects.First(s => s.Code == "BANKING_FUNDAMENTALS");
            var financialRegulation = subjects.First(s => s.Code == "FINANCIAL_REGULATION");
            var pharmacology = subjects.First(s => s.Code == "PHARMACOLOGY");
            var pharmScience = subjects.First(s => s.Code == "PHARMACEUTICAL_SCIENCE");
            var medLabScience = subjects.First(s => s.Code == "MEDICAL_LAB_SCIENCE");
            var clinicalDx = subjects.First(s => s.Code == "CLINICAL_DIAGNOSTICS");
            var eduConsulting = subjects.First(s => s.Code == "EDUCATIONAL_CONSULTING");
            var careerCounseling = subjects.First(s => s.Code == "CAREER_COUNSELING");
            var instructionalDesign = subjects.First(s => s.Code == "INSTRUCTIONAL_DESIGN");
            var eLearning = subjects.First(s => s.Code == "E_LEARNING_DEVELOPMENT");

            var seedTopics = new List<Topic>
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
                new Topic { SubjectId = webDev.Id, Name = "React Framework", Description = "Components, state, props", DifficultyLevel = 3 },
                // Topics HTML & CSS
                new Topic { SubjectId = htmlCss.Id, Name = "HTML Semantic & Structure", Description = "Thẻ HTML, cấu trúc trang, semantic elements", DifficultyLevel = 1 },
                new Topic { SubjectId = htmlCss.Id, Name = "CSS Layout & Flexbox", Description = "Flexbox, Grid, responsive design", DifficultyLevel = 2 },
                // Topics JavaScript
                new Topic { SubjectId = javaScript.Id, Name = "ES6 & Modern JavaScript", Description = "Arrow functions, destructuring, modules", DifficultyLevel = 2 },
                new Topic { SubjectId = javaScript.Id, Name = "DOM Manipulation & Events", Description = "Thao tác DOM, xử lý sự kiện", DifficultyLevel = 2 },
                // Topics React
                new Topic { SubjectId = react.Id, Name = "Components & Props", Description = "Function components, props, state", DifficultyLevel = 3 },
                new Topic { SubjectId = react.Id, Name = "Hooks & Lifecycle", Description = "useState, useEffect, custom hooks", DifficultyLevel = 3 },
                // Topics C# Fundamentals
                new Topic { SubjectId = cSharpFund.Id, Name = "Biến & Kiểu Dữ Liệu C#", Description = "int, string, bool, var, constants", DifficultyLevel = 1 },
                new Topic { SubjectId = cSharpFund.Id, Name = "Cấu Trúc Điều Khiển C#", Description = "if/else, switch, for, foreach, while", DifficultyLevel = 1 },
                new Topic { SubjectId = cSharpFund.Id, Name = "Phương Thức & Collections", Description = "Methods, arrays, List, Dictionary", DifficultyLevel = 2 },
                // Topics OOP C#
                new Topic { SubjectId = oopCSharp.Id, Name = "Class & Encapsulation", Description = "Class, properties, access modifiers", DifficultyLevel = 2 },
                new Topic { SubjectId = oopCSharp.Id, Name = "Inheritance & Polymorphism", Description = "Kế thừa, đa hình, interface", DifficultyLevel = 3 },
                // Topics SQL Database
                new Topic { SubjectId = sqlDb.Id, Name = "SELECT & WHERE", Description = "Truy vấn cơ bản, điều kiện lọc", DifficultyLevel = 1 },
                new Topic { SubjectId = sqlDb.Id, Name = "JOIN & GROUP BY", Description = "Kết hợp bảng, nhóm và tổng hợp", DifficultyLevel = 2 },
                new Topic { SubjectId = sqlDb.Id, Name = "Primary Key & Foreign Key", Description = "Khóa chính, khóa ngoại, ràng buộc", DifficultyLevel = 2 },
                // Topics ASP.NET Core
                new Topic { SubjectId = aspNetCore.Id, Name = "Web API & Routing", Description = "Controller, routing, HTTP methods", DifficultyLevel = 3 },
                new Topic { SubjectId = aspNetCore.Id, Name = "Dependency Injection & Middleware", Description = "DI container, middleware pipeline", DifficultyLevel = 3 },
                // Topics EF Core / LINQ
                new Topic { SubjectId = efCoreLinq.Id, Name = "DbContext & Relationships", Description = "DbContext, DbSet, entity relationships", DifficultyLevel = 3 },
                new Topic { SubjectId = efCoreLinq.Id, Name = "LINQ Queries & Migrations", Description = "LINQ filtering, projection, migrations", DifficultyLevel = 3 },
                // Topics Machine Learning
                new Topic { SubjectId = machineLearning.Id, Name = "Supervised Learning", Description = "Hồi quy, phân loại, đánh giá model", DifficultyLevel = 3 },
                new Topic { SubjectId = machineLearning.Id, Name = "Model Evaluation", Description = "Accuracy, precision, recall, overfitting", DifficultyLevel = 4 },
                // Topics AI Fundamentals
                new Topic { SubjectId = aiFund.Id, Name = "Neural Network Cơ Bản", Description = "Cấu trúc neural network, activation", DifficultyLevel = 3 },
                new Topic { SubjectId = aiFund.Id, Name = "Deep Learning & CNN", Description = "Deep learning, convolutional networks", DifficultyLevel = 4 },
                // Topics Python ML
                new Topic { SubjectId = pythonMl.Id, Name = "Python & NumPy", Description = "Cú pháp Python, mảng NumPy", DifficultyLevel = 1 },
                new Topic { SubjectId = pythonMl.Id, Name = "Pandas & Scikit-learn", Description = "Xử lý dữ liệu, model ML với scikit-learn", DifficultyLevel = 2 },
                // Topics Network Security
                new Topic { SubjectId = networkSecurity.Id, Name = "Firewall & IDS/IPS", Description = "Tường lửa, hệ thống phát hiện xâm nhập", DifficultyLevel = 3 },
                new Topic { SubjectId = networkSecurity.Id, Name = "Mã Hóa & Bảo Mật Dữ Liệu", Description = "Encryption, TLS/SSL, hash", DifficultyLevel = 3 },
                // Topics Cybersecurity Fundamentals
                new Topic { SubjectId = cyberFund.Id, Name = "Các Mối Đe Dọa An Ninh", Description = "Malware, phishing, social engineering", DifficultyLevel = 2 },
                new Topic { SubjectId = cyberFund.Id, Name = "Phòng Chống Tấn Công", Description = "Nguyên tắc bảo mật, patch, hardening", DifficultyLevel = 3 },
                // Topics Brand Management
                new Topic { SubjectId = brandManagement.Id, Name = "Brand Identity & Positioning", Description = "Xây dựng nhận diện và định vị thương hiệu", DifficultyLevel = 2 },
                new Topic { SubjectId = brandManagement.Id, Name = "Chiến Lược Thương Hiệu", Description = "Kế hoạch phát triển thương hiệu dài hạn", DifficultyLevel = 3 },
                // Topics Marketing Strategy
                new Topic { SubjectId = marketingStrategy.Id, Name = "Marketing Mix (4P)", Description = "Product, Price, Place, Promotion", DifficultyLevel = 1 },
                new Topic { SubjectId = marketingStrategy.Id, Name = "Phân Tích Thị Trường", Description = "Nghiên cứu thị trường, SWOT, segmentation", DifficultyLevel = 2 },
                // Topics Social Media Strategy
                new Topic { SubjectId = socialMediaStrategy.Id, Name = "Kế Hoạch Nội Dung Social", Description = "Xây dựng content calendar, đa nền tảng", DifficultyLevel = 2 },
                new Topic { SubjectId = socialMediaStrategy.Id, Name = "Phân Tích Hiệu Quả Social", Description = "Metrics, analytics, tối ưu engagement", DifficultyLevel = 3 },
                // Topics Content Strategy
                new Topic { SubjectId = contentStrategy.Id, Name = "Content Funnel & Persona", Description = "Phễu nội dung, chân dung khách hàng", DifficultyLevel = 2 },
                new Topic { SubjectId = contentStrategy.Id, Name = "Đo Lường Hiệu Quả Nội Dung", Description = "KPI, analytics, tối ưu nội dung", DifficultyLevel = 3 },
                // Topics Financial Modeling
                new Topic { SubjectId = financialModeling.Id, Name = "Mô Hình Tài Chính Cơ Bản", Description = "Xây dựng mô hình DCF, giả định", DifficultyLevel = 3 },
                new Topic { SubjectId = financialModeling.Id, Name = "Định Giá Doanh Nghiệp", Description = "Phương pháp định giá, phân tích kịch bản", DifficultyLevel = 4 },
                // Topics Banking Fundamentals
                new Topic { SubjectId = bankingFund.Id, Name = "Nghiệp Vụ Tín Dụng", Description = "Quy trình cấp tín dụng, thẩm định", DifficultyLevel = 2 },
                new Topic { SubjectId = bankingFund.Id, Name = "Quản Trị Rủi Ro Ngân Hàng", Description = "Rủi ro tín dụng, rủi ro thị trường, thanh khoản", DifficultyLevel = 3 },
                // Topics Financial Regulation
                new Topic { SubjectId = financialRegulation.Id, Name = "Luật Ngân Hàng & Chứng Khoán", Description = "Quy định pháp lý trong tài chính", DifficultyLevel = 2 },
                new Topic { SubjectId = financialRegulation.Id, Name = "Tuân Thủ & Chống Rửa Tiền", Description = "Compliance, AML, KYC", DifficultyLevel = 3 },
                // Topics Pharmacology
                new Topic { SubjectId = pharmacology.Id, Name = "Dược Động Học", Description = "Hấp thu, phân bố, chuyển hóa, thải trừ", DifficultyLevel = 2 },
                new Topic { SubjectId = pharmacology.Id, Name = "Tương Tác Thuốc", Description = "Tương tác thuốc - thuốc, thuốc - thức ăn", DifficultyLevel = 3 },
                // Topics Pharmaceutical Science
                new Topic { SubjectId = pharmScience.Id, Name = "Bào Chế Thuốc", Description = "Các dạng bào chế, quy trình sản xuất", DifficultyLevel = 2 },
                new Topic { SubjectId = pharmScience.Id, Name = "Kiểm Soát Chất Lượng Dược", Description = "GPP, kiểm nghiệm, đảm bảo chất lượng", DifficultyLevel = 3 },
                // Topics Medical Lab Science
                new Topic { SubjectId = medLabScience.Id, Name = "Xét Nghiệm Huyết Học", Description = "Công thức máu, đông máu", DifficultyLevel = 2 },
                new Topic { SubjectId = medLabScience.Id, Name = "Xét Nghiệm Sinh Hóa", Description = "Glucose, men gan, điện giải", DifficultyLevel = 3 },
                // Topics Clinical Diagnostics
                new Topic { SubjectId = clinicalDx.Id, Name = "Quy Trình Xét Nghiệm", Description = "Lấy mẫu, bảo quản, xử lý bệnh phẩm", DifficultyLevel = 2 },
                new Topic { SubjectId = clinicalDx.Id, Name = "Giải Thích Kết Quả", Description = "Đọc kết quả, giá trị tham chiếu", DifficultyLevel = 3 },
                // Topics Educational Consulting
                new Topic { SubjectId = eduConsulting.Id, Name = "Đánh Giá Năng Lực Học Viên", Description = "Phương pháp đánh giá, trắc nghiệm năng lực", DifficultyLevel = 2 },
                new Topic { SubjectId = eduConsulting.Id, Name = "Xây Dựng Lộ Trình Học Tập", Description = "Thiết kế lộ trình, mục tiêu học tập", DifficultyLevel = 3 },
                // Topics Career Counseling
                new Topic { SubjectId = careerCounseling.Id, Name = "Phương Pháp Tư Vấn", Description = "Kỹ năng lắng nghe, đặt câu hỏi, đồng cảm", DifficultyLevel = 2 },
                new Topic { SubjectId = careerCounseling.Id, Name = "Định Hướng Nghề Nghiệp", Description = "Phân tích sở thích, năng lực, xu hướng thị trường", DifficultyLevel = 3 },
                // Topics Instructional Design
                new Topic { SubjectId = instructionalDesign.Id, Name = "Mô Hình ADDIE", Description = "Analyze, Design, Develop, Implement, Evaluate", DifficultyLevel = 2 },
                new Topic { SubjectId = instructionalDesign.Id, Name = "Bloom's Taxonomy", Description = "Nhận thức, phân loại mục tiêu học tập", DifficultyLevel = 3 },
                // Topics E-Learning Development
                new Topic { SubjectId = eLearning.Id, Name = "Công Cụ E-Learning", Description = "Articulate, Moodle, SCORM", DifficultyLevel = 3 },
                new Topic { SubjectId = eLearning.Id, Name = "Thiết Kế Khóa Học Online", Description = "Cấu trúc khóa học, đánh giá trực tuyến", DifficultyLevel = 3 }
            };

            // Only insert Topics that do not already exist (detect by SubjectId + Name)
            var subjectIds = seedTopics.Select(t => t.SubjectId).Distinct().ToList();
            var existingTopics = await context.Topics
                .Where(t => subjectIds.Contains(t.SubjectId))
                .Select(t => new { t.SubjectId, t.Name })
                .ToListAsync();

            var newTopics = seedTopics
                .Where(t => !existingTopics.Any(e => e.SubjectId == t.SubjectId && e.Name == t.Name))
                .ToList();

            if (newTopics.Count > 0)
            {
                await context.Topics.AddRangeAsync(newTopics);
                await context.SaveChangesAsync();
            }

            // Return ALL Topics (existing + newly inserted) for the relevant subjects
            var allTopics = await context.Topics
                .Where(t => subjectIds.Contains(t.SubjectId))
                .OrderBy(t => t.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newTopics.Count} new Topics. Total Topics in DB: {allTopics.Count}.");
            return allTopics;
        }

        private static async Task<List<RecommendationQuestion>> SeedQuestionsAsync(ApplicationDbContext context, List<Quiz> quizzes)
        {
            Console.WriteLine("Seeding RecommendationQuestions...");
            var cnttQuiz = quizzes.First(q => q.Code == "QUIZ_CNTT");
            var marketingQuiz = quizzes.First(q => q.Code == "QUIZ_MARKETING");
            var kinhTeQuiz = quizzes.First(q => q.Code == "QUIZ_KINH_TE");
            var yTeQuiz = quizzes.First(q => q.Code == "QUIZ_Y_TE");
            var giaoDucQuiz = quizzes.First(q => q.Code == "QUIZ_GIAO_DUC");

            var seedQuestions = new List<RecommendationQuestion>
            {
                // ===== CNTT Questions (existing 5 + 3 new = 8) =====
                // Existing
                new RecommendationQuestion { Content = "Bạn thích làm việc với giao diện người dùng hay xử lý logic phía sau?", QuizId = cnttQuiz.Id },
                new RecommendationQuestion { Content = "Bạn quan tâm hơn đến dữ liệu và phân tích thống kê?", QuizId = cnttQuiz.Id },
                new RecommendationQuestion { Content = "Bạn thích làm việc với infrastructure, server và deployment?", QuizId = cnttQuiz.Id },
                new RecommendationQuestion { Content = "Bạn thích phát triển ứng dụng cho mobile devices?", QuizId = cnttQuiz.Id },
                new RecommendationQuestion { Content = "Bạn hứng thú với machine learning và AI?", QuizId = cnttQuiz.Id },
                // New CNTT
                new RecommendationQuestion { Content = "Bạn có muốn bảo vệ hệ thống và dữ liệu khỏi tấn công mạng không?", QuizId = cnttQuiz.Id },
                new RecommendationQuestion { Content = "Bạn thích phân tích dữ liệu và xây dựng mô hình dự đoán?", QuizId = cnttQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có hứng thú với trí tuệ nhân tạo và robot thông minh?", QuizId = cnttQuiz.Id },

                // ===== Marketing Questions (8) =====
                new RecommendationQuestion { Content = "Bạn thích sáng tạo nội dung truyền thông hay quản lý chiến dịch tổng thể?", QuizId = marketingQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích làm việc với các nền tảng mạng xã hội không?", QuizId = marketingQuiz.Id },
                new RecommendationQuestion { Content = "Bạn quan tâm đến việc xây dựng hình ảnh thương hiệu dài hạn?", QuizId = marketingQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích phân tích số liệu hiệu quả chiến dịch marketing?", QuizId = marketingQuiz.Id },
                new RecommendationQuestion { Content = "Bạn thích viết nội dung sáng tạo hay lập kế hoạch chiến lược?", QuizId = marketingQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích tương tác trực tiếp với cộng đồng và khách hàng?", QuizId = marketingQuiz.Id },
                new RecommendationQuestion { Content = "Bạn muốn quản lý toàn bộ nhận diện thương hiệu của một công ty?", QuizId = marketingQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có hứng thú với việc tối ưu công cụ tìm kiếm (SEO) không?", QuizId = marketingQuiz.Id },

                // ===== Kinh Tế Questions (8) =====
                new RecommendationQuestion { Content = "Bạn thích phân tích cổ phiếu và thị trường chứng khoán?", QuizId = kinhTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có hứng thú với nghiệp vụ tín dụng và ngân hàng không?", QuizId = kinhTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn thích lập báo cáo tài chính và kế toán?", QuizId = kinhTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn muốn xây dựng mô hình định giá doanh nghiệp?", QuizId = kinhTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích tư vấn đầu tư cho khách hàng không?", QuizId = kinhTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn quan tâm đến quản trị rủi ro tài chính?", QuizId = kinhTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn thích làm việc với các quy định pháp lý tài chính?", QuizId = kinhTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có hứng thú với việc phân tích báo cáo của các công ty niêm yết?", QuizId = kinhTeQuiz.Id },

                // ===== Y Tế Questions (8) =====
                new RecommendationQuestion { Content = "Bạn có hứng thú với việc tư vấn và quản lý thuốc?", QuizId = yTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích làm việc trong phòng xét nghiệm không?", QuizId = yTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn quan tâm đến cơ chế tác dụng của thuốc lên cơ thể?", QuizId = yTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích phân tích mẫu bệnh phẩm để hỗ trợ chẩn đoán?", QuizId = yTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn muốn đảm bảo chất lượng và an toàn dược phẩm?", QuizId = yTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích làm việc với các thiết bị xét nghiệm hiện đại?", QuizId = yTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn quan tâm đến tương tác thuốc và phản ứng phụ?", QuizId = yTeQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích công việc đòi hỏi sự tỉ mỉ và chính xác cao?", QuizId = yTeQuiz.Id },

                // ===== Giáo Dục Questions (8) =====
                new RecommendationQuestion { Content = "Bạn có thích tư vấn lộ trình học tập cho học viên không?", QuizId = giaoDucQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có hứng thú với việc thiết kế chương trình đào tạo?", QuizId = giaoDucQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích làm việc với người học và phụ huynh?", QuizId = giaoDucQuiz.Id },
                new RecommendationQuestion { Content = "Bạn muốn xây dựng nội dung khóa học online?", QuizId = giaoDucQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích phân tích năng lực và định hướng nghề nghiệp?", QuizId = giaoDucQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có hứng thú với các mô hình giảng dạy và công nghệ giáo dục?", QuizId = giaoDucQuiz.Id },
                new RecommendationQuestion { Content = "Bạn có thích đánh giá hiệu quả của chương trình đào tạo?", QuizId = giaoDucQuiz.Id },
                new RecommendationQuestion { Content = "Bạn muốn giúp người khác phát triển sự nghiệp và học tập?", QuizId = giaoDucQuiz.Id }
            };

            // Only insert Questions that do not already exist (detect by QuizId + Content)
            var quizIds = seedQuestions.Select(q => q.QuizId).Distinct().ToList();
            var existingQuestions = await context.RecommendationQuestions
                .Where(q => quizIds.Contains(q.QuizId))
                .Select(q => new { q.QuizId, q.Content })
                .ToListAsync();

            var newQuestions = seedQuestions
                .Where(q => !existingQuestions.Any(e => e.QuizId == q.QuizId && e.Content == q.Content))
                .ToList();

            if (newQuestions.Count > 0)
            {
                await context.RecommendationQuestions.AddRangeAsync(newQuestions);
                await context.SaveChangesAsync();
            }

            // Return ALL Questions (existing + newly inserted) so downstream methods use real DB IDs
            var allQuestions = await context.RecommendationQuestions
                .Where(q => quizIds.Contains(q.QuizId))
                .OrderBy(q => q.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newQuestions.Count} new RecommendationQuestions. Total Questions in DB: {allQuestions.Count}.");
            return allQuestions;
        }

        private static async Task<List<RecommendationAnswer>> SeedAnswersAsync(ApplicationDbContext context, List<RecommendationQuestion> questions)
        {
            Console.WriteLine("Seeding RecommendationAnswers...");
            
            // Look up questions by content so DB ordering does not matter
            // CNTT existing
            var q1 = questions.First(q => q.Content == "Bạn thích làm việc với giao diện người dùng hay xử lý logic phía sau?");
            var q2 = questions.First(q => q.Content == "Bạn quan tâm hơn đến dữ liệu và phân tích thống kê?");
            var q3 = questions.First(q => q.Content == "Bạn thích làm việc với infrastructure, server và deployment?");
            var q4 = questions.First(q => q.Content == "Bạn thích phát triển ứng dụng cho mobile devices?");
            var q5 = questions.First(q => q.Content == "Bạn hứng thú với machine learning và AI?");
            // CNTT new
            var q6 = questions.First(q => q.Content == "Bạn có muốn bảo vệ hệ thống và dữ liệu khỏi tấn công mạng không?");
            var q7 = questions.First(q => q.Content == "Bạn thích phân tích dữ liệu và xây dựng mô hình dự đoán?");
            var q8 = questions.First(q => q.Content == "Bạn có hứng thú với trí tuệ nhân tạo và robot thông minh?");
            // Marketing
            var m1 = questions.First(q => q.Content == "Bạn thích sáng tạo nội dung truyền thông hay quản lý chiến dịch tổng thể?");
            var m2 = questions.First(q => q.Content == "Bạn có thích làm việc với các nền tảng mạng xã hội không?");
            var m3 = questions.First(q => q.Content == "Bạn quan tâm đến việc xây dựng hình ảnh thương hiệu dài hạn?");
            var m4 = questions.First(q => q.Content == "Bạn có thích phân tích số liệu hiệu quả chiến dịch marketing?");
            var m5 = questions.First(q => q.Content == "Bạn thích viết nội dung sáng tạo hay lập kế hoạch chiến lược?");
            var m6 = questions.First(q => q.Content == "Bạn có thích tương tác trực tiếp với cộng đồng và khách hàng?");
            var m7 = questions.First(q => q.Content == "Bạn muốn quản lý toàn bộ nhận diện thương hiệu của một công ty?");
            var m8 = questions.First(q => q.Content == "Bạn có hứng thú với việc tối ưu công cụ tìm kiếm (SEO) không?");
            // Kinh Tế
            var k1 = questions.First(q => q.Content == "Bạn thích phân tích cổ phiếu và thị trường chứng khoán?");
            var k2 = questions.First(q => q.Content == "Bạn có hứng thú với nghiệp vụ tín dụng và ngân hàng không?");
            var k3 = questions.First(q => q.Content == "Bạn thích lập báo cáo tài chính và kế toán?");
            var k4 = questions.First(q => q.Content == "Bạn muốn xây dựng mô hình định giá doanh nghiệp?");
            var k5 = questions.First(q => q.Content == "Bạn có thích tư vấn đầu tư cho khách hàng không?");
            var k6 = questions.First(q => q.Content == "Bạn quan tâm đến quản trị rủi ro tài chính?");
            var k7 = questions.First(q => q.Content == "Bạn thích làm việc với các quy định pháp lý tài chính?");
            var k8 = questions.First(q => q.Content == "Bạn có hứng thú với việc phân tích báo cáo của các công ty niêm yết?");
            // Y Tế
            var y1 = questions.First(q => q.Content == "Bạn có hứng thú với việc tư vấn và quản lý thuốc?");
            var y2 = questions.First(q => q.Content == "Bạn có thích làm việc trong phòng xét nghiệm không?");
            var y3 = questions.First(q => q.Content == "Bạn quan tâm đến cơ chế tác dụng của thuốc lên cơ thể?");
            var y4 = questions.First(q => q.Content == "Bạn có thích phân tích mẫu bệnh phẩm để hỗ trợ chẩn đoán?");
            var y5 = questions.First(q => q.Content == "Bạn muốn đảm bảo chất lượng và an toàn dược phẩm?");
            var y6 = questions.First(q => q.Content == "Bạn có thích làm việc với các thiết bị xét nghiệm hiện đại?");
            var y7 = questions.First(q => q.Content == "Bạn quan tâm đến tương tác thuốc và phản ứng phụ?");
            var y8 = questions.First(q => q.Content == "Bạn có thích công việc đòi hỏi sự tỉ mỉ và chính xác cao?");
            // Giáo Dục
            var g1 = questions.First(q => q.Content == "Bạn có thích tư vấn lộ trình học tập cho học viên không?");
            var g2 = questions.First(q => q.Content == "Bạn có hứng thú với việc thiết kế chương trình đào tạo?");
            var g3 = questions.First(q => q.Content == "Bạn có thích làm việc với người học và phụ huynh?");
            var g4 = questions.First(q => q.Content == "Bạn muốn xây dựng nội dung khóa học online?");
            var g5 = questions.First(q => q.Content == "Bạn có thích phân tích năng lực và định hướng nghề nghiệp?");
            var g6 = questions.First(q => q.Content == "Bạn có hứng thú với các mô hình giảng dạy và công nghệ giáo dục?");
            var g7 = questions.First(q => q.Content == "Bạn có thích đánh giá hiệu quả của chương trình đào tạo?");
            var g8 = questions.First(q => q.Content == "Bạn muốn giúp người khác phát triển sự nghiệp và học tập?");

            var seedAnswers = new List<RecommendationAnswer>
            {
                // ===== CNTT Answers =====
                // Q1 (existing)
                new RecommendationAnswer { Content = "Thích thiết kế giao diện đẹp, tương tác người dùng", RecommendationQuestionId = q1.Id },
                new RecommendationAnswer { Content = "Thích xử lý logic, thuật toán, database", RecommendationQuestionId = q1.Id },
                new RecommendationAnswer { Content = "Thích cả hai", RecommendationQuestionId = q1.Id },
                // Q2 (existing)
                new RecommendationAnswer { Content = "Rất quan tâm, thích tìm kiếm pattern từ dữ liệu", RecommendationQuestionId = q2.Id },
                new RecommendationAnswer { Content = "Hơi quan tâm nhưng không phải ưu tiên", RecommendationQuestionId = q2.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = q2.Id },
                // Q3 (existing)
                new RecommendationAnswer { Content = "Thích setup server, CI/CD, cloud", RecommendationQuestionId = q3.Id },
                new RecommendationAnswer { Content = "Thích nhưng không chuyên sâu", RecommendationQuestionId = q3.Id },
                new RecommendationAnswer { Content = "Không thích, muốn tập trung vào code", RecommendationQuestionId = q3.Id },
                // Q4 (existing)
                new RecommendationAnswer { Content = "Rất thích, muốn build app cho iOS/Android", RecommendationQuestionId = q4.Id },
                new RecommendationAnswer { Content = "Hơi thích nhưng không chuyên sâu", RecommendationQuestionId = q4.Id },
                new RecommendationAnswer { Content = "Thích web hơn", RecommendationQuestionId = q4.Id },
                // Q5 (existing)
                new RecommendationAnswer { Content = "Rất hứng thú, muốn học AI/ML", RecommendationQuestionId = q5.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú nhưng không chuyên sâu", RecommendationQuestionId = q5.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = q5.Id },
                // Q6 (bảo vệ hệ thống)
                new RecommendationAnswer { Content = "Rất muốn, thích bảo mật và an toàn thông tin", RecommendationQuestionId = q6.Id },
                new RecommendationAnswer { Content = "Hơi quan tâm nhưng không chuyên sâu", RecommendationQuestionId = q6.Id },
                new RecommendationAnswer { Content = "Không quan tâm, thích phát triển tính năng hơn", RecommendationQuestionId = q6.Id },
                // Q7 (phân tích dữ liệu)
                new RecommendationAnswer { Content = "Rất thích, muốn xây model dự đoán", RecommendationQuestionId = q7.Id },
                new RecommendationAnswer { Content = "Thích nhưng không làm chuyên sâu", RecommendationQuestionId = q7.Id },
                new RecommendationAnswer { Content = "Không thích, thích viết code ứng dụng", RecommendationQuestionId = q7.Id },
                // Q8 (trí tuệ nhân tạo / robot)
                new RecommendationAnswer { Content = "Rất hứng thú với AI và robot", RecommendationQuestionId = q8.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú, muốn tìm hiểu thêm", RecommendationQuestionId = q8.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = q8.Id },

                // ===== Marketing Answers =====
                // M1
                new RecommendationAnswer { Content = "Thích sáng tạo nội dung truyền thông", RecommendationQuestionId = m1.Id },
                new RecommendationAnswer { Content = "Thích quản lý chiến dịch tổng thể", RecommendationQuestionId = m1.Id },
                new RecommendationAnswer { Content = "Thích cả hai", RecommendationQuestionId = m1.Id },
                // M2
                new RecommendationAnswer { Content = "Rất thích, dành nhiều thời gian trên social media", RecommendationQuestionId = m2.Id },
                new RecommendationAnswer { Content = "Bình thường, dùng có mức độ", RecommendationQuestionId = m2.Id },
                new RecommendationAnswer { Content = "Không thích, ít dùng mạng xã hội", RecommendationQuestionId = m2.Id },
                // M3
                new RecommendationAnswer { Content = "Rất quan tâm, thích xây dựng thương hiệu", RecommendationQuestionId = m3.Id },
                new RecommendationAnswer { Content = "Hơi quan tâm", RecommendationQuestionId = m3.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = m3.Id },
                // M4
                new RecommendationAnswer { Content = "Rất thích phân tích số liệu", RecommendationQuestionId = m4.Id },
                new RecommendationAnswer { Content = "Thích nhưng không phải ưu tiên", RecommendationQuestionId = m4.Id },
                new RecommendationAnswer { Content = "Không thích, thích sáng tạo hơn", RecommendationQuestionId = m4.Id },
                // M5
                new RecommendationAnswer { Content = "Thích viết nội dung sáng tạo", RecommendationQuestionId = m5.Id },
                new RecommendationAnswer { Content = "Thích lập kế hoạch chiến lược", RecommendationQuestionId = m5.Id },
                new RecommendationAnswer { Content = "Thích cả hai", RecommendationQuestionId = m5.Id },
                // M6
                new RecommendationAnswer { Content = "Rất thích tương tác với cộng đồng", RecommendationQuestionId = m6.Id },
                new RecommendationAnswer { Content = "Thích nhưng không thường xuyên", RecommendationQuestionId = m6.Id },
                new RecommendationAnswer { Content = "Không thích, thích làm việc độc lập", RecommendationQuestionId = m6.Id },
                // M7
                new RecommendationAnswer { Content = "Rất muốn, thích quản lý nhận diện thương hiệu", RecommendationQuestionId = m7.Id },
                new RecommendationAnswer { Content = "Hơi muốn", RecommendationQuestionId = m7.Id },
                new RecommendationAnswer { Content = "Không muốn, thích công việc chuyên môn hơn", RecommendationQuestionId = m7.Id },
                // M8
                new RecommendationAnswer { Content = "Rất hứng thú với SEO", RecommendationQuestionId = m8.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú", RecommendationQuestionId = m8.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = m8.Id },

                // ===== Kinh Tế Answers =====
                // K1
                new RecommendationAnswer { Content = "Rất thích, theo dõi thị trường chứng khoán", RecommendationQuestionId = k1.Id },
                new RecommendationAnswer { Content = "Hơi thích, muốn tìm hiểu thêm", RecommendationQuestionId = k1.Id },
                new RecommendationAnswer { Content = "Không thích, thích kế toán hơn", RecommendationQuestionId = k1.Id },
                // K2
                new RecommendationAnswer { Content = "Rất hứng thú với nghiệp vụ ngân hàng", RecommendationQuestionId = k2.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú", RecommendationQuestionId = k2.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = k2.Id },
                // K3
                new RecommendationAnswer { Content = "Rất thích lập báo cáo tài chính", RecommendationQuestionId = k3.Id },
                new RecommendationAnswer { Content = "Thích nhưng không phải ưu tiên", RecommendationQuestionId = k3.Id },
                new RecommendationAnswer { Content = "Không thích, thích phân tích đầu tư hơn", RecommendationQuestionId = k3.Id },
                // K4
                new RecommendationAnswer { Content = "Rất muốn, thích định giá doanh nghiệp", RecommendationQuestionId = k4.Id },
                new RecommendationAnswer { Content = "Hơi muốn tìm hiểu", RecommendationQuestionId = k4.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = k4.Id },
                // K5
                new RecommendationAnswer { Content = "Rất thích tư vấn đầu tư cho khách", RecommendationQuestionId = k5.Id },
                new RecommendationAnswer { Content = "Thích nhưng không chuyên sâu", RecommendationQuestionId = k5.Id },
                new RecommendationAnswer { Content = "Không thích giao tiếp với khách hàng", RecommendationQuestionId = k5.Id },
                // K6
                new RecommendationAnswer { Content = "Rất quan tâm đến quản trị rủi ro", RecommendationQuestionId = k6.Id },
                new RecommendationAnswer { Content = "Hơi quan tâm", RecommendationQuestionId = k6.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = k6.Id },
                // K7
                new RecommendationAnswer { Content = "Rất thích làm việc với quy định pháp lý", RecommendationQuestionId = k7.Id },
                new RecommendationAnswer { Content = "Hơi thích", RecommendationQuestionId = k7.Id },
                new RecommendationAnswer { Content = "Không thích, thấy khô khan", RecommendationQuestionId = k7.Id },
                // K8
                new RecommendationAnswer { Content = "Rất hứng thú phân tích báo cáo công ty", RecommendationQuestionId = k8.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú", RecommendationQuestionId = k8.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = k8.Id },

                // ===== Y Tế Answers =====
                // Y1
                new RecommendationAnswer { Content = "Rất hứng thú với tư vấn và quản lý thuốc", RecommendationQuestionId = y1.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú", RecommendationQuestionId = y1.Id },
                new RecommendationAnswer { Content = "Không hứng thú, thích xét nghiệm hơn", RecommendationQuestionId = y1.Id },
                // Y2
                new RecommendationAnswer { Content = "Rất thích làm việc trong phòng lab", RecommendationQuestionId = y2.Id },
                new RecommendationAnswer { Content = "Thích nhưng lo sợ hóa chất", RecommendationQuestionId = y2.Id },
                new RecommendationAnswer { Content = "Không thích, thích làm việc với bệnh nhân", RecommendationQuestionId = y2.Id },
                // Y3
                new RecommendationAnswer { Content = "Rất quan tâm đến cơ chế tác dụng của thuốc", RecommendationQuestionId = y3.Id },
                new RecommendationAnswer { Content = "Hơi quan tâm", RecommendationQuestionId = y3.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = y3.Id },
                // Y4
                new RecommendationAnswer { Content = "Rất thích phân tích mẫu bệnh phẩm", RecommendationQuestionId = y4.Id },
                new RecommendationAnswer { Content = "Thích nhưng không chuyên sâu", RecommendationQuestionId = y4.Id },
                new RecommendationAnswer { Content = "Không thích, thấy phức tạp", RecommendationQuestionId = y4.Id },
                // Y5
                new RecommendationAnswer { Content = "Rất muốn đảm bảo chất lượng dược phẩm", RecommendationQuestionId = y5.Id },
                new RecommendationAnswer { Content = "Hơi muốn", RecommendationQuestionId = y5.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = y5.Id },
                // Y6
                new RecommendationAnswer { Content = "Rất thích làm việc với thiết bị hiện đại", RecommendationQuestionId = y6.Id },
                new RecommendationAnswer { Content = "Thích nhưng cần đào tạo thêm", RecommendationQuestionId = y6.Id },
                new RecommendationAnswer { Content = "Không thích, thấy phức tạp", RecommendationQuestionId = y6.Id },
                // Y7
                new RecommendationAnswer { Content = "Rất quan tâm đến tương tác thuốc", RecommendationQuestionId = y7.Id },
                new RecommendationAnswer { Content = "Hơi quan tâm", RecommendationQuestionId = y7.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = y7.Id },
                // Y8
                new RecommendationAnswer { Content = "Rất thích công việc tỉ mỉ, chính xác", RecommendationQuestionId = y8.Id },
                new RecommendationAnswer { Content = "Thích nhưng cần sự đa dạng", RecommendationQuestionId = y8.Id },
                new RecommendationAnswer { Content = "Không thích, thích sáng tạo tự do", RecommendationQuestionId = y8.Id },

                // ===== Giáo Dục Answers =====
                // G1
                new RecommendationAnswer { Content = "Rất thích tư vấn lộ trình học tập", RecommendationQuestionId = g1.Id },
                new RecommendationAnswer { Content = "Thích nhưng không phải ưu tiên", RecommendationQuestionId = g1.Id },
                new RecommendationAnswer { Content = "Không thích, thích thiết kế nội dung hơn", RecommendationQuestionId = g1.Id },
                // G2
                new RecommendationAnswer { Content = "Rất hứng thú thiết kế chương trình đào tạo", RecommendationQuestionId = g2.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú", RecommendationQuestionId = g2.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = g2.Id },
                // G3
                new RecommendationAnswer { Content = "Rất thích làm việc với người học", RecommendationQuestionId = g3.Id },
                new RecommendationAnswer { Content = "Thích nhưng không thường xuyên", RecommendationQuestionId = g3.Id },
                new RecommendationAnswer { Content = "Không thích, thích làm việc độc lập", RecommendationQuestionId = g3.Id },
                // G4
                new RecommendationAnswer { Content = "Rất muốn xây dựng khóa học online", RecommendationQuestionId = g4.Id },
                new RecommendationAnswer { Content = "Hơi muốn tìm hiểu", RecommendationQuestionId = g4.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = g4.Id },
                // G5
                new RecommendationAnswer { Content = "Rất thích phân tích năng lực và hướng nghiệp", RecommendationQuestionId = g5.Id },
                new RecommendationAnswer { Content = "Thích nhưng không chuyên sâu", RecommendationQuestionId = g5.Id },
                new RecommendationAnswer { Content = "Không thích, thích giảng dạy trực tiếp", RecommendationQuestionId = g5.Id },
                // G6
                new RecommendationAnswer { Content = "Rất hứng thú với công nghệ giáo dục", RecommendationQuestionId = g6.Id },
                new RecommendationAnswer { Content = "Hơi hứng thú", RecommendationQuestionId = g6.Id },
                new RecommendationAnswer { Content = "Không hứng thú lắm", RecommendationQuestionId = g6.Id },
                // G7
                new RecommendationAnswer { Content = "Rất thích đánh giá hiệu quả đào tạo", RecommendationQuestionId = g7.Id },
                new RecommendationAnswer { Content = "Thích nhưng không phải ưu tiên", RecommendationQuestionId = g7.Id },
                new RecommendationAnswer { Content = "Không thích, thích sáng tạo nội dung", RecommendationQuestionId = g7.Id },
                // G8
                new RecommendationAnswer { Content = "Rất muốn giúp người khác phát triển", RecommendationQuestionId = g8.Id },
                new RecommendationAnswer { Content = "Thích nhưng có giới hạn", RecommendationQuestionId = g8.Id },
                new RecommendationAnswer { Content = "Không quan tâm lắm", RecommendationQuestionId = g8.Id }
            };

            // Only insert Answers that do not already exist (detect by RecommendationQuestionId + Content)
            var questionIds = seedAnswers.Select(a => a.RecommendationQuestionId).Distinct().ToList();
            var existingAnswers = await context.RecommendationAnswers
                .Where(a => questionIds.Contains(a.RecommendationQuestionId))
                .Select(a => new { a.RecommendationQuestionId, a.Content })
                .ToListAsync();

            var newAnswers = seedAnswers
                .Where(a => !existingAnswers.Any(e => e.RecommendationQuestionId == a.RecommendationQuestionId && e.Content == a.Content))
                .ToList();

            if (newAnswers.Count > 0)
            {
                await context.RecommendationAnswers.AddRangeAsync(newAnswers);
                await context.SaveChangesAsync();
            }

            // Return ALL Answers (existing + newly inserted) so downstream methods use real DB IDs
            var allAnswers = await context.RecommendationAnswers
                .Where(a => questionIds.Contains(a.RecommendationQuestionId))
                .OrderBy(a => a.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newAnswers.Count} new RecommendationAnswers. Total Answers in DB: {allAnswers.Count}.");
            return allAnswers;
        }

        private static async Task SeedAnswerCareerWeightsAsync(ApplicationDbContext context, List<RecommendationAnswer> answers, List<Career> careers)
        {
            Console.WriteLine("Seeding AnswerCareerWeights (QUAN TRỌNG NHẤT)...");
            
            var frontendDev = careers.First(c => c.Code == "FRONTEND_DEV");
            var backendDev = careers.First(c => c.Code == "BACKEND_DEV");
            var dataScientist = careers.First(c => c.Code == "DATA_SCIENTIST");
            var devOps = careers.First(c => c.Code == "DEVOPS_ENGINEER");
            var mobileDev = careers.First(c => c.Code == "MOBILE_DEV");
            var aiEngineer = careers.First(c => c.Code == "AI_ENGINEER");
            var cyberSecurity = careers.First(c => c.Code == "CYBERSECURITY_ENGINEER");
            var brandManager = careers.First(c => c.Code == "BRAND_MANAGER");
            var socialMedia = careers.First(c => c.Code == "SOCIAL_MEDIA_SPECIALIST");
            var digitalMarketing = careers.First(c => c.Code == "DIGITAL_MARKETING");
            var contentCreator = careers.First(c => c.Code == "CONTENT_CREATOR");
            var investmentAnalyst = careers.First(c => c.Code == "INVESTMENT_ANALYST");
            var bankingSpecialist = careers.First(c => c.Code == "BANKING_SPECIALIST");
            var financialAnalyst = careers.First(c => c.Code == "FINANCIAL_ANALYST");
            var accountant = careers.First(c => c.Code == "ACCOUNTANT");
            var pharmacist = careers.First(c => c.Code == "PHARMACIST");
            var medLabTech = careers.First(c => c.Code == "MEDICAL_LAB_TECHNICIAN");
            var eduConsultant = careers.First(c => c.Code == "EDUCATIONAL_CONSULTANT");
            var instructionalDesigner = careers.First(c => c.Code == "INSTRUCTIONAL_DESIGNER");

            // Look up answers by content (business key) so weights never bind to the wrong answer on incremental runs
            var answersByContent = answers.ToDictionary(a => a.Content);

            int AnswerId(string content) => answersByContent[content].Id;

            var seedWeights = new List<AnswerCareerWeight>
            {
                // ===== CNTT Weights =====
                // Q1: Frontend vs Backend vs Both
                // Answer 1: Thích giao diện → Frontend++, Backend--, Mobile+
                new AnswerCareerWeight { RecommendationAnswerId = AnswerId("Thích thiết kế giao diện đẹp, tương tác người dùng"), CareerId = frontendDev.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = AnswerId("Thích thiết kế giao diện đẹp, tương tác người dùng"), CareerId = backendDev.Id, Weight = -2 },
                new AnswerCareerWeight { RecommendationAnswerId = AnswerId("Thích thiết kế giao diện đẹp, tương tác người dùng"), CareerId = mobileDev.Id, Weight = 3 },
                // Answer 2: Thích logic → Frontend--, Backend++, Data Science+
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = frontendDev.Id, Weight = -2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = backendDev.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = dataScientist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[1].Id, CareerId = devOps.Id, Weight = 1 },
                // Answer 3: Thích cả hai → Frontend+, Backend+, Mobile+
                new AnswerCareerWeight { RecommendationAnswerId = answers[2].Id, CareerId = frontendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[2].Id, CareerId = backendDev.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[2].Id, CareerId = mobileDev.Id, Weight = 2 },
                // Q2: Data Science interest
                // Answer 4: Rất quan tâm → Data Science++, Backend+, Frontend-
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
                // Q3: DevOps interest
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
                // Q4: Mobile interest
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
                // Q5: AI/ML interest
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
                new AnswerCareerWeight { RecommendationAnswerId = answers[14].Id, CareerId = mobileDev.Id, Weight = 1 },
                // Q6: Bảo vệ hệ thống (bảo mật)
                // Answer 16: Rất muốn bảo mật → Cybersecurity++
                new AnswerCareerWeight { RecommendationAnswerId = answers[15].Id, CareerId = cyberSecurity.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[15].Id, CareerId = devOps.Id, Weight = 1 },
                // Answer 17: Hơi quan tâm → Cybersecurity+
                new AnswerCareerWeight { RecommendationAnswerId = answers[16].Id, CareerId = cyberSecurity.Id, Weight = 2 },
                // Answer 18: Không quan tâm → Cybersecurity--, Frontend+, Backend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[17].Id, CareerId = cyberSecurity.Id, Weight = -2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[17].Id, CareerId = frontendDev.Id, Weight = 1 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[17].Id, CareerId = backendDev.Id, Weight = 1 },
                // Q7: Phân tích dữ liệu & model dự đoán
                // Answer 19: Rất thích build model → Data Science++, AI++
                new AnswerCareerWeight { RecommendationAnswerId = answers[18].Id, CareerId = dataScientist.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[18].Id, CareerId = aiEngineer.Id, Weight = 4 },
                // Answer 20: Thích nhưng không chuyên → Data Science+, Backend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[19].Id, CareerId = dataScientist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[19].Id, CareerId = backendDev.Id, Weight = 1 },
                // Answer 21: Không thích → Data Science--, Frontend+
                new AnswerCareerWeight { RecommendationAnswerId = answers[20].Id, CareerId = dataScientist.Id, Weight = -3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[20].Id, CareerId = frontendDev.Id, Weight = 1 },
                // Q8: Trí tuệ nhân tạo / robot
                // Answer 22: Rất hứng thú AI → AI++, Data Science++
                new AnswerCareerWeight { RecommendationAnswerId = answers[21].Id, CareerId = aiEngineer.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[21].Id, CareerId = dataScientist.Id, Weight = 3 },
                // Answer 23: Hơi hứng thú → AI+, Data Science+
                new AnswerCareerWeight { RecommendationAnswerId = answers[22].Id, CareerId = aiEngineer.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[22].Id, CareerId = dataScientist.Id, Weight = 1 },
                // Answer 24: Không hứng thú → AI--, Frontend++ 
                new AnswerCareerWeight { RecommendationAnswerId = answers[23].Id, CareerId = aiEngineer.Id, Weight = -2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[23].Id, CareerId = frontendDev.Id, Weight = 2 },

                // ===== Marketing Weights =====
                // M1: Sáng tạo nội dung vs quản lý chiến dịch
                // Answer 24 (index) → content creator
                new AnswerCareerWeight { RecommendationAnswerId = answers[24].Id, CareerId = contentCreator.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[24].Id, CareerId = socialMedia.Id, Weight = 2 },
                // Answer 25 → brand manager / digital marketing
                new AnswerCareerWeight { RecommendationAnswerId = answers[25].Id, CareerId = brandManager.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[25].Id, CareerId = digitalMarketing.Id, Weight = 3 },
                // Answer 26 → both
                new AnswerCareerWeight { RecommendationAnswerId = answers[26].Id, CareerId = contentCreator.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[26].Id, CareerId = brandManager.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[26].Id, CareerId = digitalMarketing.Id, Weight = 2 },
                // M2: Social media
                new AnswerCareerWeight { RecommendationAnswerId = answers[27].Id, CareerId = socialMedia.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[27].Id, CareerId = contentCreator.Id, Weight = 3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[28].Id, CareerId = socialMedia.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[29].Id, CareerId = socialMedia.Id, Weight = -2 },
                // M3: Thương hiệu dài hạn
                new AnswerCareerWeight { RecommendationAnswerId = answers[30].Id, CareerId = brandManager.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[31].Id, CareerId = brandManager.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[32].Id, CareerId = brandManager.Id, Weight = -2 },
                // M4: Phân tích số liệu
                new AnswerCareerWeight { RecommendationAnswerId = answers[33].Id, CareerId = digitalMarketing.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[33].Id, CareerId = brandManager.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[34].Id, CareerId = digitalMarketing.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[35].Id, CareerId = digitalMarketing.Id, Weight = -1 },
                // M5: Viết content vs lập kế hoạch
                new AnswerCareerWeight { RecommendationAnswerId = answers[36].Id, CareerId = contentCreator.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[36].Id, CareerId = socialMedia.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[37].Id, CareerId = brandManager.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[37].Id, CareerId = digitalMarketing.Id, Weight = 3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[38].Id, CareerId = contentCreator.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[38].Id, CareerId = brandManager.Id, Weight = 2 },
                // M6: Tương tác cộng đồng
                new AnswerCareerWeight { RecommendationAnswerId = answers[39].Id, CareerId = socialMedia.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[39].Id, CareerId = contentCreator.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[40].Id, CareerId = socialMedia.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[41].Id, CareerId = socialMedia.Id, Weight = -2 },
                // M7: Quản lý nhận diện thương hiệu
                new AnswerCareerWeight { RecommendationAnswerId = answers[42].Id, CareerId = brandManager.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[43].Id, CareerId = brandManager.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[44].Id, CareerId = brandManager.Id, Weight = -2 },
                // M8: SEO
                new AnswerCareerWeight { RecommendationAnswerId = answers[45].Id, CareerId = digitalMarketing.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[46].Id, CareerId = digitalMarketing.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[47].Id, CareerId = digitalMarketing.Id, Weight = -1 },

                // ===== Kinh Tế Weights =====
                // K1: Chứng khoán
                new AnswerCareerWeight { RecommendationAnswerId = answers[48].Id, CareerId = investmentAnalyst.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[48].Id, CareerId = financialAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[49].Id, CareerId = investmentAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[50].Id, CareerId = accountant.Id, Weight = 2 },
                // K2: Ngân hàng
                new AnswerCareerWeight { RecommendationAnswerId = answers[51].Id, CareerId = bankingSpecialist.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[52].Id, CareerId = bankingSpecialist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[53].Id, CareerId = bankingSpecialist.Id, Weight = -2 },
                // K3: Báo cáo tài chính
                new AnswerCareerWeight { RecommendationAnswerId = answers[54].Id, CareerId = accountant.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[54].Id, CareerId = financialAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[55].Id, CareerId = accountant.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[56].Id, CareerId = investmentAnalyst.Id, Weight = 2 },
                // K4: Mô hình định giá
                new AnswerCareerWeight { RecommendationAnswerId = answers[57].Id, CareerId = investmentAnalyst.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[57].Id, CareerId = financialAnalyst.Id, Weight = 3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[58].Id, CareerId = investmentAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[59].Id, CareerId = investmentAnalyst.Id, Weight = -1 },
                // K5: Tư vấn đầu tư
                new AnswerCareerWeight { RecommendationAnswerId = answers[60].Id, CareerId = investmentAnalyst.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[60].Id, CareerId = financialAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[61].Id, CareerId = investmentAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[62].Id, CareerId = investmentAnalyst.Id, Weight = -2 },
                // K6: Quản trị rủi ro
                new AnswerCareerWeight { RecommendationAnswerId = answers[63].Id, CareerId = bankingSpecialist.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[63].Id, CareerId = financialAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[64].Id, CareerId = bankingSpecialist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[65].Id, CareerId = bankingSpecialist.Id, Weight = -1 },
                // K7: Quy định pháp lý
                new AnswerCareerWeight { RecommendationAnswerId = answers[66].Id, CareerId = bankingSpecialist.Id, Weight = 3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[66].Id, CareerId = accountant.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[67].Id, CareerId = accountant.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[68].Id, CareerId = accountant.Id, Weight = -1 },
                // K8: Phân tích báo cáo công ty
                new AnswerCareerWeight { RecommendationAnswerId = answers[69].Id, CareerId = investmentAnalyst.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[69].Id, CareerId = financialAnalyst.Id, Weight = 3 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[70].Id, CareerId = financialAnalyst.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[71].Id, CareerId = financialAnalyst.Id, Weight = -1 },

                // ===== Y Tế Weights =====
                // Y1: Tư vấn/quản lý thuốc
                new AnswerCareerWeight { RecommendationAnswerId = answers[72].Id, CareerId = pharmacist.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[73].Id, CareerId = pharmacist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[74].Id, CareerId = medLabTech.Id, Weight = 2 },
                // Y2: Phòng lab
                new AnswerCareerWeight { RecommendationAnswerId = answers[75].Id, CareerId = medLabTech.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[76].Id, CareerId = medLabTech.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[77].Id, CareerId = pharmacist.Id, Weight = 1 },
                // Y3: Cơ chế tác dụng thuốc
                new AnswerCareerWeight { RecommendationAnswerId = answers[78].Id, CareerId = pharmacist.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[79].Id, CareerId = pharmacist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[80].Id, CareerId = pharmacist.Id, Weight = -1 },
                // Y4: Phân tích mẫu bệnh phẩm
                new AnswerCareerWeight { RecommendationAnswerId = answers[81].Id, CareerId = medLabTech.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[82].Id, CareerId = medLabTech.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[83].Id, CareerId = medLabTech.Id, Weight = -1 },
                // Y5: Chất lượng dược phẩm
                new AnswerCareerWeight { RecommendationAnswerId = answers[84].Id, CareerId = pharmacist.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[85].Id, CareerId = pharmacist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[86].Id, CareerId = pharmacist.Id, Weight = -1 },
                // Y6: Thiết bị hiện đại
                new AnswerCareerWeight { RecommendationAnswerId = answers[87].Id, CareerId = medLabTech.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[88].Id, CareerId = medLabTech.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[89].Id, CareerId = medLabTech.Id, Weight = -1 },
                // Y7: Tương tác thuốc
                new AnswerCareerWeight { RecommendationAnswerId = answers[90].Id, CareerId = pharmacist.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[91].Id, CareerId = pharmacist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[92].Id, CareerId = pharmacist.Id, Weight = -1 },
                // Y8: Tỉ mỉ, chính xác
                new AnswerCareerWeight { RecommendationAnswerId = answers[93].Id, CareerId = medLabTech.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[93].Id, CareerId = pharmacist.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[94].Id, CareerId = medLabTech.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[95].Id, CareerId = medLabTech.Id, Weight = -1 },

                // ===== Giáo Dục Weights =====
                // G1: Tư vấn lộ trình
                new AnswerCareerWeight { RecommendationAnswerId = answers[96].Id, CareerId = eduConsultant.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[97].Id, CareerId = eduConsultant.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[98].Id, CareerId = instructionalDesigner.Id, Weight = 2 },
                // G2: Thiết kế chương trình đào tạo
                new AnswerCareerWeight { RecommendationAnswerId = answers[99].Id, CareerId = instructionalDesigner.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[100].Id, CareerId = instructionalDesigner.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[101].Id, CareerId = eduConsultant.Id, Weight = 1 },
                // G3: Làm việc với người học
                new AnswerCareerWeight { RecommendationAnswerId = answers[102].Id, CareerId = eduConsultant.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[103].Id, CareerId = eduConsultant.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[104].Id, CareerId = instructionalDesigner.Id, Weight = -1 },
                // G4: Khóa học online
                new AnswerCareerWeight { RecommendationAnswerId = answers[105].Id, CareerId = instructionalDesigner.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[106].Id, CareerId = instructionalDesigner.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[107].Id, CareerId = instructionalDesigner.Id, Weight = -1 },
                // G5: Phân tích năng lực & hướng nghiệp
                new AnswerCareerWeight { RecommendationAnswerId = answers[108].Id, CareerId = eduConsultant.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[109].Id, CareerId = eduConsultant.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[110].Id, CareerId = instructionalDesigner.Id, Weight = 1 },
                // G6: Công nghệ giáo dục
                new AnswerCareerWeight { RecommendationAnswerId = answers[111].Id, CareerId = instructionalDesigner.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[112].Id, CareerId = instructionalDesigner.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[113].Id, CareerId = eduConsultant.Id, Weight = 1 },
                // G7: Đánh giá hiệu quả đào tạo
                new AnswerCareerWeight { RecommendationAnswerId = answers[114].Id, CareerId = instructionalDesigner.Id, Weight = 4 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[115].Id, CareerId = instructionalDesigner.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[116].Id, CareerId = eduConsultant.Id, Weight = 1 },
                // G8: Giúp người khác phát triển
                new AnswerCareerWeight { RecommendationAnswerId = answers[117].Id, CareerId = eduConsultant.Id, Weight = 5 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[118].Id, CareerId = eduConsultant.Id, Weight = 2 },
                new AnswerCareerWeight { RecommendationAnswerId = answers[119].Id, CareerId = eduConsultant.Id, Weight = -1 }
            };

            // Only insert relationships that do not already exist (detect by RecommendationAnswerId + CareerId)
            var answerIds = seedWeights.Select(w => w.RecommendationAnswerId).Distinct().ToList();
            var careerIds = seedWeights.Select(w => w.CareerId).Distinct().ToList();
            var existingWeights = await context.AnswerCareerWeights
                .Where(w => answerIds.Contains(w.RecommendationAnswerId) && careerIds.Contains(w.CareerId))
                .Select(w => new { w.RecommendationAnswerId, w.CareerId })
                .ToListAsync();

            var newWeights = seedWeights
                .Where(w => !existingWeights.Any(e => e.RecommendationAnswerId == w.RecommendationAnswerId && e.CareerId == w.CareerId))
                .ToList();

            if (newWeights.Count > 0)
            {
                await context.AnswerCareerWeights.AddRangeAsync(newWeights);
                await context.SaveChangesAsync();
            }

            Console.WriteLine($"Seeded {newWeights.Count} new AnswerCareerWeights.");
        }

        private static async Task<List<LearningQuestion>> SeedLearningQuestionsAsync(ApplicationDbContext context, List<Topic> topics)
        {
            Console.WriteLine("Seeding LearningQuestions...");

            // Look up topics by name + subject-based lookup. Use the topics list passed in.
            var htmlSemantic = topics.FirstOrDefault(t => t.Name == "HTML Semantic & Structure");
            var cssLayout = topics.FirstOrDefault(t => t.Name == "CSS Layout & Flexbox");
            var es6Js = topics.FirstOrDefault(t => t.Name == "ES6 & Modern JavaScript");
            var domManip = topics.FirstOrDefault(t => t.Name == "DOM Manipulation & Events");
            var reactComponents = topics.FirstOrDefault(t => t.Name == "Components & Props");
            var reactHooks = topics.FirstOrDefault(t => t.Name == "Hooks & Lifecycle");
            var csharpVars = topics.FirstOrDefault(t => t.Name == "Biến & Kiểu Dữ Liệu C#");
            var csharpControl = topics.FirstOrDefault(t => t.Name == "Cấu Trúc Điều Khiển C#");
            var csharpMethods = topics.FirstOrDefault(t => t.Name == "Phương Thức & Collections");
            var oopClass = topics.FirstOrDefault(t => t.Name == "Class & Encapsulation");
            var oopInherit = topics.FirstOrDefault(t => t.Name == "Inheritance & Polymorphism");
            var sqlSelect = topics.FirstOrDefault(t => t.Name == "SELECT & WHERE");
            var sqlJoin = topics.FirstOrDefault(t => t.Name == "JOIN & GROUP BY");
            var sqlKeys = topics.FirstOrDefault(t => t.Name == "Primary Key & Foreign Key");
            var apiRouting = topics.FirstOrDefault(t => t.Name == "Web API & Routing");
            var diMiddleware = topics.FirstOrDefault(t => t.Name == "Dependency Injection & Middleware");
            var dbContext = topics.FirstOrDefault(t => t.Name == "DbContext & Relationships");
            var linqQueries = topics.FirstOrDefault(t => t.Name == "LINQ Queries & Migrations");

            var seedQuestions = new List<LearningQuestion>();

            // ===== HTML & CSS =====
            if (htmlSemantic != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Thẻ HTML nào dùng để tạo một heading lớn nhất?", TopicId = htmlSemantic.Id, Difficulty = 1, Explanation = "<h1> là thẻ heading cấp cao nhất trong HTML.", Hint = "Heading từ h1 đến h6.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Thẻ semantic nào dùng để định nghĩa phần điều hướng chính của trang?", TopicId = htmlSemantic.Id, Difficulty = 1, Explanation = "<nav> dùng để nhóm các liên kết điều hướng.", Hint = "Navigation.", questionIndex = 2 });
            }
            if (cssLayout != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Thuộc tính CSS nào dùng để căn giữa một phần tử con theo chiều ngang với Flexbox?", TopicId = cssLayout.Id, Difficulty = 2, Explanation = "justify-content: center căn giữa theo trục chính (mặc định ngang).", Hint = "Thuộc tính trên container flex.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "CSS Grid dùng thuộc tính nào để định nghĩa số cột?", TopicId = cssLayout.Id, Difficulty = 2, Explanation = "grid-template-columns xác định số cột và kích thước của grid.", Hint = "Định nghĩa cột của grid.", questionIndex = 2 });
            }

            // ===== JavaScript =====
            if (es6Js != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Cú pháp nào là Arrow Function hợp lệ trong ES6?", TopicId = es6Js.Id, Difficulty = 2, Explanation = "const add = (a, b) => a + b; là cú pháp arrow function hợp lệ.", Hint = "Sử dụng dấu =>.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Từ khóa nào dùng để khai báo biến có phạm vi khối (block scope) trong ES6?", TopicId = es6Js.Id, Difficulty = 1, Explanation = "let và const có phạm vi khối, khác với var có phạm vi hàm.", Hint = "Hai từ khóa mới trong ES6.", questionIndex = 2 });
            }
            if (domManip != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Phương thức nào dùng để chọn phần tử theo id trong DOM?", TopicId = domManip.Id, Difficulty = 1, Explanation = "document.getElementById('id') trả về phần tử có id tương ứng.", Hint = "getElement...", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Sự kiện nào được kích hoạt khi người dùng click vào một phần tử?", TopicId = domManip.Id, Difficulty = 1, Explanation = "Sự kiện 'click' xảy ra khi người dùng click chuột.", Hint = "Sự kiện chuột.", questionIndex = 2 });
            }

            // ===== React =====
            if (reactComponents != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Trong React, dữ liệu truyền từ cha xuống con được gọi là gì?", TopicId = reactComponents.Id, Difficulty = 2, Explanation = "Props là dữ liệu truyền từ component cha sang con.", Hint = "Không thể thay đổi từ component con.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Hook nào dùng để quản lý state trong function component?", TopicId = reactComponents.Id, Difficulty = 2, Explanation = "useState dùng để khai báo và cập nhật state.", Hint = "use...", questionIndex = 2 });
            }
            if (reactHooks != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Hook useEffect dùng để làm gì?", TopicId = reactHooks.Id, Difficulty = 2, Explanation = "useEffect thực hiện side effects như gọi API, subscribe.", Hint = "Side effects.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Mảng dependency trong useEffect dùng để làm gì?", TopicId = reactHooks.Id, Difficulty = 3, Explanation = "Mảng dependency quyết định khi nào effect chạy lại.", Hint = "Kiểm soát lần chạy lại.", questionIndex = 2 });
            }

            // ===== C# Fundamentals =====
            if (csharpVars != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Kiểu dữ liệu nào dùng để lưu số nguyên có dấu 32-bit trong C#?", TopicId = csharpVars.Id, Difficulty = 1, Explanation = "int là kiểu số nguyên 32-bit có dấu.", Hint = "Kiểu số nguyên phổ biến nhất.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Từ khóa 'var' trong C# dùng để làm gì?", TopicId = csharpVars.Id, Difficulty = 1, Explanation = "'var' cho phép trình biên dịch suy luận kiểu dữ liệu từ giá trị khởi tạo.", Hint = "Suy luận kiểu ngầm định.", questionIndex = 2 });
            }
            if (csharpControl != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Vòng lặp nào chạy ít nhất một lần?", TopicId = csharpControl.Id, Difficulty = 1, Explanation = "do-while luôn chạy thân vòng lặp trước rồi mới kiểm tra điều kiện.", Hint = "Kiểm tra sau khi thực thi.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Từ khóa nào dùng để thoát khỏi vòng lặp ngay lập tức?", TopicId = csharpControl.Id, Difficulty = 1, Explanation = "'break' dừng vòng lặp hiện tại.", Hint = "Dừng vòng lặp.", questionIndex = 2 });
            }
            if (csharpMethods != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Collection nào lưu trữ các cặp key-value trong C#?", TopicId = csharpMethods.Id, Difficulty = 2, Explanation = "Dictionary<TKey, TValue> lưu các cặp khóa - giá trị.", Hint = "Từ điển.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Phương thức có kiểu trả về void nghĩa là gì?", TopicId = csharpMethods.Id, Difficulty = 1, Explanation = "void nghĩa là phương thức không trả về giá trị nào.", Hint = "Không trả về.", questionIndex = 2 });
            }

            // ===== OOP C# =====
            if (oopClass != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Tính chất nào của OOP che giấu chi tiết triển khai nội bộ?", TopicId = oopClass.Id, Difficulty = 2, Explanation = "Encapsulation (đóng gói) che giấu dữ liệu nội bộ.", Hint = "Che giấu dữ liệu.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Access modifier nào cho phép truy cập từ bất kỳ đâu?", TopicId = oopClass.Id, Difficulty = 1, Explanation = "'public' cho phép truy cập từ mọi nơi.", Hint = "Truy cập toàn cục.", questionIndex = 2 });
            }
            if (oopInherit != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Từ khóa nào dùng để kế thừa một class trong C#?", TopicId = oopInherit.Id, Difficulty = 1, Explanation = "Dấu ':' dùng để kế thừa class trong C#.", Hint = "Dấu hai chấm.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Một class có thể implements bao nhiêu interface?", TopicId = oopInherit.Id, Difficulty = 2, Explanation = "Một class C# có thể implements nhiều interface.", Hint = "Nhiều hơn một.", questionIndex = 2 });
            }

            // ===== SQL =====
            if (sqlSelect != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Mệnh đề nào dùng để lọc dữ liệu trong câu lệnh SELECT?", TopicId = sqlSelect.Id, Difficulty = 1, Explanation = "WHERE dùng để lọc các bản ghi theo điều kiện.", Hint = "Điều kiện lọc.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Toán tử nào dùng để tìm các giá trị trùng khớp một danh sách?", TopicId = sqlSelect.Id, Difficulty = 2, Explanation = "IN dùng để kiểm tra giá trị có nằm trong danh sách cho trước.", Hint = "Kiểm tra thuộc danh sách.", questionIndex = 2 });
            }
            if (sqlJoin != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "JOIN nào trả về tất cả bản ghi từ bảng trái và các bản ghi khớp từ bảng phải?", TopicId = sqlJoin.Id, Difficulty = 2, Explanation = "LEFT JOIN trả về tất cả bản ghi bảng trái, kể cả không khớp.", Hint = "Ưu tiên bảng bên trái.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Hàm nào dùng để đếm số lượng bản ghi trong SQL?", TopicId = sqlJoin.Id, Difficulty = 1, Explanation = "COUNT() đếm số hàng trong một nhóm hoặc bảng.", Hint = "Hàm tổng hợp.", questionIndex = 2 });
            }
            if (sqlKeys != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Khóa chính (Primary Key) có đặc điểm gì?", TopicId = sqlKeys.Id, Difficulty = 1, Explanation = "Primary Key là duy nhất và không được NULL.", Hint = "Duy nhất, không null.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Khóa ngoại (Foreign Key) dùng để làm gì?", TopicId = sqlKeys.Id, Difficulty = 2, Explanation = "Foreign Key thiết lập mối quan hệ giữa hai bảng.", Hint = "Liên kết bảng.", questionIndex = 2 });
            }

            // ===== ASP.NET Core =====
            if (apiRouting != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Attribute nào dùng để định nghĩa một action trả về dữ liệu JSON trong Web API?", TopicId = apiRouting.Id, Difficulty = 2, Explanation = "[ApiController] và các action trả về IActionResult/Ok tự động serialize JSON.", Hint = "ActionResult.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "HTTP method nào dùng để cập nhật toàn bộ một tài nguyên?", TopicId = apiRouting.Id, Difficulty = 1, Explanation = "PUT dùng để cập nhật toàn bộ tài nguyên.", Hint = "Cập nhật toàn phần.", questionIndex = 2 });
            }
            if (diMiddleware != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Dependency Injection giúp gì cho ứng dụng?", TopicId = diMiddleware.Id, Difficulty = 2, Explanation = "DI giúp quản lý dependencies, tăng khả năng test và giảm coupling.", Hint = "Quản lý dependency.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Middleware pipeline trong ASP.NET Core xử lý request như thế nào?", TopicId = diMiddleware.Id, Difficulty = 3, Explanation = "Request đi qua chuỗi các middleware theo thứ tự đăng ký.", Hint = "Chuỗi xử lý.", questionIndex = 2 });
            }

            // ===== EF Core / LINQ =====
            if (dbContext != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Lớp nào đại diện cho phiên làm việc với cơ sở dữ liệu trong EF Core?", TopicId = dbContext.Id, Difficulty = 2, Explanation = "DbContext đại diện cho phiên làm việc, quản lý entities và kết nối.", Hint = "Phiên làm việc.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "DbSet<T> trong DbContext dùng để làm gì?", TopicId = dbContext.Id, Difficulty = 2, Explanation = "DbSet<T> đại diện cho một bảng trong database.", Hint = "Đại diện bảng.", questionIndex = 2 });
            }
            if (linqQueries != null)
            {
                seedQuestions.Add(new LearningQuestion { Content = "Toán tử LINQ nào dùng để lọc collection?", TopicId = linqQueries.Id, Difficulty = 1, Explanation = "Where() dùng để lọc các phần tử theo điều kiện.", Hint = "Lọc.", questionIndex = 1 });
                seedQuestions.Add(new LearningQuestion { Content = "Lệnh nào tạo migration trong EF Core?", TopicId = linqQueries.Id, Difficulty = 2, Explanation = "dotnet ef migrations add <Tên> tạo migration mới.", Hint = "ef migrations.", questionIndex = 2 });
            }

            // Only insert LearningQuestions that do not already exist (detect by TopicId + Content)
            var topicIds = seedQuestions.Select(q => q.TopicId).Distinct().ToList();
            var existingQuestions = await context.LearningQuestions
                .Where(q => topicIds.Contains(q.TopicId))
                .Select(q => new { q.TopicId, q.Content })
                .ToListAsync();

            var newQuestions = seedQuestions
                .Where(q => !existingQuestions.Any(e => e.TopicId == q.TopicId && e.Content == q.Content))
                .ToList();

            if (newQuestions.Count > 0)
            {
                await context.LearningQuestions.AddRangeAsync(newQuestions);
                await context.SaveChangesAsync();
            }

            // Return ALL LearningQuestions (existing + newly inserted) so downstream methods use real DB IDs
            var allQuestions = await context.LearningQuestions
                .Where(q => topicIds.Contains(q.TopicId))
                .OrderBy(q => q.Id)
                .ToListAsync();

            Console.WriteLine($"Seeded {newQuestions.Count} new LearningQuestions. Total LearningQuestions in DB: {allQuestions.Count}.");
            return allQuestions;
        }

        private static async Task SeedLearningAnswersAsync(ApplicationDbContext context, List<LearningQuestion> questions)
        {
            Console.WriteLine("Seeding LearningAnswers...");

            // Helper to find a question by content and add answers
            async Task AddAnswers(string questionContent, params (string content, bool isCorrect, string explanation)[] answerSpecs)
            {
                var question = questions.FirstOrDefault(q => q.Content == questionContent);
                if (question == null) return;

                var existingAnswers = await context.LearningAnswers
                    .Where(a => a.LearningQuestionId == question.Id)
                    .Select(a => a.Content)
                    .ToListAsync();

                var newAnswers = answerSpecs
                    .Where(spec => !existingAnswers.Contains(spec.content))
                    .Select(spec => new LearningAnswer
                    {
                        Content = spec.content,
                        IsCorrect = spec.isCorrect,
                        Explanation = spec.explanation,
                        LearningQuestionId = question.Id
                    })
                    .ToList();

                if (newAnswers.Count > 0)
                {
                    await context.LearningAnswers.AddRangeAsync(newAnswers);
                }
            }

            // ===== HTML & CSS =====
            await AddAnswers("Thẻ HTML nào dùng để tạo một heading lớn nhất?",
                ("<h1>", true, "<h1> là heading cấp cao nhất."),
                ("<h6>", false, "<h6> là heading nhỏ nhất."),
                ("<p>", false, "<p> dùng cho đoạn văn."),
                ("<div>", false, "<div> là khối chung."));

            await AddAnswers("Thẻ semantic nào dùng để định nghĩa phần điều hướng chính của trang?",
                ("<nav>", true, "<nav> dùng cho điều hướng."),
                ("<footer>", false, "Footer là chân trang."),
                ("<header>", false, "Header là phần đầu trang."),
                ("<section>", false, "Section là một phần nội dung."));

            await AddAnswers("Thuộc tính CSS nào dùng để căn giữa một phần tử con theo chiều ngang với Flexbox?",
                ("justify-content: center", true, "Căn giữa theo trục chính."),
                ("align-items: center", false, "Căn giữa theo trục chéo."),
                ("text-align: center", false, "Căn giữa văn bản, không phải Flexbox."),
                ("margin: 0 auto", false, "Không phải thuộc tính Flexbox."));

            await AddAnswers("CSS Grid dùng thuộc tính nào để định nghĩa số cột?",
                ("grid-template-columns", true, "Định nghĩa số cột của grid."),
                ("display: flex", false, "Đây là Flexbox, không phải Grid."),
                ("grid-gap", false, "Khoảng cách giữa các ô."),
                ("grid-row", false, "Định vị hàng cho item."));

            // ===== JavaScript =====
            await AddAnswers("Cú pháp nào là Arrow Function hợp lệ trong ES6?",
                ("const add = (a, b) => a + b;", true, "Arrow function đúng cú pháp."),
                ("function add(a,b) { return a+b; }", false, "Đây là function declaration."),
                ("const add = a, b => a+b;", false, "Thiếu dấu ngoặc không hợp lệ."),
                ("add => a + b =>", false, "Cú pháp sai."));

            await AddAnswers("Từ khóa nào dùng để khai báo biến có phạm vi khối (block scope) trong ES6?",
                ("let", true, "let có phạm vi khối."),
                ("var", false, "var có phạm vi hàm."),
                ("int", false, "Không phải từ khóa JS."),
                ("static", false, "Không liên quan."));

            await AddAnswers("Phương thức nào dùng để chọn phần tử theo id trong DOM?",
                ("document.getElementById('id')", true, "Phương thức đúng."),
                ("document.querySelectorAll('id')", false, "Trả về NodeList, không dùng cho id đơn."),
                ("document.getElementsByClass('id')", false, "Dùng cho class, không phải id."),
                ("window.getElementById('id')", false, "Sai đối tượng."));

            await AddAnswers("Sự kiện nào được kích hoạt khi người dùng click vào một phần tử?",
                ("click", true, "Sự kiện click."),
                ("mouseover", false, "Di chuột qua."),
                ("keydown", false, "Nhấn phím."),
                ("submit", false, "Gửi form."));

            // ===== React =====
            await AddAnswers("Trong React, dữ liệu truyền từ cha xuống con được gọi là gì?",
                ("Props", true, "Props truyền từ cha xuống con."),
                ("State", false, "State là dữ liệu nội bộ component."),
                ("Context", false, "Context là cách chia sẻ dữ liệu toàn cục."),
                ("Ref", false, "Ref truy cập DOM."));

            await AddAnswers("Hook nào dùng để quản lý state trong function component?",
                ("useState", true, "useState quản lý state."),
                ("useEffect", false, "useEffect cho side effects."),
                ("useRef", false, "useRef cho tham chiếu."),
                ("useMemo", false, "useMemo cho memoization."));

            await AddAnswers("Hook useEffect dùng để làm gì?",
                ("Thực hiện side effects", true, "useEffect cho side effects như gọi API."),
                ("Quản lý state", false, "Đó là useState."),
                ("Tạo ref", false, "Đó là useRef."),
                ("Tối ưu performance", false, "Đó là useMemo/useCallback."));

            await AddAnswers("Mảng dependency trong useEffect dùng để làm gì?",
                ("Quyết định khi nào effect chạy lại", true, "Mảng dependency kiểm soát lần chạy."),
                ("Lưu trữ state", false, "Không đúng."),
                ("Truyền props", false, "Không đúng."),
                ("Tạo component", false, "Không đúng."));

            // ===== C# Fundamentals =====
            await AddAnswers("Kiểu dữ liệu nào dùng để lưu số nguyên có dấu 32-bit trong C#?",
                ("int", true, "int là số nguyên 32-bit."),
                ("long", false, "long là 64-bit."),
                ("short", false, "short là 16-bit."),
                ("double", false, "double là số thực."));

            await AddAnswers("Từ khóa 'var' trong C# dùng để làm gì?",
                ("Suy luận kiểu ngầm định", true, "'var' để trình biên dịch suy ra kiểu."),
                ("Khai báo hằng số", false, "Hằng số dùng 'const'."),
                ("Khai báo nullable", false, "Dùng '?'."),
                ("Khai báo dynamic", false, "Đó là 'dynamic'."));

            await AddAnswers("Vòng lặp nào chạy ít nhất một lần?",
                ("do-while", true, "do-while chạy trước rồi kiểm tra."),
                ("while", false, "while kiểm tra trước."),
                ("for", false, "for kiểm tra trước."),
                ("foreach", false, "foreach duyệt collection."));

            await AddAnswers("Từ khóa nào dùng để thoát khỏi vòng lặp ngay lập tức?",
                ("break", true, "break dừng vòng lặp."),
                ("continue", false, "continue bỏ qua lần lặp hiện tại."),
                ("return", false, "return thoát khỏi phương thức."),
                ("exit", false, "Không phải từ khóa C#."));

            await AddAnswers("Collection nào lưu trữ các cặp key-value trong C#?",
                ("Dictionary<TKey, TValue>", true, "Dictionary lưu key-value."),
                ("List<T>", false, "List lưu danh sách."),
                ("HashSet<T>", false, "HashSet lưu tập hợp duy nhất."),
                ("Queue<T>", false, "Queue là hàng đợi."));

            await AddAnswers("Phương thức có kiểu trả về void nghĩa là gì?",
                ("Không trả về giá trị", true, "void không trả về giá trị."),
                ("Trả về int", false, "int mới trả về số."),
                ("Trả về string", false, "string trả về chuỗi."),
                ("Trả về bool", false, "bool trả về đúng/sai."));

            // ===== OOP C# =====
            await AddAnswers("Tính chất nào của OOP che giấu chi tiết triển khai nội bộ?",
                ("Encapsulation", true, "Encapsulation đóng gói và che giấu."),
                ("Inheritance", false, "Inheritance là kế thừa."),
                ("Polymorphism", false, "Polymorphism là đa hình."),
                ("Abstraction", false, "Abstraction là trừu tượng hóa."));

            await AddAnswers("Access modifier nào cho phép truy cập từ bất kỳ đâu?",
                ("public", true, "public truy cập từ mọi nơi."),
                ("private", false, "private chỉ trong class."),
                ("protected", false, "protected trong class và class kế thừa."),
                ("internal", false, "internal trong cùng assembly."));

            await AddAnswers("Từ khóa nào dùng để kế thừa một class trong C#?",
                (":", true, "Dấu ':' dùng để kế thừa."),
                ("extends", false, "extends là từ khóa Java."),
                ("inherit", false, "Không phải từ khóa C#."),
                ("base", false, "base dùng để gọi constructor cha."));

            await AddAnswers("Một class có thể implements bao nhiêu interface?",
                ("Nhiều interface", true, "C# hỗ trợ implements nhiều interface."),
                ("Chỉ một", false, "C# không giới hạn một interface."),
                ("Tối đa hai", false, "Không giới hạn."),
                ("Không được", false, "Class có thể implements interface."));

            // ===== SQL =====
            await AddAnswers("Mệnh đề nào dùng để lọc dữ liệu trong câu lệnh SELECT?",
                ("WHERE", true, "WHERE lọc theo điều kiện."),
                ("ORDER BY", false, "ORDER BY sắp xếp."),
                ("GROUP BY", false, "GROUP BY nhóm dữ liệu."),
                ("HAVING", false, "HAVING lọc theo nhóm."));

            await AddAnswers("Toán tử nào dùng để tìm các giá trị trùng khớp một danh sách?",
                ("IN", true, "IN kiểm tra thuộc danh sách."),
                ("BETWEEN", false, "BETWEEN kiểm tra trong khoảng."),
                ("LIKE", false, "LIKE tìm chuỗi gần đúng."),
                ("EXISTS", false, "EXISTS kiểm tra tồn tại."));

            await AddAnswers("JOIN nào trả về tất cả bản ghi từ bảng trái và các bản ghi khớp từ bảng phải?",
                ("LEFT JOIN", true, "LEFT JOIN ưu tiên bảng trái."),
                ("INNER JOIN", false, "INNER JOIN chỉ trả về bản ghi khớp."),
                ("RIGHT JOIN", false, "RIGHT JOIN ưu tiên bảng phải."),
                ("FULL JOIN", false, "FULL JOIN trả về cả hai."));

            await AddAnswers("Hàm nào dùng để đếm số lượng bản ghi trong SQL?",
                ("COUNT()", true, "COUNT đếm số hàng."),
                ("SUM()", false, "SUM tính tổng giá trị."),
                ("AVG()", false, "AVG tính trung bình."),
                ("MAX()", false, "MAX lấy giá trị lớn nhất."));

            await AddAnswers("Khóa chính (Primary Key) có đặc điểm gì?",
                ("Duy nhất và không NULL", true, "Primary Key duy nhất, không null."),
                ("Có thể trùng", false, "Không được trùng."),
                ("Có thể NULL", false, "Không được NULL."),
                ("Chỉ có một bảng", false, "Mỗi bảng có tối đa một PK nhưng PK là khóa."));

            await AddAnswers("Khóa ngoại (Foreign Key) dùng để làm gì?",
                ("Thiết lập mối quan hệ giữa hai bảng", true, "FK liên kết hai bảng."),
                ("Tăng tốc truy vấn", false, "Đó là INDEX."),
                ("Lưu dữ liệu lớn", false, "Không đúng."),
                ("Mã hóa dữ liệu", false, "Không đúng."));

            // ===== ASP.NET Core =====
            await AddAnswers("Attribute nào dùng để định nghĩa một action trả về dữ liệu JSON trong Web API?",
                ("[ApiController]", true, "ApiController tự serialize JSON."),
                ("[HttpGet]", false, "HttpGet định nghĩa HTTP method."),
                ("[Route]", false, "Route định nghĩa đường dẫn."),
                ("[Authorize]", false, "Authorize kiểm tra quyền."));

            await AddAnswers("HTTP method nào dùng để cập nhật toàn bộ một tài nguyên?",
                ("PUT", true, "PUT cập nhật toàn phần."),
                ("PATCH", false, "PATCH cập nhật một phần."),
                ("POST", false, "POST tạo mới."),
                ("DELETE", false, "DELETE xóa."));

            await AddAnswers("Dependency Injection giúp gì cho ứng dụng?",
                ("Giảm coupling và tăng testability", true, "DI giảm kết nối, dễ test."),
                ("Tăng tốc database", false, "Không liên quan."),
                ("Mã hóa dữ liệu", false, "Không liên quan."),
                ("Tạo giao diện", false, "Không liên quan."));

            await AddAnswers("Middleware pipeline trong ASP.NET Core xử lý request như thế nào?",
                ("Request đi qua chuỗi middleware", true, "Request qua chuỗi middleware theo thứ tự."),
                ("Request xử lý song song", false, "Xử lý tuần tự."),
                ("Middleware tự động", false, "Cần đăng ký."),
                ("Không dùng middleware", false, "ASP.NET Core dùng middleware."));

            // ===== EF Core / LINQ =====
            await AddAnswers("Lớp nào đại diện cho phiên làm việc với cơ sở dữ liệu trong EF Core?",
                ("DbContext", true, "DbContext là phiên làm việc."),
                ("DbSet", false, "DbSet đại diện bảng."),
                ("Entity", false, "Entity là đối tượng dữ liệu."),
                ("Migration", false, "Migration là cấu trúc schema."));

            await AddAnswers("DbSet<T> trong DbContext dùng để làm gì?",
                ("Đại diện cho một bảng", true, "DbSet đại diện bảng."),
                ("Quản lý migration", false, "Không đúng."),
                ("Xử lý authentication", false, "Không đúng."),
                ("Tạo view", false, "Không đúng."));

            await AddAnswers("Toán tử LINQ nào dùng để lọc collection?",
                ("Where()", true, "Where lọc phần tử."),
                ("Select()", false, "Select chiếu dữ liệu."),
                ("OrderBy()", false, "OrderBy sắp xếp."),
                ("GroupBy()", false, "GroupBy nhóm."));

            await AddAnswers("Lệnh nào tạo migration trong EF Core?",
                ("dotnet ef migrations add", true, "Lệnh tạo migration."),
                ("dotnet ef database update", false, "Lệnh cập nhật database."),
                ("dotnet build", false, "Build project."),
                ("dotnet run", false, "Chạy ứng dụng."));

            await context.SaveChangesAsync();
            Console.WriteLine("LearningAnswers seeded successfully.");
        }
 
       }
   
 }