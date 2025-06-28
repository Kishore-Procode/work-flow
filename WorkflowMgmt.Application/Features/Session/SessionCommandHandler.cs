using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Session;
using WorkflowMgmt.Domain.Models.DocumentUpload;
using WorkflowMgmt.Application.Features.DocumentWorkflow;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.Session
{
    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, IEnumerable<SessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SessionDto>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetAllAsync();
        }
    }

    public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, SessionWithDetailsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SessionWithDetailsDto?> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetByIdAsync(request.Id);
        }
    }

    public class GetSessionsByStatusQueryHandler : IRequestHandler<GetSessionsByStatusQuery, IEnumerable<SessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionsByStatusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SessionDto>> Handle(GetSessionsByStatusQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetByStatusAsync(request.Status);
        }
    }

    public class GetSessionsByInstructorQueryHandler : IRequestHandler<GetSessionsByInstructorQuery, IEnumerable<SessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionsByInstructorQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SessionDto>> Handle(GetSessionsByInstructorQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetByInstructorAsync(request.Instructor);
        }
    }

    public class GetSessionsByDepartmentQueryHandler : IRequestHandler<GetSessionsByDepartmentQuery, IEnumerable<SessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionsByDepartmentQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SessionDto>> Handle(GetSessionsByDepartmentQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetByDepartmentAsync(request.DepartmentId);
        }
    }

    public class GetSessionsByLessonPlanQueryHandler : IRequestHandler<GetSessionsByLessonPlanQuery, IEnumerable<SessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionsByLessonPlanQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SessionDto>> Handle(GetSessionsByLessonPlanQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetByLessonPlanAsync(request.LessonPlanId);
        }
    }

    public class GetSessionsByDateRangeQueryHandler : IRequestHandler<GetSessionsByDateRangeQuery, IEnumerable<SessionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionsByDateRangeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SessionDto>> Handle(GetSessionsByDateRangeQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        }
    }

    public class GetSessionsStatsQueryHandler : IRequestHandler<GetSessionsStatsQuery, SessionStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionsStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SessionStatsDto> Handle(GetSessionsStatsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SessionRepository.GetStatsAsync();
        }
    }

    public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, SessionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreateSessionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<SessionDto> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            // Get lesson plan to validate and get department
            var lessonPlan = await _unitOfWork.LessonPlanRepository.GetByIdAsync(request.LessonPlanId);
            if (lessonPlan == null)
            {
                throw new InvalidOperationException($"Lesson plan with ID {request.LessonPlanId} not found.");
            }

            // Get syllabus to get department
            if (!lessonPlan.SyllabusId.HasValue)
            {
                throw new InvalidOperationException($"Lesson plan {request.LessonPlanId} does not have a syllabus assigned.");
            }

            var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(lessonPlan.SyllabusId.Value);
            if (syllabus == null)
            {
                throw new InvalidOperationException($"Syllabus not found for lesson plan {request.LessonPlanId}.");
            }

            // Check if session with same title already exists for the same department
            var exists = await _unitOfWork.SessionRepository.ExistsByTitleAndDepartmentAsync(request.Title, syllabus.DepartmentId);
            if (exists)
            {
                throw new InvalidOperationException($"A session with title '{request.Title}' already exists for this department.");
            }

            // Handle file upload if present
            string? documentUrl = null;
            string? originalFilename = null;
            if (request.DocumentFile != null)
            {
                originalFilename = request.DocumentFile.FileName;
                // DocumentUrl will be set after file upload
            }

            var createDto = new CreateSessionDto
            {
                Title = request.Title,
                LessonPlanId = request.LessonPlanId,
                FacultyId = request.FacultyId,
                TeachingMethod = request.TeachingMethod,
                SessionDate = request.SessionDate,
                SessionTime = request.SessionTime,
                DurationMinutes = request.DurationMinutes,
                Instructor = request.Instructor,
                ContentCreationMethod = request.ContentCreationMethod,
                SessionDescription = request.SessionDescription,
                SessionObjectives = request.SessionObjectives,
                SessionActivities = request.SessionActivities,
                MaterialsEquipment = request.MaterialsEquipment,
                DetailedContent = request.DetailedContent,
                ContentResources = request.ContentResources,
                DocumentUrl = documentUrl,
                OriginalFilename = originalFilename,
                CreatedBy = "system" // TODO: Get from current user context
            };

            var sessionId = await _unitOfWork.SessionRepository.CreateAsync(createDto);

            // Handle file upload if present
            if (request.DocumentFile != null)
            {
                try
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine("wwwroot", "uploads", "sessions", sessionId.ToString());
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

                    // Update session with document URL and processing status
                    var updateDto = new UpdateSessionDto
                    {
                        DocumentUrl = $"/uploads/sessions/{sessionId}/{uniqueFileName}",
                        OriginalFilename = request.DocumentFile.FileName,
                        FileProcessingStatus = "completed",
                        FileProcessingNotes = $"File uploaded on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"
                    };
                    await _unitOfWork.SessionRepository.UpdateAsync(sessionId, updateDto);

                    // Create document upload record
                    var documentUpload = new CreateDocumentUploadDto
                    {
                        DocumentId = sessionId,
                        DocumentType = "session",
                        OriginalFilename = request.DocumentFile.FileName,
                        FileSize = request.DocumentFile.Length,
                        FileType = request.DocumentFile.ContentType ?? "application/octet-stream",
                        FileUrl = $"/uploads/sessions/{sessionId}/{uniqueFileName}",
                        UploadStatus = "completed",
                        UploadedBy = null // Set to null to avoid foreign key constraint issues
                    };

                    await _unitOfWork.DocumentUploadRepository.CreateAsync(documentUpload);

                    Console.WriteLine($"✅ File uploaded successfully: {filePath}");
                }
                catch (Exception uploadError)
                {
                    Console.WriteLine($"Failed to upload file: {uploadError.Message}");
                    // Don't fail the session creation if file upload fails
                }
            }

            // Auto-create workflow if requested
            if (request.AutoCreateWorkflow)
            {
                try
                {
                    // Get workflow template from department document mapping
                    var workflowMapping = await _unitOfWork.WorkflowDepartmentDocumentMappingRepository
                        .GetByDepartmentAndDocumentTypeAsync(syllabus.DepartmentId, "session");

                    if (workflowMapping != null)
                    {
                        var createWorkflowCommand = new CreateDocumentWorkflowCommand
                        {
                            DocumentId = sessionId.ToString(),
                            DocumentType = "session",
                            WorkflowTemplateId = workflowMapping.workflow_template_id,
                            InitiatedBy = request.FacultyId,
                            AssignedTo = request.FacultyId // Initially assign to the creator
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
                                AssignedTo = request.FacultyId, // Initially assign to the creator
                                Comments = "Workflow initiated upon session creation"
                            };

                            await _unitOfWork.WorkflowStageHistoryRepository.CreateAsync(stageHistory);
                            Console.WriteLine($"✅ Workflow stage history created for workflow {workflow.Id}");
                        }

                        Console.WriteLine($"✅ Workflow created for session {sessionId}: {workflow.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ No workflow mapping found for department {syllabus.DepartmentId} and document type 'session'");
                    }
                }
                catch (Exception workflowError)
                {
                    Console.WriteLine($"⚠️ Failed to create workflow for session: {workflowError.Message}");
                    // Don't fail the session creation if workflow creation fails
                }
            }

            // Return the created session
            var createdSession = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (createdSession == null)
            {
                throw new InvalidOperationException("Failed to retrieve created session");
            }

            _unitOfWork.Commit();

            // Convert SessionWithDetailsDto to SessionDto
            return new SessionDto
            {
                Id = createdSession.Id,
                Title = createdSession.Title,
                LessonPlanId = createdSession.LessonPlanId,
                FacultyId = createdSession.FacultyId,
                TeachingMethod = createdSession.TeachingMethod,
                SessionDate = createdSession.SessionDate,
                SessionTime = createdSession.SessionTime,
                DurationMinutes = createdSession.DurationMinutes,
                Instructor = createdSession.Instructor,
                ContentCreationMethod = createdSession.ContentCreationMethod,
                SessionDescription = createdSession.SessionDescription,
                SessionObjectives = createdSession.SessionObjectives,
                SessionActivities = createdSession.SessionActivities,
                MaterialsEquipment = createdSession.MaterialsEquipment,
                DetailedContent = createdSession.DetailedContent,
                ContentResources = createdSession.ContentResources,
                DocumentUrl = createdSession.DocumentUrl,
                Status = createdSession.Status,
                IsActive = createdSession.IsActive,
                CreatedDate = createdSession.CreatedDate,
                ModifiedDate = createdSession.ModifiedDate,
                CreatedBy = createdSession.CreatedBy,
                ModifiedBy = createdSession.ModifiedBy,
                FileProcessingStatus = createdSession.FileProcessingStatus,
                FileProcessingNotes = createdSession.FileProcessingNotes,
                OriginalFilename = createdSession.OriginalFilename
            };
        }
    }

    public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, SessionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSessionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SessionDto> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            // Check if session exists
            var existingSession = await _unitOfWork.SessionRepository.GetByIdAsync(request.Id);
            if (existingSession == null)
            {
                throw new InvalidOperationException($"Session with ID {request.Id} not found.");
            }

            // Handle file upload if present
            string? documentUrl = existingSession.DocumentUrl;
            string? originalFilename = existingSession.OriginalFilename;
            if (request.DocumentFile != null)
            {
                try
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine("wwwroot", "uploads", "sessions", request.Id.ToString());
                    Directory.CreateDirectory(uploadsPath);

                    // Generate unique filename to avoid conflicts
                    var fileExtension = Path.GetExtension(request.DocumentFile.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    // Save new file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.DocumentFile.CopyToAsync(stream);
                    }

                    // Set the document URL and original filename
                    documentUrl = $"/uploads/sessions/{request.Id}/{uniqueFileName}";
                    originalFilename = request.DocumentFile.FileName;

                    // Create new document upload record
                    var documentUpload = new CreateDocumentUploadDto
                    {
                        DocumentId = request.Id,
                        DocumentType = "session",
                        OriginalFilename = request.DocumentFile.FileName,
                        FileSize = request.DocumentFile.Length,
                        FileType = request.DocumentFile.ContentType ?? "application/octet-stream",
                        FileUrl = documentUrl,
                        UploadStatus = "completed",
                        UploadedBy = null // Set to null to avoid foreign key constraint issues
                    };

                    await _unitOfWork.DocumentUploadRepository.CreateAsync(documentUpload);

                    Console.WriteLine($"✅ File replaced successfully during update: {filePath}");
                }
                catch (Exception uploadError)
                {
                    Console.WriteLine($"Failed to upload file during update: {uploadError.Message}");
                    // Don't fail the session update if file upload fails
                }
            }

            var updateDto = new UpdateSessionDto
            {
                Id = request.Id,
                Title = request.Title,
                LessonPlanId = request.LessonPlanId,
                FacultyId = request.FacultyId,
                TeachingMethod = request.TeachingMethod,
                SessionDate = request.SessionDate,
                SessionTime = request.SessionTime,
                DurationMinutes = request.DurationMinutes,
                Instructor = request.Instructor,
                SessionDescription = request.SessionDescription,
                SessionObjectives = request.SessionObjectives,
                SessionActivities = request.SessionActivities,
                MaterialsEquipment = request.MaterialsEquipment,
                DetailedContent = request.DetailedContent,
                ContentResources = request.ContentResources,
                Status = request.Status,
                DocumentUrl = documentUrl,
                OriginalFilename = originalFilename
            };

            var success = await _unitOfWork.SessionRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update session.");
            }

            // Return the updated session
            var updatedSession = await _unitOfWork.SessionRepository.GetByIdAsync(request.Id);
            if (updatedSession == null)
            {
                throw new InvalidOperationException("Failed to retrieve updated session");
            }

            _unitOfWork.Commit();

            // Convert SessionWithDetailsDto to SessionDto
            return new SessionDto
            {
                Id = updatedSession.Id,
                Title = updatedSession.Title,
                LessonPlanId = updatedSession.LessonPlanId,
                FacultyId = updatedSession.FacultyId,
                TeachingMethod = updatedSession.TeachingMethod,
                SessionDate = updatedSession.SessionDate,
                SessionTime = updatedSession.SessionTime,
                DurationMinutes = updatedSession.DurationMinutes,
                Instructor = updatedSession.Instructor,
                ContentCreationMethod = updatedSession.ContentCreationMethod,
                SessionDescription = updatedSession.SessionDescription,
                SessionObjectives = updatedSession.SessionObjectives,
                SessionActivities = updatedSession.SessionActivities,
                MaterialsEquipment = updatedSession.MaterialsEquipment,
                DetailedContent = updatedSession.DetailedContent,
                ContentResources = updatedSession.ContentResources,
                DocumentUrl = updatedSession.DocumentUrl,
                Status = updatedSession.Status,
                IsActive = updatedSession.IsActive,
                CreatedDate = updatedSession.CreatedDate,
                ModifiedDate = updatedSession.ModifiedDate,
                CreatedBy = updatedSession.CreatedBy,
                ModifiedBy = updatedSession.ModifiedBy,
                FileProcessingStatus = updatedSession.FileProcessingStatus,
                FileProcessingNotes = updatedSession.FileProcessingNotes,
                OriginalFilename = updatedSession.OriginalFilename
            };
        }
    }

    public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSessionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.SessionRepository.DeleteAsync(request.Id);
            if (result)
            {
                _unitOfWork.Commit();
            }
            return result;
        }
    }

    public class ToggleSessionActiveCommandHandler : IRequestHandler<ToggleSessionActiveCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleSessionActiveCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ToggleSessionActiveCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.SessionRepository.ToggleActiveAsync(request.Id);
            if (result)
            {
                _unitOfWork.Commit();
            }
            return result;
        }
    }
}
