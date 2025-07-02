using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechFlowERP.Models.Requests.Department
{
    /// <summary>
    /// 부서장 할당 요청
    /// </summary>
    public class AssignManagerRequest : BaseRequest
    {
        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public Guid ManagerId { get; set; }

        [StringLength(200, ErrorMessage = "할당 사유는 200자를 초과할 수 없습니다.")]
        public string? Reason { get; set; }
    }
}
