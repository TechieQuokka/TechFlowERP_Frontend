using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.Department;
using TechFlowERP.Models.DTOs.Employee;
using TechFlowERP.Models.Requests.Department;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 부서 관리 서비스 인터페이스 (관리자용)
    /// </summary>
    public interface IDepartmentManagementService
    {
        // CRUD 기본 작업
        /// <summary>
        /// 부서 목록 조회 (페이징, 검색 지원)
        /// </summary>
        Task<PagedResult<DepartmentListDto>> GetDepartmentsAsync(SearchDepartmentsRequest request);

        /// <summary>
        /// 부서 상세 정보 조회
        /// </summary>
        Task<DepartmentDto?> GetDepartmentByIdAsync(Guid departmentId);

        /// <summary>
        /// 새 부서 생성
        /// </summary>
        Task<DepartmentDto?> CreateDepartmentAsync(CreateDepartmentRequest request);

        /// <summary>
        /// 부서 정보 수정
        /// </summary>
        Task<DepartmentDto?> UpdateDepartmentAsync(UpdateDepartmentRequest request);

        /// <summary>
        /// 부서 삭제 (소프트 삭제)
        /// </summary>
        Task<bool> DeleteDepartmentAsync(Guid departmentId);

        // 부서장 관리
        /// <summary>
        /// 부서장 할당
        /// </summary>
        Task<bool> AssignManagerAsync(AssignManagerRequest request);

        /// <summary>
        /// 부서장 해제
        /// </summary>
        Task<bool> RemoveManagerAsync(Guid departmentId, string? reason = null);

        /// <summary>
        /// 부서장 변경
        /// </summary>
        Task<bool> ChangeManagerAsync(Guid departmentId, Guid newManagerId, string? reason = null);

        // 예산 관리
        /// <summary>
        /// 부서 예산 수정
        /// </summary>
        Task<bool> UpdateBudgetAsync(UpdateBudgetRequest request);

        /// <summary>
        /// 부서별 예산 사용률 조회
        /// </summary>
        Task<Dictionary<Guid, decimal>> GetBudgetUtilizationAsync();

        /// <summary>
        /// 예산 초과 부서 목록
        /// </summary>
        Task<List<DepartmentListDto>> GetOverBudgetDepartmentsAsync();

        // 직원 관리
        /// <summary>
        /// 부서별 직원 목록 조회
        /// </summary>
        Task<List<EmployeeDto>> GetDepartmentEmployeesAsync(Guid departmentId);

        /// <summary>
        /// 직원 부서 이동
        /// </summary>
        Task<bool> TransferEmployeeAsync(TransferEmployeeRequest request);

        /// <summary>
        /// 부서별 직원 수 조회
        /// </summary>
        Task<Dictionary<Guid, int>> GetEmployeeCountByDepartmentAsync();

        // 통계 및 분석
        /// <summary>
        /// 부서 통계 조회
        /// </summary>
        Task<DepartmentStatisticsDto> GetDepartmentStatisticsAsync();

        /// <summary>
        /// 부서별 성과 분석
        /// </summary>
        Task<Dictionary<Guid, DepartmentPerformanceDto>> GetDepartmentPerformanceAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 부서 효율성 분석
        /// </summary>
        Task<List<DepartmentEfficiencyDto>> GetDepartmentEfficiencyAsync();

        // 특수 조회 기능
        /// <summary>
        /// 부서명 중복 체크
        /// </summary>
        Task<bool> IsDepartmentNameAvailableAsync(string name, Guid? excludeDepartmentId = null);

        /// <summary>
        /// 부서장이 없는 부서 목록
        /// </summary>
        Task<List<DepartmentListDto>> GetDepartmentsWithoutManagerAsync();

        /// <summary>
        /// 부서장 후보 직원 목록 (해당 부서 소속)
        /// </summary>
        Task<List<EmployeeDto>> GetManagerCandidatesAsync(Guid departmentId);

        /// <summary>
        /// 활성 부서 목록 (드롭다운용)
        /// </summary>
        Task<List<DepartmentDto>> GetActiveDepartmentsAsync();

        // 고급 기능
        /// <summary>
        /// 부서 병합
        /// </summary>
        Task<bool> MergeDepartmentsAsync(MergeDepartmentsRequest request);

        /// <summary>
        /// 부서 분할 (하위 부서 생성)
        /// </summary>
        Task<DepartmentDto?> SplitDepartmentAsync(Guid parentDepartmentId, CreateDepartmentRequest newDepartmentRequest);

        /// <summary>
        /// 부서별 월별 예산 사용 추이
        /// </summary>
        Task<Dictionary<Guid, List<MonthlyBudgetUsageDto>>> GetMonthlyBudgetUsageAsync(int year);

        /// <summary>
        /// 부서 재구조화 시뮬레이션
        /// </summary>
        Task<DepartmentRestructureAnalysisDto> SimulateRestructureAsync(List<RestructureScenarioDto> scenarios);
    }

    /// <summary>
    /// 부서 성과 DTO
    /// </summary>
    public class DepartmentPerformanceDto
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int CompletedProjects { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageProjectDuration { get; set; }
        public decimal EmployeeUtilization { get; set; }
        public decimal CustomerSatisfaction { get; set; }
        public int OnTimeDeliveryRate { get; set; }
        public decimal ProfitMargin { get; set; }
    }

    /// <summary>
    /// 부서 효율성 DTO
    /// </summary>
    public class DepartmentEfficiencyDto
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public decimal RevenuePerEmployee { get; set; }
        public decimal CostPerEmployee { get; set; }
        public decimal EfficiencyRatio { get; set; }
        public decimal BudgetEfficiency { get; set; }
        public decimal ProductivityScore { get; set; }
        public string EfficiencyGrade { get; set; } = string.Empty;
    }

    /// <summary>
    /// 월별 예산 사용 DTO
    /// </summary>
    public class MonthlyBudgetUsageDto
    {
        public int Month { get; set; }
        public decimal BudgetAllocated { get; set; }
        public decimal ActualUsage { get; set; }
        public decimal VariancePercentage { get; set; }
        public string MonthName => new DateTime(DateTime.Now.Year, Month, 1).ToString("MMMM");
    }

    /// <summary>
    /// 재구조화 시나리오 DTO
    /// </summary>
    public class RestructureScenarioDto
    {
        public string ScenarioName { get; set; } = string.Empty;
        public List<DepartmentMergeDto> Merges { get; set; } = new();
        public List<DepartmentSplitDto> Splits { get; set; } = new();
        public List<EmployeeTransferDto> Transfers { get; set; } = new();
    }

    /// <summary>
    /// 부서 병합 DTO
    /// </summary>
    public class DepartmentMergeDto
    {
        public Guid SourceDepartmentId { get; set; }
        public Guid TargetDepartmentId { get; set; }
        public string NewDepartmentName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 부서 분할 DTO
    /// </summary>
    public class DepartmentSplitDto
    {
        public Guid ParentDepartmentId { get; set; }
        public string NewDepartmentName { get; set; } = string.Empty;
        public List<Guid> EmployeeIds { get; set; } = new();
        public decimal BudgetAllocation { get; set; }
    }

    /// <summary>
    /// 직원 이동 DTO
    /// </summary>
    public class EmployeeTransferDto
    {
        public Guid EmployeeId { get; set; }
        public Guid FromDepartmentId { get; set; }
        public Guid ToDepartmentId { get; set; }
    }

    /// <summary>
    /// 재구조화 분석 결과 DTO
    /// </summary>
    public class DepartmentRestructureAnalysisDto
    {
        public string ScenarioName { get; set; } = string.Empty;
        public decimal ProjectedCostSavings { get; set; }
        public decimal ProjectedEfficiencyGain { get; set; }
        public int AffectedEmployees { get; set; }
        public List<string> Risks { get; set; } = new();
        public List<string> Benefits { get; set; } = new();
        public decimal ImplementationCost { get; set; }
        public int EstimatedImplementationDays { get; set; }
        public decimal ROI { get; set; }
    }
}