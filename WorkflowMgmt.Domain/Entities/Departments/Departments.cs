using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{   
    public class DepartmentDTO : BaseEntity
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string? description { get; set; }
        public string? head_of_department { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public int? established_year { get; set; }
        public string? programs_offered { get; set; }
        public string? accreditation { get; set; }
        public string status { get; set; } = "Active";
    }
}
