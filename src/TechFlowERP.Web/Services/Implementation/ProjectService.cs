using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.Project;
using TechFlowERP.Models.Enums;
using TechFlowERP.Models.Requests.Project;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Responses.Project;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 프로젝트 관리 서비스 구현 - 완전한 구현체
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectService> _logger;
        private readonly string _baseEndpoint = "projects";
        private readonly JsonSerializerOptions _jsonOptions;

        public ProjectService(HttpClient httpClient, ILogger<ProjectService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        #region 기본 CRUD 메서드들

        /// <summary>
        /// 프로젝트 목록 조회 (페이징)
        /// </summary>
        public async Task<ProjectListResponse> GetProjectsAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            ProjectStatus? status = null,
            Guid? clientId = null,
            Guid? managerId = null)
        {
            try
            {
                var queryParams = BuildQueryParams(new Dictionary<string, object?>
                {
                    ["pageNumber"] = pageNumber,
                    ["pageSize"] = pageSize,
                    ["searchTerm"] = searchTerm,
                    ["status"] = status,
                    ["clientId"] = clientId,
                    ["managerId"] = managerId
                });

                var response = await _httpClient.GetAsync($"{_baseEndpoint}?{queryParams}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("프로젝트 목록 조회 성공. 응답: {Json}", jsonContent);

                    // 직접 배열 파싱 시도
                    var projects = await TryParseDirectArray<ProjectDto>(jsonContent);
                    if (projects != null)
                    {
                        return new ProjectListResponse
                        {
                            Success = true,
                            Message = "프로젝트 목록 조회 성공",
                            Data = new PagedResult<ProjectDto>
                            {
                                Items = projects,
                                TotalCount = projects.Count,
                                PageNumber = pageNumber,
                                PageSize = pageSize
                            }
                        };
                    }

                    // BaseResponse 구조 파싱 시도
                    var wrappedResponse = await TryParseWrappedResponse<ProjectListResponse>(jsonContent);
                    if (wrappedResponse != null) return wrappedResponse;
                }

                return CreateErrorResponse<ProjectListResponse>(response.StatusCode, "프로젝트 목록 조회 실패");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "프로젝트 목록 조회 중 오류 발생");
                return new ProjectListResponse { Success = false, Message = "프로젝트 목록 조회 중 오류가 발생했습니다." };
            }
        }

        /// <summary>
        /// 프로젝트 상세 조회
        /// </summary>
        public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId)
        {
            return await GetSingleAsync<ProjectDto>($"{_baseEndpoint}/{projectId}");
        }

        /// <summary>
        /// 프로젝트 생성
        /// </summary>
        public async Task<ProjectDto?> CreateProjectAsync(CreateProjectRequest request)
        {
            return await PostAsync<CreateProjectRequest, ProjectDto>(_baseEndpoint, request,
                $"프로젝트 생성: {request.Name}");
        }

        /// <summary>
        /// 프로젝트 수정
        /// </summary>
        public async Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request)
        {
            return await PutAsync<UpdateProjectRequest, ProjectDto>($"{_baseEndpoint}/{projectId}", request,
                $"프로젝트 수정: {projectId}");
        }

        /// <summary>
        /// 프로젝트 삭제
        /// </summary>
        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            return await DeleteAsync($"{_baseEndpoint}/{projectId}", $"프로젝트 삭제: {projectId}");
        }

        #endregion

        #region 상태 및 검색 메서드들

        /// <summary>
        /// 프로젝트 상태 변경
        /// </summary>
        public async Task<bool> ChangeProjectStatusAsync(Guid projectId, ProjectStatus status)
        {
            var request = new { Status = status };
            return await PatchAsync<object>($"{_baseEndpoint}/{projectId}/status", request,
                $"프로젝트 상태 변경: {projectId} -> {status}");
        }

        /// <summary>
        /// 프로젝트 검색
        /// </summary>
        public async Task<List<ProjectDto>> SearchProjectsAsync(string searchTerm)
        {
            return await GetListAsync<ProjectDto>($"{_baseEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}");
        }

        /// <summary>
        /// 활성 프로젝트 목록 조회
        /// </summary>
        public async Task<List<ProjectDto>> GetActiveProjectsAsync()
        {
            return await GetListAsync<ProjectDto>($"{_baseEndpoint}/active");
        }

        /// <summary>
        /// 매니저별 프로젝트 목록 조회
        /// </summary>
        public async Task<List<ProjectDto>> GetProjectsByManagerAsync(Guid managerId)
        {
            return await GetListAsync<ProjectDto>($"{_baseEndpoint}/manager/{managerId}");
        }

        /// <summary>
        /// 클라이언트별 프로젝트 목록 조회
        /// </summary>
        public async Task<List<ProjectDto>> GetProjectsByClientAsync(Guid clientId)
        {
            return await GetListAsync<ProjectDto>($"{_baseEndpoint}/client/{clientId}");
        }

        /// <summary>
        /// 기술별 프로젝트 목록 조회
        /// </summary>
        public async Task<List<ProjectDto>> GetProjectsByTechnologyAsync(string technology)
        {
            return await GetListAsync<ProjectDto>($"{_baseEndpoint}/technology/{Uri.EscapeDataString(technology)}");
        }

        /// <summary>
        /// 프로젝트 위험도별 목록 조회
        /// </summary>
        public async Task<List<ProjectDto>> GetProjectsByRiskLevelAsync(RiskLevel riskLevel)
        {
            return await GetListAsync<ProjectDto>($"{_baseEndpoint}/risk/{riskLevel}");
        }

        /// <summary>
        /// 지연된 프로젝트 목록 조회
        /// </summary>
        public async Task<List<ProjectDto>> GetDelayedProjectsAsync()
        {
            return await GetListAsync<ProjectDto>($"{_baseEndpoint}/delayed");
        }

        #endregion

        #region 팀원 및 할당 관리

        /// <summary>
        /// 직원 프로젝트 할당
        /// </summary>
        public async Task<ProjectAssignmentDto?> AssignEmployeeAsync(AssignEmployeeRequest request)
        {
            return await PostAsync<AssignEmployeeRequest, ProjectAssignmentDto>($"{_baseEndpoint}/assignments", request,
                $"직원 프로젝트 할당: {request.EmployeeId} -> {request.ProjectId}");
        }

        /// <summary>
        /// 직원 프로젝트 할당 해제
        /// </summary>
        public async Task<bool> UnassignEmployeeAsync(Guid assignmentId)
        {
            return await DeleteAsync($"{_baseEndpoint}/assignments/{assignmentId}",
                $"직원 프로젝트 할당 해제: {assignmentId}");
        }

        /// <summary>
        /// 프로젝트 팀원 목록 조회
        /// </summary>
        public async Task<List<ProjectAssignmentDto>> GetProjectTeamAsync(Guid projectId)
        {
            return await GetListAsync<ProjectAssignmentDto>($"{_baseEndpoint}/{projectId}/team");
        }

        #endregion

        #region 마일스톤 및 진행률

        /// <summary>
        /// 프로젝트 마일스톤 목록 조회
        /// </summary>
        public async Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(Guid projectId)
        {
            return await GetListAsync<ProjectMilestoneDto>($"{_baseEndpoint}/{projectId}/milestones");
        }

        /// <summary>
        /// 마일스톤 완료 처리
        /// </summary>
        public async Task<bool> CompleteMilestoneAsync(Guid milestoneId)
        {
            return await PatchAsync<object>($"{_baseEndpoint}/milestones/{milestoneId}/complete", null,
                $"마일스톤 완료 처리: {milestoneId}");
        }

        /// <summary>
        /// 프로젝트 진행률 조회
        /// </summary>
        public async Task<decimal> GetProjectProgressAsync(Guid projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{projectId}/progress");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    // 직접 숫자 파싱 시도
                    if (decimal.TryParse(jsonContent.Trim('"'), out decimal progress))
                    {
                        return progress;
                    }

                    // BaseResponse 구조로 재시도
                    var result = JsonSerializer.Deserialize<BaseResponse<decimal>>(jsonContent, _jsonOptions);
                    return result?.Data ?? 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "프로젝트 진행률 조회 중 오류 발생: {ProjectId}", projectId);
                return 0;
            }
        }

        #endregion

        #region 통계 및 분석

        /// <summary>
        /// 프로젝트 통계 조회
        /// </summary>
        public async Task<ProjectStatisticsResponse> GetProjectStatisticsAsync()
        {
            var result = await GetSingleAsync<ProjectStatisticsResponse>($"{_baseEndpoint}/statistics");
            return result ?? new ProjectStatisticsResponse { Success = false, Message = "통계 조회 실패" };
        }

        /// <summary>
        /// 프로젝트 수익성 분석
        /// </summary>
        public async Task<ProjectProfitabilityResponse> GetProjectProfitabilityAsync()
        {
            var result = await GetSingleAsync<ProjectProfitabilityResponse>($"{_baseEndpoint}/profitability");
            return result ?? new ProjectProfitabilityResponse { Success = false, Message = "수익성 분석 실패" };
        }

        /// <summary>
        /// 프로젝트 대시보드 데이터 조회
        /// </summary>
        public async Task<ProjectDashboardData> GetProjectDashboardDataAsync()
        {
            var result = await GetSingleAsync<ProjectDashboardData>($"{_baseEndpoint}/dashboard");
            return result ?? new ProjectDashboardData();
        }

        #endregion

        #region 공통 헬퍼 메서드들

        /// <summary>
        /// 공통 리스트 조회 헬퍼
        /// </summary>
        private async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    // 직접 배열 파싱 시도
                    var items = await TryParseDirectArray<T>(jsonContent);
                    if (items != null) return items;

                    // BaseResponse 구조로 재시도
                    var result = JsonSerializer.Deserialize<BaseResponse<List<T>>>(jsonContent, _jsonOptions);
                    return result?.Data ?? new List<T>();
                }
                return new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "리스트 조회 중 오류 발생: {Endpoint}", endpoint);
                return new List<T>();
            }
        }

        /// <summary>
        /// 공통 단일 객체 조회 헬퍼
        /// </summary>
        private async Task<T?> GetSingleAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    // 직접 객체 파싱 시도
                    try
                    {
                        var item = JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
                        if (item != null) return item;
                    }
                    catch { }

                    // BaseResponse 구조로 재시도
                    var result = JsonSerializer.Deserialize<BaseResponse<T>>(jsonContent, _jsonOptions);
                    return result != null && result.Success ? result.Data : default;
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "단일 객체 조회 중 오류 발생: {Endpoint}", endpoint);
                return default;
            }
        }

        /// <summary>
        /// 공통 POST 요청 헬퍼
        /// </summary>
        private async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, string logMessage)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    // 직접 객체 파싱 시도
                    try
                    {
                        var result = JsonSerializer.Deserialize<TResponse>(jsonContent, _jsonOptions);
                        if (result != null)
                        {
                            _logger.LogInformation("{LogMessage} 성공", logMessage);
                            return result;
                        }
                    }
                    catch { }

                    // BaseResponse 구조로 재시도
                    var wrappedResult = JsonSerializer.Deserialize<BaseResponse<TResponse>>(jsonContent, _jsonOptions);
                    if (wrappedResult?.Success == true && wrappedResult.Data != null)
                    {
                        _logger.LogInformation("{LogMessage} 성공", logMessage);
                        return wrappedResult.Data;
                    }
                }

                _logger.LogWarning("{LogMessage} 실패: {StatusCode}", logMessage, response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{LogMessage} 중 오류 발생", logMessage);
                return default;
            }
        }

        /// <summary>
        /// 공통 PUT 요청 헬퍼
        /// </summary>
        private async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, string logMessage)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    // 직접 객체 파싱 시도
                    try
                    {
                        var result = JsonSerializer.Deserialize<TResponse>(jsonContent, _jsonOptions);
                        if (result != null)
                        {
                            _logger.LogInformation("{LogMessage} 성공", logMessage);
                            return result;
                        }
                    }
                    catch { }

                    // BaseResponse 구조로 재시도
                    var wrappedResult = JsonSerializer.Deserialize<BaseResponse<TResponse>>(jsonContent, _jsonOptions);
                    if (wrappedResult?.Success == true && wrappedResult.Data != null)
                    {
                        _logger.LogInformation("{LogMessage} 성공", logMessage);
                        return wrappedResult.Data;
                    }
                }

                _logger.LogWarning("{LogMessage} 실패: {StatusCode}", logMessage, response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{LogMessage} 중 오류 발생", logMessage);
                return default;
            }
        }

        /// <summary>
        /// 공통 DELETE 요청 헬퍼
        /// </summary>
        private async Task<bool> DeleteAsync(string endpoint, string logMessage)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("{LogMessage} 성공", logMessage);
                    return true;
                }

                _logger.LogWarning("{LogMessage} 실패: {StatusCode}", logMessage, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{LogMessage} 중 오류 발생", logMessage);
                return false;
            }
        }

        /// <summary>
        /// 공통 PATCH 요청 헬퍼
        /// </summary>
        private async Task<bool> PatchAsync<T>(string endpoint, T? request, string logMessage)
        {
            try
            {
                HttpContent? content = null;
                if (request != null)
                {
                    var json = JsonSerializer.Serialize(request, _jsonOptions);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.PatchAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("{LogMessage} 성공", logMessage);
                    return true;
                }

                _logger.LogWarning("{LogMessage} 실패: {StatusCode}", logMessage, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{LogMessage} 중 오류 발생", logMessage);
                return false;
            }
        }

        /// <summary>
        /// 직접 배열 파싱 시도
        /// </summary>
        private async Task<List<T>?> TryParseDirectArray<T>(string jsonContent)
        {
            try
            {
                return JsonSerializer.Deserialize<List<T>>(jsonContent, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Wrapped Response 파싱 시도
        /// </summary>
        private async Task<T?> TryParseWrappedResponse<T>(string jsonContent)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 쿼리 파라미터 빌더
        /// </summary>
        private string BuildQueryParams(Dictionary<string, object?> parameters)
        {
            var queryParams = new List<string>();

            foreach (var param in parameters)
            {
                if (param.Value != null)
                {
                    var value = param.Value is string str ? Uri.EscapeDataString(str) : param.Value.ToString();
                    queryParams.Add($"{param.Key}={value}");
                }
            }

            return string.Join("&", queryParams);
        }

        /// <summary>
        /// 에러 응답 생성 헬퍼
        /// </summary>
        private T CreateErrorResponse<T>(HttpStatusCode statusCode, string defaultMessage) where T : BaseResponse, new()
        {
            return new T
            {
                Success = false,
                Message = GetErrorMessage(statusCode, defaultMessage)
            };
        }

        /// <summary>
        /// HTTP 상태 코드에 따른 오류 메시지 반환
        /// </summary>
        private string GetErrorMessage(HttpStatusCode statusCode, string defaultMessage)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "요청한 리소스를 찾을 수 없습니다.",
                HttpStatusCode.Unauthorized => "접근 권한이 없습니다.",
                HttpStatusCode.Forbidden => "이 작업을 수행할 권한이 없습니다.",
                HttpStatusCode.BadRequest => "잘못된 요청입니다.",
                HttpStatusCode.Conflict => "리소스 충돌이 발생했습니다.",
                HttpStatusCode.InternalServerError => "서버 내부 오류가 발생했습니다.",
                _ => defaultMessage
            };
        }

        #endregion
    }
}