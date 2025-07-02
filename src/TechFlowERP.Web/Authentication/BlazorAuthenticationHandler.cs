using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TechFlowERP.Web.Authentication
{
    /// <summary>
    /// Blazor용 더미 인증 핸들러
    /// </summary>
    public class BlazorAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BlazorAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// 인증 처리 (실제로는 AuthenticationStateProvider가 처리)
        /// </summary>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // ✅ Name 클레임이 포함된 더미 Identity 생성
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "anonymous"),
                new Claim(ClaimTypes.Name, "Anonymous User") // ✅ Name 클레임 필수!
            };

            var identity = new ClaimsIdentity(claims, "BlazorAuth");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "BlazorAuth");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        /// <summary>
        /// Challenge 처리 (로그인 리다이렉트)
        /// </summary>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/login");
            return Task.CompletedTask;
        }
    }
}