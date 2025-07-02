using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechFlowERP.Models.DTOs.Position;
using TechFlowERP.Models.Requests.Position;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Responses.Position;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 직급 관리 서비스 구현
    /// </summary>
    public class PositionService : IPositionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PositionService> _logger;
        private readonly string _baseEndpoint = "positions";
        private readonly JsonSerializerOptions _jsonOptions;

        public PositionService(HttpClient httpClient, ILogger<PositionService> logger)
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
        /// 모든 직급 목록 조회
        /// </summary>
        public async Task<List<PositionDto>> GetAllPositionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<PositionDto>>>();
                    return result?.Data ?? new List<PositionDto>();
                }

                _logger.LogWarning("직급 목록 조회 실패: {StatusCode}", response.StatusCode);
                return GetFallbackPositions();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 목록 조회 중 오류 발생");
                return GetFallbackPositions();
            }
        }

        public async Task<List<PositionDto>> GetPositionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseEndpoint);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("직급 API가 아직 구현되지 않음 (404)");
                    return new List<PositionDto>();
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // 직접 배열로 파싱 시도
                        var positions = JsonSerializer.Deserialize<List<PositionDto>>(jsonContent, _jsonOptions);
                        return positions ?? new List<PositionDto>();
                    }
                    catch
                    {
                        // BaseResponse 구조로 재시도
                        var result = JsonSerializer.Deserialize<BaseResponse<List<PositionDto>>>(jsonContent, _jsonOptions);
                        return result?.Data ?? new List<PositionDto>();
                    }
                }

                _logger.LogWarning("직급 목록 조회 실패: {StatusCode}", response.StatusCode);
                return new List<PositionDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 목록 조회 중 오류 발생");
                return new List<PositionDto>();
            }
        }

        /// <summary>
        /// 직급 상세 조회
        /// </summary>
        public async Task<PositionDto?> GetPositionByIdAsync(Guid positionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{positionId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<PositionDto>>();
                    return result?.Data;
                }

                _logger.LogWarning("직급 상세 조회 실패: {PositionId}, {StatusCode}", positionId, response.StatusCode);
                return GetFallbackPositions().FirstOrDefault(p => p.Id == positionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 상세 조회 중 오류 발생: {PositionId}", positionId);
                return GetFallbackPositions().FirstOrDefault(p => p.Id == positionId);
            }
        }

        /// <summary>
        /// 직급 생성
        /// </summary>
        public async Task<PositionDto?> CreatePositionAsync(CreatePositionRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<PositionDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("직급 생성 성공: {Title}", request.Title);
                        return result.Data;
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("직급 생성 실패: {StatusCode}, {Error}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 생성 중 오류 발생: {Title}", request.Title);
                return null;
            }
        }

        /// <summary>
        /// 직급 수정
        /// </summary>
        public async Task<PositionDto?> UpdatePositionAsync(Guid positionId, UpdatePositionRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{positionId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<PositionDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        _logger.LogInformation("직급 수정 성공: {PositionId}", positionId);
                        return result.Data;
                    }
                }

                _logger.LogWarning("직급 수정 실패: {PositionId}, {StatusCode}", positionId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 수정 중 오류 발생: {PositionId}", positionId);
                return null;
            }
        }

        /// <summary>
        /// 직급 삭제
        /// </summary>
        public async Task<bool> DeletePositionAsync(Guid positionId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{positionId}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("직급 삭제 성공: {PositionId}", positionId);
                    return true;
                }

                _logger.LogWarning("직급 삭제 실패: {PositionId}, {StatusCode}", positionId, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 삭제 중 오류 발생: {PositionId}", positionId);
                return false;
            }
        }

        /// <summary>
        /// 직급명으로 검색
        /// </summary>
        public async Task<List<PositionDto>> SearchPositionsAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<PositionDto>>>();
                    return result?.Data ?? new List<PositionDto>();
                }

                return GetFallbackPositions()
                    .Where(p => p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 검색 중 오류 발생: {SearchTerm}", searchTerm);
                return GetFallbackPositions()
                    .Where(p => p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        /// <summary>
        /// 활성 직급 목록 조회
        /// </summary>
        public async Task<List<PositionDto>> GetActivePositionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/active");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<PositionDto>>>();
                    return result?.Data ?? new List<PositionDto>();
                }

                return GetFallbackPositions();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "활성 직급 목록 조회 중 오류 발생");
                return GetFallbackPositions();
            }
        }

        /// <summary>
        /// 레벨별 직급 목록 조회
        /// </summary>
        public async Task<List<PositionDto>> GetPositionsByLevelAsync(int level)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/level/{level}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<PositionDto>>>();
                    return result?.Data ?? new List<PositionDto>();
                }

                return GetFallbackPositions().Where(p => p.Level == level).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "레벨별 직급 목록 조회 중 오류 발생: {Level}", level);
                return GetFallbackPositions().Where(p => p.Level == level).ToList();
            }
        }

        /// <summary>
        /// 관리직 직급 목록 조회
        /// </summary>
        public async Task<List<PositionDto>> GetManagementPositionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/management");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<PositionDto>>>();
                    return result?.Data ?? new List<PositionDto>();
                }

                return GetFallbackPositions().Where(p => p.IsManagementPosition).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "관리직 직급 목록 조회 중 오류 발생");
                return GetFallbackPositions().Where(p => p.IsManagementPosition).ToList();
            }
        }

        /// <summary>
        /// 급여 범위로 직급 검색
        /// </summary>
        public async Task<List<PositionDto>> GetPositionsBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/salary-range?min={minSalary}&max={maxSalary}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<PositionDto>>>();
                    return result?.Data ?? new List<PositionDto>();
                }

                return GetFallbackPositions()
                    .Where(p => p.MinSalary >= minSalary && p.MaxSalary <= maxSalary)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "급여 범위별 직급 검색 중 오류 발생: {MinSalary}-{MaxSalary}", minSalary, maxSalary);
                return GetFallbackPositions()
                    .Where(p => p.MinSalary >= minSalary && p.MaxSalary <= maxSalary)
                    .ToList();
            }
        }

        /// <summary>
        /// 직급 통계 조회
        /// </summary>
        public async Task<PositionStatisticsResponse> GetPositionStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/statistics");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PositionStatisticsResponse>();
                    return result ?? GetFallbackStatistics();
                }

                return GetFallbackStatistics();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급 통계 조회 중 오류 발생");
                return GetFallbackStatistics();
            }
        }

        /// <summary>
        /// 직급별 평균 급여 조회
        /// </summary>
        public async Task<Dictionary<Guid, decimal>> GetPositionAverageSalaryAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/average-salary");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BaseResponse<Dictionary<Guid, decimal>>>();
                    return result?.Data ?? new Dictionary<Guid, decimal>();
                }

                return new Dictionary<Guid, decimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직급별 평균 급여 조회 중 오류 발생");
                return new Dictionary<Guid, decimal>();
            }
        }

        /// <summary>
        /// 폴백 직급 데이터 (백엔드 연결 실패 시 사용)
        /// </summary>
        private static List<PositionDto> GetFallbackPositions()
        {
            return new List<PositionDto>
            {
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Title = "신입 개발자",
                    Level = 1,
                    MinSalary = 2800000,
                    MaxSalary = 3500000,
                    EmployeeCount = 8,
                    AverageSalary = 3200000,
                    IsManagementPosition = false,
                    CreatedAt = DateTime.Now.AddYears(-1)
                },
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Title = "주니어 개발자",
                    Level = 2,
                    MinSalary = 3500000,
                    MaxSalary = 4500000,
                    EmployeeCount = 6,
                    AverageSalary = 4000000,
                    IsManagementPosition = false,
                    CreatedAt = DateTime.Now.AddYears(-1)
                },
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Title = "시니어 개발자",
                    Level = 3,
                    MinSalary = 4500000,
                    MaxSalary = 6000000,
                    EmployeeCount = 4,
                    AverageSalary = 5250000,
                    IsManagementPosition = false,
                    CreatedAt = DateTime.Now.AddYears(-1)
                },
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Title = "팀장",
                    Level = 4,
                    MinSalary = 6000000,
                    MaxSalary = 8000000,
                    EmployeeCount = 3,
                    AverageSalary = 7000000,
                    IsManagementPosition = true,
                    CreatedAt = DateTime.Now.AddYears(-1)
                },
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Title = "부장",
                    Level = 5,
                    MinSalary = 8000000,
                    MaxSalary = 12000000,
                    EmployeeCount = 2,
                    AverageSalary = 10000000,
                    IsManagementPosition = true,
                    CreatedAt = DateTime.Now.AddYears(-1)
                },
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    Title = "이사",
                    Level = 6,
                    MinSalary = 12000000,
                    MaxSalary = 20000000,
                    EmployeeCount = 1,
                    AverageSalary = 16000000,
                    IsManagementPosition = true,
                    CreatedAt = DateTime.Now.AddYears(-1)
                },
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                    Title = "디자이너",
                    Level = 2,
                    MinSalary = 3000000,
                    MaxSalary = 4000000,
                    EmployeeCount = 4,
                    AverageSalary = 3500000,
                    IsManagementPosition = false,
                    CreatedAt = DateTime.Now.AddYears(-1)
                },
                new()
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                    Title = "마케터",
                    Level = 2,
                    MinSalary = 2800000,
                    MaxSalary = 3800000,
                    EmployeeCount = 3,
                    AverageSalary = 3300000,
                    IsManagementPosition = false,
                    CreatedAt = DateTime.Now.AddYears(-1)
                }
            };
        }

        /// <summary>
        /// 폴백 통계 데이터
        /// </summary>
        private static PositionStatisticsResponse GetFallbackStatistics()
        {
            return new PositionStatisticsResponse
            {
                Success = true,
                Message = "폴백 데이터",
                TotalPositions = 8,
                ManagementPositions = 3,
                StaffPositions = 5,
                AverageMinSalary = 5575000,
                AverageMaxSalary = 7662500,
                HighestMaxSalary = 20000000,
                LowestMinSalary = 2800000,
                TotalEmployees = 31,
                AverageEmployeesPerPosition = 3.875m,
                LevelStats = new List<PositionLevelStats>
                {
                    new() { Level = 1, PositionCount = 1, EmployeeCount = 8, AverageSalary = 3200000 },
                    new() { Level = 2, PositionCount = 3, EmployeeCount = 13, AverageSalary = 3600000 },
                    new() { Level = 3, PositionCount = 1, EmployeeCount = 4, AverageSalary = 5250000 },
                    new() { Level = 4, PositionCount = 1, EmployeeCount = 3, AverageSalary = 7000000 },
                    new() { Level = 5, PositionCount = 1, EmployeeCount = 2, AverageSalary = 10000000 },
                    new() { Level = 6, PositionCount = 1, EmployeeCount = 1, AverageSalary = 16000000 }
                }
            };
        }

        /// <summary>
        /// HTTP 상태 코드에 따른 오류 메시지 반환
        /// </summary>
        private static string GetErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "요청한 직급을 찾을 수 없습니다.",
                HttpStatusCode.Unauthorized => "접근 권한이 없습니다.",
                HttpStatusCode.Forbidden => "이 작업을 수행할 권한이 없습니다.",
                HttpStatusCode.BadRequest => "잘못된 요청입니다.",
                HttpStatusCode.Conflict => "이미 존재하는 직급입니다.",
                HttpStatusCode.InternalServerError => "서버 내부 오류가 발생했습니다.",
                _ => "알 수 없는 오류가 발생했습니다."
            };
        }
    }
}