using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.Client;
using TechFlowERP.Models.DTOs.Project;
using TechFlowERP.Models.Requests.Client;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Responses.Client;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 클라이언트 관리 서비스 구현
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientService> _logger;
        private readonly string _baseEndpoint = "clients";
        private readonly JsonSerializerOptions _jsonOptions;

        public ClientService(HttpClient httpClient, ILogger<ClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// 클라이언트 목록 조회 (페이징)
        /// </summary>
        public async Task<ClientListResponse> GetClientsAsync(
    int pageNumber = 1,
    int pageSize = 20,
    string? searchTerm = null)
        {
            try
            {
                var queryParams = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}"
        };

                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_baseEndpoint}?{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("클라이언트 목록 조회 성공. 응답: {Json}", jsonContent);

                    try
                    {
                        // 직접 배열로 파싱 시도
                        var clients = JsonSerializer.Deserialize<List<ClientDto>>(jsonContent, _jsonOptions);

                        if (clients != null)
                        {
                            _logger.LogInformation("응답이 직접 배열 형태입니다. 클라이언트 {Count}개 조회됨", clients.Count);

                            return new ClientListResponse
                            {
                                Success = true,
                                Message = "클라이언트 목록 조회 성공",
                                Data = new PagedResult<ClientDto>
                                {
                                    Items = clients,
                                    TotalCount = clients.Count,
                                    PageNumber = pageNumber,
                                    PageSize = pageSize
                                }
                            };
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "JSON 파싱 실패: {Json}", jsonContent);

                        // BaseResponse 구조로 재시도
                        try
                        {
                            var wrappedResponse = JsonSerializer.Deserialize<ClientListResponse>(jsonContent, _jsonOptions);
                            if (wrappedResponse != null)
                            {
                                return wrappedResponse;
                            }
                        }
                        catch
                        {
                            // 무시하고 계속
                        }

                        return new ClientListResponse
                        {
                            Success = false,
                            Message = $"JSON 파싱 실패: {ex.Message}"
                        };
                    }
                }

                _logger.LogWarning("클라이언트 목록 조회 실패: {StatusCode}", response.StatusCode);
                return new ClientListResponse
                {
                    Success = false,
                    Message = GetErrorMessage(response.StatusCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "클라이언트 목록 조회 중 오류 발생");
                return new ClientListResponse
                {
                    Success = false,
                    Message = "클라이언트 목록 조회 중 오류가 발생했습니다."
                };
            }
        }

        /// <summary>
        /// 클라이언트 상세 조회
        /// </summary>
        public async Task<ClientDto?> GetClientByIdAsync(Guid clientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{clientId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<ClientDto>>();
                    return result?.Data;
                }

                _logger.LogWarning("클라이언트 상세 조회 실패: {ClientId}, {StatusCode}", clientId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "클라이언트 상세 조회 중 오류 발생: {ClientId}", clientId);
                return null;
            }
        }

        /// <summary>
        /// 클라이언트 생성
        /// </summary>
        public async Task<ClientDto?> CreateClientAsync(CreateClientRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<ClientDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("클라이언트 생성 성공: {CompanyName}", request.CompanyName);
                        return result.Data;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("클라이언트 생성 실패: {StatusCode}, {Error}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "클라이언트 생성 중 오류 발생: {CompanyName}", request.CompanyName);
                return null;
            }
        }

        /// <summary>
        /// 클라이언트 수정
        /// </summary>
        public async Task<ClientDto?> UpdateClientAsync(Guid clientId, UpdateClientRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{clientId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<ClientDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("클라이언트 수정 성공: {ClientId}", clientId);
                        return result.Data;
                    }
                }

                _logger.LogWarning("클라이언트 수정 실패: {ClientId}, {StatusCode}", clientId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "클라이언트 수정 중 오류 발생: {ClientId}", clientId);
                return null;
            }
        }

        /// <summary>
        /// 클라이언트 삭제
        /// </summary>
        public async Task<bool> DeleteClientAsync(Guid clientId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{clientId}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("클라이언트 삭제 성공: {ClientId}", clientId);
                    return true;
                }

                _logger.LogWarning("클라이언트 삭제 실패: {ClientId}, {StatusCode}", clientId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "클라이언트 삭제 중 오류 발생: {ClientId}", clientId);
                return false;
            }
        }

        /// <summary>
        /// 클라이언트 검색
        /// </summary>
        public async Task<List<ClientDto>> SearchClientsAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<ClientDto>>>();
                    return result?.Data ?? new List<ClientDto>();
                }

                return new List<ClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "클라이언트 검색 중 오류 발생: {SearchTerm}", searchTerm);
                return new List<ClientDto>();
            }
        }

        /// <summary>
        /// 클라이언트의 프로젝트 목록 조회
        /// </summary>
        public async Task<List<ProjectDto>> GetClientProjectsAsync(Guid clientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{clientId}/projects");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<ProjectDto>>>();
                    return result?.Data ?? new List<ProjectDto>();
                }

                return new List<ProjectDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "클라이언트 프로젝트 목록 조회 중 오류 발생: {ClientId}", clientId);
                return new List<ProjectDto>();
            }
        }

        /// <summary>
        /// 업종별 클라이언트 통계
        /// </summary>
        public async Task<Dictionary<string, int>> GetClientsByIndustryAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/statistics/by-industry");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<Dictionary<string, int>>>();
                    return result?.Data ?? new Dictionary<string, int>();
                }

                return new Dictionary<string, int>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "업종별 클라이언트 통계 조회 중 오류 발생");
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// HTTP 상태 코드에 따른 오류 메시지 반환
        /// </summary>
        private static string GetErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "요청한 클라이언트를 찾을 수 없습니다.",
                HttpStatusCode.Unauthorized => "접근 권한이 없습니다.",
                HttpStatusCode.Forbidden => "이 작업을 수행할 권한이 없습니다.",
                HttpStatusCode.BadRequest => "잘못된 요청입니다.",
                HttpStatusCode.InternalServerError => "서버 내부 오류가 발생했습니다.",
                _ => "알 수 없는 오류가 발생했습니다."
            };
        }
    }
}