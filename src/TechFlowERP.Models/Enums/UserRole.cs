namespace TechFlowERP.Models.Enums
{
    /// <summary>
    /// 사용자 역할 (데이터베이스 users 테이블과 일치)
    /// </summary>
    public enum UserRole
    {
        Admin,
        Manager,
        Employee,
        Finance,
        HR,
        ReadOnly
    }
}