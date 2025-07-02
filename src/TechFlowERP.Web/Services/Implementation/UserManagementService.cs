using System.Net.Http.Json;
using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.User;
using TechFlowERP.Models.Enums;
using TechFlowERP.Models.Requests.User;
using TechFlowERP.Web.Services.Interfaces;

namespace TechFlowERP.Web.Services.Implementation
{
    /// <summary>
    /// 사용자 관리 서비스 구현
    /// </summary>
    public class UserManagementService : BaseService, IUserManagementService
    {
        // Mock 데이터 (Backend API가 준비되기 전까지 사용)
        private static readonly List<UserDto> _mockUsers = new()
        {
            new UserDto
            {
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                EmployeeId = Guid.Parse("67e4cb70-f8ab-4ec2-bd67-feb1a54dfc80"),
                Username = "admin",
                Email = "admin@techflow.com",
                Role = UserRole.Admin,
                LastLogin = DateTime.Now.AddHours(-2),
                IsLocked = false,
                FailedAttempts = 0,
                PasswordChangedAt = DateTime.Now.AddDays(-30),
                TenantId = "default-tenant",
                EmployeeName = "관리자",
                Department = "IT",
                Position = "시스템관리자",
                CreatedAt = DateTime.Now.AddDays(-90)
            },
            new UserDto
            {
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                EmployeeId = Guid.Parse("77e4cb70-f8ab-4ec2-bd67-feb1a54dfc81"),
                Username = "manager1",
                Email = "manager1@techflow.com",
                Role = UserRole.Manager,
                LastLogin = DateTime.Now.AddDays(-1),
                IsLocked = false,
                FailedAttempts = 0,
                PasswordChangedAt = DateTime.Now.AddDays(-45),
                TenantId = "default-tenant",
                EmployeeName = "김매니저",
                Department = "개발팀",
                Position = "팀장",
                CreatedAt = DateTime.Now.AddDays(-60)
            },
            new UserDto
            {
                UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                EmployeeId = Guid.Parse("87e4cb70-f8ab-4ec2-bd67-feb1a54dfc82"),
                Username = "employee1",
                Email = "employee1@techflow.com",
                Role = UserRole.Employee,
                LastLogin = DateTime.Now.AddDays(-7),
                IsLocked = true,
                FailedAttempts = 5,
                PasswordChangedAt = DateTime.Now.AddDays(-100), // 비밀번호 만료
                TenantId = "default-tenant",
                EmployeeName = "이직원",
                Department = "개발팀",
                Position = "개발자",
                CreatedAt = DateTime.Now.AddDays(-30)
            }
        };

        public UserManagementService(
            HttpClient httpClient,
            ILogger<UserManagementService> logger,
            ITokenService tokenService)
            : base(httpClient, logger, tokenService, "api/v1/users")
        {
        }

        public async Task<PagedResult<UserListDto>> GetUsersAsync(SearchUsersRequest request)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await GetAsync<PagedResult<UserListDto>>("", request);

                _logger.LogInformation("사용자 목록 조회 요청: {Request}", request);

                // Mock 데이터 사용
                await Task.Delay(500); // API 호출 시뮬레이션

                var users = _mockUsers.AsQueryable();

                // 검색 필터 적용
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    users = users.Where(u =>
                        u.Username.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        u.Email.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (u.EmployeeName != null && u.EmployeeName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));
                }

                if (request.Role.HasValue)
                {
                    users = users.Where(u => u.Role == request.Role.Value);
                }

                if (request.IsLocked.HasValue)
                {
                    users = users.Where(u => u.IsLocked == request.IsLocked.Value);
                }

                if (!string.IsNullOrEmpty(request.Department))
                {
                    users = users.Where(u => u.Department == request.Department);
                }

