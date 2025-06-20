using FluentValidation;

namespace WorkflowMgmt.Application.Features.DocumentWorkflow
{
    public class CreateDocumentWorkflowCommandValidator : AbstractValidator<CreateDocumentWorkflowCommand>
    {
        public CreateDocumentWorkflowCommandValidator()
        {
            RuleFor(x => x.DocumentId)
                .NotEmpty().WithMessage("Document ID is required.")
                .MaximumLength(255).WithMessage("Document ID cannot exceed 255 characters.");

            RuleFor(x => x.DocumentType)
                .NotEmpty().WithMessage("Document type is required.")
                .MaximumLength(100).WithMessage("Document type cannot exceed 100 characters.")
                .Must(BeValidDocumentType).WithMessage("Document type must be 'syllabus', 'lesson', or 'session'.");

            RuleFor(x => x.WorkflowTemplateId)
                .NotEmpty().WithMessage("Workflow template ID is required.");

            RuleFor(x => x.InitiatedBy)
                .NotEmpty().WithMessage("Initiated by user ID is required.");
        }

        private bool BeValidDocumentType(string documentType)
        {
            var validTypes = new[] { "syllabus", "lesson", "session" };
            return validTypes.Contains(documentType.ToLower());
        }
    }

    public class AdvanceDocumentWorkflowCommandValidator : AbstractValidator<AdvanceDocumentWorkflowCommand>
    {
        public AdvanceDocumentWorkflowCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Workflow ID is required.");

            RuleFor(x => x.ActionType)
                .NotEmpty().WithMessage("Action type is required.")
                .MaximumLength(100).WithMessage("Action type cannot exceed 100 characters.")
                .Must(BeValidActionType).WithMessage("Action type must be 'approve', 'reject', 'return', or 'complete'.");

            RuleFor(x => x.Comments)
                .MaximumLength(1000).WithMessage("Comments cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Comments));
        }

        private bool BeValidActionType(string actionType)
        {
            var validTypes = new[] { "approve", "reject", "return", "complete" };
            return validTypes.Contains(actionType.ToLower());
        }
    }

    public class UpdateDocumentWorkflowCommandValidator : AbstractValidator<UpdateDocumentWorkflowCommand>
    {
        public UpdateDocumentWorkflowCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Workflow ID is required.");

            RuleFor(x => x.Status)
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters.")
                .Must(BeValidStatus).WithMessage("Status must be 'In Progress', 'Completed', or 'Cancelled'.")
                .When(x => !string.IsNullOrEmpty(x.Status));
        }

        private bool BeValidStatus(string status)
        {
            var validStatuses = new[] { "In Progress", "Completed", "Cancelled" };
            return validStatuses.Contains(status);
        }
    }
}
