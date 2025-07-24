using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{
    public class LevelDTO : BaseEntity
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string? description { get; set; }
        public int sort_order { get; set; } = 0;
        public bool is_active { get; set; } = true;
    }

    public class LevelStatsDto
    {
        public int TotalLevels { get; set; }
        public int ActiveLevels { get; set; }
    }
}
