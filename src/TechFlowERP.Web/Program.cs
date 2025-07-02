using Blazored.LocalStorage;
using Blazored.Toast;
using FluentValidation;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Radzen;
using TechFlowERP.Models.Configuration;
using TechFlowERP.Models.DTOs.Auth;
using TechFlowERP.Web.Authentication;
using TechFlowERP.Web.Components;
using TechFlowERP.Web.Services.Implementation;
using TechFlowERP.Web.Services.Interfaces;
using System.Text.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🎨 Blazor 서비스 추가
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 🎨 MudBlazor 서비스 추가
builder.Services.AddMudServices();

// 📊 Radzen 서비스 추가
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// 🍞 Toast 알림 서비스 추가
builder.Services.AddBlazoredToast();

// 💾 로컬 스토리지 서비스 추가
builder.Services.AddBlazoredLocalStorage();

// 🔄 Fluxor 상태 관리 추가
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    // DevTools는 나중에 필요시 추가
});

// ⚙️ 설정 바인딩
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Authentication:JwtSettings"));

// 🔗 인증 토큰 핸들러 추가
builder.Services.AddScoped<AuthTokenHandler>();

// 🔗 로그인 전용 HttpClient (인증 없음)
builder.Services.AddHttpClient("LoginClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/api/v1/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "TechFlowERP-Frontend/1.0");
});

// 🔗 메인 API HttpClient (토큰 핸들러 추가)
builder.Services.AddHttpClient("TechFlowERP_API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/api/v1/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "TechFlowERP-Frontend/1.0");
})
.AddHttpMessageHandler<AuthTokenHandler>();

// 🔗 기본 HttpClient도 등록
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("TechFlowERP_API"));

// 🔐 Blazor용 Authentication 설정
builder.Services.AddAuthentication("BlazorAuth")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, BlazorAuthenticationHandler>(
        "BlazorAuth", options => { });

// 🔐 인증 서비스 등록
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();

// 🔐 Authorization 서비스 추가
builder.Services.AddAuthorizationCore();

// 🎯 애플리케이션 서비스들 등록
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITimeEntryService, TimeEntryService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();

// 🆕 관리자 서비스 추가
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IDepartmentManagementService, DepartmentManagementService>();

// ✅ FluentValidation 활성화
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// 📊 Logging 설정
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 🌐 CORS 정책 추가 (개발 환경용)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

var app = builder.Build();

// 🌐 HTTP 파이프라인 설정
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseCors(); // 개발 환경에서 CORS 허용
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// 🔐 인증 및 권한
app.UseAuthentication();
app.UseAuthorization();

// 🎯 Blazor 라우팅 설정
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// 🚀 애플리케이션 실행
app.Run();

// 🔒 인증 토큰 핸들러 클래스
public class AuthTokenHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthTokenHandler> _logger;
    private static string? _cachedToken;

    public AuthTokenHandler(IServiceProvider serviceProvider, ILogger<AuthTokenHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 토큰이 없으면 로그인 시도
        if (string.IsNullOrEmpty(_cachedToken))
        {
            await LoginAndCacheToken();
        }

        // 토큰을 헤더에 추가
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // 401 응답이면 토큰 갱신 후 재시도
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("401 응답 받음. 토큰 갱신 후 재시도...");
            await LoginAndCacheToken();

            if (!string.IsNullOrEmpty(_cachedToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }

    private async Task LoginAndCacheToken()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var loginClient = httpClientFactory.CreateClient("LoginClient");

            var loginRequest = new
            {
                email = "admin@erp.com",
                password = "admin123",
                tenantId = "default-tenant"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("토큰 갱신을 위한 로그인 시도...");
            var loginResponse = await loginClient.PostAsync("auth/login", content);

            if (loginResponse.IsSuccessStatusCode)
            {
                var loginResult = await loginResponse.Content.ReadAsStringAsync();
                var loginData = JsonSerializer.Deserialize<LoginResponseDto>(loginResult,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!string.IsNullOrEmpty(loginData?.Token))
                {
                    _cachedToken = loginData.Token;
                    _logger.LogInformation("토큰 갱신 성공");
                }
                else
                {
                    _logger.LogWarning("로그인 응답에서 토큰을 찾을 수 없음");
                }
            }
            else
            {
                var errorContent = await loginResponse.Content.ReadAsStringAsync();
                _logger.LogError("토큰 갱신 실패: {StatusCode}, {Error}", loginResponse.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "토큰 갱신 중 오류 발생");
        }
    }
}