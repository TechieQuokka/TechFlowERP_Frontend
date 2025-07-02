using System.ComponentModel.DataAnnotations;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.User
{
    /// <summary>
    /// 사용자 역할 변경 요청
    /// </summary>
    public class ChangeUserRoleRequest : BaseRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public UserRole NewRole { get; set; }

        [StringLength(200, ErrorMessage = "변경 사유는 200자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }
    }
}
