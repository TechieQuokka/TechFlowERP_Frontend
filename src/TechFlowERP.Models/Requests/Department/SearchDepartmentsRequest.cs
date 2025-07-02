using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechFlowERP.Models.Requests.Department
{
    /// <summary>
    /// 부서 검색 요청
    /// </summary>
    public class SearchDepartmentsRequest : BaseRequest
    {
        public string? SearchTerm { get; set; }
        public Guid? ManagerId { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public bool? HasManager { get; set; }
        public int? MinEmployeeCount { get; set; }
        public int? MaxEmployeeCount { get; set; }

        // 예산 사용률 필터
        public decimal? MinBudgetUtilization { get; set; }
        public decimal? MaxBudgetUtilization { get; set; }

        // 생성일 필터
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }

        // 페이징
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // 정렬
        public string SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
    }
}
