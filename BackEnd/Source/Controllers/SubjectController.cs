using Microsoft.AspNetCore.Mvc;
using Source.Service.Interface;
using System.Security.AccessControl;

namespace Source.Controllers
{
    public class SubjectController: BaseController
    {
        private readonly ISubjectService _subjectService;
        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet("{LearningPathId}/subject/{subjectId}")] //  /GET /api/learningpaths/1/subjects/5
        public async Task<IActionResult> GetDetaiSubject_ListTopics(int subjectId )
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy User ID trong token");
            }
            var result = await _subjectService.GetSubjectDetail( subjectId, userId);
            return Success(result, " Get Detail Subject + List Topics Success");
        }

    }
}
