-- Notification Tables for SignalR-based notification system

-- 1. Notification Types Table
CREATE TABLE workflowmgmt.notification_types (
    id SERIAL PRIMARY KEY,
    type_name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    icon VARCHAR(50),
    color VARCHAR(20),
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT NOW(),
    modified_date TIMESTAMPTZ
);

-- Insert default notification types
INSERT INTO workflowmgmt.notification_types (type_name, description, icon, color) VALUES
('document_status_updated', 'Document status has been updated', 'file-text', 'blue'),
('document_commented', 'New comment added to document', 'message-square', 'green'),
('document_approved', 'Document has been approved', 'check-circle', 'green'),
('document_rejected', 'Document has been rejected', 'x-circle', 'red'),
('document_assigned', 'Document has been assigned to you', 'user', 'blue'),
('workflow_stage_changed', 'Document workflow stage changed', 'activity', 'orange'),
('feedback_received', 'New feedback received on document', 'message-circle', 'yellow'),
('deadline_reminder', 'Document deadline reminder', 'clock', 'red'),
('system_notification', 'System notification', 'bell', 'gray');

-- 2. Notifications Table
CREATE TABLE workflowmgmt.notifications (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL,
    notification_type_id INTEGER NOT NULL,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    document_id UUID,
    document_type VARCHAR(50), -- 'syllabus', 'lesson', 'session'
    related_user_id UUID, -- User who triggered the notification
    metadata JSONB, -- Additional data like stage_id, action_type, etc.
    is_read BOOLEAN DEFAULT false,
    is_deleted BOOLEAN DEFAULT false,
    priority INTEGER DEFAULT 1, -- 1=low, 2=medium, 3=high, 4=urgent
    expires_at TIMESTAMPTZ,
    created_date TIMESTAMPTZ DEFAULT NOW(),
    read_date TIMESTAMPTZ,
    CONSTRAINT fk_notifications_user FOREIGN KEY (user_id) REFERENCES workflowmgmt.users(id) ON DELETE CASCADE,
    CONSTRAINT fk_notifications_type FOREIGN KEY (notification_type_id) REFERENCES workflowmgmt.notification_types(id),
    CONSTRAINT fk_notifications_related_user FOREIGN KEY (related_user_id) REFERENCES workflowmgmt.users(id) ON DELETE SET NULL
);

-- 3. User Notification Preferences Table
CREATE TABLE workflowmgmt.user_notification_preferences (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL,
    notification_type_id INTEGER NOT NULL,
    is_enabled BOOLEAN DEFAULT true,
    delivery_method VARCHAR(20) DEFAULT 'in_app', -- 'in_app', 'email', 'both'
    created_date TIMESTAMPTZ DEFAULT NOW(),
    modified_date TIMESTAMPTZ,
    CONSTRAINT fk_user_notif_prefs_user FOREIGN KEY (user_id) REFERENCES workflowmgmt.users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_notif_prefs_type FOREIGN KEY (notification_type_id) REFERENCES workflowmgmt.notification_types(id),
    CONSTRAINT uk_user_notification_type UNIQUE (user_id, notification_type_id)
);

-- 4. Notification Templates Table
CREATE TABLE workflowmgmt.notification_templates (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    notification_type_id INTEGER NOT NULL,
    template_name VARCHAR(100) NOT NULL,
    title_template TEXT NOT NULL,
    message_template TEXT NOT NULL,
    variables JSONB, -- Available template variables
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT NOW(),
    modified_date TIMESTAMPTZ,
    CONSTRAINT fk_notif_templates_type FOREIGN KEY (notification_type_id) REFERENCES workflowmgmt.notification_types(id)
);

