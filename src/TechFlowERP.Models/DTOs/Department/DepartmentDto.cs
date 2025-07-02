using TechFlowERP.Models.DTOs.Employee;

namespace TechFlowERP.Models.DTOs.Department
{
    /// <summary>
    /// 부서 DTO (departments 테이블 기반)
    /// </summary>
    public class DepartmentDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid? ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public decimal Budget { get; set; }

        // 계산된 속성들
        public int EmployeeCount { get; set; }
        public decimal UsedBudget { get; set; }
        public int ActiveProjectsCount { get; set; } // 활성 프로젝트 수
        public decimal TotalSalary { get; set; } // 총 급여 (UsedBudget의 일부)

        // 읽기 전용 계산된 속성들
        public decimal RemainingBudget => Budget - UsedBudget;
        public decimal BudgetUtilization => Budget > 0 ? (UsedBudget / Budget * 100) : 0;

        // 관련 데이터
        public List<EmployeeDto> Employees { get; set; } = new();

        // UI 표시용 속성
        public string BudgetDisplayText => Budget.ToString("C");
        public string RemainingBudgetDisplayText => RemainingBudget.ToString("C");
        public string BudgetUtilizationDisplayText => $"{BudgetUtilization:F1}%";
        public string BudgetStatusBadgeClass => BudgetUtilization switch
        {
            >= 90 => "badge bg-danger",
            >= 75 => "badge bg-warning",
            >= 50 => "badge bg-info",
            _ => "badge bg-success"
        };
    }

    /// <summary>
    /// 부서 목록용 간단한 DTO
    /// </summary>
    public class DepartmentListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal Budget { get; set; }
        public decimal BudgetUtilization { get; set; }
        public DateTime CreatedAt { get; set; }

        // UI 표시용
        public string BudgetDisplayText => Budget.ToString("C");
        public string BudgetUtilizationDisplayText => $"{BudgetUtilization:F1}%";
        public string StatusBadgeClass => BudgetUtilization switch
        {
            >= 90 => "badge bg-danger",
            >= 75 => "badge bg-warning text-dark",
            >= 50 => "badge bg-info",
            _ => "badge bg-success"
        };
        public string StatusText => BudgetUtilization switch
        {
            >= 90 => "예산 초과 위험",
            >= 75 => "예산 주의",
            >= 50 => "예산 보통",
            _ => "예산 양호"
        };
    }

    /// <summary>
    /// 부서 통계 DTO
    /// </summary>
    public class DepartmentStatisticsDto
    {
        public int TotalDepartments { get; set; }
        public int DepartmentsWithManager { get; set; }
        public int DepartmentsWithoutManager { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalUsedBudget { get; set; }
        public decimal AverageEmployeesPerDepartment { get; set; }
        public int LargestDepartmentSize { get; set; }
        public string LargestDepartmentName { get; set; } = string.Empty;

        // 계산된 속성
        public decimal OverallBudgetUtilization => TotalBudget > 0 ? (TotalUsedBudget / TotalBudget * 100) : 0;
        public decimal RemainingBudget => TotalBudget - TotalUsedBudget;
        public int ManagerAssignmentRate => TotalDepartments > 0 ? (DepartmentsWithManager * 100 / TotalDepartments) : 0;
    }
}