using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests.Position
{
    /// <summary>
    /// 직급 생성 요청 DTO
    /// </summary>
    public class CreatePositionRequest : BaseRequest
    {
        [Required(ErrorMessage = "직급명은 필수입니다.")]
        [StringLength(100, ErrorMessage = "직급명은 100자를 초과할 수 없습니다.")]
        public string Title { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "레벨은 1~10 사이여야 합니다.")]
        public int Level { get; set; } = 1;

        [Range(0, double.MaxValue, ErrorMessage = "최소 급여는 0 이상이어야 합니다.")]
        public decimal MinSalary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "최대 급여는 0 이상이어야 합니다.")]
        public decimal MaxSalary { get; set; }

        /// <summary>
        /// 최소 급여가 최대 급여보다 작은지 검증
        /// </summary>
        public bool IsValidSalaryRange => MinSalary <= MaxSalary;
    }
}