using TechFlowERP.Models.DTOs.Project;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Common; // 공통 PagedResult 사용

namespace TechFlowERP.Models.Responses.Project
{
    /// <summary>
    /// 프로젝트 목록 응답 DTO
    /// </summary>
    public class ProjectListResponse : BaseResponse<PagedResult<ProjectDto>>
    {
    }

    /// <summary>
    /// 프로젝트 통계 응답 DTO
    /// </summary>
    public class ProjectStatisticsResponse : BaseResponse<ProjectStatistics>
    {
    }

    /// <summary>
    /// 프로젝트 통계 모델
    /// </summary>
    public class ProjectStatistics
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public Dictionary<string, int> ByStatus { get; set; } = new();
        public Dictionary<string, int> ByType { get; set; } = new();
        public Dictionary<string, int> ByRiskLevel { get; set; } = new();
        public Dictionary<string, decimal> RevenueByClient { get; set; } = new();
        public decimal TotalBudget { get; set; }
        public decimal AverageBudget { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageMargin { get; set; }
        public int AverageTeamSize { get; set; }
        public double AverageDuration { get; set; } // 일 단위
    }

    /// <summary>
    /// 프로젝트 수익성 분석 응답 DTO
    /// </summary>
    public class ProjectProfitabilityResponse : BaseResponse<List<ProjectProfitability>>
    {
    }

    /// <summary>
    /// 프로젝트 수익성 모델
    /// </summary>
    public class ProjectProfitability
    {
        public Guid ProjectId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal ROI { get; set; }
        public int TotalHours { get; set; }
        public decimal CostPerHour { get; set; }
    }
}