using System;
using TechFlowERP.Models.Enums;

namespace TechFlowERP.Models.DTOs.Client
{
    /// <summary>
    /// 클라이언트 DTO (clients 테이블 기반)
    /// </summary>
    public class ClientDto : BaseDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public CompanySizeCategory SizeCategory { get; set; }
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal ContractValue { get; set; }

        // 추가 정보 (연관 데이터)
        public int ActiveProjectsCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime? LastProjectDate { get; set; }
    }
}