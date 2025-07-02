using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.Auth
{
    /// <summary>
    /// 인증 상태 DTO
    /// </summary>
    public class AuthStateDto
    {
        public bool IsAuthenticated { get; set; }
        public UserInfoDto? User { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public List<string> Permissions { get; set; } = new();

        public bool IsInRole(UserRole role)
        {
            return User?.Role == role;
        }

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public bool IsTokenExpired()
        {
            return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
        }
    }
}