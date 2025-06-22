using System.ComponentModel.DataAnnotations;

namespace WorkFlow.Core.DTOs
{
    public class DepartmentWithTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? DefaultTemplateId { get; set; }
        public string? DefaultTemplateName { get; set; }
    }

    public class UpdateDepartmentDefaultTemplateRequest
    {
        [Required]
        public Guid DefaultTemplateId { get; set; }
    }
}
