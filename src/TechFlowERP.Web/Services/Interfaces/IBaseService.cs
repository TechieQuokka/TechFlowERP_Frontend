using TechFlowERP.Models.Common;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 제네릭 기본 서비스 인터페이스
    /// </summary>
    public interface IBaseService<TDto, TRequest>
        where TDto : class
        where TRequest : class
    {
        Task<TDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<PagedResult<TDto>> GetPagedAsync(int page = 1, int pageSize = 20, string? searchTerm = null);
        Task<TDto?> CreateAsync(TRequest request);
        Task<TDto?> UpdateAsync(Guid id, TRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}