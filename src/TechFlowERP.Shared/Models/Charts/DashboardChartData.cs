namespace TechFlowERP.Shared.Models.Charts
{
    /// <summary>
    /// 프로젝트 상태 차트 데이터
    /// database의 projects 테이블 기반
    /// </summary>
    public class ProjectStatusChartData
    {
        public string Status { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    /// <summary>
    /// 매출 추이 차트 데이터
    /// database의 invoices 테이블 기반
    /// </summary>
    public class RevenueChartData
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Period { get; set; } = string.Empty; // "2024-01", "Q1 2024" 등
        public string Category { get; set; } = "Revenue"; // Revenue, Cost, Profit
    }

    /// <summary>
    /// 직원 활용도 차트 데이터
    /// database의 employee_utilization 뷰 기반
    /// </summary>
    public class UtilizationChartData
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public double BillableHours { get; set; }
        public double NonBillableHours { get; set; }
        public double TotalHours { get; set; }
        public double UtilizationRate { get; set; }
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// 기술 스택 분포 차트 데이터
    /// database의 employee_skills 테이블 기반
    /// </summary>
    public class TechStackChartData
    {
        public string Technology { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public double Percentage { get; set; }
        public string SkillLevel { get; set; } = string.Empty; // Beginner, Intermediate, Expert
        public string Color { get; set; } = string.Empty;
    }

    /// <summary>
    /// 프로젝트 수익성 차트 데이터
    /// database의 project_profitability 뷰 기반
    /// </summary>
    public class ProfitabilityChartData
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public decimal Profit { get; set; }
        public double ProfitMargin { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 클라이언트 분포 차트 데이터
    /// database의 clients 테이블 기반
    /// </summary>
    public class ClientDistributionChartData
    {
        public string SizeCategory { get; set; } = string.Empty; // Small, Medium, Large
        public string SizeName { get; set; } = string.Empty; // 소규모, 중간규모, 대규모
        public int ClientCount { get; set; }
        public decimal TotalValue { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    /// <summary>
    /// KPI 게이지 차트 데이터
    /// </summary>
    public class KPIChartData
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
        public double Target { get; set; }
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 100;
        public string Unit { get; set; } = string.Empty; // %, USD, hours 등
        public string Status { get; set; } = string.Empty; // Excellent, Good, Average, Poor, Critical
        public string Color { get; set; } = string.Empty;
    }

    /// <summary>
    /// 시계열 차트 공통 데이터
    /// </summary>
    public class TimeSeriesChartData
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// 비교 차트 데이터 (바 차트, 컬럼 차트 등)
    /// </summary>
    public class ComparisonChartData
    {
        public string Category { get; set; } = string.Empty;
        public double Value { get; set; }
        public string SubCategory { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }

    /// <summary>
    /// 차트 공통 옵션 데이터
    /// </summary>
    public class ChartDataOptions
    {
        public string Title { get; set; } = string.Empty;
        public string XAxisTitle { get; set; } = string.Empty;
        public string YAxisTitle { get; set; } = string.Empty;
        public bool ShowLegend { get; set; } = true;
        public bool ShowTooltip { get; set; } = true;
        public bool IsAnimated { get; set; } = true;
        public string DateFormat { get; set; } = "MMM yyyy";
        public string ValueFormat { get; set; } = "{0:N0}";
        public string[] CustomColors { get; set; } = Array.Empty<string>();
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// 대시보드 전체 차트 데이터 컨테이너
    /// </summary>
    public class DashboardChartDataContainer
    {
        public List<ProjectStatusChartData> ProjectStatus { get; set; } = new();
        public List<RevenueChartData> Revenue { get; set; } = new();
        public List<UtilizationChartData> Utilization { get; set; } = new();
        public List<TechStackChartData> TechStack { get; set; } = new();
        public List<ProfitabilityChartData> Profitability { get; set; } = new();
        public List<ClientDistributionChartData> ClientDistribution { get; set; } = new();
        public List<KPIChartData> KPIs { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public string TenantId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 차트 로딩 상태 관리
    /// </summary>
    public class ChartLoadingState
    {
        public bool IsLoading { get; set; } = false;
        public bool HasError { get; set; } = false;
        public bool HasNoData { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime? LastLoadTime { get; set; }
        public int RetryCount { get; set; } = 0;
    }

    /// <summary>
    /// 차트 필터링 옵션
    /// </summary>
    public class ChartFilterOptions
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> SelectedProjects { get; set; } = new();
        public List<string> SelectedEmployees { get; set; } = new();
        public List<string> SelectedClients { get; set; } = new();
        public List<string> SelectedDepartments { get; set; } = new();
        public string GroupBy { get; set; } = "month"; // day, week, month, quarter, year
        public bool IncludeInactive { get; set; } = false;
    }
}