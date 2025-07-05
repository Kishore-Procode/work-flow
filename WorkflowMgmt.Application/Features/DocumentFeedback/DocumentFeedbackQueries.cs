using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentFeedback
{
    // Queries
    public class GetFeedbackByDocumentsQuery : IRequest<ApiResponse<Dictionary<Guid, List<DocumentFeedbackDto>>>>
    {
        [Required]
        public List<Guid> DocumentIds { get; set; } = new();
        public string? DocumentType { get; set; }
    }

    public class GetDocumentFeedbackQuery : IRequest<ApiResponse<List<DocumentFeedbackDto>>>
    {
        [Required]
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } = "syllabus";
    }

    public class GetRecentFeedbackQuery : IRequest<ApiResponse<List<DocumentFeedbackDto>>>
    {
        public string? DocumentType { get; set; }
        public int Limit { get; set; } = 10;
    }
}
