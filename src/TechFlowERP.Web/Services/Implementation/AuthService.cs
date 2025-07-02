using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TechFlowERP.Models.DTOs.Auth;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 인증 API 서비스 구현
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            HttpClient httpClient,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// 로그인
        /// </summary>
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                _logger.LogInformation("로그인 시도: {Email}", request.Email);

                var response = await _httpClient.PostAsJsonAsync("auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (loginResponse != null && loginResponse.Success)
                    {
                        // 토큰 저장
                        await _tokenService.SaveTokenAsync(loginResponse.Token, loginResponse.RefreshToken);
                        _logger.LogInformation("로그인 성공: {Email}", request.Email);
                    }

                    return loginResponse ?? new LoginResponseDto
                    {
                        Success = false,
                        Message = "응답 파싱 오류"
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("로그인 실패: {StatusCode}, {Error}", response.StatusCode, errorContent);

                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = response.StatusCode == HttpStatusCode.Unauthorized
                            ? "이메일 또는 비밀번호가 올바르지 않습니다."
                            : "로그인 중 오류가 발생했습니다."
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "로그인 API 호출 중 네트워크 오류");
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "서버에 연결할 수 없습니다. 네트워크 연결을 확인해주세요."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그인 중 예상치 못한 오류 발생");
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "로그인 중 오류가 발생했습니다."
                };
            }
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        public async Task<bool> LogoutAsync(LogoutRequestDto request)
        {
            try
            {
                _logger.LogInformation("로그아웃 시도");

                // 토큰을 헤더에 추가
                var token = await _tokenService.GetStoredTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsJsonAsync("auth/logout", request);

                // 성공 여부와 관계없이 로컬 토큰 제거
                await _tokenService.RemoveTokenAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("로그아웃 성공");
                    return true;
                }
                else
                {
                    _logger.LogWarning("서버 로그아웃 실패, 로컬 토큰만 제거됨: {StatusCode}", response.StatusCode);
                    return true; // 로컬 토큰은 제거했으므로 성공으로 처리
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그아웃 중 오류 발생");

                // 오류가 발생해도 로컬 토큰은 제거
                await _tokenService.RemoveTokenAsync();
                return true;
            }
        }

        /// <summary>
        /// 토큰 갱신
        /// </summary>
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            try
            {
                _logger.LogInformation("토큰 갱신 시도");

                var response = await _httpClient.PostAsJsonAsync("auth/refresh", request);

                if (response.IsSuccessStatusCode)
                {
                    var refreshResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (refreshResponse != null && refreshResponse.Success)
                    {
                        // 새 토큰 저장
                        await _tokenService.SaveTokenAsync(refreshResponse.Token, refreshResponse.RefreshToken);
                        _logger.LogInformation("토큰 갱신 성공");
                    }

                    return refreshResponse ?? new LoginResponseDto
                    {
                        Success = false,
                        Message = "토큰 갱신 응답 파싱 오류"
                    };
                }
                else
                {
                    _logger.LogWarning("토큰 갱신 실패: {StatusCode}", response.StatusCode);

                    // 갱신 실패 시 기존 토큰 제거
                    await _tokenService.RemoveTokenAsync();

                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "토큰 갱신에 실패했습니다. 다시 로그인해주세요."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰 갱신 중 오류 발생");

                // 오류 발생 시 기존 토큰 제거
                await _tokenService.RemoveTokenAsync();

                return new LoginResponseDto
                {
                    Success = false,
                    Message = "토큰 갱신 중 오류가 발생했습니다."
                };
            }
        }

        /// <summary>
        /// 현재 사용자 정보 조회
        /// </summary>
        public async Task<UserInfoDto?> GetCurrentUserAsync()
        {
            try
            {
                var token = await _tokenService.GetStoredTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("저장된 토큰이 없음");
                    return null;
                }

                // 토큰에서 사용자 정보 추출
                var userFromToken = await _tokenService.GetUserFromTokenAsync(token);

                if (userFromToken != null)
                {
                    // 필요시 서버에서 최신 정보 조회
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await _httpClient.GetAsync("auth/me");

                    if (response.IsSuccessStatusCode)
                    {
                        var serverUser = await response.Content.ReadFromJsonAsync<UserInfoDto>();
                        return serverUser ?? userFromToken;
                    }
                }

                return userFromToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "현재 사용자 정보 조회 중 오류 발생");
                return null;
            }
        }

        /// <summary>
        /// 비밀번호 변경
        /// </summary>
        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var token = await _tokenService.GetStoredTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("비밀번호 변경: 인증 토큰 없음");
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new
                {
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                var response = await _httpClient.PostAsJsonAsync("auth/change-password", request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("비밀번호 변경 성공");
                    return true;
                }
                else
                {
                    _logger.LogWarning("비밀번호 변경 실패: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "비밀번호 변경 중 오류 발생");
                return false;
            }
        }

        /// <summary>
        /// 토큰 유효성 검증
        /// </summary>
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                // 로컬 검증
                if (!_tokenService.ValidateToken(token))
                {
                    return false;
                }

                // 서버 검증 (선택사항)
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("auth/validate");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "토큰 유효성 검증 중 오류 발생");
                return false;
            }
        }
    }
}