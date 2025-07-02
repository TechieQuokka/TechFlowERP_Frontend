using Radzen.Blazor;
using Radzen;

namespace TechFlowERP.Shared.Models.Charts
{
    /// <summary>
    /// Radzen.Blazor Charts 공통 옵션 설정 클래스
    /// IT 서비스업 ERP 시스템에 최적화된 차트 설정
    /// </summary>
    public static class ChartOptions
    {
        /// <summary>
        /// 대시보드용 차트 기본 설정
        /// </summary>
        public static class Dashboard
        {
            public const int DefaultHeight = 300;
            public const int DefaultWidth = 400;
            public const bool ShowLegend = true;
            public const bool ShowTooltip = true;
            public const bool Smooth = true;
        }

        /// <summary>
        /// 리포트용 차트 기본 설정
        /// </summary>
        public static class Report
        {
            public const int DefaultHeight = 500;
            public const int DefaultWidth = 800;
            public const bool ShowLegend = true;
            public const bool ShowTooltip = true;
            public const bool Smooth = true;
        }

        /// <summary>
        /// 프로젝트 상태 파이차트 설정
        /// database의 projects 테이블 기반
        /// </summary>
        public static class ProjectStatusPie
        {
            public const int Height = 300;
            public const int Width = 300;

            public static readonly string[] Colors = new[]
            {
                ChartColors.ProjectStatus["Planning"],
                ChartColors.ProjectStatus["Active"],
                ChartColors.ProjectStatus["OnHold"],
                ChartColors.ProjectStatus["Completed"],
                ChartColors.ProjectStatus["Cancelled"]
            };

            public static readonly string[] Labels = new[]
            {
                "계획중", "진행중", "보류", "완료", "취소"
            };
        }

        /// <summary>
        /// 매출 추이 라인차트 설정
        /// database의 invoices 테이블 기반
        /// </summary>
        public static class RevenueLine
        {
            public const int Height = 300;
            public const int Width = 600;
            public const int StrokeWidth = 3;
            public const bool Smooth = true;

            public static readonly string[] Colors = new[]
            {
                ChartColors.Financial["Revenue"]
            };

            public const string XAxisTitle = "기간";
            public const string YAxisTitle = "매출 (USD)";
            public const string XAxisFormat = "MMM yyyy";
            public const string YAxisFormat = "${0:N0}";
        }

        /// <summary>
        /// 직원 활용도 바차트 설정
        /// database의 employee_utilization 뷰 기반
        /// </summary>
        public static class UtilizationBar
        {
            public const int Height = 350;
            public const int Width = 600;

            public static readonly string[] Colors = new[]
            {
                ChartColors.Utilization["Billable"],
                ChartColors.Utilization["NonBillable"]
            };

            public static readonly string[] SeriesNames = new[]
            {
                "빌러블 시간", "논빌러블 시간"
            };

            public const string XAxisTitle = "직원";
            public const string YAxisTitle = "시간 (h)";
            public const string YAxisFormat = "{0}h";
        }

        /// <summary>
        /// 기술 스택 도넛차트 설정
        /// database의 employee_skills 테이블 기반
        /// </summary>
        public static class TechStackDonut
        {
            public const int Height = 300;
            public const int Width = 300;
            public const double InnerRadius = 0.6; // 도넛 홀 크기 (0.0 = 파이, 1.0 = 링)

            // 기술 스택별 색상은 동적으로 설정
            public static string[] GetTechStackColors(string[] techStacks)
            {
                var colors = new List<string>();
                foreach (var tech in techStacks)
                {
                    if (ChartColors.TechStack.ContainsKey(tech))
                    {
                        colors.Add(ChartColors.TechStack[tech]);
                    }
                    else
                    {
                        colors.Add(ChartColors.TechStack["Other"]);
                    }
                }
                return colors.ToArray();
            }
        }

        /// <summary>
        /// 프로젝트 수익성 스캐터 차트 설정
        /// database의 project_profitability 뷰 기반
        /// </summary>
        public static class ProfitabilityScatter
        {
            public const int Height = 400;
            public const int Width = 600;

            public static readonly string[] Colors = new[]
            {
                ChartColors.Financial["Profit"]
            };

            public const string XAxisTitle = "프로젝트 예산 (USD)";
            public const string YAxisTitle = "수익률 (%)";
            public const string XAxisFormat = "${0:N0}";
            public const string YAxisFormat = "{0}%";
        }

