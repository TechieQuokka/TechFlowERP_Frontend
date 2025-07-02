using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.Department
{
    /// <summary>
     /// 부서 병합 요청
     /// </summary>
    public class MergeDepartmentsRequest : BaseRequest
    {
        [Required]
        public Guid SourceDepartmentId { get; set; }

        [Required]
        public Guid TargetDepartmentId { get; set; }

        [StringLength(200, ErrorMessage = "병합 사유는 200자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }

        public bool TransferBudget { get; set; } = true;
        public bool TransferEmployees { get; set; } = true;
    }
}
