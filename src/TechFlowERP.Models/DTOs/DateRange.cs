namespace TechFlowERP.Models.DTOs
{
    /// <summary>
    /// 날짜 범위 모델
    /// </summary>
    public class DateRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateRange()
        {
        }

        public DateRange(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// 유효한 날짜 범위인지 확인
        /// </summary>
        public bool IsValid => StartDate.HasValue && EndDate.HasValue && StartDate <= EndDate;

        /// <summary>
        /// 날짜 범위의 일수 계산
        /// </summary>
        public int Days => IsValid ? (EndDate!.Value - StartDate!.Value).Days + 1 : 0;

        /// <summary>
        /// 특정 날짜가 범위에 포함되는지 확인
        /// </summary>
        public bool Contains(DateTime date)
        {
            return IsValid && date.Date >= StartDate!.Value.Date && date.Date <= EndDate!.Value.Date;
        }

        public override string ToString()
        {
            if (!IsValid) return "Invalid Range";
            return $"{StartDate:yyyy-MM-dd} ~ {EndDate:yyyy-MM-dd}";
        }
    }

    /// <summary>
    /// 날짜 범위 타입
    /// </summary>
    public enum DateRangeType
    {
        Today,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisYear
    }
}