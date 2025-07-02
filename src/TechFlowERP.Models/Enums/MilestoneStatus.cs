namespace TechFlowERP.Models.Enums
{
    /// <summary>
    /// 마일스톤 상태 (데이터베이스 project_milestones.status와 일치)
    /// </summary>
    public enum MilestoneStatus
    {
        Pending,
        InProgress,
        Completed,
        Delayed
    }
}