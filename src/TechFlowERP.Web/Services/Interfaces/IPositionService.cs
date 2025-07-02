using TechFlowERP.Models.DTOs.Position;
using TechFlowERP.Models.Requests.Position;
using TechFlowERP.Models.Responses.Position;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 직급 관리 서비스 인터페이스
    /// </summary>
    public interface IPositionService
    {
        /// <summary>
        /// 모든 직급 목록 조회
        /// </summary>
        Task<List<PositionDto>> GetAllPositionsAsync();

        /// <summary>
        /// 직급 상세 조회
        /// </summary>
        Task<PositionDto?> GetPositionByIdAsync(Guid positionId);

        /// <summary>
        /// 직급 생성
        /// </summary>
        Task<PositionDto?> CreatePositionAsync(CreatePositionRequest request);

        /// <summary>
        /// 직급 수정
        /// </summary>
        Task<PositionDto?> UpdatePositionAsync(Guid positionId, UpdatePositionRequest request);

        /// <summary>
        /// 직급 삭제
        /// </summary>
        Task<bool> DeletePositionAsync(Guid positionId);

        /// <summary>
        /// 직급명으로 검색
        /// </summary>
        Task<List<PositionDto>> SearchPositionsAsync(string searchTerm);

        /// <summary>
        /// 활성 직급 목록 조회 (직원 배정용)
        /// </summary>
        Task<List<PositionDto>> GetActivePositionsAsync();

        /// <summary>
        /// 레벨별 직급 목록 조회
        /// </summary>
        Task<List<PositionDto>> GetPositionsByLevelAsync(int level);

        /// <summary>
        /// 관리직 직급 목록 조회 (레벨 기준)
        /// </summary>
        Task<List<PositionDto>> GetManagementPositionsAsync();

        /// <summary>
        /// 급여 범위로 직급 검색
        /// </summary>
        Task<List<PositionDto>> GetPositionsBySalaryRangeAsync(decimal minSalary, decimal maxSalary);

        /// <summary>
        /// 직급 통계 조회
        /// </summary>
        Task<PositionStatisticsResponse> GetPositionStatisticsAsync();

        /// <summary>
        /// 직급별 평균 급여 조회
        /// </summary>
        Task<Dictionary<Guid, decimal>> GetPositionAverageSalaryAsync();
    }
}