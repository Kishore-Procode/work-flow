using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Syllabus;

namespace WorkflowMgmt.Application.Features.SyllabusTemplate
{
    public class GetAllSyllabusTemplatesQueryHandler : IRequestHandler<GetAllSyllabusTemplatesQuery, IEnumerable<SyllabusTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSyllabusTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusTemplateDto>> Handle(GetAllSyllabusTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusTemplateRepository.GetAllAsync();
        }
    }

    public class GetSyllabusTemplatesByTypeQueryHandler : IRequestHandler<GetSyllabusTemplatesByTypeQuery, IEnumerable<SyllabusTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabusTemplatesByTypeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusTemplateDto>> Handle(GetSyllabusTemplatesByTypeQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusTemplateRepository.GetByTypeAsync(request.TemplateType);
        }
    }

    public class GetActiveSyllabusTemplatesQueryHandler : IRequestHandler<GetActiveSyllabusTemplatesQuery, IEnumerable<SyllabusTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveSyllabusTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SyllabusTemplateDto>> Handle(GetActiveSyllabusTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusTemplateRepository.GetActiveAsync();
        }
    }

    public class GetSyllabusTemplateByIdQueryHandler : IRequestHandler<GetSyllabusTemplateByIdQuery, SyllabusTemplateDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabusTemplateByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SyllabusTemplateDto?> Handle(GetSyllabusTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SyllabusTemplateRepository.GetByIdAsync(request.Id);
        }
    }

    public class CreateSyllabusTemplateCommandHandler : IRequestHandler<CreateSyllabusTemplateCommand, SyllabusTemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSyllabusTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SyllabusTemplateDto> Handle(CreateSyllabusTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check if template with same name and type already exists
            var exists = await _unitOfWork.SyllabusTemplateRepository.ExistsByNameAndTypeAsync(request.Name, request.TemplateType);
            if (exists)
            {
                throw new InvalidOperationException($"A syllabus template with name '{request.Name}' for type '{request.TemplateType}' already exists.");
            }

            var createDto = new CreateSyllabusTemplateDto
            {
                Name = request.Name,
                Description = request.Description,
                TemplateType = request.TemplateType,
                Sections = request.Sections
            };

            var templateId = await _unitOfWork.SyllabusTemplateRepository.CreateAsync(createDto);
            await _unitOfWork.SaveAsync();

            var result = await _unitOfWork.SyllabusTemplateRepository.GetByIdAsync(templateId);
            return result ?? throw new InvalidOperationException("Failed to retrieve created syllabus template.");
        }
    }

    public class UpdateSyllabusTemplateCommandHandler : IRequestHandler<UpdateSyllabusTemplateCommand, SyllabusTemplateDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSyllabusTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SyllabusTemplateDto?> Handle(UpdateSyllabusTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check if template exists
            var exists = await _unitOfWork.SyllabusTemplateRepository.ExistsAsync(request.Id);
            if (!exists)
            {
                return null;
            }

            // Check if another template with same name and type already exists (if name/type is being updated)
            if (!string.IsNullOrEmpty(request.Name) && !string.IsNullOrEmpty(request.TemplateType))
            {
                var nameExists = await _unitOfWork.SyllabusTemplateRepository.ExistsByNameAndTypeAsync(request.Name, request.TemplateType, request.Id);
                if (nameExists)
                {
                    throw new InvalidOperationException($"A syllabus template with name '{request.Name}' for type '{request.TemplateType}' already exists.");
                }
            }

            var updateDto = new UpdateSyllabusTemplateDto
            {
                Name = request.Name,
                Description = request.Description,
                TemplateType = request.TemplateType,
                Sections = request.Sections
            };

            var success = await _unitOfWork.SyllabusTemplateRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();
            return await _unitOfWork.SyllabusTemplateRepository.GetByIdAsync(request.Id);
        }
    }

    public class DeleteSyllabusTemplateCommandHandler : IRequestHandler<DeleteSyllabusTemplateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSyllabusTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSyllabusTemplateCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.SyllabusTemplateRepository.DeleteAsync(request.Id);
            if (success)
            {
                await _unitOfWork.SaveAsync();
            }
            return success;
        }
    }

    public class ToggleSyllabusTemplateActiveCommandHandler : IRequestHandler<ToggleSyllabusTemplateActiveCommand, SyllabusTemplateDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleSyllabusTemplateActiveCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SyllabusTemplateDto?> Handle(ToggleSyllabusTemplateActiveCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.SyllabusTemplateRepository.ToggleActiveAsync(request.Id);
            if (!success)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();
            return await _unitOfWork.SyllabusTemplateRepository.GetByIdAsync(request.Id);
        }
    }
}
