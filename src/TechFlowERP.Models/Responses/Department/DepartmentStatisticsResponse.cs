namespace TechFlowERP.Models.Responses.Department
{
    /// <summary>
    /// 부서 통계 응답 DTO
    /// </summary>
    public class DepartmentStatisticsResponse : BaseResponse
    {
        public int TotalDepartments { get; set; }
        public int DepartmentsWithManager { get; set; }
        public int DepartmentsWithoutManager { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal AverageBudgetPerDepartment { get; set; }
        public decimal TotalEmployeeSalary { get; set; }
        public decimal AverageBudgetUtilization { get; set; }
        public int TotalEmployees { get; set; }
        public decimal AverageEmployeesPerDepartment { get; set; }
    }
}