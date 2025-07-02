using System;

namespace TechFlowERP.Models.DTOs.Project
{
    /// <summary>
    /// 프로젝트 할당 DTO (project_assignments 테이블 기반)
    /// </summary>
    public class ProjectAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int AllocationPercentage { get; set; } = 100;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal HourlyRate { get; set; }

        // 추가 정보
        public int TotalHours { get; set; }
        public decimal TotalCost { get; set; }
    }
}