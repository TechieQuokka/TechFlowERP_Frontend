using TechFlowERP.Models.DTOs.Auth;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 인증 API 서비스 인터페이스
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 로그인
        /// </summary>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

        /// <summary>
        /// 로그아웃
        /// </summary>
        Task<bool> LogoutAsync(LogoutRequestDto request);

        /// <summary>
        /// 토큰 갱신
        /// </summary>
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);

        /// <summary>
        /// 현재 사용자 정보 조회
        /// </summary>
        Task<UserInfoDto?> GetCurrentUserAsync();

        /// <summary>
        /// 비밀번호 변경
        /// </summary>
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);

        /// <summary>
        /// 토큰 유효성 검증
        /// </summary>
        Task<bool> ValidateTokenAsync(string token);
    }
}