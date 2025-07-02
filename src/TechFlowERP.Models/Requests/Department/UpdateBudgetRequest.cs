using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.Department
{
    /// <summary>
    /// 부서 예산 수정 요청
    /// </summary>
    public class UpdateBudgetRequest : BaseRequest
    {
        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        [Range(0, 99999999.99, ErrorMessage = "예산은 0 이상이어야 합니다.")]
        public decimal NewBudget { get; set; }

        [StringLength(200, ErrorMessage = "변경 사유는 200자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }
    }
}
