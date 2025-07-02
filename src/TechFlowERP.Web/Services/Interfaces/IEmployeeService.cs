using TechFlowERP.Models.DTOs.Employee;
using TechFlowERP.Models.Enums;
using TechFlowERP.Models.Requests.Employee;
using TechFlowERP.Models.Responses.Employee;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 직원 관리 서비스 인터페이스
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// 직원 목록 조회 (페이징)
        /// </summary>
        Task<EmployeeListResponse> GetEmployeesAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null, Guid? departmentId = null);

        /// <summary>
        /// 직원 상세 조회
        /// </summary>
        Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId);

        /// <summary>
        /// 직원 생성
        /// </summary>
        Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeRequest request);

        /// <summary>
        /// 직원 수정
        /// </summary>
        Task<EmployeeDto?> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request);

        /// <summary>
        /// 직원 삭제 (비활성화)
        /// </summary>
        Task<bool> DeleteEmployeeAsync(Guid employeeId);

        /// <summary>
        /// 직원 검색
        /// </summary>
        Task<List<EmployeeDto>> SearchEmployeesAsync(string searchTerm);

        /// <summary>
        /// 활성 직원 목록 조회 (프로젝트 할당용)
        /// </summary>
        Task<List<EmployeeDto>> GetActiveEmployeesAsync();

        /// <summary>
        /// 직원 기술 목록 조회
        /// </summary>
        Task<List<EmployeeSkillDto>> GetEmployeeSkillsAsync(Guid employeeId);

        /// <summary>
        /// 직원 기술 추가
        /// </summary>
        Task<EmployeeSkillDto?> AddEmployeeSkillAsync(AddEmployeeSkillRequest request);

        /// <summary>
        /// 직원 기술 삭제
        /// </summary>
        Task<bool> RemoveEmployeeSkillAsync(Guid skillId);

        /// <summary>
        /// 부서별 직원 목록 조회
        /// </summary>
        Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(Guid departmentId);

        /// <summary>
        /// 매니저별 팀원 목록 조회
        /// </summary>
        Task<List<EmployeeDto>> GetTeamMembersAsync(Guid managerId);

        /// <summary>
        /// 기술별 직원 목록 조회
        /// </summary>
        Task<List<EmployeeDto>> GetEmployeesBySkillAsync(string technology, SkillLevel? minLevel = null);

        /// <summary>
        /// 직원 통계 조회
        /// </summary>
        Task<EmployeeStatisticsResponse> GetEmployeeStatisticsAsync();

        /// <summary>
        /// 직원 활용도 조회
        /// </summary>
        Task<Dictionary<Guid, decimal>> GetEmployeeUtilizationAsync(DateTime startDate, DateTime endDate);
    }
}