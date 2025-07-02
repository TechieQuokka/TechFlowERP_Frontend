using System.ComponentModel.DataAnnotations;

namespace TechFlowERP.Models.DTOs.Auth
{
    /// <summary>
    /// 토큰 갱신 요청 DTO
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}