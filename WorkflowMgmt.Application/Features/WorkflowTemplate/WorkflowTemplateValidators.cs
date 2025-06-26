using FluentValidation;

namespace WorkflowMgmt.Application.Features.WorkflowTemplate
{
    public class CreateWorkflowTemplateCommandValidator : AbstractValidator<CreateWorkflowTemplateCommand>
    {
        public CreateWorkflowTemplateCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

            RuleFor(x => x.DocumentType)
                .NotEmpty().WithMessage("Document type is required.")
                .MaximumLength(100).WithMessage("Document type cannot exceed 100 characters.")
                .Must(BeValidDocumentType).WithMessage("Document type must be 'syllabus', 'lesson', or 'session'.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleForEach(x => x.Stages)
                .SetValidator(new CreateWorkflowStageDtoValidator())
                .When(x => x.Stages != null && x.Stages.Count > 0);
        }

        private bool BeValidDocumentType(string documentType)
        {
            var validTypes = new[] { "syllabus", "lesson", "session" };
            return validTypes.Contains(documentType.ToLower());
        }
    }

    public class UpdateWorkflowTemplateCommandValidator : AbstractValidator<UpdateWorkflowTemplateCommand>
    {
        public UpdateWorkflowTemplateCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

            RuleFor(x => x.DocumentType)
                .NotEmpty().WithMessage("Document type is required.")
                .MaximumLength(100).WithMessage("Document type cannot exceed 100 characters.")
                .Must(BeValidDocumentType).WithMessage("Document type must be 'syllabus', 'lesson', or 'session'.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }

        private bool BeValidDocumentType(string documentType)
        {
            var validTypes = new[] { "syllabus", "lesson", "session" };
            return validTypes.Contains(documentType.ToLower());
        }
    }

    public class CreateWorkflowStageDtoValidator : AbstractValidator<Domain.Models.Workflow.CreateWorkflowStageDto>
    {
        public CreateWorkflowStageDtoValidator()
        {
            RuleFor(x => x.StageName)
                .NotEmpty().WithMessage("Stage name is required.")
                .MaximumLength(255).WithMessage("Stage name cannot exceed 255 characters.");

            RuleFor(x => x.StageOrder)
                .GreaterThan(0).WithMessage("Stage order must be greater than 0.");

            RuleFor(x => x.AssignedRole)
                .NotEmpty().WithMessage("Assigned role is required.")
                .MaximumLength(100).WithMessage("Assigned role cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.TimeoutDays)
                .GreaterThan(0).WithMessage("Timeout days must be greater than 0.")
                .When(x => x.TimeoutDays.HasValue);

            RuleForEach(x => x.Actions)
                .SetValidator(new CreateWorkflowStageActionDtoValidator())
                .When(x => x.Actions != null && x.Actions.Count > 0);
        }
    }

    public class CreateWorkflowStageActionDtoValidator : AbstractValidator<Domain.Models.Workflow.CreateWorkflowStageActionDto>
    {
        public CreateWorkflowStageActionDtoValidator()
        {
            RuleFor(x => x.ActionName)
                .NotEmpty().WithMessage("Action name is required.")
                .MaximumLength(255).WithMessage("Action name cannot exceed 255 characters.");

            RuleFor(x => x.ActionType)
                .NotEmpty().WithMessage("Action type is required.")
                .MaximumLength(100).WithMessage("Action type cannot exceed 100 characters.")
                .Must(BeValidActionType).WithMessage("Action type must be 'approve', 'reject', 'return', 'request_revision' or 'complete'.");
        }

        private bool BeValidActionType(string actionType)
        {
            var validTypes = new[] { "approve", "reject", "return", "complete", "request_revision" };
            return validTypes.Contains(actionType.ToLower());
        }
    }
}
