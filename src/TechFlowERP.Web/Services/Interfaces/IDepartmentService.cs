using TechFlowERP.Models.DTOs.Department;
using TechFlowERP.Models.Requests.Department;
using TechFlowERP.Models.Responses.Department;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 부서 관리 서비스 인터페이스
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// 모든 부서 목록 조회
        /// </summary>
        Task<List<DepartmentDto>> GetAllDepartmentsAsync();

        /// <summary>
        /// 부서 상세 조회
        /// </summary>
        Task<DepartmentDto?> GetDepartmentByIdAsync(Guid departmentId);

        /// <summary>
        /// 부서 생성
        /// </summary>
        Task<DepartmentDto?> CreateDepartmentAsync(CreateDepartmentRequest request);

        /// <summary>
        /// 부서 수정
        /// </summary>
        Task<DepartmentDto?> UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentRequest request);

        /// <summary>
        /// 부서 삭제
        /// </summary>
        Task<bool> DeleteDepartmentAsync(Guid departmentId);

        /// <summary>
        /// 부서명으로 검색
        /// </summary>
        Task<List<DepartmentDto>> SearchDepartmentsAsync(string searchTerm);

        /// <summary>
        /// 활성 부서 목록 조회 (직원 배정용)
        /// </summary>
        Task<List<DepartmentDto>> GetActiveDepartmentsAsync();

        /// <summary>
        /// 매니저가 없는 부서 목록 조회
        /// </summary>
        Task<List<DepartmentDto>> GetDepartmentsWithoutManagerAsync();

        /// <summary>
        /// 부서 통계 조회
        /// </summary>
        Task<DepartmentStatisticsResponse> GetDepartmentStatisticsAsync();

        /// <summary>
        /// 부서별 예산 사용률 조회
        /// </summary>
        Task<Dictionary<Guid, decimal>> GetDepartmentBudgetUtilizationAsync();
    }
}