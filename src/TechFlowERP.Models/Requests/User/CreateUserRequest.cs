using System.ComponentModel.DataAnnotations;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.User
{
    /// <summary>
    /// 사용자 생성 요청
    /// </summary>
    public class CreateUserRequest : BaseRequest
    {
        [Required(ErrorMessage = "사용자명은 필수입니다.")]
        [StringLength(50, ErrorMessage = "사용자명은 50자를 초과할 수 없습니다.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "이메일은 필수입니다.")]
        [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
        [StringLength(100, ErrorMessage = "이메일은 100자를 초과할 수 없습니다.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호는 필수입니다.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "비밀번호는 8자 이상 100자 이하여야 합니다.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
            ErrorMessage = "비밀번호는 대소문자, 숫자, 특수문자를 포함해야 합니다.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "역할은 필수입니다.")]
        public UserRole Role { get; set; }

        public Guid? EmployeeId { get; set; }
    }
}