        /// <summary>
        /// 클라이언트 규모별 분포 차트 설정
        /// database의 clients 테이블 기반
        /// </summary>
        public static class ClientSizeDistribution
        {
            public const int Height = 250;
            public const int Width = 400;

            public static readonly string[] Colors = new[]
            {
                ChartColors.ClientSize["Small"],
                ChartColors.ClientSize["Medium"],
                ChartColors.ClientSize["Large"]
            };

            public static readonly string[] Labels = new[]
            {
                "소규모", "중간규모", "대규모"
            };
        }

        /// <summary>
        /// 프로젝트 위험도 분석 차트 설정
        /// database의 projects.risk_level 기반
        /// </summary>
        public static class RiskAnalysis
        {
            public const int Height = 250;
            public const int Width = 400;

            public static readonly string[] Colors = new[]
            {
                ChartColors.RiskLevel["Low"],
                ChartColors.RiskLevel["Medium"],
                ChartColors.RiskLevel["High"]
            };

            public static readonly string[] Labels = new[]
            {
                "낮음", "보통", "높음"
            };
        }

        /// <summary>
        /// KPI 게이지 차트 설정
        /// </summary>
        public static class KPIGauge
        {
            public const int Height = 200;
            public const int Width = 200;
            public const double Min = 0;
            public const double Max = 100;

            public static readonly string[] Colors = new[]
            {
                ChartColors.KPI["Critical"],  // 0-20%
                ChartColors.KPI["Poor"],      // 20-40%
                ChartColors.KPI["Average"],   // 40-60%
                ChartColors.KPI["Good"],      // 60-80%
                ChartColors.KPI["Excellent"] // 80-100%
            };

            public static readonly double[] Ranges = new[] { 20.0, 40.0, 60.0, 80.0, 100.0 };
        }

        /// <summary>
        /// 차트 공통 스타일 설정
        /// </summary>
        public static class CommonStyles
        {
            public const string FontFamily = "Segoe UI, system-ui, sans-serif";
            public const int FontSize = 12;
            public const int TitleFontSize = 14;
            public const int LegendFontSize = 11;

            // 다크 테마 색상
            public const string DarkBackground = "#1a1a1a";
            public const string DarkTextColor = "#ffffff";
            public const string DarkGridColor = "#404040";

            // 라이트 테마 색상
            public const string LightBackground = "#ffffff";
            public const string LightTextColor = "#333333";
            public const string LightGridColor = "#e0e0e0";
        }

        /// <summary>
        /// 반응형 차트 크기 계산
        /// </summary>
        public static class Responsive
        {
            public static int GetWidth(int baseWidth, string screenSize)
            {
                return screenSize switch
                {
                    "xs" => Math.Min(baseWidth, 300),  // 모바일
                    "sm" => Math.Min(baseWidth, 400),  // 태블릿 세로
                    "md" => Math.Min(baseWidth, 600),  // 태블릿 가로
                    "lg" => baseWidth,                 // 데스크톱
                    "xl" => baseWidth,                 // 큰 데스크톱
                    _ => baseWidth
                };
            }

            public static int GetHeight(int baseHeight, string screenSize)
            {
                return screenSize switch
                {
                    "xs" => Math.Min(baseHeight, 200),  // 모바일
                    "sm" => Math.Min(baseHeight, 250),  // 태블릿 세로
                    "md" => Math.Min(baseHeight, 300),  // 태블릿 가로
                    "lg" => baseHeight,                 // 데스크톱
                    "xl" => baseHeight,                 // 큰 데스크톱
                    _ => baseHeight
                };
            }
        }

        /// <summary>
        /// 애니메이션 설정
        /// </summary>
        public static class Animation
        {
            public const int Duration = 750;
            public const string Easing = "ease-in-out";
            public const bool EnableOnLoad = true;
            public const bool EnableOnUpdate = true;
        }

        /// <summary>
        /// 툴팁 포맷터
        /// </summary>
        public static class Tooltip
        {
            public static string FormatCurrency(double value)
            {
                return $"${value:N0}";
            }

            public static string FormatPercentage(double value)
            {
                return $"{value:F1}%";
            }

            public static string FormatHours(double value)
            {
                return $"{value:F1}h";
            }

            public static string FormatCount(double value)
            {
                return $"{value:F0}개";
            }
        }
    }
}