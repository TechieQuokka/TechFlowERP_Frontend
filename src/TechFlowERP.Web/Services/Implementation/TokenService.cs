using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using TechFlowERP.Models.Configuration;
using TechFlowERP.Models.DTOs.Auth;
using TechFlowERP.Models.Enums;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// JWT 토큰 서비스 구현
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<TokenService> _logger;

        private const string TOKEN_KEY = "techflow_token";
        private const string REFRESH_TOKEN_KEY = "techflow_refresh_token";

        public TokenService(
            ILocalStorageService localStorage,
            IOptions<JwtSettings> jwtSettings,
            ILogger<TokenService> logger)
        {
            _localStorage = localStorage;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// JWT 토큰에서 사용자 정보 추출
        /// </summary>
        public async Task<UserInfoDto?> GetUserFromTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                var userInfo = new UserInfoDto
                {
                    Id = Guid.Parse(GetClaimValue(jsonToken, ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString()),
                    Email = GetClaimValue(jsonToken, ClaimTypes.Email) ?? string.Empty,
                    Name = GetClaimValue(jsonToken, ClaimTypes.Name) ?? string.Empty,
                    Role = Enum.Parse<UserRole>(GetClaimValue(jsonToken, ClaimTypes.Role) ?? "Employee"),
                    TenantId = GetClaimValue(jsonToken, "tenant_id") ?? string.Empty,
                    Department = GetClaimValue(jsonToken, "department") ?? string.Empty,
                    Position = GetClaimValue(jsonToken, "position") ?? string.Empty
                };

                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰에서 사용자 정보 추출 중 오류 발생");
                return null;
            }
        }

        /// <summary>
        /// 토큰 유효성 검증
        /// </summary>
        public bool ValidateToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                // 만료 시간 확인
                return jsonToken.ValidTo > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰 유효성 검증 중 오류 발생");
                return false;
            }
        }

        /// <summary>
        /// 토큰 만료 시간 확인
        /// </summary>
        public DateTime? GetTokenExpiration(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                return jsonToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰 만료 시간 확인 중 오류 발생");
                return null;
            }
        }

        /// <summary>
        /// 토큰에서 클레임 추출
        /// </summary>
        public string? GetClaimFromToken(string token, string claimType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                return GetClaimValue(jsonToken, claimType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰에서 클레임 추출 중 오류 발생: {ClaimType}", claimType);
                return null;
            }
        }

        /// <summary>
        /// 토큰 저장
        /// </summary>
        public async Task SaveTokenAsync(string token, string refreshToken)
        {
            try
            {
                await _localStorage.SetItemAsync(TOKEN_KEY, token);
                await _localStorage.SetItemAsync(REFRESH_TOKEN_KEY, refreshToken);
                _logger.LogInformation("토큰이 성공적으로 저장되었습니다");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰 저장 중 오류 발생");
                throw;
            }
        }

        /// <summary>
        /// 토큰 제거
        /// </summary>
        public async Task RemoveTokenAsync()
        {
            try
            {
                await _localStorage.RemoveItemAsync(TOKEN_KEY);
                await _localStorage.RemoveItemAsync(REFRESH_TOKEN_KEY);
                _logger.LogInformation("토큰이 성공적으로 제거되었습니다");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰 제거 중 오류 발생");
                throw;
            }
        }

        /// <summary>
        /// 저장된 토큰 가져오기
        /// </summary>
        public async Task<string?> GetStoredTokenAsync()
        {
            try
            {
                return await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "저장된 토큰 가져오기 중 오류 발생");
                return null;
            }
        }

        /// <summary>
        /// 저장된 리프레시 토큰 가져오기
        /// </summary>
        public async Task<string?> GetStoredRefreshTokenAsync()
        {
            try
            {
                return await _localStorage.GetItemAsync<string>(REFRESH_TOKEN_KEY);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "저장된 리프레시 토큰 가져오기 중 오류 발생");
                return null;
            }
        }

        /// <summary>
        /// JWT 토큰에서 클레임 값 추출 헬퍼 메서드
        /// </summary>
        private static string? GetClaimValue(JwtSecurityToken token, string claimType)
        {
            return token.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
        }
    }
}