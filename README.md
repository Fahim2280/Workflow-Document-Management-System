# Workflow & Document Management System

A comprehensive document management and workflow automation system built with .NET 9 Web API, designed to streamline document processing, task assignments, and workflow management in enterprise environments.

## 🚀 Features

### Document Management
- **Document Upload & Storage**: Support for multiple file formats with 100MB file size limit
- **Document Types**: Configurable document categorization and classification
- **Document Activities**: Track document lifecycle and activities
- **File Operations**: Download and view documents with secure access controls
- **Metadata Management**: Comprehensive document metadata handling

### Workflow Management
- **Custom Workflows**: Create and configure custom approval workflows
- **Multi-step Processing**: Support for complex, multi-stage document approval processes
- **Workflow Templates**: Reusable workflow configurations for different document types
- **Status Tracking**: Real-time workflow status monitoring

### Task Assignment & Management
- **Smart Task Assignment**: Automated task distribution based on workflow rules
- **Task Dashboard**: Personal task management interface for users
- **Batch Operations**: Process multiple documents simultaneously
- **Task Reassignment**: Flexible task redistribution capabilities
- **Assignment History**: Complete audit trail of task assignments

### Advanced Features
- **Activity Logging**: Comprehensive audit trails for all system activities
- **Validation Framework**: Robust input validation and data integrity checks
- **File Service Integration**: Secure file handling and storage management
- **API-First Design**: RESTful API architecture for easy integration

## 🛠️ Technology Stack

- **Framework**: .NET 9 Web API
- **Language**: C# (Latest version)
- **Database**: Microsoft SQL Server
- **Data Access**: Stored Procedures for all CRUD operations
- **Architecture**: Clean Architecture with Repository Pattern
- **API Style**: RESTful Web API

## 📁 Project Structure

```
Workflow & Document Management System/
├── Controllers/
│   ├── DocumentController.cs          # Document CRUD operations
│   ├── DocumentTypeController.cs      # Document type management
│   ├── TaskController.cs              # Task assignment & processing
│   └── WorkflowController.cs          # Workflow configuration
├── DTOs/
│   ├── CreateDocumentDto.cs           # Document creation models
│   ├── ApiResponseDto.cs              # Standardized API responses
│   ├── ProcessDocumentActionDto.cs    # Task processing models
│   └── [Other DTOs...]                # Additional data transfer objects
├── Models/
│   ├── Document.cs                    # Document entity models
│   ├── Workflow.cs                    # Workflow entity models
│   ├── Task.cs                        # Task entity models
│   └── [Other Models...]              # Additional entity models
├── Repository/
│   ├── DocumentRepository.cs          # Document data access
│   ├── WorkflowRepository.cs          # Workflow data access
│   ├── TaskRepository.cs              # Task data access
│   └── [Other Repositories...]        # Additional repositories
├── SERVICE/
│   ├── DocumentService.cs             # Document business logic
│   ├── DocumentTypeService.cs         # Document type services
│   ├── WorkflowService.cs             # Workflow management
│   ├── TaskAssignmentService.cs       # Task assignment logic
│   ├── FileService.cs                 # File handling services
│   ├── ValidationService.cs           # Input validation
│   ├── EnhancedValidationService.cs   # Advanced validation rules
│   └── [Other Services...]            # Additional business services
├── Program.cs                         # Application entry point
└── Properties/
    └── launchSettings.json            # Development settings
```

## 🔧 Installation & Setup

### Prerequisites
- .NET 9 SDK
- Microsoft SQL Server (2019 or later)
- Visual Studio 2022 or VS Code
- SQL Server Management Studio (recommended)

### Database Setup
1. Create a new SQL Server database
2. Run the database setup scripts to create tables and stored procedures
3. Update the connection string in `appsettings.json`

### Application Setup
1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd "Workflow & Document Management System"
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update configuration in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=[server];Database=[database];Trusted_Connection=true;"
     },
     "FileStorage": {
       "BasePath": "[file-storage-path]",
       "MaxFileSizeMB": 100
     }
   }
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

## 📚 API Documentation

### Document Management Endpoints

#### Create Document
```http
POST /api/Document/create
Content-Type: multipart/form-data
```

#### Get All Documents
```http
GET /api/Document/list
```

#### Get Document by ID
```http
GET /api/Document/{id}
```

#### Download Document
```http
GET /api/Document/download/{id}
```

#### View Document
```http
GET /api/Document/view/{id}
```

### Workflow Management Endpoints

#### Create Workflow
```http
POST /api/Workflow/create
Content-Type: application/json
```

#### Get All Workflows
```http
GET /api/Workflow/list
```

#### Get Workflow by ID
```http
GET /api/Workflow/{id}
```

### Task Management Endpoints

#### Get My Tasks
```http
GET /api/Task/my-tasks
```

#### Process Document Action
```http
POST /api/Task/process-action
Content-Type: application/json
```

#### Reassign Document
```http
POST /api/Task/reassign
Content-Type: application/json
```

#### Batch Process Documents
```http
POST /api/Task/batch-action
Content-Type: application/json
```

## 🔒 Security Features

- **Input Validation**: Comprehensive validation using custom validation services
- **File Upload Security**: Configurable file size limits and type restrictions
- **Error Handling**: Structured error responses with detailed logging
- **Data Integrity**: Stored procedure-based data access for enhanced security

## 📊 File Upload Specifications

- **Maximum File Size**: 100MB per file
- **Supported Formats**: Configurable based on document types
- **Upload Method**: Multipart form data
- **Storage**: Secure file system storage with metadata in database

## 🚦 Response Format

All API endpoints return standardized responses using `ApiResponseDto<T>`:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { /* response data */ },
  "errors": []
}
```

## 🔄 Workflow Process Flow

1. **Document Creation**: Users upload documents with associated metadata
2. **Workflow Assignment**: System automatically assigns documents to appropriate workflows
3. **Task Distribution**: Workflow steps create tasks assigned to specific users/roles
4. **Processing**: Users process assigned tasks (approve, reject, request changes)
5. **Status Updates**: Document status updates based on task completion
6. **Completion**: Documents reach final status upon workflow completion

## 🛠️ Development Guidelines

### Code Structure
- **Controllers**: Handle HTTP requests and responses
- **Services**: Contain business logic and orchestration
- **Repositories**: Manage data access through stored procedures
- **DTOs**: Define data contracts for API communication
- **Models**: Represent database entities

### Best Practices
- All database operations use stored procedures
- Comprehensive input validation on all endpoints
- Structured error handling and logging
- Async/await pattern for database operations
- Clean separation of concerns across layers

## 🐛 Troubleshooting

### Common Issues
1. **Database Connection**: Verify connection string and SQL Server accessibility
2. **File Upload Errors**: Check file size limits and storage path permissions
3. **Validation Failures**: Review DTO properties and validation rules
4. **Task Assignment Issues**: Verify workflow configuration and user assignments

## 📞 Support

For technical support and questions:
- Review the API documentation
- Check application logs for detailed error information
- Verify database stored procedures are properly installed
- Ensure all required services are registered in dependency injection

## 🔮 Future Enhancements

- **Authentication & Authorization**: JWT token-based security
- **Email Notifications**: Automated task assignment notifications
- **Advanced Reporting**: Workflow analytics and performance metrics
- **Mobile API**: Enhanced mobile device support
- **Integration APIs**: Third-party system integrations
- **Document Versioning**: Version control for document revisions

---

This system provides a robust foundation for enterprise document management and workflow automation, with extensible architecture supporting future enhancements and integrations.# Workflow-Document-Management-System