using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string? description { get; set; }
        public string[] permissions { get; set; } = Array.Empty<string>();
    }
}
