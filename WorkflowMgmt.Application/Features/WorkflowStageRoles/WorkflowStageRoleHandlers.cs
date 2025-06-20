using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStageRoles
{
    public class GetWorkflowStageRolesQueryHandler : IRequestHandler<GetWorkflowStageRolesQuery, IEnumerable<WorkflowStageRoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowStageRoleDto>> Handle(GetWorkflowStageRolesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageRoleRepository.GetByStageIdAsync(request.WorkflowStageId);
        }
    }

    public class GetActiveRolesQueryHandler : IRequestHandler<GetActiveRolesQuery, IEnumerable<RoleOptionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoleOptionDto>> Handle(GetActiveRolesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageDetailsRepository.GetActiveRolesAsync();
        }
    }

    public class GetWorkflowStageDetailsQueryHandler : IRequestHandler<GetWorkflowStageDetailsQuery, IEnumerable<WorkflowStageDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageDetailsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowStageDetailsDto>> Handle(GetWorkflowStageDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageDetailsRepository.GetByWorkflowTemplateIdAsync(request.WorkflowTemplateId);
        }
    }

    public class GetWorkflowStageDetailsByIdQueryHandler : IRequestHandler<GetWorkflowStageDetailsByIdQuery, WorkflowStageDetailsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageDetailsByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageDetailsDto?> Handle(GetWorkflowStageDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageDetailsRepository.GetByStageIdAsync(request.StageId);
        }
    }

    public class UpdateWorkflowStageRolesCommandHandler : IRequestHandler<UpdateWorkflowStageRolesCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkflowStageRolesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateWorkflowStageRolesCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.WorkflowStageRoleRepository.UpdateStageRolesAsync(request.StageId, request.Roles);
            if (success)
            {
                await _unitOfWork.SaveAsync();
            }
            return success;
        }
    }

    public class AddWorkflowStageRoleCommandHandler : IRequestHandler<AddWorkflowStageRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddWorkflowStageRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddWorkflowStageRoleCommand request, CancellationToken cancellationToken)
        {
            // Check if role already exists for this stage
            var exists = await _unitOfWork.WorkflowStageRoleRepository.ExistsAsync(request.StageId, request.RoleCode);
            if (exists)
            {
                throw new InvalidOperationException($"Role '{request.RoleCode}' is already assigned to this stage.");
            }

            var createDto = new CreateWorkflowStageRoleDto
            {
                RoleCode = request.RoleCode,
                IsRequired = request.IsRequired
            };

            var success = await _unitOfWork.WorkflowStageRoleRepository.CreateAsync(request.StageId, createDto);
            if (success)
            {
                await _unitOfWork.SaveAsync();
            }
            return success;
        }
    }

    public class RemoveWorkflowStageRoleCommandHandler : IRequestHandler<RemoveWorkflowStageRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveWorkflowStageRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RemoveWorkflowStageRoleCommand request, CancellationToken cancellationToken)
        {
            // For now, we'll delete all roles and recreate without the specified one
            // This is a simplified approach - in production you might want a more targeted delete
            var currentRoles = await _unitOfWork.WorkflowStageRoleRepository.GetByStageIdAsync(request.StageId);
            var rolesToKeep = currentRoles.Where(r => r.RoleCode != request.RoleCode)
                                         .Select(r => new UpdateRoleDto 
                                         { 
                                             RoleCode = r.RoleCode, 
                                             IsRequired = r.IsRequired 
                                         });

            var success = await _unitOfWork.WorkflowStageRoleRepository.UpdateStageRolesAsync(request.StageId, rolesToKeep);
            if (success)
            {
                await _unitOfWork.SaveAsync();
            }
            return success;
        }
    }
}
