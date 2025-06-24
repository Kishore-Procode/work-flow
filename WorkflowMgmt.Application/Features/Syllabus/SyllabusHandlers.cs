using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Syllabus;
using WorkflowMgmt.Domain.Models.DocumentUpload;
using WorkflowMgmt.Domain.Models.Workflow;
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
                FacultyId = request.FacultyId,
                FacultyName = request.FacultyName,
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

            // Handle file upload and create document upload record if present
            if (request.DocumentFile != null)
            {
                try
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine("wwwroot", "uploads", "syllabi", syllabusId.ToString());
                    Directory.CreateDirectory(uploadsPath);

                    // Generate unique filename to avoid conflicts
                    var fileExtension = Path.GetExtension(request.DocumentFile.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    // Save file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.DocumentFile.CopyToAsync(stream);
                    }

                    // Create document upload record
                    var documentUpload = new CreateDocumentUploadDto
                    {
                        DocumentId = syllabusId,
                        DocumentType = "syllabus",
                        OriginalFilename = request.DocumentFile.FileName,
                        FileSize = request.DocumentFile.Length,
                        FileType = request.DocumentFile.ContentType ?? "application/octet-stream",
                        FileUrl = $"/uploads/syllabi/{syllabusId}/{uniqueFileName}",
                        UploadStatus = "completed",
                        UploadedBy = request.FacultyId
                    };

                    await _unitOfWork.DocumentUploadRepository.CreateAsync(documentUpload);

                    Console.WriteLine($"✅ File uploaded successfully: {filePath}");
                }
                catch (Exception uploadError)
                {
                    Console.WriteLine($"Failed to upload file: {uploadError.Message}");
                    // Don't fail the syllabus creation if file upload fails
                }
            }

            // Auto-create workflow if requested (default: true)
            if (request.AutoCreateWorkflow)
            {
                try
                {
                    // Get workflow template from department document mapping
                    var workflowMapping = await _unitOfWork.WorkflowDepartmentDocumentMappingRepository
                        .GetByDepartmentAndDocumentTypeAsync(request.DepartmentId, "syllabus");

                    if (workflowMapping != null)
                    {
                        var createWorkflowCommand = new CreateDocumentWorkflowCommand
                        {
                            DocumentId = syllabusId.ToString(),
                            DocumentType = "syllabus",
                            WorkflowTemplateId = workflowMapping.workflow_template_id,
                            InitiatedBy = request.FacultyId
                        };

                        var workflow = await _mediator.Send(createWorkflowCommand, cancellationToken);

                        // Create initial workflow stage history record
                        if (workflow.CurrentStageId.HasValue)
                        {
                            var stageHistory = new CreateWorkflowStageHistoryDto
                            {
                                DocumentWorkflowId = workflow.Id,
                                StageId = workflow.CurrentStageId.Value,
                                ActionTaken = "initiated",
                                ProcessedBy = request.FacultyId,
                                Comments = "Workflow initiated upon syllabus creation"
                            };

                            await _unitOfWork.WorkflowStageHistoryRepository.CreateAsync(stageHistory);
                            Console.WriteLine($"✅ Workflow stage history created for workflow {workflow.Id}");
                        }

                        Console.WriteLine($"✅ Workflow created for syllabus {syllabusId}: {workflow.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ No workflow mapping found for department {request.DepartmentId} and document type 'syllabus'");
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

            _unitOfWork.Commit();

            // Convert to SyllabusDto
            return new SyllabusDto
            {
                Id = result.Id,
                Title = result.Title,
                DepartmentId = result.DepartmentId,
                CourseId = result.CourseId,
                SemesterId = result.SemesterId,
                TemplateId = result.TemplateId,
                FacultyId = result.FacultyId,
                FacultyName = result.FacultyName,
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
