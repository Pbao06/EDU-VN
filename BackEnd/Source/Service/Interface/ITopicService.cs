using Source.DTOs;

namespace Source.Service.Interface
{
    public interface ITopicService
    {
        /// <summary>
        /// Lấy chi tiết topic với LearningQuestions
        /// </summary>
        Task<TopicDetailDto> GetTopicDetail(int topicId, string userId);

        /// <summary>
        /// Submit answers cho topic và cập nhật progress
        /// </summary>
        Task<SubmitTopicAnswersResponseDto> SubmitTopicAnswers(string userId, SubmitTopicAnswersDto request);
    }
}
