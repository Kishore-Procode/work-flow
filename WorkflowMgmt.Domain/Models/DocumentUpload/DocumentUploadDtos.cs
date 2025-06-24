using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Models.DocumentUpload
{
    public class DocumentUploadDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string OriginalFilename { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string UploadStatus { get; set; } = "pending";
        public Guid? UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? ExtractedContent { get; set; }
        public string? ProcessingNotes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class CreateDocumentUploadDto
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string OriginalFilename { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        [Required]
        [MaxLength(100)]
        public string FileType { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        [MaxLength(50)]
        public string UploadStatus { get; set; } = "pending";

        public Guid? UploadedBy { get; set; }
    }

    public class UpdateDocumentUploadDto
    {
        [MaxLength(50)]
        public string? UploadStatus { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public string? ExtractedContent { get; set; }

        public string? ProcessingNotes { get; set; }
    }
}
