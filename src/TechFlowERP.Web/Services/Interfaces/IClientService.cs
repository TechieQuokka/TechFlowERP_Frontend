using TechFlowERP.Models.DTOs.Client;
using TechFlowERP.Models.DTOs.Project;
using TechFlowERP.Models.Requests.Client;
using TechFlowERP.Models.Responses.Client;

namespace TechFlowERP.Web.Services.Interfaces
{
    /// <summary>
    /// 클라이언트 관리 서비스 인터페이스
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// 클라이언트 목록 조회 (페이징)
        /// </summary>
        Task<ClientListResponse> GetClientsAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null);

        /// <summary>
        /// 클라이언트 상세 조회
        /// </summary>
        Task<ClientDto?> GetClientByIdAsync(Guid clientId);

        /// <summary>
        /// 클라이언트 생성
        /// </summary>
        Task<ClientDto?> CreateClientAsync(CreateClientRequest request);

        /// <summary>
        /// 클라이언트 수정
        /// </summary>
        Task<ClientDto?> UpdateClientAsync(Guid clientId, UpdateClientRequest request);

        /// <summary>
        /// 클라이언트 삭제
        /// </summary>
        Task<bool> DeleteClientAsync(Guid clientId);

        /// <summary>
        /// 클라이언트 검색
        /// </summary>
        Task<List<ClientDto>> SearchClientsAsync(string searchTerm);

        /// <summary>
        /// 클라이언트의 프로젝트 목록 조회
        /// </summary>
        Task<List<ProjectDto>> GetClientProjectsAsync(Guid clientId);

        /// <summary>
        /// 업종별 클라이언트 통계
        /// </summary>
        Task<Dictionary<string, int>> GetClientsByIndustryAsync();
    }
}