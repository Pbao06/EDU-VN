using Source.DTOs.Admin;

namespace Source.Service.Admin.Interface
{
    public interface IAdminTopicService
    {
        Task<List<AdminTopicDto>> GetAllTopics();
        Task<AdminTopicDto?> GetTopicById(int id);
        Task<AdminTopicDto> CreateTopic(CreateTopicDto dto);
        Task<AdminTopicDto?> UpdateTopic(int id, UpdateTopicDto dto);
        Task<bool> DeleteTopic(int id);
    }
}