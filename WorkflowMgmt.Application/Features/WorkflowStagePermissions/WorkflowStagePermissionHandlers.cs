using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStagePermissions
{
    public class GetWorkflowStagePermissionsQueryHandler : IRequestHandler<GetWorkflowStagePermissionsQuery, IEnumerable<WorkflowStagePermissionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStagePermissionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowStagePermissionDto>> Handle(GetWorkflowStagePermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStagePermissionRepository.GetByStageIdAsync(request.WorkflowStageId);
        }
    }

    public class GetAvailablePermissionsQueryHandler : IRequestHandler<GetAvailablePermissionsQuery, IEnumerable<AvailablePermissionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailablePermissionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AvailablePermissionDto>> Handle(GetAvailablePermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStagePermissionRepository.GetAvailablePermissionsAsync();
        }
    }

    public class UpdateWorkflowStagePermissionsCommandHandler : IRequestHandler<UpdateWorkflowStagePermissionsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkflowStagePermissionsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateWorkflowStagePermissionsCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.WorkflowStagePermissionRepository.UpdateStagePermissionsAsync(request.StageId, request.Permissions);
            if (success)
            {
                await _unitOfWork.SaveAsync();
            }
            return success;
        }
    }

    public class AddWorkflowStagePermissionCommandHandler : IRequestHandler<AddWorkflowStagePermissionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddWorkflowStagePermissionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddWorkflowStagePermissionCommand request, CancellationToken cancellationToken)
        {
            // Check if permission already exists for this stage
            var exists = await _unitOfWork.WorkflowStagePermissionRepository.ExistsAsync(request.StageId, request.PermissionName);
            if (exists)
            {
                throw new InvalidOperationException($"Permission '{request.PermissionName}' is already assigned to this stage.");
            }

            var createDto = new CreateWorkflowStagePermissionDto
            {
                PermissionName = request.PermissionName,
                IsRequired = request.IsRequired
            };

            var success = await _unitOfWork.WorkflowStagePermissionRepository.CreateAsync(request.StageId, createDto);
            if (success)
            {
                await _unitOfWork.SaveAsync();
            }
            return success;
        }
    }

    public class RemoveWorkflowStagePermissionCommandHandler : IRequestHandler<RemoveWorkflowStagePermissionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveWorkflowStagePermissionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RemoveWorkflowStagePermissionCommand request, CancellationToken cancellationToken)
        {
            // Get current permissions and remove the specified one
            var currentPermissions = await _unitOfWork.WorkflowStagePermissionRepository.GetByStageIdAsync(request.StageId);
            var permissionsToKeep = currentPermissions.Where(p => p.PermissionName != request.PermissionName)
                                                     .Select(p => new UpdatePermissionDto 
                                                     { 
                                                         PermissionName = p.PermissionName, 
                                                         IsRequired = p.IsRequired 
                                                     });

            var success = await _unitOfWork.WorkflowStagePermissionRepository.UpdateStagePermissionsAsync(request.StageId, permissionsToKeep);
            if (success)
            {
                await _unitOfWork.SaveAsync();
            }
            return success;
        }
    }
}
