using Source.Data;
using Source.DTOs;
using Source.Service.Interface;
using Source.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Source.Service
{
    public class LearningPathService : ILearningPathService
    {
        private readonly ApplicationDbContext _context;

        public LearningPathService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== LEARNING PATH LEVEL ====================

        /// <summary>
        /// Tạo learning path mới cho user với career đã chọn
        /// </summary>
        public async Task<CreateLearningPathResponseDto> StartLearningPath(string userId, int careerId, string? title = null)
        {
            // Kiểm tra user tồn tại
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new NotFoundException("Không tìm thấy User");

            // Kiểm tra career tồn tại
            var career = await _context.Careers.FindAsync(careerId);
            if (career == null) throw new NotFoundException("Không tìm thấy Career");

            // Kiểm tra user đã có learning path cho career này chưa
            var existingPath = await _context.UserLearningPaths
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.CareerId == careerId);

            if (existingPath != null)
            {
                // Nếu đã có, trả về learning path hiện tại
                var dto = await MapToLearningPathDto(existingPath);
                return new CreateLearningPathResponseDto
                {
                    LearningPathId = existingPath.Id,
                    Message = "Bạn đã có learning path cho career này",
                    LearningPath = dto
                };
            }

            // Tạo learning path mới
            var learningPath = new Models.LearningPath
            {
                UserId = userId,
                CareerId = careerId,
                Title = title ?? $"Learning Path - {career.Name}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserLearningPaths.Add(learningPath);
            await _context.SaveChangesAsync();

            var newDto = await MapToLearningPathDto(learningPath);
            return new CreateLearningPathResponseDto
            {
                LearningPathId = learningPath.Id,
                Message = "Tạo learning path thành công",
                LearningPath = newDto
            };
        }

        /// <summary>
        /// Lấy tất cả learning paths của user (summary only)
        /// </summary>
        public async Task<List<LearningPathDto>> GetUserLearningPaths(string userId) //user get list 
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new NotFoundException("Không tìm thấy User");

            var learningPaths = await _context.UserLearningPaths
                .Where(lp => lp.UserId == userId)
                .Include(lp => lp.Career) 
                .ToListAsync();

            var dtos = new List<LearningPathDto>();
            foreach (var lp in learningPaths)
            {
                dtos.Add(await MapToLearningPathDto(lp));
            }

            return dtos;
        }

        /// <summary>
        /// Lấy chi tiết learning path với Subjects list (summary only - không có Topics)
        /// Subject detail sẽ được lấy bởi SubjectService
        /// </summary>
        public async Task<LearningPathDetailDto> GetLearningPathDetail(int learningPathId, string userId)
        {
            // flow get learning detais 
            // learning -> career -> subject 
            var learningPath = await _context.UserLearningPaths
                .Include(lp => lp.Career)
                .FirstOrDefaultAsync(lp => lp.Id == learningPathId && lp.UserId == userId);

            if (learningPath == null) throw new NotFoundException("Không tìm thấy Learning Path");

            // Lấy các subjects của career này (qua CareerSubject) - KHÔNG include Topics
            var careerSubjects = await _context.CareerSubjects
                .Include(cs => cs.Subject)
                .Where(cs => cs.CareerId == learningPath.CareerId)
                .OrderBy(cs => cs.Priority)
                .ToListAsync();

            var subjectSummaryDtos = new List<SubjectSummaryDto>();
            int completedSubjects = 0;

            foreach (var cs in careerSubjects)
            {
                var subjectDto = await MapToSubjectSummaryDto(cs.Subject, userId);
                subjectSummaryDtos.Add(subjectDto);
                if (subjectDto.IsCompleted)
                {
                    completedSubjects++;
                }
            }

            int totalSubjects = careerSubjects.Count;
            double overallProgress = totalSubjects > 0 ? (double)completedSubjects / totalSubjects * 100 : 0;

            return new LearningPathDetailDto
            {
                Id = learningPath.Id,
                UserId = learningPath.UserId,
                CareerId = learningPath.CareerId,
                CareerName = learningPath.Career.Name,
                CareerIconUrl = learningPath.Career.IconUrl ?? string.Empty,
                Title = learningPath.Title,
                IsActive = learningPath.IsActive,
                CreatedAt = learningPath.CreatedAt,
                CompletedAt = learningPath.CompletedAt,
                TotalSubjects = totalSubjects,
                CompletedSubjects = completedSubjects,
                OverallProgress = overallProgress,
                Subjects = subjectSummaryDtos
            };
        }

        #region Helper Methods

        private async Task<LearningPathDto> MapToLearningPathDto(Models.LearningPath learningPath)
        {
            // Tính progress summary (Subjects level only)
            var careerSubjects = await _context.CareerSubjects
                .Include(cs => cs.Subject)
                    .ThenInclude(s => s.Topics)
                .Where(cs => cs.CareerId == learningPath.CareerId)
                .ToListAsync();

            int completedSubjects = 0;

            foreach (var cs in careerSubjects)
            {
                var subjectDto = await MapToSubjectSummaryDto(cs.Subject, learningPath.UserId);
                if (subjectDto.IsCompleted)
                {
                    completedSubjects++;
                }
            }

            int totalSubjects = careerSubjects.Count;
            double overallProgress = totalSubjects > 0 ? (double)completedSubjects / totalSubjects * 100 : 0;

            return new LearningPathDto
            {
                Id = learningPath.Id,
                UserId = learningPath.UserId,
                CareerId = learningPath.CareerId,
                CareerName = learningPath.Career?.Name ?? string.Empty,
                CareerIconUrl = learningPath.Career?.IconUrl ?? string.Empty,
                Title = learningPath.Title,
                IsActive = learningPath.IsActive,
                CreatedAt = learningPath.CreatedAt,
                CompletedAt = learningPath.CompletedAt,
                TotalSubjects = totalSubjects,
                CompletedSubjects = completedSubjects,
                OverallProgress = overallProgress
            };
        }

        private async Task<SubjectSummaryDto> MapToSubjectSummaryDto(Models.Subject subject, string userId)
        {
            var careerSubject = await _context.CareerSubjects
                .Include(cs => cs.Career)
                .FirstOrDefaultAsync(cs => cs.SubjectId == subject.Id);

            int totalTopics = await _context.Topics
                .Where(t => t.SubjectId == subject.Id)
                .CountAsync();

            int completedTopics = 0;

            foreach (var topic in await _context.Topics.Where(t => t.SubjectId == subject.Id).ToListAsync())
            {
                var progress = await _context.UserProgresses
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.TopicId == topic.Id);
                if (progress != null && progress.CompletionPercentage >= 100)
                {
                    completedTopics++;
                }
            }

            double subjectProgress = totalTopics > 0 ? (double)completedTopics / totalTopics * 100 : 0;
            bool isCompleted = completedTopics == totalTopics && totalTopics > 0;
            bool isInProgress = completedTopics > 0 && !isCompleted;

            return new SubjectSummaryDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                Type = subject.Type,
                Priority = careerSubject?.Priority ?? 0,
                Reason = careerSubject?.Reason ?? string.Empty,
                TotalTopics = totalTopics,
                CompletedTopics = completedTopics,
                SubjectProgress = subjectProgress,
                IsCompleted = isCompleted,
                IsInProgress = isInProgress
            };
        }

        #endregion
    }
}