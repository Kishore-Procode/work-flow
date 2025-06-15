using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Infrastructure.Helpers
{
    public static class IPHelper
    {
        public static string GetClientIp(IHttpContextAccessor httpContextAccessor)
        {
            var ip = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return string.IsNullOrEmpty(ip) ? "Unknown" : ip;
        }

        public static string GetUserAgent(IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";
        }
    }
}
