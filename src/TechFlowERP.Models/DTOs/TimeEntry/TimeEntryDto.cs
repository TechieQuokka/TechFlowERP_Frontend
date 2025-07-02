using System;

namespace TechFlowERP.Models.DTOs.TimeEntry
{
    /// <summary>
    /// 시간 기록 DTO (time_entries 테이블 기반)
    /// </summary>
    public class TimeEntryDto : BaseDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public decimal Hours { get; set; }
        public DateTime Date { get; set; }
        public bool Billable { get; set; } = true;
        public bool Approved { get; set; } = false;
        public Guid? ApprovedBy { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }

        // 추가 계산 필드
        public decimal HourlyRate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status => Approved ? "승인됨" : "대기중";
        public int WeekNumber { get; set; }
        public string WeekRange { get; set; } = string.Empty;
    }
}