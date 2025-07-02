using System.ComponentModel.DataAnnotations;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.Project
{
    /// <summary>
    /// 프로젝트 생성 요청 DTO
    /// </summary>
    public class CreateProjectRequest : BaseRequest
    {
        [Required(ErrorMessage = "프로젝트 코드는 필수입니다.")]
        [StringLength(50, ErrorMessage = "프로젝트 코드는 50자를 초과할 수 없습니다.")]
        public string ProjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "프로젝트명은 필수입니다.")]
        [StringLength(200, ErrorMessage = "프로젝트명은 200자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "클라이언트는 필수입니다.")]
        public Guid ClientId { get; set; }

        public ProjectType ProjectType { get; set; } = ProjectType.TimeAndMaterial;

        [Required(ErrorMessage = "시작일은 필수입니다.")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "예산은 0 이상이어야 합니다.")]
        public decimal Budget { get; set; }

        [Range(0, 100, ErrorMessage = "수익률은 0~100% 사이여야 합니다.")]
        public decimal ProfitMargin { get; set; } = 20;

        public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;

        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        [Required(ErrorMessage = "프로젝트 매니저는 필수입니다.")]
        public Guid ManagerId { get; set; }

        public List<string> Technologies { get; set; } = new();

        // 초기 마일스톤 (선택사항)
        public List<CreateMilestoneRequest> Milestones { get; set; } = new();

        // 초기 팀 할당 (선택사항)
        public List<CreateProjectAssignmentRequest> TeamAssignments { get; set; } = new();
    }

    /// <summary>
    /// 마일스톤 생성 요청
    /// </summary>
    public class CreateMilestoneRequest
    {
        [Required(ErrorMessage = "마일스톤명은 필수입니다.")]
        [StringLength(200, ErrorMessage = "마일스톤명은 200자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "완료 예정일은 필수입니다.")]
        public DateTime DueDate { get; set; }

        public string Deliverables { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "지불 비율은 0~100% 사이여야 합니다.")]
        public decimal PaymentPercentage { get; set; }
    }

    /// <summary>
    /// 프로젝트 할당 생성 요청
    /// </summary>
    public class CreateProjectAssignmentRequest
    {
        [Required(ErrorMessage = "직원은 필수입니다.")]
        public Guid EmployeeId { get; set; }

        [StringLength(100, ErrorMessage = "역할은 100자를 초과할 수 없습니다.")]
        public string Role { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "할당률은 1~100% 사이여야 합니다.")]
        public int AllocationPercentage { get; set; } = 100;

        [Required(ErrorMessage = "시작일은 필수입니다.")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "시간당 요금은 0 이상이어야 합니다.")]
        public decimal HourlyRate { get; set; }
    }
}