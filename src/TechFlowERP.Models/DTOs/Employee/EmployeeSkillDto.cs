using System;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.Employee
{
    /// <summary>
    /// 직원 기술 DTO (employee_skills 테이블 기반)
    /// </summary>
    public class EmployeeSkillDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string Technology { get; set; } = string.Empty;
        public SkillLevel SkillLevel { get; set; }
        public int YearsExperience { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public string Certification { get; set; } = string.Empty;
    }
}