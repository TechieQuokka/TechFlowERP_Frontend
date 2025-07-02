using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TechFlowERP.Models.DTOs.Auth;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Authentication
{
    /// <summary>
    /// 커스텀 인증 상태 제공자
    /// </summary>
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        private AuthenticationState _anonymous;
        private bool _isInitialized = false;

        public CustomAuthenticationStateProvider(
            ITokenService tokenService,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        /// <summary>
        /// 현재 인증 상태 가져오기
        /// </summary>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // 초기화되지 않았으면 익명 상태 반환 (Prerendering 대응)
            if (!_isInitialized)
            {
                return _anonymous;
            }

            try
            {
                var token = await _tokenService.GetStoredTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug("저장된 토큰이 없음");
                    return _anonymous;
                }

                // 토큰 유효성 검증
                if (!_tokenService.ValidateToken(token))
                {
                    _logger.LogWarning("토큰이 유효하지 않음");
                    await _tokenService.RemoveTokenAsync();
                    return _anonymous;
                }

                // 토큰에서 사용자 정보 추출
                var user = await _tokenService.GetUserFromTokenAsync(token);

                if (user == null)
                {
                    _logger.LogWarning("토큰에서 사용자 정보 추출 실패");
                    await _tokenService.RemoveTokenAsync();
                    return _anonymous;
                }

                // ClaimsPrincipal 생성
                var claimsPrincipal = CreateClaimsPrincipal(user);
                var authState = new AuthenticationState(claimsPrincipal);

                _logger.LogInformation("인증 상태 확인 완료: {Email}", user.Email);
                return authState;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "인증 상태 확인 중 오류 발생");
                return _anonymous;
            }
        }

        /// <summary>
        /// 초기화 (JavaScript가 로드된 후 호출)
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _isInitialized = true;
                var currentState = await GetAuthenticationStateAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(currentState));
                _logger.LogDebug("AuthenticationStateProvider 초기화 완료");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthenticationStateProvider 초기화 중 오류 발생");
            }
        }

        /// <summary>
        /// 로그인 상태로 표시
        /// </summary>
        public async Task MarkUserAsAuthenticated(UserInfoDto user, string token, string refreshToken)
        {
            try
            {
                _isInitialized = true;

                // 토큰 저장
                await _tokenService.SaveTokenAsync(token, refreshToken);

                // ClaimsPrincipal 생성
                var claimsPrincipal = CreateClaimsPrincipal(user);
                var authState = new AuthenticationState(claimsPrincipal);

                // 인증 상태 변경 알림
                NotifyAuthenticationStateChanged(Task.FromResult(authState));

                _logger.LogInformation("사용자 인증 상태 업데이트: {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 인증 상태 업데이트 중 오류 발생");
            }
        }

        /// <summary>
        /// 로그아웃 상태로 표시
        /// </summary>
        public async Task MarkUserAsLoggedOut()
        {
            try
            {
                // 토큰 제거
                await _tokenService.RemoveTokenAsync();

                // 익명 상태로 변경
                NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));

                _logger.LogInformation("사용자 로그아웃 상태 업데이트");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 로그아웃 상태 업데이트 중 오류 발생");
            }
        }

        /// <summary>
        /// UserInfoDto에서 ClaimsPrincipal 생성
        /// </summary>
        private static ClaimsPrincipal CreateClaimsPrincipal(UserInfoDto user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name), // ✅ Name 클레임 필수!
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("tenant_id", user.TenantId),
                new("department", user.Department),
                new("position", user.Position)
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// 현재 사용자 정보 가져오기
        /// </summary>
        public async Task<UserInfoDto?> GetCurrentUserAsync()
        {
            try
            {
                if (!_isInitialized)
                    return null;

                var token = await _tokenService.GetStoredTokenAsync();

                if (string.IsNullOrEmpty(token))
                    return null;

                return await _tokenService.GetUserFromTokenAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "현재 사용자 정보 가져오기 중 오류 발생");
                return null;
            }
        }
    }
}