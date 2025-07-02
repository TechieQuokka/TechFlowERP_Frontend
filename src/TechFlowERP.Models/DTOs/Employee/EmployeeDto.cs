using System;
using System.Collections.Generic;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.Employee
{
    /// <summary>
    /// 직원 DTO (employees 테이블 기반)
    /// </summary>
    public class EmployeeDto : BaseDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public Guid? PositionId { get; set; }
        public string PositionTitle { get; set; } = string.Empty;
        public Guid? ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public int LeaveBalance { get; set; }
        public bool IsActive { get; set; } = true;

        // 기술 스택 정보
        public List<EmployeeSkillDto> Skills { get; set; } = new();

        // 추가 정보
        public int CurrentProjectsCount { get; set; }
        public decimal UtilizationRate { get; set; }
        public decimal TotalHoursThisMonth { get; set; }
    }
}