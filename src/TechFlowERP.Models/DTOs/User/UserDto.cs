using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.User
{
    /// <summary>
    /// 사용자 DTO (관리자용)
    /// </summary>
    public class UserDto : BaseDto
    {
        public Guid UserId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsLocked { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime PasswordChangedAt { get; set; }

        // Employee 관련 정보 (Join된 데이터)
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }

        // 계산된 속성
        public bool IsActive => !IsLocked;
        public string StatusText => IsLocked ? "잠김" : "활성";
        public string LastLoginText => LastLogin?.ToString("yyyy-MM-dd HH:mm") ?? "로그인 기록 없음";
        public bool RequiresPasswordReset => PasswordChangedAt < DateTime.UtcNow.AddDays(-90); // 90일 이상
    }

    /// <summary>
    /// 사용자 목록용 간단한 DTO
    /// </summary>
    public class UserListDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }

        // UI 표시용
        public string RoleDisplayName => Role switch
        {
            UserRole.Admin => "관리자",
            UserRole.Manager => "매니저",
            UserRole.Employee => "직원",
            UserRole.Finance => "재무",
            UserRole.HR => "인사",
            UserRole.ReadOnly => "읽기전용",
            _ => "알 수 없음"
        };

        public string StatusBadgeClass => IsLocked ? "badge bg-danger" : "badge bg-success";
        public string StatusText => IsLocked ? "잠김" : "활성";
    }
}