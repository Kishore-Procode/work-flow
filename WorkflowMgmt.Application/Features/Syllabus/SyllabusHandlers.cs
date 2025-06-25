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
                originalFilename = request.DocumentFile.FileName;
                // DocumentUrl will be set after file upload
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
                DocumentFile = request.DocumentFile,
                OriginalFilename = originalFilename
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

                    // Update syllabus with document URL and processing status
                    var updateDto = new UpdateSyllabusDto
                    {
                        DocumentUrl = $"/uploads/syllabi/{syllabusId}/{uniqueFileName}",
                        FileProcessingStatus = "completed",
                        FileProcessingNotes = $"File uploaded on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"
                    };
                    await _unitOfWork.SyllabusRepository.UpdateAsync(syllabusId, updateDto);

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

    public class UpdateSyllabusCommandHandler : IRequestHandler<UpdateSyllabusCommand, SyllabusDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSyllabusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SyllabusDto?> Handle(UpdateSyllabusCommand request, CancellationToken cancellationToken)
        {
            // Check if syllabus exists
            var exists = await _unitOfWork.SyllabusRepository.ExistsAsync(request.Id);
            if (!exists)
            {
                return null;
            }

            // Check if title is being updated and if it conflicts with another syllabus in the same department
            if (!string.IsNullOrEmpty(request.Title))
            {
                var currentSyllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(request.Id);
                if (currentSyllabus != null && currentSyllabus.Title != request.Title)
                {
                    var titleExists = await _unitOfWork.SyllabusRepository.ExistsByTitleAndDepartmentAsync(request.Title, currentSyllabus.DepartmentId, request.Id);
                    if (titleExists)
                    {
                        throw new InvalidOperationException($"A syllabus with title '{request.Title}' already exists in this department.");
                    }
                }
            }

            // Handle file upload if present
            string? documentUrl = null;
            string? originalFilename = null;
            if (request.DocumentFile != null)
            {
                try
                {
                    // Get existing syllabus to check for old file
                    var existingSyllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(request.Id);

                    // Delete old file if it exists
                    if (existingSyllabus != null && !string.IsNullOrEmpty(existingSyllabus.DocumentUrl))
                    {
                        try
                        {
                            var oldFilePath = Path.Combine("wwwroot", existingSyllabus.DocumentUrl.TrimStart('/'));
                            if (File.Exists(oldFilePath))
                            {
                                File.Delete(oldFilePath);
                                Console.WriteLine($"🗑️ Deleted old file: {oldFilePath}");
                            }
                        }
                        catch (Exception deleteError)
                        {
                            Console.WriteLine($"Warning: Failed to delete old file: {deleteError.Message}");
                            // Continue with upload even if old file deletion fails
                        }
                    }

                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine("wwwroot", "uploads", "syllabi", request.Id.ToString());
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
                    documentUrl = $"/uploads/syllabi/{request.Id}/{uniqueFileName}";
                    originalFilename = request.DocumentFile.FileName;

                    // Create new document upload record
                    var documentUpload = new CreateDocumentUploadDto
                    {
                        DocumentId = request.Id,
                        DocumentType = "syllabus",
                        OriginalFilename = request.DocumentFile.FileName,
                        FileSize = request.DocumentFile.Length,
                        FileType = request.DocumentFile.ContentType ?? "application/octet-stream",
                        FileUrl = documentUrl,
                        UploadStatus = "completed",
                        UploadedBy = request.FacultyId ?? Guid.Empty
                    };

                    await _unitOfWork.DocumentUploadRepository.CreateAsync(documentUpload);

                    Console.WriteLine($"✅ File replaced successfully during update: {filePath}");
                }
                catch (Exception uploadError)
                {
                    Console.WriteLine($"Failed to upload file during update: {uploadError.Message}");
                    // Don't fail the syllabus update if file upload fails
                }
            }

            var updateDto = new UpdateSyllabusDto
            {
                Title = request.Title,
                CourseId = request.CourseId,
                SemesterId = request.SemesterId,
                FacultyId = request.FacultyId,
                FacultyName = request.FacultyName,
                Credits = request.Credits,
                DurationWeeks = request.DurationWeeks,
                CourseDescription = request.CourseDescription,
                LearningObjectives = request.LearningObjectives,
                LearningOutcomes = request.LearningOutcomes,
                CourseTopics = request.CourseTopics,
                AssessmentMethods = request.AssessmentMethods,
                DetailedContent = request.DetailedContent,
                ReferenceMaterials = request.ReferenceMaterials,
                Status = request.Status,
                DocumentUrl = documentUrl, // Only update if new file was uploaded
                OriginalFilename = originalFilename, // Only update if new file was uploaded
                FileProcessingStatus = documentUrl != null ? "completed" : null, // Update processing status if file was uploaded
                FileProcessingNotes = documentUrl != null ? $"File updated on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC" : null
            };

            var success = await _unitOfWork.SyllabusRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                return null;
            }

            // Handle file upload if present
            if (request.DocumentFile != null)
            {
                try
                {
                    // Check if the same file is already uploaded
                    var currentSyllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(request.Id);
                    if (currentSyllabus != null &&
                        !string.IsNullOrEmpty(currentSyllabus.OriginalFilename) &&
                        currentSyllabus.OriginalFilename == request.DocumentFile.FileName)
                    {
                        // Same file name, skip upload but continue with other updates
                        Console.WriteLine($"⚠️ Skipping file upload - same file already exists: {request.DocumentFile.FileName}");
                    }
                    else
                    {
                        // Different file, proceed with upload
                        // Create uploads directory if it doesn't exist
                        var uploadsPath = Path.Combine("wwwroot", "uploads", "syllabi", request.Id.ToString());
                        Directory.CreateDirectory(uploadsPath);

                        // Generate unique filename to avoid conflicts
                        var fileExtension = Path.GetExtension(request.DocumentFile.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadsPath, uniqueFileName);

                        // Save file to disk
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.DocumentFile.CopyToAsync(stream, cancellationToken);
                        }

                        // Update syllabus with new file information
                        var fileUpdateDto = new UpdateSyllabusDto
                        {
                            DocumentUrl = $"/uploads/syllabi/{request.Id}/{uniqueFileName}",
                            OriginalFilename = request.DocumentFile.FileName
                        };
                        await _unitOfWork.SyllabusRepository.UpdateAsync(request.Id, fileUpdateDto);

                        // Create document upload record
                        var documentUpload = new CreateDocumentUploadDto
                        {
                            DocumentId = request.Id,
                            DocumentType = "syllabus",
                            OriginalFilename = request.DocumentFile.FileName,
                            FileSize = request.DocumentFile.Length,
                            FileType = request.DocumentFile.ContentType ?? "application/octet-stream",
                            FileUrl = $"/uploads/syllabi/{request.Id}/{uniqueFileName}",
                            UploadStatus = "completed",
                            UploadedBy = request.FacultyId ?? Guid.Empty
                        };

                        await _unitOfWork.DocumentUploadRepository.CreateAsync(documentUpload);

                        Console.WriteLine($"✅ File uploaded successfully for syllabus update: {filePath}");
                    }
                }
                catch (Exception uploadError)
                {
                    Console.WriteLine($"Failed to upload file during syllabus update: {uploadError.Message}");
                    // Don't fail the syllabus update if file upload fails
                }
            }

            // Return the updated syllabus
            var result = await _unitOfWork.SyllabusRepository.GetByIdAsync(request.Id);
            if (result == null)
            {
                return null;
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
