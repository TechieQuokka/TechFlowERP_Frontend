using System.ComponentModel.DataAnnotations;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.Client
{
    /// <summary>
    /// 클라이언트 수정 요청 DTO
    /// </summary>
    public class UpdateClientRequest : BaseRequest
    {
        [Required(ErrorMessage = "회사명은 필수입니다.")]
        [StringLength(200, ErrorMessage = "회사명은 200자를 초과할 수 없습니다.")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "업종은 100자를 초과할 수 없습니다.")]
        public string Industry { get; set; } = string.Empty;

        public CompanySizeCategory SizeCategory { get; set; } = CompanySizeCategory.Medium;

        [StringLength(100, ErrorMessage = "담당자명은 100자를 초과할 수 없습니다.")]
        public string ContactPerson { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다.")]
        [StringLength(100, ErrorMessage = "이메일은 100자를 초과할 수 없습니다.")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "올바른 전화번호 형식이 아닙니다.")]
        [StringLength(50, ErrorMessage = "전화번호는 50자를 초과할 수 없습니다.")]
        public string ContactPhone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "계약 금액은 0 이상이어야 합니다.")]
        public decimal ContractValue { get; set; }
    }
}