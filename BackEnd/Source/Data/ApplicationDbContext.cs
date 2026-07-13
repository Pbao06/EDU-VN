using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Source.Models;
namespace Source.Data
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            // o trn cau hinh thg nay vao 1 bien options de su dung cho cac dbcontext khac neu can
            // option se duoc Program.cs truyen vao ket noi Dbdatabse
        }
        // DbSet cho các bảng
        public DbSet<Career> Careers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<CareerSubject> CareerSubjects { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        
        // Recommendation Domain
        public DbSet<RecommendationQuestion> RecommendationQuestions { get; set; }
        public DbSet<RecommendationAnswer> RecommendationAnswers { get; set; }
        public DbSet<RecommendationUserAnswer> RecommendationUserAnswers { get; set; }
        
        // Learning Domain
        public DbSet<LearningQuestion> LearningQuestions { get; set; }
        public DbSet<LearningAnswer> LearningAnswers { get; set; }
        public DbSet<LearningUserAnswer> LearningUserAnswers { get; set; }
        
        public DbSet<QuizCareerRecommendation> QuizCareerRecommendations { get; set; }
        public DbSet<LearningPath> UserLearningPaths { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AnswerCareerWeight> AnswerCareerWeights { get; set; }
        public DbSet<Field> Fields { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Unique Index cho Code columns
            builder.Entity<Field>()
                .HasIndex(f => f.Code)
                .IsUnique();

            builder.Entity<Career>()
                .HasIndex(c => c.Code)
                .IsUnique();

            builder.Entity<Subject>()
                .HasIndex(s => s.Code)
                .IsUnique();

            builder.Entity<Quiz>()
                .HasIndex(q => q.Code)
                .IsUnique();

            // CareerSubject - Many-to-Many
            builder.Entity<CareerSubject>()
                .HasOne(cs => cs.Career)
                .WithMany(c => c.CareerSubjects)
                .HasForeignKey(cs => cs.CareerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CareerSubject>()
                .HasOne(cs => cs.Subject)
                .WithMany(s => s.CareerSubjects)
                .HasForeignKey(cs => cs.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint cho CareerSubject để tránh trùng lặp
            builder.Entity<CareerSubject>()
                .HasIndex(cs => new { cs.CareerId, cs.SubjectId })
                .IsUnique();
            // 
            // QuizCareerRecommendation
            builder.Entity<QuizCareerRecommendation>()
                .HasOne(q => q.User)
                .WithMany(u => u.Recommendations)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<QuizCareerRecommendation>()
                .HasOne(q => q.Career)
                .WithMany(c => c.Recommendations)
                .HasForeignKey(q => q.CareerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<QuizCareerRecommendation>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.QuizCareerRecommendations)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            // UserLearningPath
            builder.Entity<LearningPath>()
                .HasOne(ulp => ulp.User)
                .WithMany(u => u.LearningPaths)
                .HasForeignKey(ulp => ulp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LearningPath>()
                .HasOne(ulp => ulp.Career)
                .WithMany(c => c.LearningPaths)
                .HasForeignKey(ulp => ulp.CareerId)
                .OnDelete(DeleteBehavior.Cascade);
            // UserProgress
            builder.Entity<UserProgress>()
                .HasOne(up => up.User)
                .WithMany(u => u.Progresses)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserProgress>()
                .HasOne(up => up.Topic)
                .WithMany(s => s.UserProgresses)
                .HasForeignKey(up => up.TopicId)
                .OnDelete(DeleteBehavior.Restrict);
            // Topic -> Subject
            builder.Entity<Topic>()
                .HasOne(t => t.Subject)
                .WithMany(s => s.Topics)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

          

            // RecommendationQuestion -> Quiz
            builder.Entity<RecommendationQuestion>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // LearningQuestion -> Topic
            builder.Entity<LearningQuestion>()
                .HasOne(q => q.Topic)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            // RecommendationAnswer -> RecommendationQuestion
            builder.Entity<RecommendationAnswer>()
                .HasOne(a => a.RecommendationQuestion)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.RecommendationQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // LearningAnswer -> LearningQuestion
            builder.Entity<LearningAnswer>()
                .HasOne(a => a.LearningQuestion)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.LearningQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index cho performance
            builder.Entity<QuizCareerRecommendation>()
                .HasIndex(q => new { q.UserId, q.CreatedAt });

            builder.Entity<LearningPath>()
                .HasIndex(ulp => new { ulp.UserId, ulp.IsActive });

            // RefreshToken -> User
            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index cho RefreshToken
            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            // AnswerCareerWeight -> RecommendationAnswer & Career
            builder.Entity<AnswerCareerWeight>()
                .HasOne(acw => acw.RecommendationAnswer)
                .WithMany(a => a.AnswerCareerWeights)
                .HasForeignKey(acw => acw.RecommendationAnswerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AnswerCareerWeight>()
                .HasOne(acw => acw.Career)
                .WithMany(c => c.AnswerCareerWeights)
                .HasForeignKey(acw => acw.CareerId)
                .OnDelete(DeleteBehavior.Cascade);

            // RecommendationUserAnswer -> User, Quiz, RecommendationQuestion, RecommendationAnswer
            builder.Entity<RecommendationUserAnswer>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.RecommendationUserAnswers)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RecommendationUserAnswer>()
                .HasOne(ua => ua.Quiz)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RecommendationUserAnswer>()
                .HasOne(ua => ua.RecommendationQuestion)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.RecommendationQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RecommendationUserAnswer>()
                .HasOne(ua => ua.RecommendationAnswer)
                .WithMany(a => a.UserAnswers)
                .HasForeignKey(ua => ua.RecommendationAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // LearningUserAnswer -> User, LearningQuestion, LearningAnswer
            builder.Entity<LearningUserAnswer>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.LearningUserAnswers)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LearningUserAnswer>()
                .HasOne(ua => ua.LearningQuestion)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.LearningQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LearningUserAnswer>()
                .HasOne(ua => ua.LearningAnswer)
                .WithMany(a => a.UserAnswers)
                .HasForeignKey(ua => ua.LearningAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Field -> Users, Careers, Quizzes
            builder.Entity<Field>()
                .HasMany(f => f.Users)
                .WithOne(u => u.Field)
                .HasForeignKey(u => u.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Field>()
                .HasMany(f => f.Careers)
                .WithOne(c => c.Field)
                .HasForeignKey(c => c.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Quiz>()
                .HasOne(q => q.Field)
                .WithMany(f => f.Quizzes)
                .HasForeignKey(q => q.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index cho AnswerCareerWeight
            builder.Entity<AnswerCareerWeight>()
                .HasIndex(acw => new { acw.RecommendationAnswerId, acw.CareerId })
                .IsUnique();

            // Index cho RecommendationUserAnswer
            builder.Entity<RecommendationUserAnswer>()
                .HasIndex(ua => new { ua.UserId, ua.QuizId, ua.RecommendationQuestionId });

            // Index cho LearningUserAnswer
            builder.Entity<LearningUserAnswer>()
                .HasIndex(ua => new { ua.UserId, ua.LearningQuestionId });

            // Configure decimal precision for Career salaries
            builder.Entity<Career>()
                .Property(c => c.MinSalary)
                .HasPrecision(18, 2);

            builder.Entity<Career>()
                .Property(c => c.MaxSalary)
                .HasPrecision(18, 2);

        }
    }
}
