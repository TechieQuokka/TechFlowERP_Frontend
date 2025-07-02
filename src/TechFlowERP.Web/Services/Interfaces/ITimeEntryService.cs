using TechFlowERP.Models.DTOs.TimeEntry;
using TechFlowERP.Models.Requests.TimeEntry;
using TechFlowERP.Models.Responses.TimeEntry;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 시간 기록 관리 서비스 인터페이스
    /// </summary>
    public interface ITimeEntryService
    {
        /// <summary>
        /// 시간 기록 목록 조회 (페이징)
        /// </summary>
        Task<TimeEntryListResponse> GetTimeEntriesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            Guid? employeeId = null,
            Guid? projectId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? billable = null,
            bool? approved = null);

        /// <summary>
        /// 시간 기록 상세 조회
        /// </summary>
        Task<TimeEntryDto?> GetTimeEntryByIdAsync(Guid timeEntryId);

        /// <summary>
        /// 시간 기록 생성
        /// </summary>
        Task<TimeEntryDto?> CreateTimeEntryAsync(CreateTimeEntryRequest request);

        /// <summary>
        /// 시간 기록 수정
        /// </summary>
        Task<TimeEntryDto?> UpdateTimeEntryAsync(Guid timeEntryId, UpdateTimeEntryRequest request);

        /// <summary>
        /// 시간 기록 삭제
        /// </summary>
        Task<bool> DeleteTimeEntryAsync(Guid timeEntryId);

        /// <summary>
        /// 시간 기록 일괄 승인
        /// </summary>
        Task<bool> ApproveTimeEntriesAsync(ApproveTimeEntryRequest request);

        /// <summary>
        /// 시간 기록 일괄 거부
        /// </summary>
        Task<bool> RejectTimeEntriesAsync(RejectTimeEntryRequest request);

        /// <summary>
        /// 직원별 시간 기록 조회
        /// </summary>
        Task<List<TimeEntryDto>> GetTimeEntriesByEmployeeAsync(
            Guid employeeId,
            DateTime? startDate = null,
            DateTime? endDate = null);

        /// <summary>
        /// 프로젝트별 시간 기록 조회
        /// </summary>
        Task<List<TimeEntryDto>> GetTimeEntriesByProjectAsync(
            Guid projectId,
            DateTime? startDate = null,
            DateTime? endDate = null);

        /// <summary>
        /// 승인 대기 중인 시간 기록 조회
        /// </summary>
        Task<List<TimeEntryDto>> GetPendingTimeEntriesAsync(Guid? managerId = null);

        /// <summary>
        /// 오늘 시간 기록 조회
        /// </summary>
        Task<List<TimeEntryDto>> GetTodayTimeEntriesAsync(Guid employeeId);

        /// <summary>
        /// 주간 타임시트 조회
        /// </summary>
        Task<TimesheetResponse> GetWeeklyTimesheetAsync(Guid employeeId, DateTime weekStartDate);

        /// <summary>
        /// 월간 타임시트 조회
        /// </summary>
        Task<TimesheetResponse> GetMonthlyTimesheetAsync(Guid employeeId, int year, int month);

        /// <summary>
        /// 시간 기록 통계 조회
        /// </summary>
        Task<TimeEntryStatisticsResponse> GetTimeEntryStatisticsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            Guid? employeeId = null,
            Guid? projectId = null);

        /// <summary>
        /// 직원 활용도 조회
        /// </summary>
        Task<Dictionary<Guid, decimal>> GetEmployeeUtilizationAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 프로젝트 시간 사용 현황 조회
        /// </summary>
        Task<Dictionary<Guid, decimal>> GetProjectTimeUsageAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 시간 기록 복사 (반복 작업용)
        /// </summary>
        Task<List<TimeEntryDto>> CopyTimeEntriesAsync(
            Guid sourceEmployeeId,
            DateTime sourceDate,
            DateTime targetDate,
            List<Guid>? projectIds = null);

        /// <summary>
        /// 시간 기록 템플릿 생성
        /// </summary>
        Task<bool> CreateTimeEntryTemplateAsync(Guid employeeId, string templateName, List<CreateTimeEntryRequest> entries);

        /// <summary>
        /// 시간 기록 내보내기 (CSV/Excel)
        /// </summary>
        Task<byte[]> ExportTimeEntriesAsync(
            DateTime startDate,
            DateTime endDate,
            Guid? employeeId = null,
            Guid? projectId = null,
            string format = "csv");
    }
}