using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Syllabus;
using WorkflowMgmt.Application.Features.DocumentWorkflow;

namespace WorkflowMgmt.Application.Features.Syllabus
{
    public class GetAllSyllabiQueryHandler : IRequestHandler<GetAllSyllabiQuery, IEnumerable<SyllabusWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSyllabiQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> Handle(GetAllSyllabiQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusRepository.GetAllAsync();
        }
    }

    public class GetSyllabiByDepartmentQueryHandler : IRequestHandler<GetSyllabiByDepartmentQuery, IEnumerable<SyllabusWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabiByDepartmentQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> Handle(GetSyllabiByDepartmentQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusRepository.GetByDepartmentAsync(request.DepartmentId);
        }
    }

    public class GetSyllabiByStatusQueryHandler : IRequestHandler<GetSyllabiByStatusQuery, IEnumerable<SyllabusWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabiByStatusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> Handle(GetSyllabiByStatusQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusRepository.GetByStatusAsync(request.Status);
        }
    }

    public class GetSyllabiByFacultyQueryHandler : IRequestHandler<GetSyllabiByFacultyQuery, IEnumerable<SyllabusWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabiByFacultyQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> Handle(GetSyllabiByFacultyQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusRepository.GetByFacultyAsync(request.FacultyName);
        }
    }

    public class GetSyllabiByTemplateQueryHandler : IRequestHandler<GetSyllabiByTemplateQuery, IEnumerable<SyllabusWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabiByTemplateQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> Handle(GetSyllabiByTemplateQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusRepository.GetByTemplateAsync(request.TemplateId);
        }
    }

    public class GetSyllabusByIdQueryHandler : IRequestHandler<GetSyllabusByIdQuery, SyllabusWithDetailsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabusByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SyllabusWithDetailsDto?> Handle(GetSyllabusByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusRepository.GetByIdAsync(request.Id);
        }
    }

    public class CreateSyllabusCommandHandler : IRequestHandler<CreateSyllabusCommand, SyllabusDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreateSyllabusCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<SyllabusDto> Handle(CreateSyllabusCommand request, CancellationToken cancellationToken)
        {
            // Check if syllabus with same title already exists in department
            var exists = await _unitOfWork.SyllabusRepository.ExistsByTitleAndDepartmentAsync(request.Title, request.DepartmentId);
            if (exists)
            {
                throw new InvalidOperationException($"A syllabus with title '{request.Title}' already exists in this department.");
            }

            // Handle file upload if present
            string? documentUrl = null;
            string? originalFilename = null;
            if (request.DocumentFile != null)
            {
                // TODO: Implement file upload logic
                // For now, just store the filename
                originalFilename = request.DocumentFile.FileName;
                // documentUrl = await UploadFileAsync(request.DocumentFile);
            }

            var createDto = new CreateSyllabusDto
            {
                Title = request.Title,
                DepartmentId = request.DepartmentId,
                CourseId = request.CourseId,
                SemesterId = request.SemesterId,
                TemplateId = request.TemplateId,
                FacultyName = request.FacultyName,
                FacultyEmail = request.FacultyEmail,
                Credits = request.Credits,
                DurationWeeks = request.DurationWeeks,
                ContentCreationMethod = request.ContentCreationMethod,
                CourseDescription = request.CourseDescription,
                LearningObjectives = request.LearningObjectives,
                LearningOutcomes = request.LearningOutcomes,
                CourseTopics = request.CourseTopics,
                AssessmentMethods = request.AssessmentMethods,
                DetailedContent = request.DetailedContent,
                ReferenceMaterials = request.ReferenceMaterials,
                DocumentFile = request.DocumentFile
            };

            var syllabusId = await _unitOfWork.SyllabusRepository.CreateAsync(createDto);
            
            // Update document URL if file was uploaded
            if (!string.IsNullOrEmpty(documentUrl))
            {
                await _unitOfWork.SyllabusRepository.UpdateDocumentUrlAsync(syllabusId, documentUrl);
            }

            await _unitOfWork.SaveAsync();

            // Auto-create workflow if requested (default: true)
            if (request.AutoCreateWorkflow)
            {
                try
                {
                    // Get default workflow template for syllabus
                    var workflowTemplates = await _unitOfWork.WorkflowTemplateRepository.GetByDocumentTypeAsync("syllabus");
                    var defaultTemplate = workflowTemplates.FirstOrDefault(t => t.IsActive);

                    if (defaultTemplate != null)
                    {
                        var createWorkflowCommand = new CreateDocumentWorkflowCommand
                        {
                            DocumentId = syllabusId.ToString(),
                            DocumentType = "syllabus",
                            WorkflowTemplateId = defaultTemplate.Id
                        };

                        var workflow = await _mediator.Send(createWorkflowCommand, cancellationToken);

                        // Update syllabus with workflow ID
                        await _unitOfWork.SyllabusRepository.UpdateWorkflowIdAsync(syllabusId, workflow.Id);
                        await _unitOfWork.SaveAsync();

                        Console.WriteLine($"✅ Workflow created for syllabus {syllabusId}: {workflow.Id}");
                    }
                }
                catch (Exception workflowError)
                {
                    Console.WriteLine($"Failed to create workflow for syllabus: {workflowError.Message}");
                    // Don't fail the syllabus creation if workflow creation fails
                }
            }

            // Get the created syllabus with details
            var result = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to retrieve created syllabus.");
            }

            // Convert to SyllabusDto
            return new SyllabusDto
            {
                Id = result.Id,
                Title = result.Title,
                DepartmentId = result.DepartmentId,
                CourseId = result.CourseId,
                SemesterId = result.SemesterId,
                TemplateId = result.TemplateId,
                FacultyName = result.FacultyName,
                FacultyEmail = result.FacultyEmail,
                Credits = result.Credits,
                DurationWeeks = result.DurationWeeks,
                ContentCreationMethod = result.ContentCreationMethod,
                CourseDescription = result.CourseDescription,
                LearningObjectives = result.LearningObjectives,
                LearningOutcomes = result.LearningOutcomes,
                CourseTopics = result.CourseTopics,
                AssessmentMethods = result.AssessmentMethods,
                DetailedContent = result.DetailedContent,
                ReferenceMaterials = result.ReferenceMaterials,
                DocumentUrl = result.DocumentUrl,
                Status = result.Status,
                WorkflowId = result.WorkflowId,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate,
                CreatedBy = result.CreatedBy,
                ModifiedBy = result.ModifiedBy,
                FileProcessingStatus = result.FileProcessingStatus,
                FileProcessingNotes = result.FileProcessingNotes,
                OriginalFilename = result.OriginalFilename
            };
        }
    }

    public class GetSyllabusStatsQueryHandler : IRequestHandler<GetSyllabusStatsQuery, SyllabusStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabusStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SyllabusStatsDto> Handle(GetSyllabusStatsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusRepository.GetStatsAsync();
        }
    }
}
