using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechFlowERP.Models.Requests.Department
{
    /// <summary>
    /// 부서별 직원 이동 요청
    /// </summary>
    public class TransferEmployeeRequest : BaseRequest
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public Guid FromDepartmentId { get; set; }

        [Required]
        public Guid ToDepartmentId { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

        [StringLength(200, ErrorMessage = "이동 사유는 200자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }
    }
}
