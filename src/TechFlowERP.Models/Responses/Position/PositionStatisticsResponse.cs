namespace TechFlowERP.Models.Responses.Position
{
    /// <summary>
    /// 직급 통계 응답 DTO
    /// </summary>
    public class PositionStatisticsResponse : BaseResponse
    {
        public int TotalPositions { get; set; }
        public int ManagementPositions { get; set; }
        public int StaffPositions { get; set; }
        public decimal AverageMinSalary { get; set; }
        public decimal AverageMaxSalary { get; set; }
        public decimal HighestMaxSalary { get; set; }
        public decimal LowestMinSalary { get; set; }
        public int TotalEmployees { get; set; }
        public decimal AverageEmployeesPerPosition { get; set; }
        public List<PositionLevelStats> LevelStats { get; set; } = new();
    }

    /// <summary>
    /// 레벨별 직급 통계
    /// </summary>
    public class PositionLevelStats
    {
        public int Level { get; set; }
        public int PositionCount { get; set; }
        public int EmployeeCount { get; set; }
        public decimal AverageSalary { get; set; }
    }
}