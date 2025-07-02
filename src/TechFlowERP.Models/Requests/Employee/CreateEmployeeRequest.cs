using System.ComponentModel.DataAnnotations;
using TechFlowERP.Models.DTOs.Employee;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.Employee
{
    /// <summary>
    /// 직원 생성 요청 DTO
    /// </summary>
    public class CreateEmployeeRequest : BaseRequest
    {
        [Required(ErrorMessage = "이메일은 필수입니다.")]
        [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
        [StringLength(100, ErrorMessage = "이메일은 100자를 초과할 수 없습니다.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "이름은 필수입니다.")]
        [StringLength(100, ErrorMessage = "이름은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }

        public Guid? PositionId { get; set; }

        public Guid? ManagerId { get; set; }

        [Required(ErrorMessage = "입사일은 필수입니다.")]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Range(0, double.MaxValue, ErrorMessage = "급여는 0 이상이어야 합니다.")]
        public decimal Salary { get; set; }

        [Range(0, 365, ErrorMessage = "연차 잔여일수는 0~365일 사이여야 합니다.")]
        public int LeaveBalance { get; set; } = 15;

        public bool IsActive { get; set; } = true;

        // 초기 기술 스택 (선택사항)
        public List<CreateEmployeeSkillRequest> Skills { get; set; } = new();
    }

    /// <summary>
    /// 직원 기술 생성 요청
    /// </summary>
    public class CreateEmployeeSkillRequest
    {
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