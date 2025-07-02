namespace TechFlowERP.Models.DTOs
{
    /// <summary>
    /// 브레드크럼 항목 모델 (MudBlazor의 BreadcrumbItem과 구별하기 위해 네임스페이스 명시)
    /// </summary>
    public class BreadcrumbItem
    {
        public string Text { get; set; } = string.Empty;
        public string? Href { get; set; }
        public string Icon { get; set; } = string.Empty;

        public BreadcrumbItem()
        {
        }

        public BreadcrumbItem(string text, string? href = null, string icon = "")
        {
            Text = text;
            Href = href;
            Icon = icon;
        }
    }
}