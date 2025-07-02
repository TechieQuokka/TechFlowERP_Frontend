using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.TimeEntry;
using TechFlowERP.Models.Requests.TimeEntry;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Responses.TimeEntry;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 시간 기록 관리 서비스 구현
    /// </summary>
    public class TimeEntryService : ITimeEntryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TimeEntryService> _logger;
        private readonly string _baseEndpoint = "timeentries";
        private readonly JsonSerializerOptions _jsonOptions;

        public TimeEntryService(HttpClient httpClient, ILogger<TimeEntryService> logger)
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
        /// 시간 기록 목록 조회 (페이징)
        /// </summary>
        public async Task<TimeEntryListResponse> GetTimeEntriesAsync(
    int pageNumber = 1,
    int pageSize = 20,
    Guid? employeeId = null,
    Guid? projectId = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    bool? billable = null,
    bool? approved = null)
        {
            try
            {
                var queryParams = BuildQueryParams(new Dictionary<string, object?>
                {
                    ["pageNumber"] = pageNumber,
                    ["pageSize"] = pageSize,
                    ["employeeId"] = employeeId,
                    ["projectId"] = projectId,
                    ["startDate"] = startDate?.ToString("yyyy-MM-dd"),
                    ["endDate"] = endDate?.ToString("yyyy-MM-dd"),
                    ["billable"] = billable,
                    ["approved"] = approved
                });

                var response = await _httpClient.GetAsync($"{_baseEndpoint}?{queryParams}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("시간 기록 목록 조회 성공. 응답: {Json}", jsonContent);

                    try
                    {
                        // 직접 배열로 파싱 시도
                        var timeEntries = JsonSerializer.Deserialize<List<TimeEntryDto>>(jsonContent, _jsonOptions);

                        if (timeEntries != null)
                        {
                            _logger.LogInformation("응답이 직접 배열 형태입니다. 시간 기록 {Count}개 조회됨", timeEntries.Count);

                            return new TimeEntryListResponse
                            {
                                Success = true,
                                Message = "시간 기록 목록 조회 성공",
                                Data = new PagedResult<TimeEntryDto>
                                {
                                    Items = timeEntries,
                                    TotalCount = timeEntries.Count,
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
                            var wrappedResponse = JsonSerializer.Deserialize<TimeEntryListResponse>(jsonContent, _jsonOptions);
                            if (wrappedResponse != null)
                            {
                                return wrappedResponse;
                            }
                        }
                        catch
                        {
                            // 무시하고 계속
                        }

                        return new TimeEntryListResponse
                        {
                            Success = false,
                            Message = $"JSON 파싱 실패: {ex.Message}"
                        };
                    }
                }

                _logger.LogWarning("시간 기록 목록 조회 실패: {StatusCode}", response.StatusCode);
                return new TimeEntryListResponse
                {
                    Success = false,
                    Message = GetErrorMessage(response.StatusCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 목록 조회 중 오류 발생");
                return new TimeEntryListResponse
                {
                    Success = false,
                    Message = "시간 기록 목록 조회 중 오류가 발생했습니다."
                };
            }
        }

        /// <summary>
        /// 시간 기록 상세 조회
        /// </summary>
        public async Task<TimeEntryDto?> GetTimeEntryByIdAsync(Guid timeEntryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{timeEntryId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<TimeEntryDto>>();
                    return result?.Data;
                }

                _logger.LogWarning("시간 기록 상세 조회 실패: {TimeEntryId}, {StatusCode}", timeEntryId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 상세 조회 중 오류 발생: {TimeEntryId}", timeEntryId);
                return null;
            }
        }

        /// <summary>
        /// 시간 기록 생성
        /// </summary>
        public async Task<TimeEntryDto?> CreateTimeEntryAsync(CreateTimeEntryRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<TimeEntryDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("시간 기록 생성 성공: {EmployeeId}, {Hours}시간", request.EmployeeId, request.Hours);
                        return result.Data;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("시간 기록 생성 실패: {StatusCode}, {Error}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 생성 중 오류 발생: {EmployeeId}", request.EmployeeId);
                return null;
            }
        }

        /// <summary>
        /// 시간 기록 수정
        /// </summary>
        public async Task<TimeEntryDto?> UpdateTimeEntryAsync(Guid timeEntryId, UpdateTimeEntryRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{timeEntryId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<TimeEntryDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("시간 기록 수정 성공: {TimeEntryId}", timeEntryId);
                        return result.Data;
                    }
                }

                _logger.LogWarning("시간 기록 수정 실패: {TimeEntryId}, {StatusCode}", timeEntryId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 수정 중 오류 발생: {TimeEntryId}", timeEntryId);
                return null;
            }
        }

        /// <summary>
        /// 시간 기록 삭제
        /// </summary>
        public async Task<bool> DeleteTimeEntryAsync(Guid timeEntryId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{timeEntryId}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("시간 기록 삭제 성공: {TimeEntryId}", timeEntryId);
                    return true;
                }

                _logger.LogWarning("시간 기록 삭제 실패: {TimeEntryId}, {StatusCode}", timeEntryId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 삭제 중 오류 발생: {TimeEntryId}", timeEntryId);
                return false;
            }
        }

        /// <summary>
        /// 시간 기록 일괄 승인
        /// </summary>
        public async Task<bool> ApproveTimeEntriesAsync(ApproveTimeEntryRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseEndpoint}/approve", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("시간 기록 일괄 승인 성공: {Count}건", request.TimeEntryIds.Count);
                    return true;
                }

                _logger.LogWarning("시간 기록 일괄 승인 실패: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 일괄 승인 중 오류 발생");
                return false;
            }
        }

        /// <summary>
        /// 시간 기록 일괄 거부
        /// </summary>
        public async Task<bool> RejectTimeEntriesAsync(RejectTimeEntryRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseEndpoint}/reject", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("시간 기록 일괄 거부 성공: {Count}건", request.TimeEntryIds.Count);
                    return true;
                }

                _logger.LogWarning("시간 기록 일괄 거부 실패: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 일괄 거부 중 오류 발생");
                return false;
            }
        }

        /// <summary>
        /// 직원별 시간 기록 조회
        /// </summary>
        public async Task<List<TimeEntryDto>> GetTimeEntriesByEmployeeAsync(
            Guid employeeId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var queryParams = new List<string> { $"employeeId={employeeId}" };

                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate:yyyy-MM-dd}");

                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate:yyyy-MM-dd}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/employee?{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<TimeEntryDto>>>();
                    return result?.Data ?? new List<TimeEntryDto>();
                }

                return new List<TimeEntryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원별 시간 기록 조회 중 오류 발생: {EmployeeId}", employeeId);
                return new List<TimeEntryDto>();
            }
        }

        /// <summary>
        /// 프로젝트별 시간 기록 조회
        /// </summary>
        public async Task<List<TimeEntryDto>> GetTimeEntriesByProjectAsync(
            Guid projectId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var queryParams = new List<string> { $"projectId={projectId}" };

                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate:yyyy-MM-dd}");

                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate:yyyy-MM-dd}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/project?{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<TimeEntryDto>>>();
                    return result?.Data ?? new List<TimeEntryDto>();
                }

                return new List<TimeEntryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "프로젝트별 시간 기록 조회 중 오류 발생: {ProjectId}", projectId);
                return new List<TimeEntryDto>();
            }
        }

        /// <summary>
        /// 승인 대기 중인 시간 기록 조회
        /// </summary>
        public async Task<List<TimeEntryDto>> GetPendingTimeEntriesAsync(Guid? managerId = null)
        {
            try
            {
                var query = managerId.HasValue ? $"managerId={managerId}" : "";
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/pending?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<TimeEntryDto>>>();
                    return result?.Data ?? new List<TimeEntryDto>();
                }

                return new List<TimeEntryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "승인 대기 중인 시간 기록 조회 중 오류 발생");
                return new List<TimeEntryDto>();
            }
        }

        /// <summary>
        /// 오늘 시간 기록 조회
        /// </summary>
        public async Task<List<TimeEntryDto>> GetTodayTimeEntriesAsync(Guid employeeId)
        {
            try
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/today?employeeId={employeeId}&date={today}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<TimeEntryDto>>>();
                    return result?.Data ?? new List<TimeEntryDto>();
                }

                return new List<TimeEntryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "오늘 시간 기록 조회 중 오류 발생: {EmployeeId}", employeeId);
                return new List<TimeEntryDto>();
            }
        }

        /// <summary>
        /// 주간 타임시트 조회
        /// </summary>
        public async Task<TimesheetResponse> GetWeeklyTimesheetAsync(Guid employeeId, DateTime weekStartDate)
        {
            try
            {
                var query = $"employeeId={employeeId}&weekStartDate={weekStartDate:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/timesheet/weekly?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TimesheetResponse>();
                    return result ?? new TimesheetResponse
                    {
                        Success = false,
                        Message = "타임시트 데이터 파싱 실패"
                    };
                }

                return new TimesheetResponse
                {
                    Success = false,
                    Message = GetErrorMessage(response.StatusCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "주간 타임시트 조회 중 오류 발생: {EmployeeId}", employeeId);
                return new TimesheetResponse
                {
                    Success = false,
                    Message = "주간 타임시트 조회 중 오류가 발생했습니다."
                };
            }
        }

        /// <summary>
        /// 월간 타임시트 조회
        /// </summary>
        public async Task<TimesheetResponse> GetMonthlyTimesheetAsync(Guid employeeId, int year, int month)
        {
            try
            {
                var query = $"employeeId={employeeId}&year={year}&month={month}";
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/timesheet/monthly?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TimesheetResponse>();
                    return result ?? new TimesheetResponse
                    {
                        Success = false,
                        Message = "타임시트 데이터 파싱 실패"
                    };
                }

                return new TimesheetResponse
                {
                    Success = false,
                    Message = GetErrorMessage(response.StatusCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "월간 타임시트 조회 중 오류 발생: {EmployeeId}", employeeId);
                return new TimesheetResponse
                {
                    Success = false,
                    Message = "월간 타임시트 조회 중 오류가 발생했습니다."
                };
            }
        }

        /// <summary>
        /// 시간 기록 통계 조회
        /// </summary>
        public async Task<TimeEntryStatisticsResponse> GetTimeEntryStatisticsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            Guid? employeeId = null,
            Guid? projectId = null)
        {
            try
            {
                var queryParams = new List<string>();

                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate:yyyy-MM-dd}");

                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate:yyyy-MM-dd}");

                if (employeeId.HasValue)
                    queryParams.Add($"employeeId={employeeId}");

                if (projectId.HasValue)
                    queryParams.Add($"projectId={projectId}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/statistics{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TimeEntryStatisticsResponse>();
                    return result ?? new TimeEntryStatisticsResponse
                    {
                        Success = false,
                        Message = "통계 데이터 파싱 실패"
                    };
                }

                return new TimeEntryStatisticsResponse
                {
                    Success = false,
                    Message = GetErrorMessage(response.StatusCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 통계 조회 중 오류 발생");
                return new TimeEntryStatisticsResponse
                {
                    Success = false,
                    Message = "시간 기록 통계 조회 중 오류가 발생했습니다."
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
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/utilization/employee?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<Dictionary<Guid, decimal>>>();
                    return result?.Data ?? new Dictionary<Guid, decimal>();
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
        /// 프로젝트 시간 사용 현황 조회
        /// </summary>
        public async Task<Dictionary<Guid, decimal>> GetProjectTimeUsageAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var query = $"startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/utilization/project?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<Dictionary<Guid, decimal>>>();
                    return result?.Data ?? new Dictionary<Guid, decimal>();
                }

                return new Dictionary<Guid, decimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "프로젝트 시간 사용 현황 조회 중 오류 발생");
                return new Dictionary<Guid, decimal>();
            }
        }

        /// <summary>
        /// 시간 기록 복사 (반복 작업용)
        /// </summary>
        public async Task<List<TimeEntryDto>> CopyTimeEntriesAsync(
            Guid sourceEmployeeId,
            DateTime sourceDate,
            DateTime targetDate,
            List<Guid>? projectIds = null)
        {
            try
            {
                var request = new
                {
                    SourceEmployeeId = sourceEmployeeId,
                    SourceDate = sourceDate,
                    TargetDate = targetDate,
                    ProjectIds = projectIds
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseEndpoint}/copy", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<TimeEntryDto>>>();
                    return result?.Data ?? new List<TimeEntryDto>();
                }

                return new List<TimeEntryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 복사 중 오류 발생: {EmployeeId}", sourceEmployeeId);
                return new List<TimeEntryDto>();
            }
        }

        /// <summary>
        /// 시간 기록 템플릿 생성
        /// </summary>
        public async Task<bool> CreateTimeEntryTemplateAsync(Guid employeeId, string templateName, List<CreateTimeEntryRequest> entries)
        {
            try
            {
                var request = new
                {
                    EmployeeId = employeeId,
                    TemplateName = templateName,
                    Entries = entries
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseEndpoint}/template", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("시간 기록 템플릿 생성 성공: {TemplateName}", templateName);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 템플릿 생성 중 오류 발생: {TemplateName}", templateName);
                return false;
            }
        }

        /// <summary>
        /// 시간 기록 내보내기 (CSV/Excel)
        /// </summary>
        public async Task<byte[]> ExportTimeEntriesAsync(
            DateTime startDate,
            DateTime endDate,
            Guid? employeeId = null,
            Guid? projectId = null,
            string format = "csv")
        {
            try
            {
                var queryParams = new List<string>
               {
                   $"startDate={startDate:yyyy-MM-dd}",
                   $"endDate={endDate:yyyy-MM-dd}",
                   $"format={format}"
               };

                if (employeeId.HasValue)
                    queryParams.Add($"employeeId={employeeId}");

                if (projectId.HasValue)
                    queryParams.Add($"projectId={projectId}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/export?{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    _logger.LogInformation("시간 기록 내보내기 성공: {Format}, {Size}bytes", format, fileBytes.Length);
                    return fileBytes;
                }

                _logger.LogWarning("시간 기록 내보내기 실패: {StatusCode}", response.StatusCode);
                return Array.Empty<byte>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "시간 기록 내보내기 중 오류 발생");
                return Array.Empty<byte>();
            }
        }

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

        private string GetErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "요청한 리소스를 찾을 수 없습니다.",
                HttpStatusCode.Unauthorized => "접근 권한이 없습니다.",
                HttpStatusCode.Forbidden => "이 작업을 수행할 권한이 없습니다.",
                HttpStatusCode.BadRequest => "잘못된 요청입니다.",
                HttpStatusCode.InternalServerError => "서버 내부 오류가 발생했습니다.",
                _ => "알 수 없는 오류가 발생했습니다."
            };
        }
    }
}