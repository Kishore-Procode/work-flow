using System;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Services
{
    /// <summary>
    /// Example class demonstrating how to use the enhanced workflow action processing
    /// This shows the complete flow from validation to execution to notifications
    /// </summary>
    public class WorkflowActionProcessingExample
    {
        private readonly IWorkflowActionProcessorService _actionProcessor;

        public WorkflowActionProcessingExample(IWorkflowActionProcessorService actionProcessor)
        {
            _actionProcessor = actionProcessor;
        }

        /// <summary>
        /// Example: Process a syllabus approval action
        /// </summary>
        public async Task<bool> ProcessSyllabusApprovalAsync(Guid syllabusId, Guid approverUserId, string comments)
        {
            try
            {
                // Create the action DTO
                var actionDto = new ProcessDocumentActionDto
                {
                    DocumentId = syllabusId,
                    DocumentType = "syllabus",
                    ActionId = Guid.NewGuid(), // This would be the actual action ID from the workflow
                    Comments = comments,
                    FeedbackType = "approval"
                };

                // Process the action
                var result = await _actionProcessor.ProcessActionAsync(actionDto, approverUserId);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"✅ Syllabus {syllabusId} approved successfully");
                    Console.WriteLine($"📝 Comments: {comments}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Failed to approve syllabus: {result.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error processing approval: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Example: Process a syllabus rejection action
        /// </summary>
        public async Task<bool> ProcessSyllabusRejectionAsync(Guid syllabusId, Guid reviewerUserId, string reason)
        {
            try
            {
                var actionDto = new ProcessDocumentActionDto
                {
                    DocumentId = syllabusId,
                    DocumentType = "syllabus",
                    ActionId = Guid.NewGuid(), // This would be the reject action ID
                    Comments = reason,
                    FeedbackType = "rejection"
                };

                var result = await _actionProcessor.ProcessActionAsync(actionDto, reviewerUserId);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"🚫 Syllabus {syllabusId} rejected");
                    Console.WriteLine($"📝 Reason: {reason}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Failed to reject syllabus: {result.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error processing rejection: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Example: Validate if a user can perform an action before attempting it
        /// </summary>
        public async Task<bool> ValidateUserActionAsync(Guid userId, Guid documentId, Guid actionId)
        {
            try
            {
                var canPerform = await _actionProcessor.CanUserPerformActionAsync(userId, documentId, "syllabus", actionId);

                if (canPerform)
                {
                    Console.WriteLine($"✅ User {userId} can perform action {actionId} on document {documentId}");
                }
                else
                {
                    Console.WriteLine($"🚫 User {userId} cannot perform action {actionId} on document {documentId}");
                }

                return canPerform;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error validating user action: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Example: Process a request for revision action
        /// </summary>
        public async Task<bool> ProcessRevisionRequestAsync(Guid documentId, Guid reviewerUserId, string revisionNotes)
        {
            try
            {
                var actionDto = new ProcessDocumentActionDto
                {
                    DocumentId = documentId,
                    DocumentType = "syllabus",
                    ActionId = Guid.NewGuid(), // This would be the revision request action ID
                    Comments = revisionNotes,
                    FeedbackType = "revision"
                };

                var result = await _actionProcessor.ProcessActionAsync(actionDto, reviewerUserId);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"📝 Revision requested for document {documentId}");
                    Console.WriteLine($"📋 Notes: {revisionNotes}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ Failed to request revision: {result.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error processing revision request: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Example: Demonstrate the complete workflow processing flow
        /// </summary>
        public async Task DemonstrateWorkflowProcessingAsync()
        {
            Console.WriteLine("🔄 Workflow Action Processing Demonstration");
            Console.WriteLine("==========================================");

            var syllabusId = Guid.NewGuid();
            var facultyUserId = Guid.NewGuid();
            var departmentHeadUserId = Guid.NewGuid();
            var deanUserId = Guid.NewGuid();

            Console.WriteLine($"📄 Processing syllabus: {syllabusId}");
            Console.WriteLine($"👨‍🏫 Faculty: {facultyUserId}");
            Console.WriteLine($"👨‍💼 Department Head: {departmentHeadUserId}");
            Console.WriteLine($"👨‍💼 Dean: {deanUserId}");
            Console.WriteLine();

            // Step 1: Department Head Review
            Console.WriteLine("Step 1: Department Head Review");
            Console.WriteLine("------------------------------");
            
            var canReview = await ValidateUserActionAsync(departmentHeadUserId, syllabusId, Guid.NewGuid());
            if (canReview)
            {
                await ProcessSyllabusApprovalAsync(syllabusId, departmentHeadUserId, 
                    "Syllabus content looks good. Approved for next stage.");
            }
            Console.WriteLine();

            // Step 2: Dean Review
            Console.WriteLine("Step 2: Dean Review");
            Console.WriteLine("-------------------");
            
            var canApprove = await ValidateUserActionAsync(deanUserId, syllabusId, Guid.NewGuid());
            if (canApprove)
            {
                await ProcessSyllabusApprovalAsync(syllabusId, deanUserId, 
                    "Final approval granted. Syllabus is now published.");
            }
            Console.WriteLine();

            // Alternative: Rejection scenario
            Console.WriteLine("Alternative: Rejection Scenario");
            Console.WriteLine("-------------------------------");
            
            await ProcessSyllabusRejectionAsync(syllabusId, departmentHeadUserId, 
                "Course objectives need to be more specific. Please revise and resubmit.");
            Console.WriteLine();

            // Alternative: Revision request scenario
            Console.WriteLine("Alternative: Revision Request Scenario");
            Console.WriteLine("--------------------------------------");
            
            await ProcessRevisionRequestAsync(syllabusId, deanUserId, 
                "Please add more details about assessment methods and grading criteria.");
            Console.WriteLine();

            Console.WriteLine("✅ Workflow processing demonstration completed!");
        }
    }
}
