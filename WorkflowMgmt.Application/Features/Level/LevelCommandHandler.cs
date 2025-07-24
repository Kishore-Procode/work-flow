using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Level
{
    public class LevelCommandHandler : IRequestHandler<GetLevelCommand, ApiResponse<List<LevelDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<LevelDTO>>> Handle(GetLevelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var levels = await _unitOfWork.LevelRepository.GetAllLevels();

                return ApiResponse<List<LevelDTO>>.SuccessResponse(levels, "Levels retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<LevelDTO>>.ErrorResponse($"Error during fetching levels: {ex.Message}");
            }
        }
    }

    public class GetLevelByIdCommandHandler : IRequestHandler<GetLevelByIdCommand, ApiResponse<LevelDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLevelByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<LevelDTO>> Handle(GetLevelByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var level = await _unitOfWork.LevelRepository.GetLevelById(request.id);

                if (level == null)
                {
                    return ApiResponse<LevelDTO>.ErrorResponse("Level not found");
                }

                return ApiResponse<LevelDTO>.SuccessResponse(level, "Level retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<LevelDTO>.ErrorResponse($"Error during fetching level: {ex.Message}");
            }
        }
    }

    public class CreateLevelCommandHandler : IRequestHandler<CreateLevelCommand, ApiResponse<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> Handle(CreateLevelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var levelId = await _unitOfWork.LevelRepository.InsertLevel(request.Level);
                _unitOfWork.Commit();

                return ApiResponse<int>.SuccessResponse(levelId, "Level created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<int>.ErrorResponse($"Error during creating level: {ex.Message}");
            }
        }
    }

    public class UpdateLevelCommandHandler : IRequestHandler<UpdateLevelCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateLevelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.LevelRepository.UpdateLevel(request.Level);
                _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(result, "Level updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<bool>.ErrorResponse($"Error during updating level: {ex.Message}");
            }
        }
    }

    public class DeleteOrRestoreLevelCommandHandler : IRequestHandler<DeleteOrRestoreLevelCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrRestoreLevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreLevelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.LevelRepository.DeleteOrRestoreLevel(request.id, request.modifiedBy, request.isRestore);
                _unitOfWork.Commit();

                var message = request.isRestore ? "Level restored successfully" : "Level deleted successfully";
                return ApiResponse<bool>.SuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                var action = request.isRestore ? "restoring" : "deleting";
                return ApiResponse<bool>.ErrorResponse($"Error during {action} level: {ex.Message}");
            }
        }
    }
}