-- Insert default notification templates
INSERT INTO workflowmgmt.notification_templates (notification_type_id, template_name, title_template, message_template, variables) VALUES
(1, 'document_status_updated', 'Document Status Updated', 'The status of "{document_title}" has been changed to {new_status} by {user_name}', '{"document_title": "string", "new_status": "string", "user_name": "string"}'),
(2, 'document_commented', 'New Comment Added', '{user_name} added a comment to "{document_title}"', '{"document_title": "string", "user_name": "string", "comment": "string"}'),
(3, 'document_approved', 'Document Approved', 'Your document "{document_title}" has been approved by {user_name}', '{"document_title": "string", "user_name": "string"}'),
(4, 'document_rejected', 'Document Rejected', 'Your document "{document_title}" has been rejected by {user_name}', '{"document_title": "string", "user_name": "string", "reason": "string"}'),
(5, 'document_assigned', 'Document Assigned', 'Document "{document_title}" has been assigned to you for {stage_name}', '{"document_title": "string", "stage_name": "string", "assigned_by": "string"}'),
(6, 'workflow_stage_changed', 'Workflow Stage Changed', 'Document "{document_title}" has moved to {new_stage} stage', '{"document_title": "string", "new_stage": "string", "previous_stage": "string"}'),
(7, 'feedback_received', 'Feedback Received', 'You received feedback on "{document_title}" from {user_name}', '{"document_title": "string", "user_name": "string", "feedback_type": "string"}'),
(8, 'deadline_reminder', 'Deadline Reminder', 'Document "{document_title}" deadline is approaching', '{"document_title": "string", "deadline": "datetime", "days_remaining": "number"}'),
(9, 'system_notification', 'System Notification', '{message}', '{"message": "string"}');

-- 5. Notification Delivery Log Table (for tracking delivery status)
CREATE TABLE workflowmgmt.notification_delivery_log (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    notification_id UUID NOT NULL,
    delivery_method VARCHAR(20) NOT NULL,
    delivery_status VARCHAR(20) DEFAULT 'pending', -- 'pending', 'sent', 'delivered', 'failed'
    delivery_attempt INTEGER DEFAULT 1,
    error_message TEXT,
    delivered_at TIMESTAMPTZ,
    created_date TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT fk_delivery_log_notification FOREIGN KEY (notification_id) REFERENCES workflowmgmt.notifications(id) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX idx_notifications_user_id ON workflowmgmt.notifications(user_id);
CREATE INDEX idx_notifications_document_id ON workflowmgmt.notifications(document_id);
CREATE INDEX idx_notifications_is_read ON workflowmgmt.notifications(is_read);
CREATE INDEX idx_notifications_created_date ON workflowmgmt.notifications(created_date DESC);
CREATE INDEX idx_notifications_user_unread ON workflowmgmt.notifications(user_id, is_read) WHERE is_read = false;
CREATE INDEX idx_notifications_document_type ON workflowmgmt.notifications(document_type);
CREATE INDEX idx_notifications_priority ON workflowmgmt.notifications(priority DESC);

-- Create indexes for notification preferences
CREATE INDEX idx_user_notif_prefs_user_id ON workflowmgmt.user_notification_preferences(user_id);

-- Create indexes for delivery log
CREATE INDEX idx_delivery_log_notification_id ON workflowmgmt.notification_delivery_log(notification_id);
CREATE INDEX idx_delivery_log_status ON workflowmgmt.notification_delivery_log(delivery_status);

-- Function to automatically set default notification preferences for new users
CREATE OR REPLACE FUNCTION workflowmgmt.create_default_notification_preferences()
RETURNS TRIGGER AS $$
BEGIN
    -- Insert default preferences for all notification types for the new user
    INSERT INTO workflowmgmt.user_notification_preferences (user_id, notification_type_id, is_enabled, delivery_method)
    SELECT NEW.id, nt.id, true, 'in_app'
    FROM workflowmgmt.notification_types nt
    WHERE nt.is_active = true;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create trigger to automatically create default preferences for new users
CREATE TRIGGER trigger_create_default_notification_preferences
    AFTER INSERT ON workflowmgmt.users
    FOR EACH ROW
    EXECUTE FUNCTION workflowmgmt.create_default_notification_preferences();

-- Function to clean up old notifications (can be called by a scheduled job)
CREATE OR REPLACE FUNCTION workflowmgmt.cleanup_old_notifications(days_to_keep INTEGER DEFAULT 90)
RETURNS INTEGER AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM workflowmgmt.notifications 
    WHERE created_date < NOW() - INTERVAL '1 day' * days_to_keep
    AND is_read = true;
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RETURN deleted_count;
END;
$$ LANGUAGE plpgsql;

-- View for notification summary
CREATE OR REPLACE VIEW workflowmgmt.v_notification_summary AS
SELECT 
    n.id,
    n.user_id,
    n.title,
    n.message,
    n.document_id,
    n.document_type,
    n.is_read,
    n.priority,
    n.created_date,
    n.read_date,
    nt.type_name,
    nt.icon,
    nt.color,
    ru.first_name || ' ' || ru.last_name as related_user_name,
    ru.email as related_user_email
FROM workflowmgmt.notifications n
JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
LEFT JOIN workflowmgmt.users ru ON n.related_user_id = ru.id
WHERE n.is_deleted = false
ORDER BY n.created_date DESC;
