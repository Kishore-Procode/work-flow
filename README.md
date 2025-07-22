# WorkFlow Management API

## 📖 Overview

The WorkFlow Management API is a comprehensive enterprise-grade workflow management system designed for educational institutions. It provides a robust platform for managing document workflows, user authentication, role-based access control, and educational content management including lesson plans and syllabi.

## 🏛️ Architectural Patterns & Design

### Clean Architecture Implementation

The application strictly follows **Clean Architecture** principles, ensuring separation of concerns, testability, and maintainability:

```
┌─────────────────────────────────────────────────────────────┐
│                    WorkflowMgmt.WebAPI                      │
│                  (Presentation Layer)                       │
│  • Controllers                                              │
│  • Middleware                                               │
│  • API Configuration                                        │
│  • Authentication/Authorization                             │
└─────────────────────┬───────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                WorkflowMgmt.Application                     │
│                 (Application Layer)                         │
│  • Commands & Queries (CQRS)                               │
│  • Command/Query Handlers                                   │
│  • Application Services                                     │
│  • DTOs & Mapping                                           │
│  • Business Logic Orchestration                             │
└─────────────────────┬───────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│                  WorkflowMgmt.Domain                        │
│                   (Domain Layer)                            │
│  • Entities                                                 │
│  • Domain Services                                          │
│  • Repository Interfaces                                    │
│  • Domain Events                                            │
│  • Business Rules & Validation                              │
└─────────────────────┬───────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────────┐
│               WorkflowMgmt.Infrastructure                   │
│                (Infrastructure Layer)                       │
│  • Repository Implementations                               │
│  • Database Context                                         │
│  • External Service Integrations                            │
│  • File System Access                                       │
│  • Email Services                                           │
└─────────────────────────────────────────────────────────────┘
```

### Design Patterns Implemented

#### 1. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Handle write operations (Create, Update, Delete)
- **Queries**: Handle read operations (Get, List, Search)
- **Handlers**: Process commands and queries using MediatR

```csharp
// Example Command
public class CreateUserCommand : IRequest<UserDto>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}

// Example Query
public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }
}
```

#### 2. **Repository Pattern**
- Abstracts data access logic
- Provides consistent interface for data operations
- Enables easy testing with mock implementations

```csharp
public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(string role);
}
```

#### 3. **Unit of Work Pattern**
- Manages database transactions
- Ensures data consistency across multiple repository operations
- Provides atomic operations

#### 4. **Dependency Injection**
- Constructor injection throughout all layers
- Service registration in `Program.cs`
- Promotes loose coupling and testability

#### 5. **Mediator Pattern**
- Decouples request/response handling
- Centralizes cross-cutting concerns
- Implemented using MediatR library

## 🎯 Core Domain Models

### 1. **User Management Domain**

```csharp
public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastLoginDate { get; set; }
}

public class Department : BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public ICollection<User> Users { get; set; }
}
```

### 2. **Document Workflow Domain**

```csharp
public class DocumentWorkflow : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }
    public WorkflowStatus Status { get; set; }
    public Guid CurrentStageId { get; set; }
    public WorkflowStage CurrentStage { get; set; }
    public ICollection<WorkflowStage> Stages { get; set; }
    public ICollection<DocumentFeedback> Feedbacks { get; set; }
}

public class WorkflowStage : BaseEntity
{
    public string Name { get; set; }
    public int Order { get; set; }
    public Guid WorkflowId { get; set; }
    public DocumentWorkflow Workflow { get; set; }
    public ICollection<WorkflowStageRole> RequiredRoles { get; set; }
    public ICollection<WorkflowAction> Actions { get; set; }
}

public enum WorkflowStatus
{
    Draft,
    InProgress,
    UnderReview,
    Approved,
    Rejected,
    Completed
}
```

### 3. **Educational Content Domain**

```csharp
public class LessonPlan : BaseEntity
{
    public string Title { get; set; }
    public string Subject { get; set; }
    public string Grade { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Objectives { get; set; }
    public string Materials { get; set; }
    public string Activities { get; set; }
    public string Assessment { get; set; }
    public Guid TeacherId { get; set; }
    public User Teacher { get; set; }
    public Guid? TemplateId { get; set; }
    public LessonPlanTemplate Template { get; set; }
}

public class Syllabus : BaseEntity
{
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public string Description { get; set; }
    public string Objectives { get; set; }
    public string Content { get; set; }
    public string Assessment { get; set; }
    public Guid SemesterId { get; set; }
    public Semester Semester { get; set; }
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }
}
```

