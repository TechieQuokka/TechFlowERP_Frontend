using System.ComponentModel.DataAnnotations;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.Employee
{
    /// <summary>
    /// 직원 기술 추가 요청 DTO
    /// </summary>
    public class AddEmployeeSkillRequest : BaseRequest
    {
        [Required(ErrorMessage = "직원 ID는 필수입니다.")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "기술명은 필수입니다.")]
        [StringLength(50, ErrorMessage = "기술명은 50자를 초과할 수 없습니다.")]
        public string Technology { get; set; } = string.Empty;

        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;

        [Range(0, 50, ErrorMessage = "경력은 0~50년 사이여야 합니다.")]
        public int YearsExperience { get; set; }

        public DateTime? LastUsedDate { get; set; }

        [StringLength(200, ErrorMessage = "자격증명은 200자를 초과할 수 없습니다.")]
        public string Certification { get; set; } = string.Empty;
    }
}