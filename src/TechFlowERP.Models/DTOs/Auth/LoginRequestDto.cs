using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.DTOs.Auth
{
    /// <summary>
    /// 로그인 요청 DTO
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "사용자명은 필수입니다.")]
        [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호는 필수입니다.")]
        [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상이어야 합니다.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}