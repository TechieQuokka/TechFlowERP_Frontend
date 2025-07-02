using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.User
{
    /// <summary>
    /// 비밀번호 재설정 요청
    /// </summary>
    public class ResetPasswordRequest : BaseRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "새 비밀번호는 필수입니다.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "비밀번호는 8자 이상 100자 이하여야 합니다.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
            ErrorMessage = "비밀번호는 대소문자, 숫자, 특수문자를 포함해야 합니다.")]
        public string NewPassword { get; set; } = string.Empty;

        public bool ForcePasswordChange { get; set; } = true;
    }
}
