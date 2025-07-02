using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.Department;
using TechFlowERP.Models.DTOs.Employee;
using TechFlowERP.Models.Enums;
using TechFlowERP.Models.Requests.Department;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 부서 관리 서비스 구현 (관리자용)
    /// </summary>
    public class DepartmentManagementService : BaseService, IDepartmentManagementService
    {
        // Mock 데이터 (Backend API가 준비되기 전까지 사용)
        private static readonly List<DepartmentDto> _mockDepartments = new()
        {
            new DepartmentDto
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "개발팀",
                ManagerId = Guid.Parse("67e4cb70-f8ab-4ec2-bd67-feb1a54dfc80"),
                ManagerName = "김개발",
                Budget = 50000000,
                UsedBudget = 42000000,
                EmployeeCount = 12,
                ActiveProjectsCount = 5,
                TotalSalary = 40000000,
                TenantId = "default-tenant",
                CreatedAt = DateTime.Now.AddYears(-1),
                UpdatedAt = DateTime.Now
            },
            new DepartmentDto
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "디자인팀",
                ManagerId = null,
                ManagerName = "",
                Budget = 30000000,
                UsedBudget = 18000000,
                EmployeeCount = 6,
                ActiveProjectsCount = 3,
                TotalSalary = 15000000,
                TenantId = "default-tenant",
                CreatedAt = DateTime.Now.AddYears(-1),
                UpdatedAt = DateTime.Now
            },
            new DepartmentDto
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "마케팅팀",
                ManagerId = Guid.Parse("77e4cb70-f8ab-4ec2-bd67-feb1a54dfc81"),
                ManagerName = "이마케팅",
                Budget = 25000000,
                UsedBudget = 14000000,
                EmployeeCount = 4,
                ActiveProjectsCount = 2,
                TotalSalary = 12000000,
                TenantId = "default-tenant",
                CreatedAt = DateTime.Now.AddYears(-1),
                UpdatedAt = DateTime.Now
            }
        };

        public DepartmentManagementService(
            HttpClient httpClient,
            ILogger<DepartmentManagementService> logger,
            ITokenService tokenService)
            : base(httpClient, logger, tokenService, "api/v1/departments")
        {
        }

        public async Task<PagedResult<DepartmentListDto>> GetDepartmentsAsync(SearchDepartmentsRequest request)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await GetAsync<PagedResult<DepartmentListDto>>("", request);

                _logger.LogInformation("부서 목록 조회 요청: {Request}", request);

                await Task.Delay(500); // API 호출 시뮬레이션

                var departments = _mockDepartments.AsQueryable();

                // 검색 필터 적용
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    departments = departments.Where(d =>
                        d.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (d.ManagerName != null && d.ManagerName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));
                }

                if (request.ManagerId.HasValue)
                {
                    departments = departments.Where(d => d.ManagerId == request.ManagerId.Value);
                }

                if (request.HasManager.HasValue)
                {
                    departments = departments.Where(d => request.HasManager.Value ? d.ManagerId.HasValue : !d.ManagerId.HasValue);
                }

                if (request.MinBudget.HasValue)
                {
                    departments = departments.Where(d => d.Budget >= request.MinBudget.Value);
                }

                if (request.MaxBudget.HasValue)
                {
                    departments = departments.Where(d => d.Budget <= request.MaxBudget.Value);
                }

                if (request.MinEmployeeCount.HasValue)
                {
                    departments = departments.Where(d => d.EmployeeCount >= request.MinEmployeeCount.Value);
                }

                if (request.MaxEmployeeCount.HasValue)
                {
                    departments = departments.Where(d => d.EmployeeCount <= request.MaxEmployeeCount.Value);
                }

                if (request.MinBudgetUtilization.HasValue)
                {
                    departments = departments.Where(d => d.BudgetUtilization >= request.MinBudgetUtilization.Value);
                }

                if (request.MaxBudgetUtilization.HasValue)
                {
                    departments = departments.Where(d => d.BudgetUtilization <= request.MaxBudgetUtilization.Value);
                }

                // 정렬 적용
                departments = request.SortBy.ToLower() switch
                {
                    "name" => request.SortDescending ? departments.OrderByDescending(d => d.Name) : departments.OrderBy(d => d.Name),
                    "budget" => request.SortDescending ? departments.OrderByDescending(d => d.Budget) : departments.OrderBy(d => d.Budget),
                    "employeecount" => request.SortDescending ? departments.OrderByDescending(d => d.EmployeeCount) : departments.OrderBy(d => d.EmployeeCount),
                    "budgetutilization" => request.SortDescending ? departments.OrderByDescending(d => d.BudgetUtilization) : departments.OrderBy(d => d.BudgetUtilization),
                    _ => request.SortDescending ? departments.OrderByDescending(d => d.CreatedAt) : departments.OrderBy(d => d.CreatedAt)
                };

                var totalCount = departments.Count();
                var pagedDepartments = departments
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(d => new DepartmentListDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        ManagerName = d.ManagerName,
                        EmployeeCount = d.EmployeeCount,
                        Budget = d.Budget,
                        BudgetUtilization = d.BudgetUtilization,
                        CreatedAt = d.CreatedAt
                    })
                    .ToList();

                return new PagedResult<DepartmentListDto>
                {
                    Items = pagedDepartments,
                    TotalCount = totalCount,
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 목록 조회 중 오류 발생");
                return new PagedResult<DepartmentListDto>();
            }
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(Guid departmentId)
        {
            try
            {
                // TODO: Backend API 연동
                _logger.LogInformation("부서 상세 조회: {DepartmentId}", departmentId);
                await Task.Delay(300);

                return _mockDepartments.FirstOrDefault(d => d.Id == departmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 상세 조회 중 오류 발생: {DepartmentId}", departmentId);
                return null;
            }
        }

        public async Task<DepartmentDto?> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            try
            {
                _logger.LogInformation("부서 생성 요청: {Name}", request.Name);
                await Task.Delay(500);

                // 중복 체크
                if (_mockDepartments.Any(d => d.Name == request.Name))
                {
                    _logger.LogWarning("부서명 중복: {Name}", request.Name);
                    return null;
                }

                var newDepartment = new DepartmentDto
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    ManagerId = request.ManagerId,
                    Budget = request.Budget,
                    UsedBudget = 0,
                    EmployeeCount = 0,
                    ActiveProjectsCount = 0,
                    TotalSalary = 0,
                    TenantId = request.TenantId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _mockDepartments.Add(newDepartment);
                _logger.LogInformation("부서 생성 완료: {DepartmentId}", newDepartment.Id);
                return newDepartment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 생성 중 오류 발생");
                return null;
            }
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(UpdateDepartmentRequest request)
        {
            try
            {
                _logger.LogInformation("부서 수정 요청: {DepartmentId}", request.DepartmentId);
                await Task.Delay(400);

                var existingDepartment = _mockDepartments.FirstOrDefault(d => d.Id == request.DepartmentId);
                if (existingDepartment == null)
                {
                    _logger.LogWarning("부서를 찾을 수 없음: {DepartmentId}", request.DepartmentId);
                    return null;
                }

                // 중복 체크 (본인 제외)
                if (_mockDepartments.Any(d => d.Name == request.Name && d.Id != request.DepartmentId))
                {
                    _logger.LogWarning("부서명 중복: {Name}", request.Name);
                    return null;
                }

                existingDepartment.Name = request.Name;
                existingDepartment.ManagerId = request.ManagerId;
                existingDepartment.Budget = request.Budget;
                existingDepartment.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("부서 수정 완료: {DepartmentId}", request.DepartmentId);
                return existingDepartment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 수정 중 오류 발생");
                return null;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(Guid departmentId)
        {
            try
            {
                _logger.LogInformation("부서 삭제 요청: {DepartmentId}", departmentId);
                await Task.Delay(300);

                var department = _mockDepartments.FirstOrDefault(d => d.Id == departmentId);
                if (department != null)
                {
                    // 직원이 있는 부서는 삭제 불가
                    if (department.EmployeeCount > 0)
                    {
                        _logger.LogWarning("직원이 있는 부서는 삭제할 수 없음: {DepartmentId}", departmentId);
                        return false;
                    }

                    _mockDepartments.Remove(department);
                    _logger.LogInformation("부서 삭제 완료: {DepartmentId}", departmentId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 삭제 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> AssignManagerAsync(AssignManagerRequest request)
        {
            try
            {
                _logger.LogInformation("부서장 할당: {DepartmentId}, {ManagerId}",
                    request.DepartmentId, request.ManagerId);
                await Task.Delay(200);

                var department = _mockDepartments.FirstOrDefault(d => d.Id == request.DepartmentId);
                if (department != null)
                {
                    department.ManagerId = request.ManagerId;
                    department.ManagerName = $"매니저{request.ManagerId.ToString()[..8]}"; // Mock 이름
                    department.UpdatedAt = DateTime.UtcNow;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서장 할당 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> RemoveManagerAsync(Guid departmentId, string? reason = null)
        {
            try
            {
                _logger.LogInformation("부서장 해제: {DepartmentId}", departmentId);
                await Task.Delay(200);

                var department = _mockDepartments.FirstOrDefault(d => d.Id == departmentId);
                if (department != null)
                {
                    department.ManagerId = null;
                    department.ManagerName = "";
                    department.UpdatedAt = DateTime.UtcNow;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서장 해제 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> ChangeManagerAsync(Guid departmentId, Guid newManagerId, string? reason = null)
        {
            try
            {
                var request = new AssignManagerRequest
                {
                    DepartmentId = departmentId,
                    ManagerId = newManagerId,
                    Reason = reason
                };

                return await AssignManagerAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서장 변경 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> UpdateBudgetAsync(UpdateBudgetRequest request)
        {
            try
            {
                _logger.LogInformation("부서 예산 수정: {DepartmentId}, {NewBudget}",
                    request.DepartmentId, request.NewBudget);
                await Task.Delay(300);

                var department = _mockDepartments.FirstOrDefault(d => d.Id == request.DepartmentId);
                if (department != null)
                {
                    department.Budget = request.NewBudget;
                    department.UpdatedAt = DateTime.UtcNow;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 예산 수정 중 오류 발생");
                return false;
            }
        }

        public async Task<Dictionary<Guid, decimal>> GetBudgetUtilizationAsync()
        {
            await Task.Delay(200);

            return _mockDepartments.ToDictionary(
                d => d.Id,
                d => d.BudgetUtilization
            );
        }

        public async Task<List<DepartmentListDto>> GetOverBudgetDepartmentsAsync()
        {
            await Task.Delay(200);

            return _mockDepartments
                .Where(d => d.BudgetUtilization > 100)
                .Select(d => new DepartmentListDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ManagerName = d.ManagerName,
                    EmployeeCount = d.EmployeeCount,
                    Budget = d.Budget,
                    BudgetUtilization = d.BudgetUtilization,
                    CreatedAt = d.CreatedAt
                })
                .ToList();
        }

        public async Task<List<EmployeeDto>> GetDepartmentEmployeesAsync(Guid departmentId)
        {
            await Task.Delay(300);

            // Mock 데이터 반환
            return new List<EmployeeDto>
            {
                new EmployeeDto
                {
                    Id = Guid.NewGuid(),
                    Name = "김직원",
                    Email = "kim@company.com",
                    DepartmentId = departmentId,
                    IsActive = true
                }
            };
        }

        public async Task<bool> TransferEmployeeAsync(TransferEmployeeRequest request)
        {
            try
            {
                _logger.LogInformation("직원 부서 이동: {EmployeeId}, {FromDepartmentId} -> {ToDepartmentId}",
                    request.EmployeeId, request.FromDepartmentId, request.ToDepartmentId);
                await Task.Delay(400);

                // Mock: 직원 수 업데이트
                var fromDept = _mockDepartments.FirstOrDefault(d => d.Id == request.FromDepartmentId);
                var toDept = _mockDepartments.FirstOrDefault(d => d.Id == request.ToDepartmentId);

                if (fromDept != null && toDept != null)
                {
                    fromDept.EmployeeCount = Math.Max(0, fromDept.EmployeeCount - 1);
                    toDept.EmployeeCount += 1;

                    fromDept.UpdatedAt = DateTime.UtcNow;
                    toDept.UpdatedAt = DateTime.UtcNow;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "직원 부서 이동 중 오류 발생");
                return false;
            }
        }

        public async Task<Dictionary<Guid, int>> GetEmployeeCountByDepartmentAsync()
        {
            await Task.Delay(200);

            return _mockDepartments.ToDictionary(
                d => d.Id,
                d => d.EmployeeCount
            );
        }

        public async Task<DepartmentStatisticsDto> GetDepartmentStatisticsAsync()
        {
            await Task.Delay(300);

            var departments = _mockDepartments;

            return new DepartmentStatisticsDto
            {
                TotalDepartments = departments.Count,
                DepartmentsWithManager = departments.Count(d => d.ManagerId.HasValue),
                DepartmentsWithoutManager = departments.Count(d => !d.ManagerId.HasValue),
                TotalBudget = departments.Sum(d => d.Budget),
                TotalUsedBudget = departments.Sum(d => d.UsedBudget),
                AverageEmployeesPerDepartment = departments.Count > 0 ? (decimal)departments.Average(d => d.EmployeeCount) : 0,
                LargestDepartmentSize = departments.Count > 0 ? departments.Max(d => d.EmployeeCount) : 0,
                LargestDepartmentName = departments.Count > 0 ?
                    departments.OrderByDescending(d => d.EmployeeCount).First().Name : ""
            };
        }

        public async Task<Dictionary<Guid, DepartmentPerformanceDto>> GetDepartmentPerformanceAsync(DateTime startDate, DateTime endDate)
        {
            await Task.Delay(400);

            return _mockDepartments.ToDictionary(
                d => d.Id,
                d => new DepartmentPerformanceDto
                {
                    DepartmentId = d.Id,
                    DepartmentName = d.Name,
                    CompletedProjects = d.ActiveProjectsCount + 2, // Mock 완료 프로젝트
                    TotalRevenue = d.Budget * 1.2m, // Mock 수익
                    AverageProjectDuration = 45.5m, // Mock 평균 기간
                    EmployeeUtilization = 85.0m + (decimal)(new Random().NextDouble() * 20 - 10), // Mock 활용도
                    CustomerSatisfaction = 4.2m + (decimal)(new Random().NextDouble() * 0.8), // Mock 만족도
                    OnTimeDeliveryRate = 88 + new Random().Next(-10, 12), // Mock 정시 납기율
                    ProfitMargin = 15.5m + (decimal)(new Random().NextDouble() * 10 - 5) // Mock 수익률
                }
            );
        }

        public async Task<List<DepartmentEfficiencyDto>> GetDepartmentEfficiencyAsync()
        {
            await Task.Delay(350);

            return _mockDepartments.Select(d => new DepartmentEfficiencyDto
            {
                DepartmentId = d.Id,
                DepartmentName = d.Name,
                RevenuePerEmployee = d.EmployeeCount > 0 ? (d.Budget * 1.3m) / d.EmployeeCount : 0,
                CostPerEmployee = d.EmployeeCount > 0 ? d.UsedBudget / d.EmployeeCount : 0,
                EfficiencyRatio = d.UsedBudget > 0 ? (d.Budget * 1.2m) / d.UsedBudget : 0,
                BudgetEfficiency = 100 - d.BudgetUtilization,
                ProductivityScore = 75 + (decimal)(new Random().NextDouble() * 30), // Mock 생산성 점수
                EfficiencyGrade = d.BudgetUtilization switch
                {
                    <= 60 => "A",
                    <= 80 => "B",
                    <= 95 => "C",
                    _ => "D"
                }
            }).ToList();
        }

        public async Task<bool> IsDepartmentNameAvailableAsync(string name, Guid? excludeDepartmentId = null)
        {
            await Task.Delay(100);

            if (excludeDepartmentId.HasValue)
            {
                return !_mockDepartments.Any(d => d.Name == name && d.Id != excludeDepartmentId.Value);
            }

            return !_mockDepartments.Any(d => d.Name == name);
        }

        public async Task<List<DepartmentListDto>> GetDepartmentsWithoutManagerAsync()
        {
            await Task.Delay(200);

            return _mockDepartments
                .Where(d => !d.ManagerId.HasValue)
                .Select(d => new DepartmentListDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ManagerName = d.ManagerName,
                    EmployeeCount = d.EmployeeCount,
                    Budget = d.Budget,
                    BudgetUtilization = d.BudgetUtilization,
                    CreatedAt = d.CreatedAt
                })
                .ToList();
        }

        public async Task<List<EmployeeDto>> GetManagerCandidatesAsync(Guid departmentId)
        {
            await Task.Delay(250);

            // Mock 부서장 후보 데이터
            return new List<EmployeeDto>
            {
                new EmployeeDto
                {
                    Id = Guid.NewGuid(),
                    Name = "김팀장",
                    Email = "kim.manager@company.com",
                    DepartmentId = departmentId,
                    IsActive = true,
                    HireDate = DateTime.Now.AddYears(-3)
                },
                new EmployeeDto
                {
                    Id = Guid.NewGuid(),
                    Name = "이선임",
                    Email = "lee.senior@company.com",
                    DepartmentId = departmentId,
                    IsActive = true,
                    HireDate = DateTime.Now.AddYears(-5)
                }
            };
        }

        public async Task<List<DepartmentDto>> GetActiveDepartmentsAsync()
        {
            await Task.Delay(200);
            return _mockDepartments.ToList();
        }

        public async Task<bool> MergeDepartmentsAsync(MergeDepartmentsRequest request)
        {
            try
            {
                _logger.LogInformation("부서 병합: {SourceDepartmentId} -> {TargetDepartmentId}",
                    request.SourceDepartmentId, request.TargetDepartmentId);
                await Task.Delay(600);

                var sourceDept = _mockDepartments.FirstOrDefault(d => d.Id == request.SourceDepartmentId);
                var targetDept = _mockDepartments.FirstOrDefault(d => d.Id == request.TargetDepartmentId);

                if (sourceDept != null && targetDept != null)
                {
                    if (request.TransferBudget)
                    {
                        targetDept.Budget += sourceDept.Budget;
                        targetDept.UsedBudget += sourceDept.UsedBudget;
                    }

                    if (request.TransferEmployees)
                    {
                        targetDept.EmployeeCount += sourceDept.EmployeeCount;
                    }

                    targetDept.ActiveProjectsCount += sourceDept.ActiveProjectsCount;
                    targetDept.UpdatedAt = DateTime.UtcNow;

                    _mockDepartments.Remove(sourceDept);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 병합 중 오류 발생");
                return false;
            }
        }

        public async Task<DepartmentDto?> SplitDepartmentAsync(Guid parentDepartmentId, CreateDepartmentRequest newDepartmentRequest)
        {
            try
            {
                _logger.LogInformation("부서 분할: {ParentDepartmentId} -> {NewDepartmentName}",
                    parentDepartmentId, newDepartmentRequest.Name);
                await Task.Delay(500);

                var parentDept = _mockDepartments.FirstOrDefault(d => d.Id == parentDepartmentId);
                if (parentDept == null || parentDept.EmployeeCount < 2)
                {
                    return null; // 분할하기에는 인원이 부족
                }

                var newDepartment = await CreateDepartmentAsync(newDepartmentRequest);
                if (newDepartment != null)
                {
                    // 부모 부서에서 일부 리소스 이동 (Mock)
                    var transferEmployees = parentDept.EmployeeCount / 2;
                    var transferBudget = parentDept.Budget * 0.3m;

                    parentDept.EmployeeCount -= transferEmployees;
                    parentDept.Budget -= transferBudget;
                    parentDept.UpdatedAt = DateTime.UtcNow;

                    newDepartment.EmployeeCount = transferEmployees;
                    newDepartment.Budget = transferBudget;
                }

                return newDepartment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "부서 분할 중 오류 발생");
                return null;
            }
        }

        public async Task<Dictionary<Guid, List<MonthlyBudgetUsageDto>>> GetMonthlyBudgetUsageAsync(int year)
        {
            await Task.Delay(400);

            var result = new Dictionary<Guid, List<MonthlyBudgetUsageDto>>();

            foreach (var dept in _mockDepartments)
            {
                var monthlyUsage = new List<MonthlyBudgetUsageDto>();

                for (int month = 1; month <= 12; month++)
                {
                    var monthlyBudget = dept.Budget / 12;
                    var variance = (decimal)(new Random().NextDouble() * 20 - 10); // -10% ~ +10% 변동
                    var actualUsage = monthlyBudget + (monthlyBudget * variance / 100);

                    monthlyUsage.Add(new MonthlyBudgetUsageDto
                    {
                        Month = month,
                        BudgetAllocated = monthlyBudget,
                        ActualUsage = actualUsage,
                        VariancePercentage = variance
                    });
                }

                result[dept.Id] = monthlyUsage;
            }

            return result;
        }

        public async Task<DepartmentRestructureAnalysisDto> SimulateRestructureAsync(List<RestructureScenarioDto> scenarios)
        {
            await Task.Delay(800);

            // Mock 재구조화 분석 결과
            var scenario = scenarios.FirstOrDefault() ?? new RestructureScenarioDto { ScenarioName = "기본 시나리오" };

            return new DepartmentRestructureAnalysisDto
            {
                ScenarioName = scenario.ScenarioName,
                ProjectedCostSavings = 5000000 + (decimal)(new Random().NextDouble() * 10000000),
                ProjectedEfficiencyGain = 15.5m + (decimal)(new Random().NextDouble() * 10),
                AffectedEmployees = _mockDepartments.Sum(d => d.EmployeeCount) / 2,
                Risks = new List<string>
                {
                    "직원 적응 기간 필요",
                    "일시적 생산성 저하 가능성",
                    "의사소통 체계 재정립 필요"
                },
                Benefits = new List<string>
                {
                    "운영 효율성 향상",
                    "의사결정 속도 개선",
                    "리소스 최적화",
                    "협업 강화"
                },
                ImplementationCost = 2000000 + (decimal)(new Random().NextDouble() * 3000000),
                EstimatedImplementationDays = 30 + new Random().Next(0, 60),
                ROI = 250.5m + (decimal)(new Random().NextDouble() * 100)
            };
        }
    }
}