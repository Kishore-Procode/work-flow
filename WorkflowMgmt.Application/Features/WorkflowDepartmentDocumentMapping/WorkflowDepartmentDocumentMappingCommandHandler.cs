using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowDepartmentDocumentMapping
{
    public class GetDepartmentDocumentMappingsCommandHandler : IRequestHandler<GetDepartmentDocumentMappingsCommand, ApiResponse<List<DocumentTypeWorkflowMappingDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentDocumentMappingsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentTypeWorkflowMappingDto>>> Handle(GetDepartmentDocumentMappingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate department exists
                var departmentExists = await _unitOfWork.DepartmentRepository.GetDepartmentById(request.departmentId);
                if (departmentExists == null)
                {
                    return ApiResponse<List<DocumentTypeWorkflowMappingDto>>.ErrorResponse($"Department with ID {request.departmentId} not found");
                }

                var mappings = await _unitOfWork.WorkflowDepartmentDocumentMappingRepository.GetDepartmentDocumentMappings(request.departmentId);
                return ApiResponse<List<DocumentTypeWorkflowMappingDto>>.SuccessResponse(mappings, "Department document mappings retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DocumentTypeWorkflowMappingDto>>.ErrorResponse($"Error during fetching department document mappings: {ex.Message}");
            }
        }
    }

    public class UpdateDepartmentDocumentMappingsCommandHandler : IRequestHandler<UpdateDepartmentDocumentMappingsCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentDocumentMappingsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDepartmentDocumentMappingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate department exists
                var departmentExists = await _unitOfWork.DepartmentRepository.GetDepartmentById(request.request.department_id);
                if (departmentExists == null)
                {
                    return ApiResponse<bool>.ErrorResponse($"Department with ID {request.request.department_id} not found");
                }

                // Validate document types
                var validDocumentTypes = new[] { "syllabus", "lesson", "session" };
                var invalidMappings = request.request.mappings.Where(m => !validDocumentTypes.Contains(m.document_type.ToLower())).ToList();
                
                if (invalidMappings.Any())
                {
                    var invalidTypes = string.Join(", ", invalidMappings.Select(m => m.document_type));
                    return ApiResponse<bool>.ErrorResponse($"Invalid document types: {invalidTypes}. Valid types are: syllabus, lesson, session");
                }

                // Check for duplicate document types in the request
                var duplicateDocumentTypes = request.request.mappings
                    .GroupBy(m => m.document_type.ToLower())
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateDocumentTypes.Any())
                {
                    return ApiResponse<bool>.ErrorResponse($"Duplicate document types found: {string.Join(", ", duplicateDocumentTypes)}");
                }

                var success = await _unitOfWork.WorkflowDepartmentDocumentMappingRepository.UpdateDepartmentDocumentMappings(
                    request.request.department_id, 
                    request.request.mappings);

                if (success)
                    _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(success, "Department document mappings updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating department document mappings: {ex.Message}");
            }
        }
    }
}
