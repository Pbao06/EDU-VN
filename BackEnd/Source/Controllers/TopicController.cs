using Microsoft.AspNetCore.Mvc;
using Source.Service.Interface;

namespace Source.Controllers
{
    public class TopicController : BaseController
    {
        private readonly ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }
        
        [HttpGet("{topicid}")]
        public async Task<IActionResult> GetDetailTopic_ListQuestions(int topicid)
        {
            string userid= User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userid == null) return Unauthorized(" KHong the xac thuc user");
            var result = await _topicService.GetTopicDetail(topicid,userid);
            return Success(result, " Return lai Detail topic + List Question + answer thanh cong");
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitTopicAnswers([FromBody] DTOs.SubmitTopicAnswersDto request)
        {
            string userid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userid == null) return Unauthorized(" KHong the xac thuc user");
            var result = await _topicService.SubmitTopicAnswers(userid, request);
            return Success(result, " Submit answers thanh cong");
        }
    }
}
