using System;
using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.Requests
{
    /// <summary>
    /// 모든 API 요청의 기본 클래스
    /// </summary>
    public abstract class BaseRequest
    {
        [StringLength(36, ErrorMessage = "테넌트 ID는 36자를 초과할 수 없습니다.")]
        public string TenantId { get; set; } = "default-tenant";
        public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
    }
}