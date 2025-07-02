using System;
using System.Collections.Generic;

namespace TechFlowERP.Models.Responses
{
    /// <summary>
    /// 모든 API 응답의 기본 클래스
    /// </summary>
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ResponseTimestamp { get; set; } = DateTime.UtcNow;
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// 데이터를 포함하는 API 응답 클래스
    /// </summary>
    public class BaseResponse<T> : BaseResponse
    {
        public T? Data { get; set; }
    }
}