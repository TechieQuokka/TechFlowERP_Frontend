using TechFlowERP.Models.Common;
using TechFlowERP.Models.DTOs.User;
using TechFlowERP.Models.Requests.User;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 사용자 관리 서비스 인터페이스
    /// </summary>
    public interface IUserManagementService
    {
        // CRUD 기본 작업
        /// <summary>
        /// 사용자 목록 조회 (페이징, 검색 지원)
        /// </summary>
        Task<PagedResult<UserListDto>> GetUsersAsync(SearchUsersRequest request);

        /// <summary>
        /// 사용자 상세 정보 조회
        /// </summary>
        Task<UserDto?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// 새 사용자 생성
        /// </summary>
        Task<UserDto?> CreateUserAsync(CreateUserRequest request);

        /// <summary>
        /// 사용자 정보 수정
        /// </summary>
        Task<UserDto?> UpdateUserAsync(UpdateUserRequest request);

        /// <summary>
        /// 사용자 삭제 (소프트 삭제)
        /// </summary>
        Task<bool> DeleteUserAsync(Guid userId);

        // 사용자 상태 관리
        /// <summary>
        /// 사용자 계정 잠금/해제
        /// </summary>
        Task<bool> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        /// <summary>
        /// 사용자 역할 변경
        /// </summary>
        Task<bool> ChangeUserRoleAsync(ChangeUserRoleRequest request);

        // 비밀번호 관리
        /// <summary>
        /// 관리자가 사용자 비밀번호 재설정
        /// </summary>
        Task<bool> ResetUserPasswordAsync(ResetPasswordRequest request);

        /// <summary>
        /// 사용자 비밀번호 만료 강제
        /// </summary>
        Task<bool> ForcePasswordChangeAsync(Guid userId);

        // 특수 조회 기능
        /// <summary>
        /// 사용자명으로 중복 체크
        /// </summary>
        Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null);

        /// <summary>
        /// 이메일로 중복 체크
        /// </summary>
        Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null);

        /// <summary>
        /// 역할별 사용자 수 통계
        /// </summary>
        Task<Dictionary<string, int>> GetUserRoleStatisticsAsync();

        /// <summary>
        /// 잠긴 계정 목록
        /// </summary>
        Task<List<UserListDto>> GetLockedUsersAsync();

        /// <summary>
        /// 비밀번호 만료 예정 사용자 목록
        /// </summary>
        Task<List<UserListDto>> GetPasswordExpiringUsersAsync(int daysAhead = 7);

        // 로그인 기록 관리
        /// <summary>
        /// 사용자 로그인 기록 조회
        /// </summary>
        Task<List<LoginHistoryDto>> GetUserLoginHistoryAsync(Guid userId, int days = 30);

        /// <summary>
        /// 최근 로그인한 사용자 목록
        /// </summary>
        Task<List<UserListDto>> GetRecentlyActiveUsersAsync(int days = 7);
    }

    /// <summary>
    /// 로그인 기록 DTO
    /// </summary>
    public class LoginHistoryDto
    {
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string? FailureReason { get; set; }
    }
}