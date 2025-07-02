using TechFlowERP.Models.DTOs.Client;
using TechFlowERP.Models.Responses;
using TechFlowERP.Models.Common;

namespace TechFlowERP.Models.Responses.Client
{
    /// <summary>
    /// 클라이언트 목록 응답 DTO
    /// </summary>
    public class ClientListResponse : BaseResponse<PagedResult<ClientDto>>
    {
        /// <summary>
        /// 편의를 위한 Items 속성 (Data.Items의 shortcut)
        /// </summary>
        public List<ClientDto> Items => Data?.Items ?? new List<ClientDto>();

        /// <summary>
        /// 편의를 위한 TotalCount 속성 (Data.TotalCount의 shortcut)
        /// </summary>
        public int TotalCount => Data?.TotalCount ?? 0;

        /// <summary>
        /// 편의를 위한 TotalPages 속성 (Data.TotalPages의 shortcut)
        /// </summary>
        public int TotalPages => Data?.TotalPages ?? 0;

        /// <summary>
        /// 편의를 위한 PageNumber 속성 (Data.PageNumber의 shortcut)
        /// </summary>
        public int PageNumber => Data?.PageNumber ?? 1;

        /// <summary>
        /// 편의를 위한 PageSize 속성 (Data.PageSize의 shortcut)
        /// </summary>
        public int PageSize => Data?.PageSize ?? 20;

        /// <summary>
        /// 편의를 위한 HasNextPage 속성 (Data.HasNextPage의 shortcut)
        /// </summary>
        public bool HasNextPage => Data?.HasNextPage ?? false;

        /// <summary>
        /// 편의를 위한 HasPreviousPage 속성 (Data.HasPreviousPage의 shortcut)
        /// </summary>
        public bool HasPreviousPage => Data?.HasPreviousPage ?? false;

        /// <summary>
        /// 성공적인 클라이언트 목록 응답 생성
        /// </summary>
        /// <param name="clients">클라이언트 목록</param>
        /// <param name="totalCount">전체 클라이언트 수</param>
        /// <param name="pageNumber">현재 페이지 번호</param>
        /// <param name="pageSize">페이지 크기</param>
        /// <param name="message">응답 메시지</param>
        /// <returns>클라이언트 목록 응답</returns>
        public static ClientListResponse CreateSuccess(
            List<ClientDto> clients,
            int totalCount,
            int pageNumber,
            int pageSize,
            string message = "클라이언트 목록을 성공적으로 조회했습니다.")
        {
            var pagedResult = new PagedResult<ClientDto>
            {
                Items = clients ?? new List<ClientDto>(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return new ClientListResponse
            {
                Success = true,
                Message = message,
                Data = pagedResult,
                ResponseTimestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// 빈 클라이언트 목록 응답 생성
        /// </summary>
        /// <param name="pageNumber">현재 페이지 번호</param>
        /// <param name="pageSize">페이지 크기</param>
        /// <param name="message">응답 메시지</param>
        /// <returns>빈 클라이언트 목록 응답</returns>
        public static ClientListResponse CreateEmpty(
            int pageNumber = 1,
            int pageSize = 20,
            string message = "조회된 클라이언트가 없습니다.")
        {
            return CreateSuccess(new List<ClientDto>(), 0, pageNumber, pageSize, message);
        }

        /// <summary>
        /// 실패한 클라이언트 목록 응답 생성
        /// </summary>
        /// <param name="message">에러 메시지</param>
        /// <param name="errors">에러 목록</param>
        /// <returns>실패 응답</returns>
        public static ClientListResponse CreateError(string message, List<string>? errors = null)
        {
            return new ClientListResponse
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = errors ?? new List<string>(),
                ResponseTimestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// 전체 컬렉션으로부터 페이지된 클라이언트 목록 응답 생성
        /// </summary>
        /// <param name="allClients">전체 클라이언트 목록</param>
        /// <param name="pageNumber">페이지 번호</param>
        /// <param name="pageSize">페이지 크기</param>
        /// <param name="message">응답 메시지</param>
        /// <returns>페이지된 클라이언트 목록 응답</returns>
        public static ClientListResponse CreateFromCollection(
            IEnumerable<ClientDto> allClients,
            int pageNumber,
            int pageSize,
            string message = "클라이언트 목록을 성공적으로 조회했습니다.")
        {
            var clientList = allClients?.ToList() ?? new List<ClientDto>();
            var totalCount = clientList.Count;
            var pagedClients = clientList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return CreateSuccess(pagedClients, totalCount, pageNumber, pageSize, message);
        }
    }
}