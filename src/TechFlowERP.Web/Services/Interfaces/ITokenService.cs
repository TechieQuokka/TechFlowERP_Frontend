using TechFlowERP.Models.DTOs.Auth;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// JWT 토큰 서비스 인터페이스
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// JWT 토큰에서 사용자 정보 추출
        /// </summary>
        Task<UserInfoDto?> GetUserFromTokenAsync(string token);

        /// <summary>
        /// 토큰 유효성 검증
        /// </summary>
        bool ValidateToken(string token);

        /// <summary>
        /// 토큰 만료 시간 확인
        /// </summary>
        DateTime? GetTokenExpiration(string token);

        /// <summary>
        /// 토큰에서 클레임 추출
        /// </summary>
        string? GetClaimFromToken(string token, string claimType);

        /// <summary>
        /// 토큰 저장
        /// </summary>
        Task SaveTokenAsync(string token, string refreshToken);

        /// <summary>
        /// 토큰 제거
        /// </summary>
        Task RemoveTokenAsync();

        /// <summary>
        /// 저장된 토큰 가져오기
        /// </summary>
        Task<string?> GetStoredTokenAsync();

        /// <summary>
        /// 저장된 리프레시 토큰 가져오기
        /// </summary>
        Task<string?> GetStoredRefreshTokenAsync();
    }
}