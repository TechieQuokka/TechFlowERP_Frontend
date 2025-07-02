using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.TimeEntry
{
    /// <summary>
    /// 시간 기록 승인 요청 DTO
    /// </summary>
    public class ApproveTimeEntryRequest : BaseRequest
    {
        [Required(ErrorMessage = "승인자 ID는 필수입니다.")]
        public Guid ApprovedBy { get; set; }

        public List<Guid> TimeEntryIds { get; set; } = new();

        public string ApprovalNote { get; set; } = string.Empty;
    }

    /// <summary>
    /// 시간 기록 거부 요청 DTO
    /// </summary>
    public class RejectTimeEntryRequest : BaseRequest
    {
        [Required(ErrorMessage = "거부자 ID는 필수입니다.")]
        public Guid RejectedBy { get; set; }

        public List<Guid> TimeEntryIds { get; set; } = new();

        [Required(ErrorMessage = "거부 사유는 필수입니다.")]
        [StringLength(500, ErrorMessage = "거부 사유는 500자를 초과할 수 없습니다.")]
        public string RejectionReason { get; set; } = string.Empty;
    }
}