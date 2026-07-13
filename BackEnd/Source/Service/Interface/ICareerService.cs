using Source.DTOs;

namespace Source.Service.Interface
{
    public interface ICareerService
    {
        Task<List<ListCareerDto>> GetListCareer(string UserId);
        Task<CareerDetailDto> GetDetailCareer(string userid,int id);
    }
}
