using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.Department
{
    /// <summary>
    /// 부서 생성 요청 DTO
    /// </summary>
    public class CreateDepartmentRequest : BaseRequest
    {
        [Required(ErrorMessage = "부서명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "부서명은 100자를 초과할 수 없습니다.")]
        public string Name { get; set; } = string.Empty;

        public Guid? ManagerId { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "예산은 0 이상이어야 합니다.")]
        public decimal Budget { get; set; }

        [StringLength(500, ErrorMessage = "설명은 500자를 초과할 수 없습니다.")]
        public string? Description { get; set; }
    }
}