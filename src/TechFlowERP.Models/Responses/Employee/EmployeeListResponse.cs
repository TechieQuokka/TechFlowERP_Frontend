using TechFlowERP.Models.DTOs.Employee;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Common; // Common 추가

namespace TechFlowERP.Models.Responses.Employee
{
    /// <summary>
    /// 직원 목록 응답 DTO
    /// </summary>
    public class EmployeeListResponse : BaseResponse<PagedResult<EmployeeDto>>
    {
    }

    /// <summary>
    /// 직원 통계 응답 DTO
    /// </summary>
    public class EmployeeStatisticsResponse : BaseResponse<EmployeeStatistics>
    {
    }

    /// <summary>
    /// 직원 통계 모델
    /// </summary>
    public class EmployeeStatistics
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public Dictionary<string, int> ByDepartment { get; set; } = new();
        public Dictionary<string, int> ByPosition { get; set; } = new();
        public Dictionary<string, int> BySkillLevel { get; set; } = new();
        public decimal AverageSalary { get; set; }
        public decimal AverageUtilization { get; set; }
    }
}