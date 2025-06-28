using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.LessonPlan;
using WorkflowMgmt.Domain.Models.DocumentUpload;

namespace WorkflowMgmt.Application.Features.LessonPlan
{
    public class GetLessonPlansQueryHandler : IRequestHandler<GetLessonPlansQuery, IEnumerable<LessonPlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlansQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanDto>> Handle(GetLessonPlansQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetAllAsync();
        }
    }

    public class GetLessonPlanByIdQueryHandler : IRequestHandler<GetLessonPlanByIdQuery, LessonPlanWithDetailsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlanByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonPlanWithDetailsDto?> Handle(GetLessonPlanByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetByIdAsync(request.Id);
        }
    }

    public class GetLessonPlansByStatusQueryHandler : IRequestHandler<GetLessonPlansByStatusQuery, IEnumerable<LessonPlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlansByStatusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanDto>> Handle(GetLessonPlansByStatusQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetByStatusAsync(request.Status);
        }
    }

    public class GetLessonPlansByFacultyQueryHandler : IRequestHandler<GetLessonPlansByFacultyQuery, IEnumerable<LessonPlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlansByFacultyQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanDto>> Handle(GetLessonPlansByFacultyQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetByFacultyAsync(request.FacultyName);
        }
    }

    public class GetLessonPlansByTemplateQueryHandler : IRequestHandler<GetLessonPlansByTemplateQuery, IEnumerable<LessonPlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlansByTemplateQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanDto>> Handle(GetLessonPlansByTemplateQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetByTemplateAsync(request.TemplateId);
        }
    }

    public class GetLessonPlansBySyllabusQueryHandler : IRequestHandler<GetLessonPlansBySyllabusQuery, IEnumerable<LessonPlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlansBySyllabusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanDto>> Handle(GetLessonPlansBySyllabusQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetBySyllabusAsync(request.SyllabusId);
        }
    }

    public class GetLessonPlansStatsQueryHandler : IRequestHandler<GetLessonPlansStatsQuery, LessonPlanStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlansStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonPlanStatsDto> Handle(GetLessonPlansStatsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetStatsAsync();
        }
    }

    public class CreateLessonPlanCommandHandler : IRequestHandler<CreateLessonPlanCommand, LessonPlanDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLessonPlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonPlanDto> Handle(CreateLessonPlanCommand request, CancellationToken cancellationToken)
        {
            // Check if lesson plan with same title already exists for the same syllabus/template
            var exists = await _unitOfWork.LessonPlanRepository.ExistsByTitleAndTemplateAsync(request.Title, request.TemplateId);
            if (exists)
            {
                throw new InvalidOperationException($"A lesson plan with title '{request.Title}' already exists for this template.");
            }

            // Handle file upload if present
            string? documentUrl = null;
            string? originalFilename = null;
            if (request.DocumentFile != null)
            {
                originalFilename = request.DocumentFile.FileName;
                // DocumentUrl will be set after file upload
            }

            var createDto = new CreateLessonPlanDto
            {
                Title = request.Title,
                SyllabusId = request.SyllabusId,
                TemplateId = request.TemplateId,
                ModuleName = request.ModuleName,
                DurationMinutes = request.DurationMinutes,
                NumberOfSessions = request.NumberOfSessions,
                ScheduledDate = request.ScheduledDate,
                FacultyId = request.FacultyId,
                FacultyName = request.FacultyName,
                ContentCreationMethod = request.ContentCreationMethod,
                LessonDescription = request.LessonDescription,
                LearningObjectives = request.LearningObjectives,
                TeachingMethods = request.TeachingMethods,
                LearningActivities = request.LearningActivities,
                DetailedContent = request.DetailedContent,
                Resources = request.Resources,
                AssessmentMethods = request.AssessmentMethods,
                Prerequisites = request.Prerequisites,
                DocumentUrl = documentUrl,
                OriginalFilename = originalFilename
            };

            var lessonPlanId = await _unitOfWork.LessonPlanRepository.CreateAsync(createDto);

            // Handle file upload and create document upload record if present
            if (request.DocumentFile != null)
            {
                try
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine("wwwroot", "uploads", "lesson-plans", lessonPlanId.ToString());
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
                        DocumentId = lessonPlanId,
                        DocumentType = "lesson_plan",
                        OriginalFilename = request.DocumentFile.FileName,
                        FileSize = request.DocumentFile.Length,
                        FileType = request.DocumentFile.ContentType ?? "application/octet-stream",
                        FileUrl = $"/uploads/lesson-plans/{lessonPlanId}/{uniqueFileName}",
                        UploadStatus = "completed",
                        UploadedBy = request.FacultyId // TODO: Get from current user context
                    };

                    await _unitOfWork.DocumentUploadRepository.CreateAsync(documentUpload);

                    // Update lesson plan with document URL and processing status
                    var updateDto = new UpdateLessonPlanDto
                    {
                        DocumentUrl = $"/uploads/lesson-plans/{lessonPlanId}/{uniqueFileName}",
                        OriginalFilename = request.DocumentFile.FileName
                    };
                    await _unitOfWork.LessonPlanRepository.UpdateAsync(lessonPlanId, updateDto);

                    Console.WriteLine($"✅ File uploaded successfully: {filePath}");
                }
                catch (Exception uploadError)
                {
                    Console.WriteLine($"Failed to upload file: {uploadError.Message}");
                    // Don't fail the lesson plan creation if file upload fails
                }
            }

            // Return the created lesson plan
            var createdLessonPlan = await _unitOfWork.LessonPlanRepository.GetByIdAsync(lessonPlanId);
            if (createdLessonPlan == null)
            {
                throw new InvalidOperationException("Failed to retrieve created lesson plan");
            }

            _unitOfWork.Commit();

            // Convert LessonPlanWithDetailsDto to LessonPlanDto
            return new LessonPlanDto
            {
                Id = createdLessonPlan.Id,
                Title = createdLessonPlan.Title,
                SyllabusId = createdLessonPlan.SyllabusId,
                TemplateId = createdLessonPlan.TemplateId,
                ModuleName = createdLessonPlan.ModuleName,
                DurationMinutes = createdLessonPlan.DurationMinutes,
                NumberOfSessions = createdLessonPlan.NumberOfSessions,
                ScheduledDate = createdLessonPlan.ScheduledDate,
                FacultyId = createdLessonPlan.FacultyId,
                FacultyName = createdLessonPlan.FacultyName,
                ContentCreationMethod = createdLessonPlan.ContentCreationMethod,
                LessonDescription = createdLessonPlan.LessonDescription,
                LearningObjectives = createdLessonPlan.LearningObjectives,
                TeachingMethods = createdLessonPlan.TeachingMethods,
                LearningActivities = createdLessonPlan.LearningActivities,
                DetailedContent = createdLessonPlan.DetailedContent,
                Resources = createdLessonPlan.Resources,
                AssessmentMethods = createdLessonPlan.AssessmentMethods,
                Prerequisites = createdLessonPlan.Prerequisites,
                DocumentUrl = createdLessonPlan.DocumentUrl,
                Status = createdLessonPlan.Status,
                IsActive = createdLessonPlan.IsActive,
                CreatedDate = createdLessonPlan.CreatedDate,
                ModifiedDate = createdLessonPlan.ModifiedDate,
                CreatedBy = createdLessonPlan.CreatedBy,
                ModifiedBy = createdLessonPlan.ModifiedBy,
                FileProcessingStatus = createdLessonPlan.FileProcessingStatus,
                FileProcessingNotes = createdLessonPlan.FileProcessingNotes,
                OriginalFilename = createdLessonPlan.OriginalFilename
            };
        }
    }

    public class UpdateLessonPlanCommandHandler : IRequestHandler<UpdateLessonPlanCommand, LessonPlanDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLessonPlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonPlanDto> Handle(UpdateLessonPlanCommand request, CancellationToken cancellationToken)
        {
            // Check if lesson plan exists
            var existingLessonPlan = await _unitOfWork.LessonPlanRepository.GetByIdAsync(request.Id);
            if (existingLessonPlan == null)
            {
                throw new InvalidOperationException($"Lesson plan with ID {request.Id} not found.");
            }

            // Handle file upload if present
            string? documentUrl = existingLessonPlan.DocumentUrl;
            string? originalFilename = existingLessonPlan.OriginalFilename;
            if (request.DocumentFile != null)
            {
                try
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine("wwwroot", "uploads", "lesson-plans", request.Id.ToString());
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
                    documentUrl = $"/uploads/lesson-plans/{request.Id}/{uniqueFileName}";
                    originalFilename = request.DocumentFile.FileName;

                    // Create new document upload record
                    var documentUpload = new CreateDocumentUploadDto
                    {
                        DocumentId = request.Id,
                        DocumentType = "lesson_plan",
                        OriginalFilename = request.DocumentFile.FileName,
                        FileSize = request.DocumentFile.Length,
                        FileType = request.DocumentFile.ContentType ?? "application/octet-stream",
                        FileUrl = documentUrl,
                        UploadStatus = "completed",
                        UploadedBy = Guid.Empty // TODO: Get from current user context
                    };

                    await _unitOfWork.DocumentUploadRepository.CreateAsync(documentUpload);

                    Console.WriteLine($"✅ File replaced successfully during update: {filePath}");
                }
                catch (Exception uploadError)
                {
                    Console.WriteLine($"Failed to upload file during update: {uploadError.Message}");
                    // Don't fail the lesson plan update if file upload fails
                }
            }

            var updateDto = new UpdateLessonPlanDto
            {
                Id = request.Id,
                Title = request.Title,
                SyllabusId = request.SyllabusId,
                ModuleName = request.ModuleName,
                DurationMinutes = request.DurationMinutes,
                NumberOfSessions = request.NumberOfSessions,
                ScheduledDate = request.ScheduledDate,
                FacultyId = request.FacultyId,
                FacultyName = request.FacultyName,
                LessonDescription = request.LessonDescription,
                LearningObjectives = request.LearningObjectives,
                TeachingMethods = request.TeachingMethods,
                LearningActivities = request.LearningActivities,
                DetailedContent = request.DetailedContent,
                Resources = request.Resources,
                AssessmentMethods = request.AssessmentMethods,
                Prerequisites = request.Prerequisites,
                Status = request.Status,
                DocumentUrl = documentUrl,
                OriginalFilename = originalFilename
            };

            var success = await _unitOfWork.LessonPlanRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update lesson plan.");
            }

            _unitOfWork.Commit();

            // Return the updated lesson plan
            var updatedLessonPlan = await _unitOfWork.LessonPlanRepository.GetByIdAsync(request.Id);
            if (updatedLessonPlan == null)
            {
                throw new InvalidOperationException("Failed to retrieve updated lesson plan");
            }

            // Convert LessonPlanWithDetailsDto to LessonPlanDto
            return new LessonPlanDto
            {
                Id = updatedLessonPlan.Id,
                Title = updatedLessonPlan.Title,
                SyllabusId = updatedLessonPlan.SyllabusId,
                TemplateId = updatedLessonPlan.TemplateId,
                ModuleName = updatedLessonPlan.ModuleName,
                DurationMinutes = updatedLessonPlan.DurationMinutes,
                NumberOfSessions = updatedLessonPlan.NumberOfSessions,
                ScheduledDate = updatedLessonPlan.ScheduledDate,
                FacultyId = updatedLessonPlan.FacultyId,
                FacultyName = updatedLessonPlan.FacultyName,
                ContentCreationMethod = updatedLessonPlan.ContentCreationMethod,
                LessonDescription = updatedLessonPlan.LessonDescription,
                LearningObjectives = updatedLessonPlan.LearningObjectives,
                TeachingMethods = updatedLessonPlan.TeachingMethods,
                LearningActivities = updatedLessonPlan.LearningActivities,
                DetailedContent = updatedLessonPlan.DetailedContent,
                Resources = updatedLessonPlan.Resources,
                AssessmentMethods = updatedLessonPlan.AssessmentMethods,
                Prerequisites = updatedLessonPlan.Prerequisites,
                DocumentUrl = updatedLessonPlan.DocumentUrl,
                Status = updatedLessonPlan.Status,
                IsActive = updatedLessonPlan.IsActive,
                CreatedDate = updatedLessonPlan.CreatedDate,
                ModifiedDate = updatedLessonPlan.ModifiedDate,
                CreatedBy = updatedLessonPlan.CreatedBy,
                ModifiedBy = updatedLessonPlan.ModifiedBy,
                FileProcessingStatus = updatedLessonPlan.FileProcessingStatus,
                FileProcessingNotes = updatedLessonPlan.FileProcessingNotes,
                OriginalFilename = updatedLessonPlan.OriginalFilename
            };
        }
    }

    public class DeleteLessonPlanCommandHandler : IRequestHandler<DeleteLessonPlanCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLessonPlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteLessonPlanCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.LessonPlanRepository.DeleteAsync(request.Id);
            if (success)
            {
                _unitOfWork.Commit();
            }
            return success;
        }
    }

    public class ToggleLessonPlanActiveCommandHandler : IRequestHandler<ToggleLessonPlanActiveCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleLessonPlanActiveCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ToggleLessonPlanActiveCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.LessonPlanRepository.ToggleActiveAsync(request.Id);
            if (success)
            {
                _unitOfWork.Commit();
            }
            return success;
        }
    }

    // Template Query Handlers
    public class GetLessonPlanTemplatesQueryHandler : IRequestHandler<GetLessonPlanTemplatesQuery, IEnumerable<LessonPlanTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlanTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanTemplateDto>> Handle(GetLessonPlanTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetAllTemplatesAsync();
        }
    }

    public class GetLessonPlanTemplateByIdQueryHandler : IRequestHandler<GetLessonPlanTemplateByIdQuery, LessonPlanTemplateDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlanTemplateByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonPlanTemplateDto?> Handle(GetLessonPlanTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetTemplateByIdAsync(request.Id);
        }
    }

    public class GetActiveLessonPlanTemplatesQueryHandler : IRequestHandler<GetActiveLessonPlanTemplatesQuery, IEnumerable<LessonPlanTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveLessonPlanTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanTemplateDto>> Handle(GetActiveLessonPlanTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetActiveTemplatesAsync();
        }
    }

    public class GetLessonPlanTemplatesByTypeQueryHandler : IRequestHandler<GetLessonPlanTemplatesByTypeQuery, IEnumerable<LessonPlanTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlanTemplatesByTypeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LessonPlanTemplateDto>> Handle(GetLessonPlanTemplatesByTypeQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.LessonPlanRepository.GetTemplatesByTypeAsync(request.TemplateType);
        }
    }

    // Template Command Handlers
    public class CreateLessonPlanTemplateCommandHandler : IRequestHandler<CreateLessonPlanTemplateCommand, LessonPlanTemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLessonPlanTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonPlanTemplateDto> Handle(CreateLessonPlanTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check if template with same name already exists
            var exists = await _unitOfWork.LessonPlanRepository.TemplateExistsByNameAsync(request.Name);
            if (exists)
            {
                throw new InvalidOperationException($"A lesson plan template with name '{request.Name}' already exists.");
            }

            var createDto = new CreateLessonPlanTemplateDto
            {
                Name = request.Name,
                Description = request.Description,
                TemplateType = request.TemplateType,
                DurationMinutes = request.DurationMinutes,
                Sections = request.Sections
            };

            var templateId = await _unitOfWork.LessonPlanRepository.CreateTemplateAsync(createDto);
            _unitOfWork.Commit();

            // Return the created template
            var createdTemplate = await _unitOfWork.LessonPlanRepository.GetTemplateByIdAsync(templateId);
            return createdTemplate ?? throw new InvalidOperationException("Failed to retrieve created lesson plan template");
        }
    }

    public class UpdateLessonPlanTemplateCommandHandler : IRequestHandler<UpdateLessonPlanTemplateCommand, LessonPlanTemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLessonPlanTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonPlanTemplateDto> Handle(UpdateLessonPlanTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check if template exists
            var existingTemplate = await _unitOfWork.LessonPlanRepository.GetTemplateByIdAsync(request.Id);
            if (existingTemplate == null)
            {
                throw new InvalidOperationException($"Lesson plan template with ID {request.Id} not found.");
            }

            // Check if name is being changed and if new name already exists
            if (!string.IsNullOrEmpty(request.Name) && request.Name != existingTemplate.Name)
            {
                var nameExists = await _unitOfWork.LessonPlanRepository.TemplateExistsByNameAsync(request.Name, request.Id);
                if (nameExists)
                {
                    throw new InvalidOperationException($"A lesson plan template with name '{request.Name}' already exists.");
                }
            }

            var updateDto = new UpdateLessonPlanTemplateDto
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                TemplateType = request.TemplateType,
                DurationMinutes = request.DurationMinutes,
                Sections = request.Sections
            };

            var success = await _unitOfWork.LessonPlanRepository.UpdateTemplateAsync(request.Id, updateDto);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update lesson plan template.");
            }

            _unitOfWork.Commit();

            // Return the updated template
            var updatedTemplate = await _unitOfWork.LessonPlanRepository.GetTemplateByIdAsync(request.Id);
            return updatedTemplate ?? throw new InvalidOperationException("Failed to retrieve updated lesson plan template");
        }
    }

    public class DeleteLessonPlanTemplateCommandHandler : IRequestHandler<DeleteLessonPlanTemplateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLessonPlanTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteLessonPlanTemplateCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.LessonPlanRepository.DeleteTemplateAsync(request.Id);
            if (success)
            {
                _unitOfWork.Commit();
            }
            return success;
        }
    }

    public class ToggleLessonPlanTemplateActiveCommandHandler : IRequestHandler<ToggleLessonPlanTemplateActiveCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleLessonPlanTemplateActiveCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ToggleLessonPlanTemplateActiveCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.LessonPlanRepository.ToggleTemplateActiveAsync(request.Id);
            if (success)
            {
                _unitOfWork.Commit();
            }
            return success;
        }
    }
}
