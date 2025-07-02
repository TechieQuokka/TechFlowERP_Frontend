using System;
using System.Collections.Generic;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.Project
{
    /// <summary>
    /// 프로젝트 DTO (projects 테이블 기반)
    /// </summary>
    public class ProjectDto : BaseDto
    {
        public string ProjectCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public ProjectType ProjectType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal ProfitMargin { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public ProjectStatus Status { get; set; }
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public List<string> Technologies { get; set; } = new();

        // 프로젝트 할당 정보
        public List<ProjectAssignmentDto> Assignments { get; set; } = new();

        // 마일스톤 정보
        public List<ProjectMilestoneDto> Milestones { get; set; } = new();

        // 추가 계산 정보
        public decimal ActualCost { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal CompletionPercentage { get; set; }
        public int TotalHours { get; set; }
        public int RemainingDays { get; set; }
    }
}