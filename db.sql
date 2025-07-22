-- DROP SCHEMA workflowmgmt;

CREATE SCHEMA workflowmgmt AUTHORIZATION pg_database_owner;

COMMENT ON SCHEMA workflowmgmt IS 'standard public schema';

-- DROP SEQUENCE workflowmgmt.acadamic_year_id_seq;

CREATE SEQUENCE workflowmgmt.acadamic_year_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE workflowmgmt.courses_id_seq;

CREATE SEQUENCE workflowmgmt.courses_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE workflowmgmt.degree_id_seq;

CREATE SEQUENCE workflowmgmt.degree_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE workflowmgmt.departments_id_seq;

CREATE SEQUENCE workflowmgmt.departments_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE workflowmgmt.notification_types_id_seq;

CREATE SEQUENCE workflowmgmt.notification_types_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE workflowmgmt.roles_id_seq;

CREATE SEQUENCE workflowmgmt.roles_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE workflowmgmt.semesters_id_seq;

CREATE SEQUENCE workflowmgmt.semesters_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE workflowmgmt.view_feedback_id_seq;

CREATE SEQUENCE workflowmgmt.view_feedback_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;-- workflowmgmt.acadamic_year definition

-- Drop table

-- DROP TABLE workflowmgmt.acadamic_year;

CREATE TABLE workflowmgmt.acadamic_year (
	id serial4 NOT NULL,
	"name" varchar(100) NULL,
	is_active bool DEFAULT true NULL,
	created_date date NULL,
	modified_d date NULL,
	created_by uuid NULL,
	modified_by uuid NULL,
	CONSTRAINT acadamic_year_pkey PRIMARY KEY (id)
);


-- workflowmgmt."degree" definition

-- Drop table

-- DROP TABLE workflowmgmt."degree";

CREATE TABLE workflowmgmt."degree" (
	id serial4 NOT NULL,
	"name" varchar(100) NULL,
	is_active bool DEFAULT true NULL,
	created_date date NULL,
	modified_d date NULL,
	created_by uuid NULL,
	modified_by uuid NULL,
	CONSTRAINT degree_pkey PRIMARY KEY (id)
);


-- workflowmgmt.departments definition

-- Drop table

-- DROP TABLE workflowmgmt.departments;