                // 정렬 적용
                users = request.SortBy.ToLower() switch
                {
                    "username" => request.SortDescending ? users.OrderByDescending(u => u.Username) : users.OrderBy(u => u.Username),
                    "email" => request.SortDescending ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email),
                    "role" => request.SortDescending ? users.OrderByDescending(u => u.Role) : users.OrderBy(u => u.Role),
                    "lastlogin" => request.SortDescending ? users.OrderByDescending(u => u.LastLogin) : users.OrderBy(u => u.LastLogin),
                    _ => request.SortDescending ? users.OrderByDescending(u => u.CreatedAt) : users.OrderBy(u => u.CreatedAt)
                };

                var totalCount = users.Count();
                var pagedUsers = users
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(u => new UserListDto
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        Role = u.Role,
                        EmployeeName = u.EmployeeName,
                        Department = u.Department,
                        IsLocked = u.IsLocked,
                        LastLogin = u.LastLogin,
                        CreatedAt = u.CreatedAt
                    })
                    .ToList();

                return new PagedResult<UserListDto>
                {
                    Items = pagedUsers,
                    TotalCount = totalCount,
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 목록 조회 중 오류 발생");
                return new PagedResult<UserListDto>();
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await GetAsync<UserDto>($"{userId}");

                _logger.LogInformation("사용자 상세 조회: {UserId}", userId);

                await Task.Delay(300);

                return _mockUsers.FirstOrDefault(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 상세 조회 중 오류 발생: {UserId}", userId);
                return null;
            }
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await PostAsync<UserDto>("", request);

                _logger.LogInformation("사용자 생성 요청: {Username}", request.Username);

                await Task.Delay(500);

                // 중복 체크
                if (_mockUsers.Any(u => u.Username == request.Username))
                {
                    _logger.LogWarning("사용자명 중복: {Username}", request.Username);
                    return null;
                }

                if (_mockUsers.Any(u => u.Email == request.Email))
                {
                    _logger.LogWarning("이메일 중복: {Email}", request.Email);
                    return null;
                }

                var newUser = new UserDto
                {
                    UserId = Guid.NewGuid(),
                    EmployeeId = request.EmployeeId,
                    Username = request.Username,
                    Email = request.Email,
                    Role = request.Role,
                    LastLogin = null,
                    IsLocked = false,
                    FailedAttempts = 0,
                    PasswordChangedAt = DateTime.UtcNow,
                    TenantId = request.TenantId,
                    CreatedAt = DateTime.UtcNow
                };

                _mockUsers.Add(newUser);

                _logger.LogInformation("사용자 생성 완료: {UserId}", newUser.UserId);
                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 생성 중 오류 발생");
                return null;
            }
        }

        public async Task<UserDto?> UpdateUserAsync(UpdateUserRequest request)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await PutAsync<UserDto>($"{request.UserId}", request);

                _logger.LogInformation("사용자 수정 요청: {UserId}", request.UserId);

                await Task.Delay(400);

                var existingUser = _mockUsers.FirstOrDefault(u => u.UserId == request.UserId);
                if (existingUser == null)
                {
                    _logger.LogWarning("사용자를 찾을 수 없음: {UserId}", request.UserId);
                    return null;
                }

                // 중복 체크 (본인 제외)
                if (_mockUsers.Any(u => u.Username == request.Username && u.UserId != request.UserId))
                {
                    _logger.LogWarning("사용자명 중복: {Username}", request.Username);
                    return null;
                }

                if (_mockUsers.Any(u => u.Email == request.Email && u.UserId != request.UserId))
                {
                    _logger.LogWarning("이메일 중복: {Email}", request.Email);
                    return null;
                }

                // 업데이트
                existingUser.Username = request.Username;
                existingUser.Email = request.Email;
                existingUser.Role = request.Role;
                existingUser.EmployeeId = request.EmployeeId;
                existingUser.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("사용자 수정 완료: {UserId}", request.UserId);
                return existingUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 수정 중 오류 발생");
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await DeleteAsync($"{userId}");

                _logger.LogInformation("사용자 삭제 요청: {UserId}", userId);

                await Task.Delay(300);

                var user = _mockUsers.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    _mockUsers.Remove(user);
                    _logger.LogInformation("사용자 삭제 완료: {UserId}", userId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 삭제 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await PostAsync<object>($"{request.UserId}/toggle-status", request);

                _logger.LogInformation("사용자 상태 변경: {UserId}, Locked: {IsLocked}",
                    request.UserId, request.IsLocked);

                await Task.Delay(200);

                var user = _mockUsers.FirstOrDefault(u => u.UserId == request.UserId);
                if (user != null)
                {
                    user.IsLocked = request.IsLocked;
                    if (!request.IsLocked)
                    {
                        user.FailedAttempts = 0; // 잠금 해제 시 실패 횟수 초기화
                    }
                    user.UpdatedAt = DateTime.UtcNow;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 상태 변경 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeUserRoleRequest request)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await PostAsync<object>($"{request.UserId}/change-role", request);

                _logger.LogInformation("사용자 역할 변경: {UserId}, NewRole: {NewRole}",
                    request.UserId, request.NewRole);

                await Task.Delay(200);

                var user = _mockUsers.FirstOrDefault(u => u.UserId == request.UserId);
                if (user != null)
                {
                    user.Role = request.NewRole;
                    user.UpdatedAt = DateTime.UtcNow;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "사용자 역할 변경 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> ResetUserPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await PostAsync<object>($"{request.UserId}/reset-password", request);

                _logger.LogInformation("비밀번호 재설정: {UserId}", request.UserId);

                await Task.Delay(300);

                var user = _mockUsers.FirstOrDefault(u => u.UserId == request.UserId);
                if (user != null)
                {
                    user.PasswordChangedAt = DateTime.UtcNow;
                    user.FailedAttempts = 0;
                    user.IsLocked = false; // 비밀번호 재설정 시 잠금 해제
                    user.UpdatedAt = DateTime.UtcNow;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "비밀번호 재설정 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> ForcePasswordChangeAsync(Guid userId)
        {
            try
            {
                // TODO: Backend API 연동
                // var response = await PostAsync<object>($"{userId}/force-password-change", new { });

                _logger.LogInformation("비밀번호 변경 강제: {UserId}", userId);

                await Task.Delay(200);

                var user = _mockUsers.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    user.PasswordChangedAt = DateTime.UtcNow.AddDays(-91); // 강제로 만료시킴
                    user.UpdatedAt = DateTime.UtcNow;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "비밀번호 변경 강제 중 오류 발생");
                return false;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null)
        {
            await Task.Delay(100);

            if (excludeUserId.HasValue)
            {
                return !_mockUsers.Any(u => u.Username == username && u.UserId != excludeUserId.Value);
            }

            return !_mockUsers.Any(u => u.Username == username);
        }

        public async Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null)
        {
            await Task.Delay(100);

            if (excludeUserId.HasValue)
            {
                return !_mockUsers.Any(u => u.Email == email && u.UserId != excludeUserId.Value);
            }

            return !_mockUsers.Any(u => u.Email == email);
        }

        public async Task<Dictionary<string, int>> GetUserRoleStatisticsAsync()
        {
            await Task.Delay(200);

            return _mockUsers
                .GroupBy(u => u.Role)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Count()
                );
        }

        public async Task<List<UserListDto>> GetLockedUsersAsync()
        {
            await Task.Delay(200);

            return _mockUsers
                .Where(u => u.IsLocked)
                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    EmployeeName = u.EmployeeName,
                    Department = u.Department,
                    IsLocked = u.IsLocked,
                    LastLogin = u.LastLogin,
                    CreatedAt = u.CreatedAt
                })
                .ToList();
        }

        public async Task<List<UserListDto>> GetPasswordExpiringUsersAsync(int daysAhead = 7)
        {
            await Task.Delay(200);

            var expirationDate = DateTime.UtcNow.AddDays(-90 + daysAhead); // 90일 후 만료 가정

            return _mockUsers
                .Where(u => u.PasswordChangedAt <= expirationDate)
                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    EmployeeName = u.EmployeeName,
                    Department = u.Department,
                    IsLocked = u.IsLocked,
                    LastLogin = u.LastLogin,
                    CreatedAt = u.CreatedAt
                })
                .ToList();
        }

        public async Task<List<LoginHistoryDto>> GetUserLoginHistoryAsync(Guid userId, int days = 30)
        {
            await Task.Delay(300);

            // Mock 데이터
            return new List<LoginHistoryDto>
            {
                new LoginHistoryDto
                {
                    LoginTime = DateTime.Now.AddHours(-2),
                    IpAddress = "192.168.1.100",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    IsSuccessful = true
                },
                new LoginHistoryDto
                {
                    LoginTime = DateTime.Now.AddDays(-1),
                    IpAddress = "192.168.1.100",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    IsSuccessful = true
                }
            };
        }

        public async Task<List<UserListDto>> GetRecentlyActiveUsersAsync(int days = 7)
        {
            await Task.Delay(200);

            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            return _mockUsers
                .Where(u => u.LastLogin.HasValue && u.LastLogin.Value >= cutoffDate)
                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    EmployeeName = u.EmployeeName,
                    Department = u.Department,
                    IsLocked = u.IsLocked,
                    LastLogin = u.LastLogin,
                    CreatedAt = u.CreatedAt
                })
                .OrderByDescending(u => u.LastLogin)
                .ToList();
        }
    }
}