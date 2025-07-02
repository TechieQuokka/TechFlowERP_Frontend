using System.ComponentModel.DataAnnotations;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.Project
{
    /// <summary>
    /// 프로젝트 수정 요청 DTO
    /// </summary>
    public class UpdateProjectRequest : BaseRequest
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

        public ProjectType ProjectType { get; set; }

        [Required(ErrorMessage = "시작일은 필수입니다.")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "예산은 0 이상이어야 합니다.")]
        public decimal Budget { get; set; }

        [Range(0, 100, ErrorMessage = "수익률은 0~100% 사이여야 합니다.")]
        public decimal ProfitMargin { get; set; }

        public RiskLevel RiskLevel { get; set; }

        public ProjectStatus Status { get; set; }

        [Required(ErrorMessage = "프로젝트 매니저는 필수입니다.")]
        public Guid ManagerId { get; set; }

        public List<string> Technologies { get; set; } = new();
    }
}