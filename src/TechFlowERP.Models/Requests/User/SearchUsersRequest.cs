using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.Requests.User
{
    /// <summary>
    /// 사용자 검색 요청
    /// </summary>
    public class SearchUsersRequest : BaseRequest
    {
        public string? SearchTerm { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsLocked { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? Department { get; set; }

        // 페이징
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // 정렬
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }
}
