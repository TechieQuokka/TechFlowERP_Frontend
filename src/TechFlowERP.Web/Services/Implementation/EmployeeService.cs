using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.Employee;
using TechFlowERP.Models.Enums;
using TechFlowERP.Models.Requests.Employee;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Responses.Employee;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 직원 관리 서비스 구현
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmployeeService> _logger;
        private readonly string _baseEndpoint = "employees";
        private readonly JsonSerializerOptions _jsonOptions;

        public EmployeeService(HttpClient httpClient, ILogger<EmployeeService> logger)
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
        /// 직원 목록 조회 (페이징)
        /// </summary>
        public async Task<EmployeeListResponse> GetEmployeesAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null, Guid? departmentId = null)
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

                if (departmentId.HasValue)
                    queryParams.Add($"departmentId={departmentId}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_baseEndpoint}?{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("백엔드 응답 JSON: {Json}", jsonContent);

                    try
                    {
                        // 직접 배열로 파싱 시도
                        var employees = JsonSerializer.Deserialize<List<EmployeeDto>>(jsonContent, _jsonOptions);

                        if (employees != null)
                        {
                            _logger.LogInformation("응답이 직접 배열 형태입니다. 직원 {Count}개 조회됨", employees.Count);

                            return new EmployeeListResponse
                            {
                                Success = true,
                                Message = "직원 목록 조회 성공",
                                Data = new PagedResult<EmployeeDto>
                                {
                                    Items = employees,
                                    TotalCount = employees.Count,
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
                            var wrappedResponse = JsonSerializer.Deserialize<EmployeeListResponse>(jsonContent, _jsonOptions);
                            if (wrappedResponse != null)
                            {
                                return wrappedResponse;
                            }
                        }
                        catch
                        {
                            // 무시하고 계속
                        }

                        return new EmployeeListResponse
                        {
                            Success = false,
                            Message = $"JSON 파싱 실패: {ex.Message}"
                        };
                    }
                }

                _logger.LogWarning("직원 목록 조회 실패: {StatusCode}", response.StatusCode);
                return new EmployeeListResponse
                {
                    Success = false,
                    Message = GetErrorMessage(response.StatusCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 목록 조회 중 오류 발생");
                return new EmployeeListResponse
                {
                    Success = false,
                    Message = "직원 목록 조회 중 오류가 발생했습니다."
                };
            }

            // 기본 실패 응답
            return new EmployeeListResponse
            {
                Success = false,
                Message = "알 수 없는 오류가 발생했습니다."
            };
        }

        /// <summary>
        /// 직원 상세 조회
        /// </summary>
        public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{employeeId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 객체로 파싱 시도
                        var employee = JsonSerializer.Deserialize<EmployeeDto>(jsonContent, _jsonOptions);
                        if (employee != null)
                        {
                            return employee;
                        }
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<EmployeeDto>>(jsonContent, _jsonOptions);
                        return result?.Data;
                    }
                }

                _logger.LogWarning("직원 상세 조회 실패: {EmployeeId}, {StatusCode}", employeeId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 상세 조회 중 오류 발생: {EmployeeId}", employeeId);
                return null;
            }
        }

        /// <summary>
        /// 직원 생성
        /// </summary>
        public async Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 객체로 파싱 시도
                        var employee = JsonSerializer.Deserialize<EmployeeDto>(jsonContent, _jsonOptions);
                        if (employee != null)
                        {
                            _logger.LogInformation("직원 생성 성공: {Name}", request.Name);
                            return employee;
                        }
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<EmployeeDto>>(jsonContent, _jsonOptions);
                        if (result?.Success == true && result.Data != null)
                        {
                            _logger.LogInformation("직원 생성 성공: {Name}", request.Name);
                            return result.Data;
                        }
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("직원 생성 실패: {StatusCode}, {Error}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 생성 중 오류 발생: {Name}", request.Name);
                return null;
            }
        }

        /// <summary>
        /// 직원 수정
        /// </summary>
        public async Task<EmployeeDto?> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{employeeId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 객체로 파싱 시도
                        var employee = JsonSerializer.Deserialize<EmployeeDto>(jsonContent, _jsonOptions);
                        if (employee != null)
                        {
                            _logger.LogInformation("직원 수정 성공: {EmployeeId}", employeeId);
                            return employee;
                        }
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<EmployeeDto>>(jsonContent, _jsonOptions);
                        if (result?.Success == true && result.Data != null)
                        {
                            _logger.LogInformation("직원 수정 성공: {EmployeeId}", employeeId);
                            return result.Data;
                        }
                    }
                }

                _logger.LogWarning("직원 수정 실패: {EmployeeId}, {StatusCode}", employeeId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 수정 중 오류 발생: {EmployeeId}", employeeId);
                return null;
            }
        }

        /// <summary>
        /// 직원 삭제 (비활성화)
        /// </summary>
        public async Task<bool> DeleteEmployeeAsync(Guid employeeId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{employeeId}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("직원 삭제 성공: {EmployeeId}", employeeId);
                    return true;
                }

                _logger.LogWarning("직원 삭제 실패: {EmployeeId}, {StatusCode}", employeeId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 삭제 중 오류 발생: {EmployeeId}", employeeId);
                return false;
            }
        }

        /// <summary>
        /// 직원 검색
        /// </summary>
        public async Task<List<EmployeeDto>> SearchEmployeesAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 배열로 파싱 시도
                        var employees = JsonSerializer.Deserialize<List<EmployeeDto>>(jsonContent, _jsonOptions);
                        return employees ?? new List<EmployeeDto>();
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<List<EmployeeDto>>>(jsonContent, _jsonOptions);
                        return result?.Data ?? new List<EmployeeDto>();
                    }
                }

                return new List<EmployeeDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 검색 중 오류 발생: {SearchTerm}", searchTerm);
                return new List<EmployeeDto>();
            }
        }

        /// <summary>
        /// 활성 직원 목록 조회
        /// </summary>
        public async Task<List<EmployeeDto>> GetActiveEmployeesAsync()
        {
            return await GetListAsync<EmployeeDto>($"{_baseEndpoint}/active");
        }

        /// <summary>
        /// 직원 기술 목록 조회
        /// </summary>
        public async Task<List<EmployeeSkillDto>> GetEmployeeSkillsAsync(Guid employeeId)
        {
            return await GetListAsync<EmployeeSkillDto>($"{_baseEndpoint}/{employeeId}/skills");
        }

        /// <summary>
        /// 직원 기술 추가
        /// </summary>
        public async Task<EmployeeSkillDto?> AddEmployeeSkillAsync(AddEmployeeSkillRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseEndpoint}/{request.EmployeeId}/skills", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 객체로 파싱 시도
                        var skill = JsonSerializer.Deserialize<EmployeeSkillDto>(jsonContent, _jsonOptions);
                        return skill;
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<EmployeeSkillDto>>(jsonContent, _jsonOptions);
                        return result?.Data;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 기술 추가 중 오류 발생: {EmployeeId}, {Technology}", request.EmployeeId, request.Technology);
                return null;
            }
        }

        /// <summary>
        /// 직원 기술 삭제
        /// </summary>
        public async Task<bool> RemoveEmployeeSkillAsync(Guid skillId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/skills/{skillId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 기술 삭제 중 오류 발생: {SkillId}", skillId);
                return false;
            }
        }

        /// <summary>
        /// 부서별 직원 목록 조회
        /// </summary>
        public async Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(Guid departmentId)
        {
            return await GetListAsync<EmployeeDto>($"{_baseEndpoint}/department/{departmentId}");
        }

        /// <summary>
        /// 매니저별 팀원 목록 조회
        /// </summary>
        public async Task<List<EmployeeDto>> GetTeamMembersAsync(Guid managerId)
        {
            return await GetListAsync<EmployeeDto>($"{_baseEndpoint}/manager/{managerId}/team");
        }

        /// <summary>
        /// 기술별 직원 목록 조회
        /// </summary>
        public async Task<List<EmployeeDto>> GetEmployeesBySkillAsync(string technology, SkillLevel? minLevel = null)
        {
            try
            {
                var query = $"technology={Uri.EscapeDataString(technology)}";
                if (minLevel.HasValue)
                    query += $"&minLevel={minLevel}";

                return await GetListAsync<EmployeeDto>($"{_baseEndpoint}/skill?{query}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "기술별 직원 목록 조회 중 오류 발생: {Technology}", technology);
                return new List<EmployeeDto>();
            }
        }

        /// <summary>
        /// 직원 통계 조회
        /// </summary>
        public async Task<EmployeeStatisticsResponse> GetEmployeeStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/statistics");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 객체로 파싱 시도
                        var stats = JsonSerializer.Deserialize<EmployeeStatisticsResponse>(jsonContent, _jsonOptions);
                        return stats ?? new EmployeeStatisticsResponse
                        {
                            Success = false,
                            Message = "통계 데이터 파싱 실패"
                        };
                    }
                    catch
                    {
                        // 다른 구조로 재시도 (필요시)
                        return new EmployeeStatisticsResponse
                        {
                            Success = false,
                            Message = "통계 데이터 파싱 실패"
                        };
                    }
                }

                return new EmployeeStatisticsResponse
                {
                    Success = false,
                    Message = GetErrorMessage(response.StatusCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 통계 조회 중 오류 발생");
                return new EmployeeStatisticsResponse
                {
                    Success = false,
                    Message = "직원 통계 조회 중 오류가 발생했습니다."
                };
            }
        }

        /// <summary>
        /// 직원 활용도 조회
        /// </summary>
        public async Task<Dictionary<Guid, decimal>> GetEmployeeUtilizationAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var query = $"startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/utilization?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 딕셔너리로 파싱 시도
                        var utilization = JsonSerializer.Deserialize<Dictionary<Guid, decimal>>(jsonContent, _jsonOptions);
                        return utilization ?? new Dictionary<Guid, decimal>();
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<Dictionary<Guid, decimal>>>(jsonContent, _jsonOptions);
                        return result?.Data ?? new Dictionary<Guid, decimal>();
                    }
                }

                return new Dictionary<Guid, decimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 활용도 조회 중 오류 발생");
                return new Dictionary<Guid, decimal>();
            }
        }

        /// <summary>
        /// HTTP 상태 코드에 따른 오류 메시지 반환
        /// </summary>
        private static string GetErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "요청한 직원을 찾을 수 없습니다.",
                HttpStatusCode.Unauthorized => "접근 권한이 없습니다.",
                HttpStatusCode.Forbidden => "이 작업을 수행할 권한이 없습니다.",
                HttpStatusCode.BadRequest => "잘못된 요청입니다.",
                HttpStatusCode.Conflict => "이미 존재하는 이메일입니다.",
                HttpStatusCode.InternalServerError => "서버 내부 오류가 발생했습니다.",
                _ => "알 수 없는 오류가 발생했습니다."
            };
        }

        /// <summary>
        /// 리스트 조회를 위한 헬퍼 메서드
        /// </summary>
        private async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 배열로 파싱 시도
                        var items = JsonSerializer.Deserialize<List<T>>(jsonContent, _jsonOptions);
                        return items ?? new List<T>();
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<List<T>>>(jsonContent, _jsonOptions);
                        return result?.Data ?? new List<T>();
                    }
                }
                return new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }
    }
}