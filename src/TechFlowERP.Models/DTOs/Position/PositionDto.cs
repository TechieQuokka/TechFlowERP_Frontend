using System;

namespace TechFlowERP.Models.DTOs.Position
{
    /// <summary>
    /// 직급 DTO (positions 테이블 기반)
    /// </summary>
    public class PositionDto : BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public int Level { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }

        // 추가 정보 (계산된 값들)
        public int EmployeeCount { get; set; }
        public decimal AverageSalary { get; set; }
        public bool IsManagementPosition { get; set; } // Level 기준으로 관리직 여부
    }
}