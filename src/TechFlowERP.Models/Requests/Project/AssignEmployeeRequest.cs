using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.Project
{
    /// <summary>
    /// 직원 프로젝트 할당 요청 DTO
    /// </summary>
    public class AssignEmployeeRequest : BaseRequest
    {
        [Required(ErrorMessage = "프로젝트 ID는 필수입니다.")]
        public Guid ProjectId { get; set; }

        [Required(ErrorMessage = "직원 ID는 필수입니다.")]
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