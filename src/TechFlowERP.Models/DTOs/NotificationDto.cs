namespace TechFlowERP.Models.DTOs
{
    /// <summary>
    /// 알림 DTO
    /// </summary>
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public string? ActionUrl { get; set; }
        public string? Data { get; set; } // JSON 데이터
    }

    /// <summary>
    /// 알림 타입
    /// </summary>
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success,
        ProjectUpdate,
        TimeApproval,
        SystemNotice
    }
}