CREATE TABLE workflowmgmt.departments (
	id serial4 NOT NULL,
	"name" varchar(100) NOT NULL,
	code varchar(20) NOT NULL,
	description text NULL,
	head_of_department varchar(100) NULL,
	email varchar(100) NULL,
	phone varchar(20) NULL,
	established_year int4 NULL,
	programs_offered text NULL,
	accreditation text NULL,
	status varchar(20) DEFAULT 'Active'::character varying NOT NULL,
	created_date timestamp DEFAULT now() NOT NULL,
	modified_date timestamp NULL,
	created_by varchar(50) NULL,
	modified_by varchar(50) NULL,
	is_active bool DEFAULT true NOT NULL,
	email_notify bool DEFAULT false NULL,
	sms_notify bool DEFAULT false NULL,
	in_app_notify bool DEFAULT false NULL,
	digest_frequency varchar(20) NULL,
	CONSTRAINT departments_code_key UNIQUE (code),
	CONSTRAINT departments_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_departments_code ON workflowmgmt.departments USING btree (code);
CREATE INDEX idx_departments_established_year ON workflowmgmt.departments USING btree (established_year);
CREATE INDEX idx_departments_status ON workflowmgmt.departments USING btree (status);
CREATE INDEX idx_departments_status_active ON workflowmgmt.departments USING btree (status, is_active);


-- workflowmgmt.lesson_plan_templates definition

-- Drop table

-- DROP TABLE workflowmgmt.lesson_plan_templates;

CREATE TABLE workflowmgmt.lesson_plan_templates (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	"name" text NOT NULL,
	description text NULL,
	template_type text NOT NULL,
	duration_minutes int4 DEFAULT 60 NOT NULL,
	sections jsonb DEFAULT '[]'::jsonb NOT NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	CONSTRAINT lesson_plan_templates_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_lesson_plan_templates_active ON workflowmgmt.lesson_plan_templates USING btree (is_active);
CREATE INDEX idx_lesson_plan_templates_type ON workflowmgmt.lesson_plan_templates USING btree (template_type);


-- workflowmgmt.notification_types definition

-- Drop table

-- DROP TABLE workflowmgmt.notification_types;

CREATE TABLE workflowmgmt.notification_types (
	id serial4 NOT NULL,
	type_name varchar(50) NOT NULL,
	description text NULL,
	icon varchar(50) NULL,
	color varchar(20) NULL,
	is_active bool DEFAULT true NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	CONSTRAINT notification_types_pkey PRIMARY KEY (id),
	CONSTRAINT notification_types_type_name_key UNIQUE (type_name)
);


-- workflowmgmt.roles definition

-- Drop table

-- DROP TABLE workflowmgmt.roles;

CREATE TABLE workflowmgmt.roles (
	id serial4 NOT NULL,
	"name" varchar(50) NOT NULL,
	code varchar(20) NOT NULL,
	description text NULL,
	permissions _text NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamp DEFAULT now() NOT NULL,
	modified_date timestamp NULL,
	created_by varchar(50) NULL,
	modified_by varchar(50) NULL,
	list_order int4 DEFAULT 0 NOT NULL,
	CONSTRAINT roles_code_key UNIQUE (code),
	CONSTRAINT roles_name_key UNIQUE (name),
	CONSTRAINT roles_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_roles_code ON workflowmgmt.roles USING btree (code);
CREATE INDEX idx_roles_code_active ON workflowmgmt.roles USING btree (code, is_active);
CREATE INDEX idx_roles_is_active ON workflowmgmt.roles USING btree (is_active);


-- workflowmgmt.syllabus_templates definition

-- Drop table

-- DROP TABLE workflowmgmt.syllabus_templates;

CREATE TABLE workflowmgmt.syllabus_templates (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	"name" text NOT NULL,
	description text NULL,
	template_type text NOT NULL,
	sections jsonb DEFAULT '[]'::jsonb NOT NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	html_form_template text NULL,
	CONSTRAINT syllabus_templates_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_syllabus_templates_active ON workflowmgmt.syllabus_templates USING btree (is_active);
CREATE INDEX idx_syllabus_templates_type ON workflowmgmt.syllabus_templates USING btree (template_type);


-- workflowmgmt.view_feedback definition

-- Drop table

-- DROP TABLE workflowmgmt.view_feedback;

CREATE TABLE workflowmgmt.view_feedback (
	id serial4 NOT NULL,
	lesson_id uuid NULL,
	feedback text NOT NULL,
	user_email varchar(256) NULL,
	category varchar(256) NULL,
	rating int4 NULL,
	created_date timestamp DEFAULT now() NOT NULL,
	modified_date timestamp NULL,
	created_by varchar(50) NULL,
	modified_by varchar(50) NULL,
	is_active bool DEFAULT true NOT NULL,
	CONSTRAINT view_feedback_pkey PRIMARY KEY (id),
	CONSTRAINT view_feedback_rating_check CHECK (((rating >= 1) AND (rating <= 5)))
);


-- workflowmgmt.workflow_templates definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_templates;

CREATE TABLE workflowmgmt.workflow_templates (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	"name" text NOT NULL,
	description text NULL,
	document_type text NOT NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	CONSTRAINT workflow_templates_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_workflow_templates_active ON workflowmgmt.workflow_templates USING btree (is_active);
CREATE INDEX idx_workflow_templates_document_type ON workflowmgmt.workflow_templates USING btree (document_type);
CREATE INDEX idx_workflow_templates_document_type_active ON workflowmgmt.workflow_templates USING btree (document_type, is_active);


-- workflowmgmt.courses definition

-- Drop table

-- DROP TABLE workflowmgmt.courses;

CREATE TABLE workflowmgmt.courses (
	id serial4 NOT NULL,
	"name" varchar(100) NOT NULL,
	code varchar(20) NOT NULL,
	description text NULL,
	credits int4 NOT NULL,
	course_type varchar(50) DEFAULT 'Core'::character varying NOT NULL,
	duration_weeks int4 NOT NULL,
	max_capacity int4 NOT NULL,
	status varchar(20) DEFAULT 'Active'::character varying NOT NULL,
	prerequisites text NULL,
	learning_objectives text NULL,
	learning_outcomes text NULL,
	created_date timestamp DEFAULT now() NOT NULL,
	modified_date timestamp NULL,
	created_by varchar(50) NULL,
	modified_by varchar(50) NULL,
	is_active bool DEFAULT true NOT NULL,
	CONSTRAINT courses_code_key UNIQUE (code),
	CONSTRAINT courses_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_courses_code ON workflowmgmt.courses USING btree (code);
CREATE INDEX idx_courses_status ON workflowmgmt.courses USING btree (status);


-- workflowmgmt.notification_templates definition

-- Drop table

-- DROP TABLE workflowmgmt.notification_templates;

CREATE TABLE workflowmgmt.notification_templates (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	notification_type_id int4 NOT NULL,
	template_name varchar(100) NOT NULL,
	title_template text NOT NULL,
	message_template text NOT NULL,
	variables jsonb NULL,
	is_active bool DEFAULT true NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	CONSTRAINT notification_templates_pkey PRIMARY KEY (id),
	CONSTRAINT fk_notif_templates_type FOREIGN KEY (notification_type_id) REFERENCES workflowmgmt.notification_types(id)
);


-- workflowmgmt.semesters definition

-- Drop table

-- DROP TABLE workflowmgmt.semesters;

CREATE TABLE workflowmgmt.semesters (
	id serial4 NOT NULL,
	"name" varchar(100) NOT NULL,
	code varchar(20) NOT NULL,
	academic_year varchar(20) NOT NULL,
	department_id int4 NOT NULL,
	course_id int4 NULL,
	start_date date NOT NULL,
	end_date date NOT NULL,
	duration_weeks int4 NOT NULL,
	"level" varchar(50) DEFAULT 'Undergraduate'::character varying NOT NULL,
	total_students int4 DEFAULT 0 NOT NULL,
	status varchar(20) DEFAULT 'Upcoming'::character varying NOT NULL,
	description text NULL,
	exam_scheduled bool DEFAULT false NOT NULL,
	created_date timestamp DEFAULT now() NOT NULL,
	modified_date timestamp NULL,
	created_by varchar(50) NULL,
	modified_by varchar(50) NULL,
	is_active bool DEFAULT true NOT NULL,
	CONSTRAINT semesters_code_key UNIQUE (code),
	CONSTRAINT semesters_pkey PRIMARY KEY (id),
	CONSTRAINT semesters_course_id_fkey FOREIGN KEY (course_id) REFERENCES workflowmgmt.courses(id),
	CONSTRAINT semesters_department_id_fkey FOREIGN KEY (department_id) REFERENCES workflowmgmt.departments(id)
);
CREATE INDEX idx_semesters_academic_year ON workflowmgmt.semesters USING btree (academic_year);
CREATE INDEX idx_semesters_code ON workflowmgmt.semesters USING btree (code);
CREATE INDEX idx_semesters_course_id ON workflowmgmt.semesters USING btree (course_id);
CREATE INDEX idx_semesters_department_id ON workflowmgmt.semesters USING btree (department_id);
CREATE INDEX idx_semesters_level ON workflowmgmt.semesters USING btree (level);
CREATE INDEX idx_semesters_start_date ON workflowmgmt.semesters USING btree (start_date);
CREATE INDEX idx_semesters_status ON workflowmgmt.semesters USING btree (status);


-- workflowmgmt.syllabi definition

-- Drop table

-- DROP TABLE workflowmgmt.syllabi;

CREATE TABLE workflowmgmt.syllabi (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	title text NOT NULL,
	department_id int4 NOT NULL,
	course_id int4 NULL,
	semester_id int4 NULL,
	template_id uuid NULL,
	faculty_id uuid NOT NULL,
	faculty_name text NOT NULL,
	credits int4 NOT NULL,
	duration_weeks int4 NOT NULL,
	content_creation_method text DEFAULT 'form_entry'::text NOT NULL,
	course_description text NULL,
	learning_objectives text NULL,
	learning_outcomes text NULL,
	course_topics text NULL,
	assessment_methods text NULL,
	detailed_content text NULL,
	reference_materials text NULL,
	document_url text NULL,
	status text DEFAULT 'Draft'::text NOT NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	file_processing_status text DEFAULT 'not_applicable'::text NULL,
	file_processing_notes text NULL,
	original_filename text NULL,
	html_form_data text NULL,
	CONSTRAINT syllabi_pkey PRIMARY KEY (id),
	CONSTRAINT fk_syllabi_course FOREIGN KEY (course_id) REFERENCES workflowmgmt.courses(id),
	CONSTRAINT fk_syllabi_department FOREIGN KEY (department_id) REFERENCES workflowmgmt.departments(id),
	CONSTRAINT fk_syllabi_template FOREIGN KEY (template_id) REFERENCES workflowmgmt.syllabus_templates(id)
);
CREATE INDEX idx_syllabi_active ON workflowmgmt.syllabi USING btree (is_active);
CREATE INDEX idx_syllabi_course_id ON workflowmgmt.syllabi USING btree (course_id);
CREATE INDEX idx_syllabi_department_id ON workflowmgmt.syllabi USING btree (department_id);
CREATE INDEX idx_syllabi_department_status ON workflowmgmt.syllabi USING btree (department_id, status) WHERE (is_active = true);
CREATE INDEX idx_syllabi_faculty_status ON workflowmgmt.syllabi USING btree (faculty_name, status) WHERE (is_active = true);
CREATE INDEX idx_syllabi_semester_id ON workflowmgmt.syllabi USING btree (semester_id);
CREATE INDEX idx_syllabi_status ON workflowmgmt.syllabi USING btree (status);
CREATE INDEX idx_syllabi_status_created ON workflowmgmt.syllabi USING btree (status, created_date) WHERE (is_active = true);
CREATE INDEX idx_syllabi_template_id ON workflowmgmt.syllabi USING btree (template_id);


-- workflowmgmt.users definition

-- Drop table

-- DROP TABLE workflowmgmt.users;

CREATE TABLE workflowmgmt.users (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	username varchar(50) NOT NULL,
	email varchar(100) NOT NULL,
	password_hash varchar(255) NOT NULL,
	first_name varchar(50) NOT NULL,
	last_name varchar(50) NOT NULL,
	role_id int4 NOT NULL,
	department_id int4 NULL,
	phone varchar(20) NULL,
	profile_image_url text NULL,
	is_active bool DEFAULT true NOT NULL,
	last_login timestamp NULL,
	created_date timestamp DEFAULT now() NOT NULL,
	modified_date timestamp NULL,
	created_by varchar(50) NULL,
	modified_by varchar(50) NULL,
	allowed_departments _int4 NULL,
	allowed_roles _int4 NULL,
	CONSTRAINT users_email_key UNIQUE (email),
	CONSTRAINT users_pkey PRIMARY KEY (id),
	CONSTRAINT users_username_key UNIQUE (username),
	CONSTRAINT users_department_id_fkey FOREIGN KEY (department_id) REFERENCES workflowmgmt.departments(id),
	CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES workflowmgmt.roles(id)
);
CREATE INDEX idx_users_department_active ON workflowmgmt.users USING btree (department_id, is_active) WHERE (department_id IS NOT NULL);
CREATE INDEX idx_users_department_id ON workflowmgmt.users USING btree (department_id);
CREATE INDEX idx_users_email ON workflowmgmt.users USING btree (email);
CREATE INDEX idx_users_is_active ON workflowmgmt.users USING btree (is_active);
CREATE INDEX idx_users_last_login ON workflowmgmt.users USING btree (last_login) WHERE (last_login IS NOT NULL);
CREATE INDEX idx_users_role_active ON workflowmgmt.users USING btree (role_id, is_active);
CREATE INDEX idx_users_role_id ON workflowmgmt.users USING btree (role_id);
CREATE INDEX idx_users_username ON workflowmgmt.users USING btree (username);

-- Table Triggers

create trigger trigger_create_default_notification_preferences after
insert
    on
    workflowmgmt.users for each row execute function workflowmgmt.create_default_notification_preferences();


-- workflowmgmt.workflow_department_document_mapping definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_department_document_mapping;

CREATE TABLE workflowmgmt.workflow_department_document_mapping (
	department_id int4 NOT NULL,
	document_type varchar(50) NOT NULL,
	workflow_template_id uuid NOT NULL,
	CONSTRAINT workflow_department_document__department_id_document_type_w_key UNIQUE (department_id, document_type, workflow_template_id),
	CONSTRAINT workflow_department_document_mapping_department_id_fkey FOREIGN KEY (department_id) REFERENCES workflowmgmt.departments(id),
	CONSTRAINT workflow_department_document_mapping_workflow_template_id_fkey FOREIGN KEY (workflow_template_id) REFERENCES workflowmgmt.workflow_templates(id)
);


-- workflowmgmt.workflow_role_mapping definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_role_mapping;

CREATE TABLE workflowmgmt.workflow_role_mapping (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	department_id int4 NOT NULL,
	role_id int4 NOT NULL,
	user_id uuid NOT NULL,
	isprimary bool NULL,
	CONSTRAINT workflow_role_mapping_unique_key UNIQUE (department_id, role_id, user_id),
	CONSTRAINT workflow_role_mapping_department_id_fkey FOREIGN KEY (department_id) REFERENCES workflowmgmt.departments(id),
	CONSTRAINT workflow_role_mapping_role_id_fkey FOREIGN KEY (role_id) REFERENCES workflowmgmt.roles(id),
	CONSTRAINT workflow_role_mapping_user_id_fkey FOREIGN KEY (user_id) REFERENCES workflowmgmt.users(id)
);


-- workflowmgmt.workflow_stages definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_stages;

CREATE TABLE workflowmgmt.workflow_stages (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	workflow_template_id uuid NOT NULL,
	stage_name text NOT NULL,
	stage_order int4 NOT NULL,
	assigned_role text NOT NULL,
	description text NULL,
	is_required bool DEFAULT true NOT NULL,
	auto_approve bool DEFAULT false NOT NULL,
	timeout_days int4 NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	CONSTRAINT workflow_stages_pkey PRIMARY KEY (id),
	CONSTRAINT workflow_stages_workflow_template_id_fkey FOREIGN KEY (workflow_template_id) REFERENCES workflowmgmt.workflow_templates(id) ON DELETE CASCADE
);
CREATE INDEX idx_workflow_stages_assigned_role ON workflowmgmt.workflow_stages USING btree (assigned_role) WHERE (is_active = true);
CREATE INDEX idx_workflow_stages_order ON workflowmgmt.workflow_stages USING btree (workflow_template_id, stage_order);
CREATE INDEX idx_workflow_stages_role ON workflowmgmt.workflow_stages USING btree (assigned_role);
CREATE INDEX idx_workflow_stages_template_id ON workflowmgmt.workflow_stages USING btree (workflow_template_id);
CREATE INDEX idx_workflow_stages_template_order ON workflowmgmt.workflow_stages USING btree (workflow_template_id, stage_order) WHERE (is_active = true);


-- workflowmgmt.department_user_mappings definition

-- Drop table

-- DROP TABLE workflowmgmt.department_user_mappings;

CREATE TABLE workflowmgmt.department_user_mappings (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	user_id uuid NOT NULL,
	department_id int4 NOT NULL,
	"name" text NOT NULL,
	email text NOT NULL,
	is_primary bool DEFAULT false NOT NULL,
	is_backup bool DEFAULT false NOT NULL,
	can_approve_documents _text DEFAULT '{}'::text[] NULL,
	can_reject_documents _text DEFAULT '{}'::text[] NULL,
	email_notifications bool DEFAULT true NOT NULL,
	sms_notifications bool DEFAULT false NOT NULL,
	in_app_notifications bool DEFAULT true NOT NULL,
	digest_frequency text DEFAULT 'Immediate'::text NOT NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	CONSTRAINT department_user_mappings_pkey PRIMARY KEY (id),
	CONSTRAINT department_user_mappings_department_id_fkey FOREIGN KEY (department_id) REFERENCES workflowmgmt.departments(id) ON DELETE CASCADE,
	CONSTRAINT department_user_mappings_user_id_fkey FOREIGN KEY (user_id) REFERENCES workflowmgmt.users(id) ON DELETE CASCADE
);
CREATE INDEX idx_department_user_mappings_active ON workflowmgmt.department_user_mappings USING btree (is_active);
CREATE INDEX idx_department_user_mappings_backup ON workflowmgmt.department_user_mappings USING btree (department_id, is_backup) WHERE (is_backup = true);
CREATE INDEX idx_department_user_mappings_department_id ON workflowmgmt.department_user_mappings USING btree (department_id);
CREATE INDEX idx_department_user_mappings_dept_active ON workflowmgmt.department_user_mappings USING btree (department_id, is_active);
CREATE INDEX idx_department_user_mappings_email ON workflowmgmt.department_user_mappings USING btree (email);
CREATE UNIQUE INDEX idx_department_user_mappings_one_primary ON workflowmgmt.department_user_mappings USING btree (department_id) WHERE ((is_primary = true) AND (is_active = true));
CREATE INDEX idx_department_user_mappings_primary ON workflowmgmt.department_user_mappings USING btree (department_id, is_primary) WHERE (is_primary = true);
CREATE UNIQUE INDEX idx_department_user_mappings_unique ON workflowmgmt.department_user_mappings USING btree (user_id, department_id) WHERE (is_active = true);
CREATE INDEX idx_department_user_mappings_user_active ON workflowmgmt.department_user_mappings USING btree (user_id, is_active);
CREATE INDEX idx_department_user_mappings_user_id ON workflowmgmt.department_user_mappings USING btree (user_id);


-- workflowmgmt.document_feedback definition

-- Drop table

-- DROP TABLE workflowmgmt.document_feedback;

CREATE TABLE workflowmgmt.document_feedback (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	document_id uuid NOT NULL,
	document_type text NOT NULL,
	workflow_stage_id uuid NULL,
	feedback_provider uuid NULL,
	feedback_text text NOT NULL,
	feedback_type text DEFAULT 'general'::text NULL,
	is_addressed bool DEFAULT false NULL,
	addressed_by uuid NULL,
	addressed_date timestamptz NULL,
	is_active bool DEFAULT true NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	CONSTRAINT document_feedback_pkey PRIMARY KEY (id),
	CONSTRAINT document_feedback_addressed_by_fkey FOREIGN KEY (addressed_by) REFERENCES workflowmgmt.users(id),
	CONSTRAINT document_feedback_feedback_provider_fkey FOREIGN KEY (feedback_provider) REFERENCES workflowmgmt.users(id),
	CONSTRAINT document_feedback_workflow_stage_id_fkey FOREIGN KEY (workflow_stage_id) REFERENCES workflowmgmt.workflow_stages(id)
);
CREATE INDEX idx_document_feedback_active ON workflowmgmt.document_feedback USING btree (is_active);
CREATE INDEX idx_document_feedback_document ON workflowmgmt.document_feedback USING btree (document_id, document_type);
CREATE INDEX idx_document_feedback_provider ON workflowmgmt.document_feedback USING btree (feedback_provider);
CREATE INDEX idx_document_feedback_stage ON workflowmgmt.document_feedback USING btree (workflow_stage_id);
CREATE INDEX idx_document_feedback_type ON workflowmgmt.document_feedback USING btree (feedback_type);


-- workflowmgmt.document_uploads definition

-- Drop table

-- DROP TABLE workflowmgmt.document_uploads;

CREATE TABLE workflowmgmt.document_uploads (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	document_id uuid NOT NULL,
	document_type text NOT NULL,
	original_filename text NOT NULL,
	file_size int8 NOT NULL,
	file_type text NOT NULL,
	file_url text NOT NULL,
	upload_status text DEFAULT 'pending'::text NOT NULL,
	uploaded_by uuid NULL,
	upload_date timestamptz DEFAULT now() NULL,
	processed_date timestamptz NULL,
	extracted_content text NULL,
	processing_notes text NULL,
	is_active bool DEFAULT true NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	CONSTRAINT document_uploads_pkey PRIMARY KEY (id),
	CONSTRAINT document_uploads_uploaded_by_fkey FOREIGN KEY (uploaded_by) REFERENCES workflowmgmt.users(id)
);
CREATE INDEX idx_document_uploads_document ON workflowmgmt.document_uploads USING btree (document_id, document_type);
CREATE INDEX idx_document_uploads_status ON workflowmgmt.document_uploads USING btree (upload_status);
CREATE INDEX idx_document_uploads_upload_date ON workflowmgmt.document_uploads USING btree (upload_date);
CREATE INDEX idx_document_uploads_uploaded_by ON workflowmgmt.document_uploads USING btree (uploaded_by);


-- workflowmgmt.document_workflows definition

-- Drop table

-- DROP TABLE workflowmgmt.document_workflows;

CREATE TABLE workflowmgmt.document_workflows (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	document_id uuid NOT NULL,
	document_type text NOT NULL,
	workflow_template_id uuid NOT NULL,
	current_stage_id uuid NULL,
	status text DEFAULT 'In Progress'::text NOT NULL,
	initiated_by uuid NOT NULL,
	initiated_date timestamptz DEFAULT now() NULL,
	completed_date timestamptz NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	assigned_to uuid NULL, -- User currently assigned to handle this workflow
	CONSTRAINT document_workflows_pkey PRIMARY KEY (id),
	CONSTRAINT document_workflows_assigned_to_fkey FOREIGN KEY (assigned_to) REFERENCES workflowmgmt.users(id),
	CONSTRAINT document_workflows_current_stage_id_fkey FOREIGN KEY (current_stage_id) REFERENCES workflowmgmt.workflow_stages(id),
	CONSTRAINT document_workflows_initiated_by_fkey FOREIGN KEY (initiated_by) REFERENCES workflowmgmt.users(id),
	CONSTRAINT document_workflows_workflow_template_id_fkey FOREIGN KEY (workflow_template_id) REFERENCES workflowmgmt.workflow_templates(id)
);
CREATE INDEX idx_document_workflows_assigned_to ON workflowmgmt.document_workflows USING btree (assigned_to);
CREATE INDEX idx_document_workflows_completed_date ON workflowmgmt.document_workflows USING btree (completed_date) WHERE (completed_date IS NOT NULL);
CREATE INDEX idx_document_workflows_current_stage ON workflowmgmt.document_workflows USING btree (current_stage_id);
CREATE INDEX idx_document_workflows_document ON workflowmgmt.document_workflows USING btree (document_id, document_type);
CREATE INDEX idx_document_workflows_document_type_status ON workflowmgmt.document_workflows USING btree (document_type, status);
CREATE INDEX idx_document_workflows_initiated_by ON workflowmgmt.document_workflows USING btree (initiated_by);
CREATE INDEX idx_document_workflows_initiated_date ON workflowmgmt.document_workflows USING btree (initiated_date);
CREATE INDEX idx_document_workflows_status ON workflowmgmt.document_workflows USING btree (status);
CREATE INDEX idx_document_workflows_status_active ON workflowmgmt.document_workflows USING btree (status, is_active);
CREATE INDEX idx_document_workflows_template ON workflowmgmt.document_workflows USING btree (workflow_template_id);

-- Column comments

COMMENT ON COLUMN workflowmgmt.document_workflows.assigned_to IS 'User currently assigned to handle this workflow';


-- workflowmgmt.lesson_plans definition

-- Drop table

-- DROP TABLE workflowmgmt.lesson_plans;

CREATE TABLE workflowmgmt.lesson_plans (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	title text NOT NULL,
	syllabus_id uuid NULL,
	template_id uuid NOT NULL,
	module_name text NOT NULL,
	duration_minutes int4 DEFAULT 60 NOT NULL,
	number_of_sessions int4 DEFAULT 1 NOT NULL,
	scheduled_date date NULL,
	faculty_id uuid NOT NULL,
	faculty_name text NOT NULL,
	content_creation_method text DEFAULT 'form_entry'::text NOT NULL,
	lesson_description text NULL,
	learning_objectives text NULL,
	teaching_methods text NULL,
	learning_activities text NULL,
	detailed_content text NULL,
	resources text NULL,
	assessment_methods text NULL,
	prerequisites text NULL,
	document_url text NULL,
	status text DEFAULT 'Draft'::text NOT NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	file_processing_status text DEFAULT 'not_applicable'::text NULL,
	file_processing_notes text NULL,
	original_filename text NULL,
	CONSTRAINT lesson_plans_pkey PRIMARY KEY (id),
	CONSTRAINT fk_lesson_plans_syllabus FOREIGN KEY (syllabus_id) REFERENCES workflowmgmt.syllabi(id),
	CONSTRAINT fk_lesson_plans_template FOREIGN KEY (template_id) REFERENCES workflowmgmt.lesson_plan_templates(id)
);
CREATE INDEX idx_lesson_plans_active ON workflowmgmt.lesson_plans USING btree (is_active);
CREATE INDEX idx_lesson_plans_faculty ON workflowmgmt.lesson_plans USING btree (faculty_name);
CREATE INDEX idx_lesson_plans_faculty_status ON workflowmgmt.lesson_plans USING btree (faculty_name, status) WHERE (is_active = true);
CREATE INDEX idx_lesson_plans_scheduled_date ON workflowmgmt.lesson_plans USING btree (scheduled_date);
CREATE INDEX idx_lesson_plans_status ON workflowmgmt.lesson_plans USING btree (status);
CREATE INDEX idx_lesson_plans_status_created ON workflowmgmt.lesson_plans USING btree (status, created_date) WHERE (is_active = true);
CREATE INDEX idx_lesson_plans_syllabus_id ON workflowmgmt.lesson_plans USING btree (syllabus_id);
CREATE INDEX idx_lesson_plans_template_id ON workflowmgmt.lesson_plans USING btree (template_id);


-- workflowmgmt.notifications definition

-- Drop table

-- DROP TABLE workflowmgmt.notifications;

CREATE TABLE workflowmgmt.notifications (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	user_id uuid NOT NULL,
	notification_type_id int4 NOT NULL,
	title varchar(255) NOT NULL,
	message text NOT NULL,
	document_id uuid NULL,
	document_type varchar(50) NULL,
	related_user_id uuid NULL,
	metadata jsonb NULL,
	is_read bool DEFAULT false NULL,
	is_deleted bool DEFAULT false NULL,
	priority int4 DEFAULT 1 NULL,
	expires_at timestamptz NULL,
	created_date timestamptz DEFAULT now() NULL,
	read_date timestamptz NULL,
	CONSTRAINT notifications_pkey PRIMARY KEY (id),
	CONSTRAINT fk_notifications_related_user FOREIGN KEY (related_user_id) REFERENCES workflowmgmt.users(id) ON DELETE SET NULL,
	CONSTRAINT fk_notifications_type FOREIGN KEY (notification_type_id) REFERENCES workflowmgmt.notification_types(id),
	CONSTRAINT fk_notifications_user FOREIGN KEY (user_id) REFERENCES workflowmgmt.users(id) ON DELETE CASCADE
);
CREATE INDEX idx_notifications_created_date ON workflowmgmt.notifications USING btree (created_date DESC);
CREATE INDEX idx_notifications_document_id ON workflowmgmt.notifications USING btree (document_id);
CREATE INDEX idx_notifications_document_type ON workflowmgmt.notifications USING btree (document_type);
CREATE INDEX idx_notifications_is_read ON workflowmgmt.notifications USING btree (is_read);
CREATE INDEX idx_notifications_priority ON workflowmgmt.notifications USING btree (priority DESC);
CREATE INDEX idx_notifications_user_id ON workflowmgmt.notifications USING btree (user_id);
CREATE INDEX idx_notifications_user_unread ON workflowmgmt.notifications USING btree (user_id, is_read) WHERE (is_read = false);


-- workflowmgmt.refresh_tokens definition

-- Drop table

-- DROP TABLE workflowmgmt.refresh_tokens;

CREATE TABLE workflowmgmt.refresh_tokens (
	id uuid DEFAULT gen_random_uuid() NOT NULL, -- Unique identifier for the refresh token record
	"token" varchar(500) NOT NULL, -- The actual refresh token string
	user_id uuid NOT NULL, -- Reference to the user who owns this token
	expires_at timestamptz NOT NULL, -- When this refresh token expires
	created_at timestamptz DEFAULT now() NOT NULL, -- When this refresh token was created
	is_revoked bool DEFAULT false NOT NULL, -- Whether this token has been revoked
	revoked_reason varchar(255) NULL, -- Reason why the token was revoked
	revoked_at timestamptz NULL, -- When the token was revoked
	replaced_by_token varchar(500) NULL, -- Token that replaced this one during refresh
	CONSTRAINT refresh_tokens_pkey PRIMARY KEY (id),
	CONSTRAINT refresh_tokens_token_key UNIQUE (token),
	CONSTRAINT fk_refresh_tokens_user_id FOREIGN KEY (user_id) REFERENCES workflowmgmt.users(id) ON DELETE CASCADE
);
CREATE INDEX idx_refresh_tokens_expires_at ON workflowmgmt.refresh_tokens USING btree (expires_at);
CREATE INDEX idx_refresh_tokens_is_revoked ON workflowmgmt.refresh_tokens USING btree (is_revoked);
CREATE INDEX idx_refresh_tokens_token ON workflowmgmt.refresh_tokens USING btree (token);
CREATE INDEX idx_refresh_tokens_user_id ON workflowmgmt.refresh_tokens USING btree (user_id);
COMMENT ON TABLE workflowmgmt.refresh_tokens IS 'Stores JWT refresh tokens for user authentication';

-- Column comments

COMMENT ON COLUMN workflowmgmt.refresh_tokens.id IS 'Unique identifier for the refresh token record';
COMMENT ON COLUMN workflowmgmt.refresh_tokens."token" IS 'The actual refresh token string';
COMMENT ON COLUMN workflowmgmt.refresh_tokens.user_id IS 'Reference to the user who owns this token';
COMMENT ON COLUMN workflowmgmt.refresh_tokens.expires_at IS 'When this refresh token expires';
COMMENT ON COLUMN workflowmgmt.refresh_tokens.created_at IS 'When this refresh token was created';
COMMENT ON COLUMN workflowmgmt.refresh_tokens.is_revoked IS 'Whether this token has been revoked';
COMMENT ON COLUMN workflowmgmt.refresh_tokens.revoked_reason IS 'Reason why the token was revoked';
COMMENT ON COLUMN workflowmgmt.refresh_tokens.revoked_at IS 'When the token was revoked';
COMMENT ON COLUMN workflowmgmt.refresh_tokens.replaced_by_token IS 'Token that replaced this one during refresh';


-- workflowmgmt.sessions definition

-- Drop table

-- DROP TABLE workflowmgmt.sessions;

CREATE TABLE workflowmgmt.sessions (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	title text NOT NULL,
	lesson_plan_id uuid NULL,
	faculty_id uuid NOT NULL,
	teaching_method text DEFAULT 'Lecture'::text NOT NULL,
	session_date date NULL,
	session_time time NULL,
	duration_minutes int4 DEFAULT 90 NOT NULL,
	instructor text NOT NULL,
	content_creation_method text DEFAULT 'form_entry'::text NOT NULL,
	session_description text NULL,
	session_objectives text NULL,
	session_activities text NULL,
	materials_equipment text NULL,
	detailed_content text NULL,
	content_resources text NULL,
	document_url text NULL,
	status text DEFAULT 'Draft'::text NOT NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	created_by text NULL,
	modified_by text NULL,
	file_processing_status text DEFAULT 'not_applicable'::text NULL,
	file_processing_notes text NULL,
	original_filename text NULL,
	CONSTRAINT sessions_pkey PRIMARY KEY (id),
	CONSTRAINT fk_sessions_faculty FOREIGN KEY (faculty_id) REFERENCES workflowmgmt.users(id)
);
CREATE INDEX idx_sessions_active ON workflowmgmt.sessions USING btree (is_active);
CREATE INDEX idx_sessions_date ON workflowmgmt.sessions USING btree (session_date);
CREATE INDEX idx_sessions_instructor ON workflowmgmt.sessions USING btree (instructor);
CREATE INDEX idx_sessions_instructor_status ON workflowmgmt.sessions USING btree (instructor, status) WHERE (is_active = true);
CREATE INDEX idx_sessions_lesson_plan_id ON workflowmgmt.sessions USING btree (lesson_plan_id);
CREATE INDEX idx_sessions_session_date ON workflowmgmt.sessions USING btree (session_date) WHERE (session_date IS NOT NULL);
CREATE INDEX idx_sessions_status ON workflowmgmt.sessions USING btree (status);
CREATE INDEX idx_sessions_status_created ON workflowmgmt.sessions USING btree (status, created_date) WHERE (is_active = true);
CREATE INDEX idx_sessions_teaching_method ON workflowmgmt.sessions USING btree (teaching_method);


-- workflowmgmt.user_notification_preferences definition

-- Drop table

-- DROP TABLE workflowmgmt.user_notification_preferences;

CREATE TABLE workflowmgmt.user_notification_preferences (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	user_id uuid NOT NULL,
	notification_type_id int4 NOT NULL,
	is_enabled bool DEFAULT true NULL,
	delivery_method varchar(20) DEFAULT 'in_app'::character varying NULL,
	created_date timestamptz DEFAULT now() NULL,
	modified_date timestamptz NULL,
	CONSTRAINT uk_user_notification_type UNIQUE (user_id, notification_type_id),
	CONSTRAINT user_notification_preferences_pkey PRIMARY KEY (id),
	CONSTRAINT fk_user_notif_prefs_type FOREIGN KEY (notification_type_id) REFERENCES workflowmgmt.notification_types(id),
	CONSTRAINT fk_user_notif_prefs_user FOREIGN KEY (user_id) REFERENCES workflowmgmt.users(id) ON DELETE CASCADE
);
CREATE INDEX idx_user_notif_prefs_user_id ON workflowmgmt.user_notification_preferences USING btree (user_id);


-- workflowmgmt.workflow_stage_actions definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_stage_actions;

CREATE TABLE workflowmgmt.workflow_stage_actions (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	workflow_stage_id uuid NOT NULL,
	action_name text NOT NULL,
	action_type text NOT NULL,
	next_stage_id uuid NULL,
	is_active bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NULL,
	CONSTRAINT workflow_stage_actions_pkey PRIMARY KEY (id),
	CONSTRAINT workflow_stage_actions_next_stage_id_fkey FOREIGN KEY (next_stage_id) REFERENCES workflowmgmt.workflow_stages(id) ON DELETE SET NULL,
	CONSTRAINT workflow_stage_actions_workflow_stage_id_fkey FOREIGN KEY (workflow_stage_id) REFERENCES workflowmgmt.workflow_stages(id) ON DELETE CASCADE
);
CREATE INDEX idx_workflow_stage_actions_stage_id ON workflowmgmt.workflow_stage_actions USING btree (workflow_stage_id);
CREATE INDEX idx_workflow_stage_actions_type ON workflowmgmt.workflow_stage_actions USING btree (action_type);


-- workflowmgmt.workflow_stage_history definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_stage_history;

CREATE TABLE workflowmgmt.workflow_stage_history (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	document_workflow_id uuid NOT NULL,
	stage_id uuid NOT NULL,
	action_taken text NOT NULL,
	processed_by uuid NOT NULL,
	processed_date timestamptz DEFAULT now() NULL,
	"comments" text NULL,
	attachments _text NULL,
	created_date timestamptz DEFAULT now() NULL,
	assigned_to uuid NULL, -- User assigned to the next stage after this action
	CONSTRAINT workflow_stage_history_pkey PRIMARY KEY (id),
	CONSTRAINT workflow_stage_history_assigned_to_fkey FOREIGN KEY (assigned_to) REFERENCES workflowmgmt.users(id),
	CONSTRAINT workflow_stage_history_document_workflow_id_fkey FOREIGN KEY (document_workflow_id) REFERENCES workflowmgmt.document_workflows(id) ON DELETE CASCADE,
	CONSTRAINT workflow_stage_history_processed_by_fkey FOREIGN KEY (processed_by) REFERENCES workflowmgmt.users(id),
	CONSTRAINT workflow_stage_history_stage_id_fkey FOREIGN KEY (stage_id) REFERENCES workflowmgmt.workflow_stages(id)
);
CREATE INDEX idx_workflow_stage_history_action_date ON workflowmgmt.workflow_stage_history USING btree (action_taken, processed_date);
CREATE INDEX idx_workflow_stage_history_assigned_to ON workflowmgmt.workflow_stage_history USING btree (assigned_to);
CREATE INDEX idx_workflow_stage_history_date ON workflowmgmt.workflow_stage_history USING btree (processed_date);
CREATE INDEX idx_workflow_stage_history_processed_by ON workflowmgmt.workflow_stage_history USING btree (processed_by);
CREATE INDEX idx_workflow_stage_history_stage ON workflowmgmt.workflow_stage_history USING btree (stage_id);
CREATE INDEX idx_workflow_stage_history_workflow ON workflowmgmt.workflow_stage_history USING btree (document_workflow_id);
CREATE INDEX idx_workflow_stage_history_workflow_action ON workflowmgmt.workflow_stage_history USING btree (document_workflow_id, action_taken);

-- Column comments

COMMENT ON COLUMN workflowmgmt.workflow_stage_history.assigned_to IS 'User assigned to the next stage after this action';


-- workflowmgmt.workflow_stage_permissions definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_stage_permissions;

CREATE TABLE workflowmgmt.workflow_stage_permissions (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	workflow_stage_id uuid NOT NULL,
	permission_name varchar(100) NOT NULL,
	is_required bool DEFAULT false NOT NULL,
	created_date timestamptz DEFAULT now() NOT NULL,
	created_by text NULL,
	CONSTRAINT workflow_stage_permissions_pkey PRIMARY KEY (id),
	CONSTRAINT workflow_stage_permissions_unique UNIQUE (workflow_stage_id, permission_name),
	CONSTRAINT workflow_stage_permissions_stage_fkey FOREIGN KEY (workflow_stage_id) REFERENCES workflowmgmt.workflow_stages(id) ON DELETE CASCADE
);
CREATE INDEX idx_workflow_stage_permissions_name ON workflowmgmt.workflow_stage_permissions USING btree (permission_name);
CREATE INDEX idx_workflow_stage_permissions_required ON workflowmgmt.workflow_stage_permissions USING btree (workflow_stage_id, is_required);
CREATE INDEX idx_workflow_stage_permissions_stage_id ON workflowmgmt.workflow_stage_permissions USING btree (workflow_stage_id);


-- workflowmgmt.workflow_stage_roles definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_stage_roles;

CREATE TABLE workflowmgmt.workflow_stage_roles (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	workflow_stage_id uuid NOT NULL,
	role_code varchar(20) NOT NULL,
	is_required bool DEFAULT true NOT NULL,
	created_date timestamptz DEFAULT now() NOT NULL,
	created_by text NULL,
	CONSTRAINT workflow_stage_roles_pkey PRIMARY KEY (id),
	CONSTRAINT workflow_stage_roles_unique UNIQUE (workflow_stage_id, role_code),
	CONSTRAINT workflow_stage_roles_role_fkey FOREIGN KEY (role_code) REFERENCES workflowmgmt.roles(code) ON DELETE CASCADE,
	CONSTRAINT workflow_stage_roles_stage_fkey FOREIGN KEY (workflow_stage_id) REFERENCES workflowmgmt.workflow_stages(id) ON DELETE CASCADE
);
CREATE INDEX idx_workflow_stage_roles_required ON workflowmgmt.workflow_stage_roles USING btree (workflow_stage_id, is_required);
CREATE INDEX idx_workflow_stage_roles_role_code ON workflowmgmt.workflow_stage_roles USING btree (role_code);
CREATE INDEX idx_workflow_stage_roles_stage_id ON workflowmgmt.workflow_stage_roles USING btree (workflow_stage_id);


-- workflowmgmt.notification_delivery_log definition

-- Drop table

-- DROP TABLE workflowmgmt.notification_delivery_log;

CREATE TABLE workflowmgmt.notification_delivery_log (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	notification_id uuid NOT NULL,
	delivery_method varchar(20) NOT NULL,
	delivery_status varchar(20) DEFAULT 'pending'::character varying NULL,
	delivery_attempt int4 DEFAULT 1 NULL,
	error_message text NULL,
	delivered_at timestamptz NULL,
	created_date timestamptz DEFAULT now() NULL,
	CONSTRAINT notification_delivery_log_pkey PRIMARY KEY (id),
	CONSTRAINT fk_delivery_log_notification FOREIGN KEY (notification_id) REFERENCES workflowmgmt.notifications(id) ON DELETE CASCADE
);
CREATE INDEX idx_delivery_log_notification_id ON workflowmgmt.notification_delivery_log USING btree (notification_id);
CREATE INDEX idx_delivery_log_status ON workflowmgmt.notification_delivery_log USING btree (delivery_status);


-- workflowmgmt.workflow_audit_log definition

-- Drop table

-- DROP TABLE workflowmgmt.workflow_audit_log;

CREATE TABLE workflowmgmt.workflow_audit_log (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	document_workflow_id uuid NOT NULL,
	action_name varchar(100) NOT NULL,
	processed_by uuid NOT NULL,
	"comments" text NULL,
	feedback_id uuid NULL,
	history_id uuid NULL,
	created_date timestamptz DEFAULT CURRENT_TIMESTAMP NOT NULL,
	CONSTRAINT workflow_audit_log_pkey PRIMARY KEY (id),
	CONSTRAINT fk_audit_document_workflow FOREIGN KEY (document_workflow_id) REFERENCES workflowmgmt.document_workflows(id) ON DELETE CASCADE,
	CONSTRAINT fk_audit_feedback FOREIGN KEY (feedback_id) REFERENCES workflowmgmt.document_feedback(id) ON DELETE SET NULL,
	CONSTRAINT fk_audit_history FOREIGN KEY (history_id) REFERENCES workflowmgmt.workflow_stage_history(id) ON DELETE SET NULL,
	CONSTRAINT fk_audit_processed_by FOREIGN KEY (processed_by) REFERENCES workflowmgmt.users(id) ON DELETE RESTRICT
);
CREATE INDEX idx_workflow_audit_log_action_name ON workflowmgmt.workflow_audit_log USING btree (action_name);
CREATE INDEX idx_workflow_audit_log_created_date ON workflowmgmt.workflow_audit_log USING btree (created_date DESC);
CREATE INDEX idx_workflow_audit_log_document_workflow_id ON workflowmgmt.workflow_audit_log USING btree (document_workflow_id);
CREATE INDEX idx_workflow_audit_log_processed_by ON workflowmgmt.workflow_audit_log USING btree (processed_by);


-- workflowmgmt.v_notification_summary source

CREATE OR REPLACE VIEW workflowmgmt.v_notification_summary
AS SELECT n.id,
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
    (ru.first_name::text || ' '::text) || ru.last_name::text AS related_user_name,
    ru.email AS related_user_email
   FROM workflowmgmt.notifications n
     JOIN workflowmgmt.notification_types nt ON n.notification_type_id = nt.id
     LEFT JOIN workflowmgmt.users ru ON n.related_user_id = ru.id
  WHERE n.is_deleted = false
  ORDER BY n.created_date DESC;


-- workflowmgmt.workflow_stage_details source

CREATE OR REPLACE VIEW workflowmgmt.workflow_stage_details
AS SELECT ws.id AS stage_id,
    ws.workflow_template_id,
    ws.stage_name,
    ws.stage_order,
    ws.assigned_role,
    ws.description,
    ws.is_required,
    ws.auto_approve,
    ws.timeout_days,
    ws.is_active,
    ws.created_date,
    ws.modified_date,
    COALESCE(json_agg(json_build_object('role_code', wsr.role_code, 'role_name', r.name, 'is_required', wsr.is_required) ORDER BY r.name) FILTER (WHERE wsr.role_code IS NOT NULL), '[]'::json) AS required_roles,
    COALESCE(json_agg(json_build_object('permission_name', wsp.permission_name, 'is_required', wsp.is_required) ORDER BY wsp.permission_name) FILTER (WHERE wsp.permission_name IS NOT NULL), '[]'::json) AS stage_permissions,
    COALESCE(( SELECT json_agg(json_build_object('id', wsa.id, 'action_name', wsa.action_name, 'action_type', wsa.action_type, 'next_stage_id', wsa.next_stage_id) ORDER BY wsa.action_name) AS json_agg
           FROM workflowmgmt.workflow_stage_actions wsa
          WHERE wsa.workflow_stage_id = ws.id AND wsa.is_active = true), '[]'::json) AS actions
   FROM workflowmgmt.workflow_stages ws
     LEFT JOIN workflowmgmt.workflow_stage_roles wsr ON ws.id = wsr.workflow_stage_id
     LEFT JOIN workflowmgmt.roles r ON wsr.role_code::text = r.code::text AND r.is_active = true
     LEFT JOIN workflowmgmt.workflow_stage_permissions wsp ON ws.id = wsp.workflow_stage_id
  GROUP BY ws.id, ws.workflow_template_id, ws.stage_name, ws.stage_order, ws.assigned_role, ws.description, ws.is_required, ws.auto_approve, ws.timeout_days, ws.is_active, ws.created_date, ws.modified_date;



-- DROP FUNCTION workflowmgmt.can_user_access_document(uuid, text, text);

CREATE OR REPLACE FUNCTION workflowmgmt.can_user_access_document(user_uuid uuid, doc_id text, doc_type text)
 RETURNS boolean
 LANGUAGE plpgsql
 SECURITY DEFINER
AS $function$
DECLARE
    user_role_code text;
    is_document_owner boolean := false;
BEGIN
    -- Get user role
    SELECT r.code INTO user_role_code
    FROM users u
    JOIN roles r ON u.role_id = r.id
    WHERE u.id = user_uuid AND u.is_active = true;
    
    -- Check if user is document owner
    CASE doc_type
        WHEN 'Syllabus' THEN
            SELECT (s.created_by = user_uuid::text OR s.faculty_name = (SELECT first_name || ' ' || last_name FROM users WHERE id = user_uuid))
            INTO is_document_owner
            FROM syllabi s WHERE s.id = doc_id::uuid;
        WHEN 'LessonPlan' THEN
            SELECT (lp.created_by = user_uuid::text OR lp.faculty_name = (SELECT first_name || ' ' || last_name FROM users WHERE id = user_uuid))
            INTO is_document_owner
            FROM lesson_plans lp WHERE lp.id = doc_id::uuid;
        WHEN 'Session' THEN
            SELECT (sess.created_by = user_uuid::text OR sess.instructor = (SELECT first_name || ' ' || last_name FROM users WHERE id = user_uuid))
            INTO is_document_owner
            FROM sessions sess WHERE sess.id = doc_id::uuid;
    END CASE;
    
    -- Admin and leadership can access all documents
    IF user_role_code IN ('ADMIN', 'LEADERSHIP') THEN
        RETURN true;
    END IF;
    
    -- Document owners can access their documents
    IF is_document_owner THEN
        RETURN true;
    END IF;
    
    -- Program conveners can access documents in workflow
    IF user_role_code = 'PROGRAM_CONVENER' THEN
        RETURN EXISTS (
            SELECT 1 FROM document_workflows dw
            JOIN workflow_stages ws ON dw.current_stage_id = ws.id
            WHERE dw.document_id = doc_id 
            AND dw.document_type = doc_type
            AND ws.assigned_role = user_role_code
            AND dw.is_active = true
        );
    END IF;
    
    -- Feedback providers (Shareholders and Industry Experts) can access documents in their feedback stages
    IF user_role_code IN ('SHAREHOLDER', 'INDUSTRY_EXPERT') THEN
        RETURN EXISTS (
            SELECT 1 FROM document_workflows dw
            JOIN workflow_stages ws ON dw.current_stage_id = ws.id
            WHERE dw.document_id = doc_id 
            AND dw.document_type = doc_type
            AND ws.assigned_role = user_role_code
            AND dw.is_active = true
        ) OR EXISTS (
            -- Also allow access if they have previously provided feedback
            SELECT 1 FROM document_feedback df
            WHERE df.document_id = doc_id
            AND df.document_type = doc_type
            AND df.feedback_provider = user_uuid
            AND df.is_active = true
        );
    END IF;
    
    RETURN false;
END;
$function$
;

-- DROP FUNCTION workflowmgmt.cleanup_old_notifications(int4);

CREATE OR REPLACE FUNCTION workflowmgmt.cleanup_old_notifications(days_to_keep integer DEFAULT 90)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM workflowmgmt.notifications 
    WHERE created_date < NOW() - INTERVAL '1 day' * days_to_keep
    AND is_read = true;
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RETURN deleted_count;
END;
$function$
;

-- DROP FUNCTION workflowmgmt.cleanup_old_uploads();

CREATE OR REPLACE FUNCTION workflowmgmt.cleanup_old_uploads()
 RETURNS void
 LANGUAGE plpgsql
 SECURITY DEFINER
AS $function$
BEGIN
  -- Mark uploads older than 30 days as inactive if they're still pending
  UPDATE document_uploads 
  SET is_active = false, modified_date = now()
  WHERE upload_status = 'pending' 
    AND upload_date < now() - INTERVAL '30 days'
    AND is_active = true;
    
  -- Clean up failed uploads older than 7 days
  UPDATE document_uploads 
  SET is_active = false, modified_date = now()
  WHERE upload_status = 'failed' 
    AND upload_date < now() - INTERVAL '7 days'
    AND is_active = true;
END;
$function$
;

-- DROP FUNCTION workflowmgmt.create_default_notification_preferences();

CREATE OR REPLACE FUNCTION workflowmgmt.create_default_notification_preferences()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    -- Insert default preferences for all notification types for the new user
    INSERT INTO workflowmgmt.user_notification_preferences (user_id, notification_type_id, is_enabled, delivery_method)
    SELECT NEW.id, nt.id, true, 'in_app'
    FROM workflowmgmt.notification_types nt
    WHERE nt.is_active = true;
    
    RETURN NEW;
END;
$function$
;

-- DROP FUNCTION workflowmgmt.get_available_permissions();

CREATE OR REPLACE FUNCTION workflowmgmt.get_available_permissions()
 RETURNS TABLE(permission_name character varying, display_name character varying, category character varying)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT 
        p.permission_name::VARCHAR(100),
        p.permission_name::VARCHAR(100) as display_name, -- You can customize this
        CASE 
            WHEN p.permission_name LIKE '%syllabus%' THEN 'syllabus'
            WHEN p.permission_name LIKE '%lesson%' THEN 'lesson'
            WHEN p.permission_name LIKE '%session%' THEN 'session'
            WHEN p.permission_name LIKE '%workflow%' THEN 'workflow'
            ELSE 'general'
        END::VARCHAR(50) as category
    FROM (
        VALUES 
            ('create_syllabus'),
            ('edit_own_syllabus'),
            ('create_lesson'),
            ('review_syllabus_level1'),
            ('review_syllabus_level2'),
            ('final_approval'),
            ('view_all_content'),
            ('create_session'),
            ('manage_workflows')
    ) AS p(permission_name);
END;
$function$
;

-- DROP FUNCTION workflowmgmt.get_document_upload_status(text, text);

CREATE OR REPLACE FUNCTION workflowmgmt.get_document_upload_status(p_document_id text, p_document_type text)
 RETURNS TABLE(upload_id uuid, original_filename text, file_size bigint, file_type text, upload_status text, upload_date timestamp with time zone, processed_date timestamp with time zone, processing_notes text)
 LANGUAGE plpgsql
 SECURITY DEFINER
AS $function$
BEGIN
  RETURN QUERY
  SELECT 
    du.id,
    du.original_filename,
    du.file_size,
    du.file_type,
    du.upload_status,
    du.upload_date,
    du.processed_date,
    du.processing_notes
  FROM document_uploads du
  WHERE du.document_id = p_document_id 
    AND du.document_type = p_document_type
    AND du.is_active = true
  ORDER BY du.upload_date DESC;
END;
$function$
;

-- DROP FUNCTION workflowmgmt.get_user_permissions(uuid);

CREATE OR REPLACE FUNCTION workflowmgmt.get_user_permissions(user_uuid uuid)
 RETURNS text[]
 LANGUAGE plpgsql
 SECURITY DEFINER
AS $function$
DECLARE
    user_permissions text[];
BEGIN
    SELECT r.permissions INTO user_permissions
    FROM users u
    JOIN roles r ON u.role_id = r.id
    WHERE u.id = user_uuid AND u.is_active = true;
    
    RETURN COALESCE(user_permissions, ARRAY[]::text[]);
END;
$function$
;

-- DROP FUNCTION workflowmgmt.process_document_upload(uuid, text, text);

CREATE OR REPLACE FUNCTION workflowmgmt.process_document_upload(p_upload_id uuid, p_extracted_content text, p_processing_notes text DEFAULT NULL::text)
 RETURNS boolean
 LANGUAGE plpgsql
 SECURITY DEFINER
AS $function$
DECLARE
  upload_record document_uploads%ROWTYPE;
  success boolean := false;
BEGIN
  -- Get upload record
  SELECT * INTO upload_record FROM document_uploads WHERE id = p_upload_id;
  
  IF NOT FOUND THEN
    RAISE EXCEPTION 'Upload record not found';
  END IF;

  -- Update the corresponding document table based on document_type
  CASE upload_record.document_type
    WHEN 'Syllabus' THEN
      UPDATE syllabi 
      SET 
        detailed_content = COALESCE(detailed_content, '') || E'\n\n--- Extracted from uploaded document ---\n' || p_extracted_content,
        file_processing_status = 'completed',
        file_processing_notes = p_processing_notes,
        original_filename = upload_record.original_filename,
        modified_date = now()
      WHERE id = upload_record.document_id::uuid;
      
    WHEN 'LessonPlan' THEN
      UPDATE lesson_plans 
      SET 
        detailed_content = COALESCE(detailed_content, '') || E'\n\n--- Extracted from uploaded document ---\n' || p_extracted_content,
        file_processing_status = 'completed',
        file_processing_notes = p_processing_notes,
        original_filename = upload_record.original_filename,
        modified_date = now()
      WHERE id = upload_record.document_id::uuid;
      
    WHEN 'Session' THEN
      UPDATE sessions 
      SET 
        detailed_content = COALESCE(detailed_content, '') || E'\n\n--- Extracted from uploaded document ---\n' || p_extracted_content,
        file_processing_status = 'completed',
        file_processing_notes = p_processing_notes,
        original_filename = upload_record.original_filename,
        modified_date = now()
      WHERE id = upload_record.document_id::uuid;
  END CASE;

  -- Update upload record
  UPDATE document_uploads 
  SET 
    upload_status = 'completed',
    processed_date = now(),
    extracted_content = p_extracted_content,
    processing_notes = p_processing_notes,
    modified_date = now()
  WHERE id = p_upload_id;

  success := true;
  RETURN success;

EXCEPTION
  WHEN OTHERS THEN
    -- Update upload record with error status
    UPDATE document_uploads 
    SET 
      upload_status = 'failed',
      processed_date = now(),
      processing_notes = SQLERRM,
      modified_date = now()
    WHERE id = p_upload_id;
    
    RETURN false;
END;
$function$
;