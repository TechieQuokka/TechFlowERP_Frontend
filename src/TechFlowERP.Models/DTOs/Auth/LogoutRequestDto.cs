namespace TechFlowERP.Models.DTOs.Auth
{
    /// <summary>
    /// 로그아웃 요청 DTO
    /// </summary>
    public class LogoutRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public bool LogoutFromAllDevices { get; set; } = false;
    }
}