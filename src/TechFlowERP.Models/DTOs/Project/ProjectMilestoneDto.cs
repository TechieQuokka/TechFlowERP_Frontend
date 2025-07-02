using System;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.Project
{
    /// <summary>
    /// 프로젝트 마일스톤 DTO (project_milestones 테이블 기반)
    /// </summary>
    public class ProjectMilestoneDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Deliverables { get; set; } = string.Empty;
        public decimal PaymentPercentage { get; set; }
        public MilestoneStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}