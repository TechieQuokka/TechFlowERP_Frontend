using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.Employee
{
    /// <summary>
    /// 직원 수정 요청 DTO
    /// </summary>
    public class UpdateEmployeeRequest : BaseRequest
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
        public DateTime HireDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "급여는 0 이상이어야 합니다.")]
        public decimal Salary { get; set; }

        [Range(0, 365, ErrorMessage = "연차 잔여일수는 0~365일 사이여야 합니다.")]
        public int LeaveBalance { get; set; }

        public bool IsActive { get; set; } = true;
    }
}