namespace TechFlowERP.Shared.Models.Charts
{

    /// <summary>
    /// Chart 컴포넌트에서 사용할 색상 팔레트 정의
    /// IT 서비스업 특화 ERP 시스템 테마에 맞는 색상 구성
    /// </summary>
    public static class ChartColors
    {
        /// <summary>
        /// 기본 색상 팔레트 (프로젝트 상태, 일반적인 차트용)
        /// </summary>
        public static readonly string[] Primary = new[]
        {
            "#1f77b4", // 블루 (Active 프로젝트)
            "#ff7f0e", // 오렌지 (Planning 프로젝트) 
            "#2ca02c", // 그린 (Completed 프로젝트)
            "#d62728", // 레드 (Cancelled/OnHold 프로젝트)
            "#9467bd", // 퍼플 (기타)
            "#8c564b", // 브라운
            "#e377c2", // 핑크
            "#7f7f7f", // 그레이
            "#bcbd22", // 올리브
            "#17becf"  // 시안
        };

        /// <summary>
        /// 프로젝트 상태별 전용 색상
        /// database의 projects.status ENUM과 매핑
        /// </summary>
        public static readonly Dictionary<string, string> ProjectStatus = new()
        {
            { "Planning", "#ff7f0e" },    // 오렌지 - 계획중
            { "Active", "#1f77b4" },      // 블루 - 진행중  
            { "OnHold", "#ffbb78" },      // 연한 오렌지 - 보류
            { "Completed", "#2ca02c" },   // 그린 - 완료
            { "Cancelled", "#d62728" }    // 레드 - 취소
        };

        /// <summary>
        /// 재무 관련 차트 색상 (수익, 비용, 매출 등)
        /// </summary>
        public static readonly Dictionary<string, string> Financial = new()
        {
            { "Revenue", "#2ca02c" },     // 그린 - 매출
            { "Cost", "#d62728" },        // 레드 - 비용
            { "Profit", "#1f77b4" },      // 블루 - 이익
            { "Budget", "#ff7f0e" },      // 오렌지 - 예산
            { "Expense", "#9467bd" }      // 퍼플 - 지출
        };

        /// <summary>
        /// 직원 활용도 관련 색상
        /// </summary>
        public static readonly Dictionary<string, string> Utilization = new()
        {
            { "Billable", "#2ca02c" },    // 그린 - 빌러블 시간
            { "NonBillable", "#d62728" }, // 레드 - 논빌러블 시간
            { "Available", "#1f77b4" },   // 블루 - 가용 시간
            { "Overtime", "#ff7f0e" }     // 오렌지 - 초과근무
        };

        /// <summary>
        /// 기술 스택별 색상 (employees.skills 관련)
        /// </summary>
        public static readonly Dictionary<string, string> TechStack = new()
        {
            { "C#", "#239120" },
            { "JavaScript", "#f7df1e" },
            { "Python", "#3776ab" },
            { "Java", "#007396" },
            { "React", "#61dafb" },
            { "Angular", "#dd0031" },
            { "Vue", "#4fc08d" },
            { "Node.js", "#339933" },
            { "ASP.NET", "#512bd4" },
            { "Blazor", "#512bd4" },
            { "Other", "#7f7f7f" }
        };

        /// <summary>
        /// 클라이언트 규모별 색상 (clients.size_category와 매핑)
        /// </summary>
        public static readonly Dictionary<string, string> ClientSize = new()
        {
            { "Small", "#2ca02c" },   // 그린 - 소규모
            { "Medium", "#ff7f0e" },  // 오렌지 - 중간규모  
            { "Large", "#1f77b4" }    // 블루 - 대규모
        };

        /// <summary>
        /// 위험도별 색상 (projects.risk_level과 매핑)
        /// </summary>
        public static readonly Dictionary<string, string> RiskLevel = new()
        {
            { "Low", "#2ca02c" },     // 그린 - 낮음
            { "Medium", "#ff7f0e" },  // 오렌지 - 보통
            { "High", "#d62728" }     // 레드 - 높음
        };

        /// <summary>
        /// 그라데이션 색상 배열 (히트맵, 영역 차트 등에 사용)
        /// </summary>
        public static readonly string[] Gradient = new[]
        {
            "#ff9999", "#ffcc99", "#ffff99",
            "#ccff99", "#99ff99", "#99ffcc",
            "#99ffff", "#99ccff", "#9999ff"
        };

        /// <summary>
        /// 다크 테마용 색상 팔레트
        /// </summary>
        public static readonly string[] DarkTheme = new[]
        {
            "#6baed6", "#fd8d3c", "#74c476",
            "#fb6a4a", "#c994c7", "#d6616b",
            "#f768a1", "#a6a6a6", "#d4b13f",
            "#5cb3cc"
        };

        /// <summary>
        /// 성과 지표별 색상 (KPI 차트용)
        /// </summary>
        public static readonly Dictionary<string, string> KPI = new()
        {
            { "Excellent", "#2ca02c" },   // 그린 - 우수
            { "Good", "#74c476" },        // 연한 그린 - 양호
            { "Average", "#ff7f0e" },     // 오렌지 - 보통
            { "Poor", "#fd8d3c" },        // 연한 오렌지 - 미흡
            { "Critical", "#d62728" }     // 레드 - 위험
        };

        /// <summary>
        /// 특정 색상명으로 색상값 가져오기
        /// </summary>
        /// <param name="category">색상 카테고리 (ProjectStatus, Financial 등)</param>
        /// <param name="key">색상 키</param>
        /// <returns>16진수 색상값</returns>
        public static string GetColor(string category, string key)
        {
            return category switch
            {
                nameof(ProjectStatus) => ProjectStatus.GetValueOrDefault(key, Primary[0]),
                nameof(Financial) => Financial.GetValueOrDefault(key, Primary[0]),
                nameof(Utilization) => Utilization.GetValueOrDefault(key, Primary[0]),
                nameof(TechStack) => TechStack.GetValueOrDefault(key, Primary[0]),
                nameof(ClientSize) => ClientSize.GetValueOrDefault(key, Primary[0]),
                nameof(RiskLevel) => RiskLevel.GetValueOrDefault(key, Primary[0]),
                nameof(KPI) => KPI.GetValueOrDefault(key, Primary[0]),
                _ => Primary[0]
            };
        }

        /// <summary>
        /// 배열 인덱스로 기본 색상 가져오기
        /// </summary>
        /// <param name="index">색상 인덱스</param>
        /// <param name="isDarkTheme">다크테마 여부</param>
        /// <returns>16진수 색상값</returns>
        public static string GetColorByIndex(int index, bool isDarkTheme = false)
        {
            var colors = isDarkTheme ? DarkTheme : Primary;
            return colors[index % colors.Length];
        }
    }
}