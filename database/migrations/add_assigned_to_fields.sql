-- Migration: Add assigned_to fields to workflow tables
-- Date: 2025-01-25
-- Description: Add assigned_to UUID field to document_workflows and workflow_stage_history tables

-- Add assigned_to field to document_workflows table
ALTER TABLE workflowmgmt.document_workflows 
ADD COLUMN assigned_to UUID NULL;

-- Add foreign key constraint for assigned_to in document_workflows
ALTER TABLE workflowmgmt.document_workflows 
ADD CONSTRAINT document_workflows_assigned_to_fkey 
FOREIGN KEY (assigned_to) REFERENCES workflowmgmt.users(id);

-- Add assigned_to field to workflow_stage_history table
ALTER TABLE workflowmgmt.workflow_stage_history 
ADD COLUMN assigned_to UUID NULL;

-- Add foreign key constraint for assigned_to in workflow_stage_history
ALTER TABLE workflowmgmt.workflow_stage_history 
ADD CONSTRAINT workflow_stage_history_assigned_to_fkey 
FOREIGN KEY (assigned_to) REFERENCES workflowmgmt.users(id);

-- Add indexes for better performance
CREATE INDEX idx_document_workflows_assigned_to 
ON workflowmgmt.document_workflows USING btree (assigned_to);

CREATE INDEX idx_workflow_stage_history_assigned_to 
ON workflowmgmt.workflow_stage_history USING btree (assigned_to);

-- Update existing records to set assigned_to = initiated_by for document_workflows
UPDATE workflowmgmt.document_workflows 
SET assigned_to = initiated_by 
WHERE assigned_to IS NULL;

-- Add comments for documentation
COMMENT ON COLUMN workflowmgmt.document_workflows.assigned_to IS 'User currently assigned to handle this workflow';
COMMENT ON COLUMN workflowmgmt.workflow_stage_history.assigned_to IS 'User assigned to the next stage after this action';
