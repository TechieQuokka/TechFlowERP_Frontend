using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.TimeEntry
{
    /// <summary>
    /// 시간 기록 생성 요청 DTO
    /// </summary>
    public class CreateTimeEntryRequest : BaseRequest
    {
        [Required(ErrorMessage = "직원 ID는 필수입니다.")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "프로젝트 ID는 필수입니다.")]
        public Guid ProjectId { get; set; }

        [Required(ErrorMessage = "작업 내용은 필수입니다.")]
        [StringLength(1000, ErrorMessage = "작업 내용은 1000자를 초과할 수 없습니다.")]
        public string TaskDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "시간은 필수입니다.")]
        [Range(0.25, 24, ErrorMessage = "시간은 0.25~24시간 사이여야 합니다.")]
        public decimal Hours { get; set; }

        [Required(ErrorMessage = "날짜는 필수입니다.")]
        public DateTime Date { get; set; } = DateTime.Today;

        public bool Billable { get; set; } = true;
    }
}