using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TechFlowERP.Models.Common;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 기본 서비스 구현체 - 공통 기능 제공
    /// </summary>
    public abstract class BaseService
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly ITokenService _tokenService;
        protected readonly string _baseEndpoint;

        protected BaseService(
            HttpClient httpClient,
            ILogger logger,
            ITokenService tokenService,
            string baseEndpoint)
        {
            _httpClient = httpClient;
            _logger = logger;
            _tokenService = tokenService;
            _baseEndpoint = baseEndpoint.TrimEnd('/');
        }

        /// <summary>
        /// HTTP 요청 전 Authorization 헤더 설정
        /// </summary>
        protected async Task SetAuthorizationHeaderAsync()
        {
            try
            {
                var token = await _tokenService.GetStoredTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "토큰 설정 중 오류 발생");
            }
        }

        /// <summary>
        /// 쿼리 스트링 빌더
        /// </summary>
        protected string BuildQueryString(object parameters)
        {
            if (parameters == null) return string.Empty;

            var properties = parameters.GetType().GetProperties();
            var queryParams = new List<string>();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(parameters);
                if (value != null)
                {
                    var stringValue = value.ToString();
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        queryParams.Add($"{prop.Name}={Uri.EscapeDataString(stringValue)}");
                    }
                }
            }

            return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        }

        /// <summary>
        /// HTTP 응답 처리 공통 로직
        /// </summary>
        protected async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response, string operation)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(content))
                    {
                        return default(T);
                    }

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    return JsonSerializer.Deserialize<T>(content, options);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("{Operation} 실패: {StatusCode}, {Error}",
                        operation, response.StatusCode, errorContent);

                    // 401 Unauthorized인 경우 토큰 갱신 시도
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorizedAsync();
                    }

                    return default(T);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "{Operation} 응답 JSON 파싱 오류", operation);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Operation} 응답 처리 중 오류", operation);
                return default(T);
            }
        }

        /// <summary>
        /// 401 Unauthorized 처리
        /// </summary>
        protected async Task HandleUnauthorizedAsync()
        {
            try
            {
                _logger.LogInformation("토큰 만료 감지, 갱신 시도");

                var refreshToken = await _tokenService.GetStoredRefreshTokenAsync();
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogInformation("리프레시 토큰으로 갱신 필요");
                }
                else
                {
                    _logger.LogWarning("리프레시 토큰이 없어 재로그인 필요");
                    await _tokenService.RemoveTokenAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unauthorized 처리 중 오류");
            }
        }

        /// <summary>
        /// 네트워크 오류 처리
        /// </summary>
        protected void HandleNetworkError(Exception ex, string operation)
        {
            if (ex is HttpRequestException)
            {
                _logger.LogError(ex, "{Operation} 네트워크 오류", operation);
            }
            else if (ex is TaskCanceledException)
            {
                _logger.LogError(ex, "{Operation} 요청 타임아웃", operation);
            }
            else
            {
                _logger.LogError(ex, "{Operation} 예상치 못한 오류", operation);
            }
        }
    }

    /// <summary>
    /// 제네릭 CRUD 서비스 구현체
    /// </summary>
    public abstract class BaseService<TDto, TRequest> : BaseService, IBaseService<TDto, TRequest>
        where TDto : class
        where TRequest : class
    {
        protected BaseService(
            HttpClient httpClient,
            ILogger logger,
            ITokenService tokenService,
            string baseEndpoint)
            : base(httpClient, logger, tokenService, baseEndpoint)
        {
        }

        public virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                _logger.LogInformation("{Entity} 상세 조회: {Id}", typeof(TDto).Name, id);
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{id}");
                return await HandleResponseAsync<TDto>(response, $"{typeof(TDto).Name} 조회");
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"{typeof(TDto).Name} 조회");
                return null;
            }
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                _logger.LogInformation("{Entity} 목록 조회", typeof(TDto).Name);
                var response = await _httpClient.GetAsync(_baseEndpoint);
                var result = await HandleResponseAsync<IEnumerable<TDto>>(response, $"{typeof(TDto).Name} 목록 조회");
                return result ?? new List<TDto>();
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"{typeof(TDto).Name} 목록 조회");
                return new List<TDto>();
            }
        }

        public virtual async Task<PagedResult<TDto>> GetPagedAsync(int page = 1, int pageSize = 20, string? searchTerm = null)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var queryString = BuildQueryString(new { page, pageSize, searchTerm });
                _logger.LogInformation("{Entity} 페이징 조회: Page {Page}, Size {PageSize}", typeof(TDto).Name, page, pageSize);
                var response = await _httpClient.GetAsync($"{_baseEndpoint}{queryString}");
                var result = await HandleResponseAsync<PagedResult<TDto>>(response, $"{typeof(TDto).Name} 페이징 조회");
                return result ?? new PagedResult<TDto>();
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"{typeof(TDto).Name} 페이징 조회");
                return new PagedResult<TDto>();
            }
        }

        public virtual async Task<TDto?> CreateAsync(TRequest request)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                _logger.LogInformation("{Entity} 생성 요청", typeof(TDto).Name);
                var response = await _httpClient.PostAsJsonAsync(_baseEndpoint, request);
                return await HandleResponseAsync<TDto>(response, $"{typeof(TDto).Name} 생성");
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"{typeof(TDto).Name} 생성");
                return null;
            }
        }

        public virtual async Task<TDto?> UpdateAsync(Guid id, TRequest request)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                _logger.LogInformation("{Entity} 수정 요청: {Id}", typeof(TDto).Name, id);
                var response = await _httpClient.PutAsJsonAsync($"{_baseEndpoint}/{id}", request);
                return await HandleResponseAsync<TDto>(response, $"{typeof(TDto).Name} 수정");
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"{typeof(TDto).Name} 수정");
                return null;
            }
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                _logger.LogInformation("{Entity} 삭제 요청: {Id}", typeof(TDto).Name, id);
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("{Entity} 삭제 완료: {Id}", typeof(TDto).Name, id);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("{Entity} 삭제 실패: {StatusCode}, {Error}", typeof(TDto).Name, response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"{typeof(TDto).Name} 삭제");
                return false;
            }
        }

        // 헬퍼 메서드들
        protected async Task<T?> GetAsync<T>(string endpoint, object? parameters = null)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var queryString = parameters != null ? BuildQueryString(parameters) : string.Empty;
                var fullEndpoint = $"{_baseEndpoint}/{endpoint.TrimStart('/')}{queryString}";
                var response = await _httpClient.GetAsync(fullEndpoint);
                return await HandleResponseAsync<T>(response, $"커스텀 GET {endpoint}");
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"커스텀 GET {endpoint}");
                return default(T);
            }
        }

        protected async Task<T?> PostAsync<T>(string endpoint, object request)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var fullEndpoint = $"{_baseEndpoint}/{endpoint.TrimStart('/')}";
                var response = await _httpClient.PostAsJsonAsync(fullEndpoint, request);
                return await HandleResponseAsync<T>(response, $"커스텀 POST {endpoint}");
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"커스텀 POST {endpoint}");
                return default(T);
            }
        }

        protected async Task<T?> PutAsync<T>(string endpoint, object request)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var fullEndpoint = $"{_baseEndpoint}/{endpoint.TrimStart('/')}";
                var response = await _httpClient.PutAsJsonAsync(fullEndpoint, request);
                return await HandleResponseAsync<T>(response, $"커스텀 PUT {endpoint}");
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"커스텀 PUT {endpoint}");
                return default(T);
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                var fullEndpoint = $"{_baseEndpoint}/{endpoint.TrimStart('/')}";
                var response = await _httpClient.DeleteAsync(fullEndpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                HandleNetworkError(ex, $"커스텀 DELETE {endpoint}");
                return false;
            }
        }
    }
}