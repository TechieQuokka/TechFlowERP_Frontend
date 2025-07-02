using System;

namespace TechFlowERP.Models.DTOs
{
    /// <summary>
    /// 모든 DTO의 기본 클래스
    /// </summary>
    public abstract class BaseDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TenantId { get; set; } = "default-tenant";
    }
}