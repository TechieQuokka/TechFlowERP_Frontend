using TechFlowERP.Models.Responses;

namespace TechFlowERP.Models.Common
{
    /// <summary>
    /// 페이징된 결과 모델
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new(); // 기존 구현과 일치
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }
}