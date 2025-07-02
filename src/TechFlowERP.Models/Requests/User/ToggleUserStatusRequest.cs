using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.User
{
    /// <summary>
    /// 사용자 상태 변경 요청
    /// </summary>
    public class ToggleUserStatusRequest : BaseRequest
    {
        [Required]
        public Guid UserId { get; set; }

        public bool IsLocked { get; set; }

        public string? Reason { get; set; }
    }
}
