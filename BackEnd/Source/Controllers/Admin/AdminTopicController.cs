using Microsoft.AspNetCore.Mvc;
using Source.DTOs.Admin;
using Source.Service.Admin.Interface;

namespace Source.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminTopicController : BaseController
    {
        private readonly IAdminTopicService _adminTopicService;

        public AdminTopicController(IAdminTopicService adminTopicService)
        {
            _adminTopicService = adminTopicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopics()
        {
            var topics = await _adminTopicService.GetAllTopics();
            return Success(topics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById(int id)
        {
            var topic = await _adminTopicService.GetTopicById(id);
            if (topic == null)
            {
                return NotFound(new { message = "Topic not found" });
            }
            return Success(topic);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto dto)
        {
            var topic = await _adminTopicService.CreateTopic(dto);
            return CreatedAtAction(nameof(GetTopicById), new { id = topic.Id }, Success(topic));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] UpdateTopicDto dto)
        {
            var topic = await _adminTopicService.UpdateTopic(id, dto);
            if (topic == null)
            {
                return NotFound(new { message = "Topic not found" });
            }
            return Success(topic);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var result = await _adminTopicService.DeleteTopic(id);
            if (!result)
            {
                return NotFound(new { message = "Topic not found" });
            }
            return Success(" xoa dc r ");
        }
    }
}