using TechFlowERP.Models.DTOs.TimeEntry;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Common; // Client 대신 Common 사용

namespace TechFlowERP.Models.Responses.TimeEntry
{
    /// <summary>
    /// 시간 기록 목록 응답 DTO
    /// </summary>
    public class TimeEntryListResponse : BaseResponse<PagedResult<TimeEntryDto>>
    {
    }

    /// <summary>
    /// 시간 기록 통계 응답 DTO
    /// </summary>
    public class TimeEntryStatisticsResponse : BaseResponse<TimeEntryStatistics>
    {
    }

    /// <summary>
    /// 시간 기록 통계 모델
    /// </summary>
    public class TimeEntryStatistics
    {
        public decimal TotalHours { get; set; }
        public decimal BillableHours { get; set; }
        public decimal NonBillableHours { get; set; }
        public decimal ApprovedHours { get; set; }
        public decimal PendingHours { get; set; }
        public int TotalEntries { get; set; }
        public int ApprovedEntries { get; set; }
        public int PendingEntries { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageHoursPerDay { get; set; }
        public Dictionary<string, decimal> HoursByProject { get; set; } = new();
        public Dictionary<string, decimal> HoursByEmployee { get; set; } = new();
        public Dictionary<DateTime, decimal> DailyHours { get; set; } = new();
        public Dictionary<int, decimal> WeeklyHours { get; set; } = new();
    }

    /// <summary>
    /// 타임시트 응답 DTO
    /// </summary>
    public class TimesheetResponse : BaseResponse<TimesheetData>
    {
    }

    /// <summary>
    /// 타임시트 데이터 모델
    /// </summary>
    public class TimesheetData
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TimeEntryDto> TimeEntries { get; set; } = new();
        public Dictionary<DateTime, decimal> DailyTotals { get; set; } = new();
        public decimal WeeklyTotal { get; set; }
        public decimal BillableTotal { get; set; }
        public decimal NonBillableTotal { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}