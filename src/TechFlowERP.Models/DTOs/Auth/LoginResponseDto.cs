using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.Auth
{
    /// <summary>
    /// 로그인 응답 DTO
    /// </summary>
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto? User { get; set; }
    }

    /// <summary>
    /// 사용자 정보 DTO
    /// </summary>
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}