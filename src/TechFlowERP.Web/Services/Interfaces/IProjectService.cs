using TechFlowERP.Models.DTOs.Project;
using TechFlowERP.Models.Enums;
using TechFlowERP.Models.Requests.Project;
using TechFlowERP.Models.Responses.Project;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 프로젝트 관리 서비스 인터페이스
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// 프로젝트 목록 조회 (페이징)
        /// </summary>
        Task<ProjectListResponse> GetProjectsAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            ProjectStatus? status = null,
            Guid? clientId = null,
            Guid? managerId = null);

        /// <summary>
        /// 프로젝트 상세 조회
        /// </summary>
        Task<ProjectDto?> GetProjectByIdAsync(Guid projectId);

        /// <summary>
        /// 프로젝트 생성
        /// </summary>
        Task<ProjectDto?> CreateProjectAsync(CreateProjectRequest request);

        /// <summary>
        /// 프로젝트 수정
        /// </summary>
        Task<ProjectDto?> UpdateProjectAsync(Guid projectId, UpdateProjectRequest request);

        /// <summary>
        /// 프로젝트 삭제
        /// </summary>
        Task<bool> DeleteProjectAsync(Guid projectId);

        /// <summary>
        /// 프로젝트 상태 변경
        /// </summary>
        Task<bool> ChangeProjectStatusAsync(Guid projectId, ProjectStatus status);

        /// <summary>
        /// 프로젝트 검색
        /// </summary>
        Task<List<ProjectDto>> SearchProjectsAsync(string searchTerm);

        /// <summary>
        /// 활성 프로젝트 목록 조회
        /// </summary>
        Task<List<ProjectDto>> GetActiveProjectsAsync();

        /// <summary>
        /// 매니저별 프로젝트 목록 조회
        /// </summary>
        Task<List<ProjectDto>> GetProjectsByManagerAsync(Guid managerId);

        /// <summary>
        /// 클라이언트별 프로젝트 목록 조회
        /// </summary>
        Task<List<ProjectDto>> GetProjectsByClientAsync(Guid clientId);

        /// <summary>
        /// 직원 프로젝트 할당
        /// </summary>
        Task<ProjectAssignmentDto?> AssignEmployeeAsync(AssignEmployeeRequest request);

        /// <summary>
        /// 직원 프로젝트 할당 해제
        /// </summary>
        Task<bool> UnassignEmployeeAsync(Guid assignmentId);

        /// <summary>
        /// 프로젝트 팀원 목록 조회
        /// </summary>
        Task<List<ProjectAssignmentDto>> GetProjectTeamAsync(Guid projectId);

        /// <summary>
        /// 프로젝트 마일스톤 목록 조회
        /// </summary>
        Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(Guid projectId);

        /// <summary>
        /// 마일스톤 완료 처리
        /// </summary>
        Task<bool> CompleteMilestoneAsync(Guid milestoneId);

        /// <summary>
        /// 프로젝트 진행률 조회
        /// </summary>
        Task<decimal> GetProjectProgressAsync(Guid projectId);

        /// <summary>
        /// 프로젝트 통계 조회
        /// </summary>
        Task<ProjectStatisticsResponse> GetProjectStatisticsAsync();

        /// <summary>
        /// 프로젝트 수익성 분석
        /// </summary>
        Task<ProjectProfitabilityResponse> GetProjectProfitabilityAsync();

        /// <summary>
        /// 기술별 프로젝트 목록 조회
        /// </summary>
        Task<List<ProjectDto>> GetProjectsByTechnologyAsync(string technology);

        /// <summary>
        /// 프로젝트 위험도별 목록 조회
        /// </summary>
        Task<List<ProjectDto>> GetProjectsByRiskLevelAsync(RiskLevel riskLevel);

        /// <summary>
        /// 지연된 프로젝트 목록 조회
        /// </summary>
        Task<List<ProjectDto>> GetDelayedProjectsAsync();

        /// <summary>
        /// 프로젝트 대시보드 데이터 조회
        /// </summary>
        Task<ProjectDashboardData> GetProjectDashboardDataAsync();
    }

    /// <summary>
    /// 프로젝트 대시보드 데이터 모델
    /// </summary>
    public class ProjectDashboardData
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedThisMonth { get; set; }
        public int DelayedProjects { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<ProjectDto> RecentProjects { get; set; } = new();
        public List<ProjectDto> HighRiskProjects { get; set; } = new();
        public Dictionary<string, int> ProjectsByStatus { get; set; } = new();
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new();
    }
}