## 🔧 Technical Implementation Details

### Authentication & Authorization

#### JWT Token Implementation
```csharp
public class JwtService
{
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("DepartmentId", user.DepartmentId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

#### Role-Based Authorization
- **Admin**: Full system access
- **Manager**: Department-level management
- **Teacher**: Content creation and management
- **Reviewer**: Document review and approval
- **Student**: Read-only access to approved content

### Real-Time Communication with SignalR

#### Notification Hub Implementation
```csharp
[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinDepartmentGroup(string departmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Department_{departmentId}");
    }

    public async Task SendWorkflowUpdate(string workflowId, string message)
    {
        await Clients.Group($"Workflow_{workflowId}").SendAsync("WorkflowUpdated", message);
    }
}
```

#### Real-Time Features
- **Workflow Status Updates**: Instant notifications when documents move through stages
- **Comment Notifications**: Real-time alerts for new feedback and comments
- **Assignment Notifications**: Immediate alerts for new tasks and assignments
- **System Announcements**: Broadcast messages to specific departments or roles

### Data Access Layer

#### Repository Implementation
```csharp
public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetByDepartmentAsync(Guid departmentId)
    {
        return await _context.Users
            .Where(u => u.DepartmentId == departmentId && u.IsActive)
            .Include(u => u.Department)
            .ToListAsync();
    }
}
```

#### Unit of Work Implementation
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository Users { get; }
    public IDocumentWorkflowRepository DocumentWorkflows { get; }
    public ILessonPlanRepository LessonPlans { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }
}
```

## 🌐 API Endpoints Structure

### Authentication Endpoints
```
POST   /api/auth/login           - User authentication
POST   /api/auth/refresh         - Refresh JWT token
POST   /api/auth/logout          - User logout
POST   /api/auth/change-password - Change user password
```

### User Management Endpoints
```
GET    /api/users                - Get all users (paginated)
GET    /api/users/{id}           - Get user by ID
POST   /api/users                - Create new user
PUT    /api/users/{id}           - Update user
DELETE /api/users/{id}           - Delete user
GET    /api/users/department/{id} - Get users by department
```

### Document Workflow Endpoints
```
GET    /api/workflows            - Get all workflows
GET    /api/workflows/{id}       - Get workflow by ID
POST   /api/workflows            - Create new workflow
PUT    /api/workflows/{id}       - Update workflow
DELETE /api/workflows/{id}       - Delete workflow
POST   /api/workflows/{id}/approve - Approve workflow stage
POST   /api/workflows/{id}/reject  - Reject workflow
GET    /api/workflows/{id}/history - Get workflow history
```

### Educational Content Endpoints
```
GET    /api/lessonplans          - Get lesson plans
POST   /api/lessonplans          - Create lesson plan
PUT    /api/lessonplans/{id}     - Update lesson plan
DELETE /api/lessonplans/{id}     - Delete lesson plan
GET    /api/syllabi              - Get syllabi
POST   /api/syllabi              - Create syllabus
PUT    /api/syllabi/{id}         - Update syllabus
```

### File Management Endpoints
```
POST   /api/files/upload         - Upload file
GET    /api/files/{id}           - Download file
DELETE /api/files/{id}           - Delete file
GET    /api/files/workflow/{id}  - Get workflow files
```

## �️ Middleware & Cross-Cutting Concerns

