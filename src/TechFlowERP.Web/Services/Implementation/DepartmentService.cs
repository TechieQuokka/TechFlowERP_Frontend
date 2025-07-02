using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechFlowERP.Models.DTOs.Department;
using TechFlowERP.Models.Requests.Department;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Responses.Department;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 부서 관리 서비스 구현
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DepartmentService> _logger;
        private readonly string _baseEndpoint = "departments";
        private readonly JsonSerializerOptions _jsonOptions;

        public DepartmentService(HttpClient httpClient, ILogger<DepartmentService> logger)
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
        /// 모든 부서 목록 조회
        /// </summary>
        public async Task<List<DepartmentDto>> GetAllDepartmentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<DepartmentDto>>>();
                    return result?.Data ?? new List<DepartmentDto>();
                }

                _logger.LogWarning("부서 목록 조회 실패: {StatusCode}", response.StatusCode);
                return GetFallbackDepartments();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 목록 조회 중 오류 발생");
                return GetFallbackDepartments();
            }
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseEndpoint);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("부서 API가 아직 구현되지 않음 (404)");
                    return new List<DepartmentDto>();
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 배열로 파싱 시도
                        var departments = JsonSerializer.Deserialize<List<DepartmentDto>>(jsonContent, _jsonOptions);
                        return departments ?? new List<DepartmentDto>();
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<List<DepartmentDto>>>(jsonContent, _jsonOptions);
                        return result?.Data ?? new List<DepartmentDto>();
                    }
                }

                _logger.LogWarning("부서 목록 조회 실패: {StatusCode}", response.StatusCode);
                return new List<DepartmentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 목록 조회 중 오류 발생");
                return new List<DepartmentDto>();
            }
        }

        /// <summary>
        /// 부서 상세 조회
        /// </summary>
        public async Task<DepartmentDto?> GetDepartmentByIdAsync(Guid departmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{departmentId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<DepartmentDto>>();
                    return result?.Data;
                }

                _logger.LogWarning("부서 상세 조회 실패: {DepartmentId}, {StatusCode}", departmentId, response.StatusCode);
                return GetFallbackDepartments().FirstOrDefault(d => d.Id == departmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 상세 조회 중 오류 발생: {DepartmentId}", departmentId);
                return GetFallbackDepartments().FirstOrDefault(d => d.Id == departmentId);
            }
        }

        /// <summary>
        /// 부서 생성
        /// </summary>
        public async Task<DepartmentDto?> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<DepartmentDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("부서 생성 성공: {Name}", request.Name);
                        return result.Data;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("부서 생성 실패: {StatusCode}, {Error}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 생성 중 오류 발생: {Name}", request.Name);
                return null;
            }
        }

        /// <summary>
        /// 부서 수정
        /// </summary>
        public async Task<DepartmentDto?> UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{departmentId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<DepartmentDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("부서 수정 성공: {DepartmentId}", departmentId);
                        return result.Data;
                    }
                }

                _logger.LogWarning("부서 수정 실패: {DepartmentId}, {StatusCode}", departmentId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 수정 중 오류 발생: {DepartmentId}", departmentId);
                return null;
            }
        }

        /// <summary>
        /// 부서 삭제
        /// </summary>
        public async Task<bool> DeleteDepartmentAsync(Guid departmentId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{departmentId}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("부서 삭제 성공: {DepartmentId}", departmentId);
                    return true;
                }

                _logger.LogWarning("부서 삭제 실패: {DepartmentId}, {StatusCode}", departmentId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 삭제 중 오류 발생: {DepartmentId}", departmentId);
                return false;
            }
        }

        /// <summary>
        /// 부서명으로 검색
        /// </summary>
        public async Task<List<DepartmentDto>> SearchDepartmentsAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<DepartmentDto>>>();
                    return result?.Data ?? new List<DepartmentDto>();
                }

                return GetFallbackDepartments()
                    .Where(d => d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 검색 중 오류 발생: {SearchTerm}", searchTerm);
                return GetFallbackDepartments()
                    .Where(d => d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        /// <summary>
        /// 활성 부서 목록 조회
        /// </summary>
        public async Task<List<DepartmentDto>> GetActiveDepartmentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/active");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<DepartmentDto>>>();
                    return result?.Data ?? new List<DepartmentDto>();
                }

                return GetFallbackDepartments();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "활성 부서 목록 조회 중 오류 발생");
                return GetFallbackDepartments();
            }
        }

        /// <summary>
        /// 매니저가 없는 부서 목록 조회
        /// </summary>
        public async Task<List<DepartmentDto>> GetDepartmentsWithoutManagerAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/without-manager");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<DepartmentDto>>>();
                    return result?.Data ?? new List<DepartmentDto>();
                }

                return GetFallbackDepartments().Where(d => d.ManagerId == null).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "매니저 없는 부서 목록 조회 중 오류 발생");
                return GetFallbackDepartments().Where(d => d.ManagerId == null).ToList();
            }
        }

        /// <summary>
        /// 부서 통계 조회
        /// </summary>
        public async Task<DepartmentStatisticsResponse> GetDepartmentStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/statistics");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DepartmentStatisticsResponse>();
                    return result ?? GetFallbackStatistics();
                }

                return GetFallbackStatistics();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 통계 조회 중 오류 발생");
                return GetFallbackStatistics();
            }
        }

        /// <summary>
        /// 부서별 예산 사용률 조회
        /// </summary>
        public async Task<Dictionary<Guid, decimal>> GetDepartmentBudgetUtilizationAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/budget-utilization");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<Dictionary<Guid, decimal>>>();
                    return result?.Data ?? new Dictionary<Guid, decimal>();
                }

                return new Dictionary<Guid, decimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서별 예산 사용률 조회 중 오류 발생");
                return new Dictionary<Guid, decimal>();
            }
        }

        /// <summary>
        /// 폴백 부서 데이터 (백엔드 연결 실패 시 사용)
        /// </summary>
        private static List<DepartmentDto> GetFallbackDepartments()
        {
            return new List<DepartmentDto>
            {
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "개발팀",
                    ManagerId = null,
                    ManagerName = "",
                    Budget = 50000000,
                    UsedBudget = 42000000,
                    EmployeeCount = 12,
                    TenantId = "default-tenant",
                    CreatedAt = DateTime.Now.AddYears(-1),
                    UpdatedAt = DateTime.Now
                },
                new()
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "디자인팀",
                    ManagerId = null,
                    ManagerName = "",
                    Budget = 30000000,
                    UsedBudget = 18000000,
                    EmployeeCount = 6,
                    TenantId = "default-tenant",
                    CreatedAt = DateTime.Now.AddYears(-1),
                    UpdatedAt = DateTime.Now
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "마케팅팀",
                    ManagerId = null,
                    ManagerName = "",
                    Budget = 25000000,
                    UsedBudget = 14000000,
                    EmployeeCount = 4,
                    TenantId = "default-tenant",
                    CreatedAt = DateTime.Now.AddYears(-1),
                    UpdatedAt = DateTime.Now
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "인사총무팀",
                    ManagerId = null,
                    ManagerName = "",
                    Budget = 20000000,
                    UsedBudget = 12000000,
                    EmployeeCount = 3,
                    TenantId = "default-tenant",
                    CreatedAt = DateTime.Now.AddYears(-1),
                    UpdatedAt = DateTime.Now
                }
            };
        }

        /// <summary>
        /// 폴백 통계 데이터
        /// </summary>
        private static DepartmentStatisticsResponse GetFallbackStatistics()
        {
            return new DepartmentStatisticsResponse
            {
                Success = true,
                Message = "폴백 데이터",
                TotalDepartments = 4,
                DepartmentsWithManager = 0,
                DepartmentsWithoutManager = 4,
                TotalBudget = 125000000,
                AverageBudgetPerDepartment = 31250000,
                TotalEmployeeSalary = 86000000,
                AverageBudgetUtilization = 65.0m,
                TotalEmployees = 25,
                AverageEmployeesPerDepartment = 6.25m
            };
        }

        /// <summary>
        /// HTTP 상태 코드에 따른 오류 메시지 반환
        /// </summary>
        private static string GetErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "요청한 부서를 찾을 수 없습니다.",
                HttpStatusCode.Unauthorized => "접근 권한이 없습니다.",
                HttpStatusCode.Forbidden => "이 작업을 수행할 권한이 없습니다.",
                HttpStatusCode.BadRequest => "잘못된 요청입니다.",
                HttpStatusCode.Conflict => "이미 존재하는 부서명입니다.",
                HttpStatusCode.InternalServerError => "서버 내부 오류가 발생했습니다.",
                _ => "알 수 없는 오류가 발생했습니다."
            };
        }
    }
}