### Exception Handling Middleware
```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new
        {
            Message = "An error occurred while processing your request",
            Details = exception.Message,
            StatusCode = GetStatusCode(exception)
        };

        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

### Logging with Serilog
```csharp
// Program.cs configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Usage in controllers
public class UserController : BaseApiController
{
    private readonly ILogger<UserController> _logger;

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        _logger.LogInformation("Creating user with email: {Email}", command.Email);
        var result = await Mediator.Send(command);
        _logger.LogInformation("User created successfully with ID: {UserId}", result.Id);
        return Ok(result);
    }
}
```

### CORS Configuration
```csharp
// Environment-specific CORS setup
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        if (builder.Environment.IsDevelopment())
        {
            corsBuilder.SetIsOriginAllowed(origin => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
        else
        {
            var allowedOrigins = builder.Configuration
                .GetSection("CORS:AllowedOrigins").Get<string[]>();
            corsBuilder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
    });
});
```

## ⚙️ Configuration Management

### Environment-Specific Configuration
The application uses a hierarchical configuration system:

1. **Base Configuration** (`appsettings.json`)
   - Common settings shared across environments
   - Application metadata
   - Default logging configuration

2. **Development Configuration** (`appsettings.Development.json`)
   - Development database connections
   - Detailed logging
   - Permissive CORS settings
   - Swagger enabled

3. **Production Configuration** (`appsettings.Production.json`)
   - Production-optimized settings
   - Restricted CORS origins
   - Warning-level logging
   - Environment variable overrides

### Environment Variable Override Pattern
```csharp
// Program.cs - Environment variable override
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPassword))
{
    var connectionString = $"Host={dbHost};Port={dbPort ?? "5432"};Database={dbName ?? "workflow"};Username={dbUser ?? "postgres"};Password={dbPassword}";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
}
```

### Configuration Validation
```csharp
public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 30;
    public int RefreshTokenExpiresInDays { get; set; } = 7;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Key) || Key.Length < 32)
            throw new InvalidOperationException("JWT Key must be at least 32 characters long");

        if (string.IsNullOrEmpty(Issuer))
            throw new InvalidOperationException("JWT Issuer is required");
    }
}
```

## 🧪 Testing Patterns

### Unit Testing with xUnit
```csharp
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserService _userService;

    [Fact]
    public async Task CreateUser_ValidUser_ReturnsUserDto()
    {
        // Arrange
        var command = new CreateUserCommand { Name = "Test User", Email = "test@example.com" };
        var user = new User { Id = Guid.NewGuid(), Name = command.Name, Email = command.Email };

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(user);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.CreateUserAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Email, result.Email);
    }
}
```

### Integration Testing
```csharp
public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    [Fact]
    public async Task GetUsers_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }
}
```

## 🛠️ Development & Deployment

### Prerequisites
- .NET 8 SDK
- PostgreSQL 15+
- Visual Studio 2022 or VS Code

### Local Development
```bash
# Clone and setup
git clone <repository-url>
cd Work-Flow-API
dotnet restore

# Run the application
dotnet run --project WorkflowMgmt.WebAPI
```

### Production Deployment
```bash
# Build for production
dotnet publish -c Release -o out

# Set environment variables and run
export ASPNETCORE_ENVIRONMENT=Production
export DB_HOST=your_db_host
export DB_PASSWORD=your_secure_password
export JWT_SECRET_KEY=your_32_char_minimum_secret
cd out && dotnet WorkflowMgmt.WebAPI.dll
```

## 🎯 Key Business Features

### Workflow Engine
- **Multi-stage approval processes** with role-based assignments
- **Automatic notifications** on status changes
- **Audit trail** for all workflow actions
- **Template-based workflow creation**

### Educational Management
- **Lesson plan creation** with template support
- **Syllabus management** by semester and department
- **Content version control** and approval workflows
- **Department-specific content organization**

### Real-Time Features
- **Live notifications** for workflow updates
- **Real-time collaboration** on documents
- **Instant messaging** for workflow participants
- **Dashboard updates** without page refresh

## 📊 Performance & Security

### Performance Optimizations
- **Entity Framework Core** with optimized queries
- **Async/Await** throughout the application
- **Connection pooling** and caching strategies
- **Pagination** for large data sets

### Security Features
- **JWT Bearer Token** authentication
- **Role-based authorization** with custom policies
- **Input validation** using FluentValidation
- **HTTPS enforcement** in production

## 🔧 Technical Highlights

### Architecture Benefits
- **Clean separation** of concerns across layers
- **Testable design** with dependency injection
- **SOLID principles** implementation
- **Domain-driven design** patterns

### Modern .NET Features
- **Minimal APIs** for lightweight endpoints
- **Background services** for async processing
- **Health checks** for monitoring
- **Configuration binding** with validation

This API represents a comprehensive, enterprise-grade solution built with modern .NET practices, clean architecture principles, and production-ready patterns for educational workflow management.
