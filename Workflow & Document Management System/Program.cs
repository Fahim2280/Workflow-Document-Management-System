using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Workflow___Document_Management_System.SERVICE;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;

namespace Workflow___Document_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (string.IsNullOrEmpty(builder.Environment.WebRootPath))
            {
                builder.Environment.WebRootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
            }

            // Add services
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            // Configure form options for file uploads
            builder.Services.Configure<FormOptions>(options =>
            {
                options.KeyLengthLimit = int.MaxValue; // Remove key length limit
                options.ValueCountLimit = 1024;
                options.ValueLengthLimit = int.MaxValue; // Remove value length limit
                options.MultipartBodyLengthLimit = 1024 * 1024 * 100; // 100MB for file uploads
                options.MultipartHeadersCountLimit = 32; // Increase header count
                options.MultipartHeadersLengthLimit = 65536; // Increase header length
                options.BufferBody = true;
                options.BufferBodyLengthLimit = 1024 * 1024 * 100; // 100MB
                options.MultipartBoundaryLengthLimit = 128; // Set boundary limit
            });

            // Configure Kestrel server limits
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 104857600; // 100MB
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            });

            // Add session support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register custom services
            builder.Services.AddScoped<AdminRepository>(provider =>
                new AdminRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<SessionService>();
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<WorkflowService>();
            builder.Services.AddScoped<DocumentService>();
            builder.Services.AddScoped<DocumentTypeService>();
            builder.Services.AddScoped<FileService>();
            builder.Services.AddScoped<ValidationService>();
            builder.Services.AddScoped<DocumentRepository>(provider =>
                new DocumentRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<WorkflowRepository>(provider =>
                new WorkflowRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<DocumentTypeRepository>(provider =>
                new DocumentTypeRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<FileRepository>();

            // Add CORS if needed
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Configure Swagger - MUST be before app.Build()
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Document Management API", Version = "v1" });

                // Configure file upload for Swagger
                c.OperationFilter<FileUploadOperationFilter>();
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseDeveloperExceptionPage();

                // Enable Swagger
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Document Management API V1");
                    options.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
                });
            }

            // Ensure upload directories exist
            CreateUploadDirectories(app.Environment);

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Important: Enable static files for wwwroot access
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.UseSession(); // Important: Add session middleware
            app.MapControllers();
            app.Run();
        }

        private static void CreateUploadDirectories(IWebHostEnvironment environment)
        {
            try
            {
                // Handle null WebRootPath by using ContentRootPath + wwwroot
                var basePath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");

                // Ensure the base wwwroot directory exists
                Directory.CreateDirectory(basePath);

                var uploadPath = Path.Combine(basePath, "uploads");
                Directory.CreateDirectory(uploadPath);
                Directory.CreateDirectory(Path.Combine(uploadPath, "documents"));
                Directory.CreateDirectory(Path.Combine(uploadPath, "images"));
                Directory.CreateDirectory(Path.Combine(uploadPath, "temp"));

                Console.WriteLine($"Upload directories created at: {uploadPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not create upload directories: {ex.Message}");
            }
        }
    }

    // File Upload Operation Filter for Swagger
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) ||
                           p.ParameterType == typeof(IFormFile[]) ||
                           p.ParameterType == typeof(IEnumerable<IFormFile>))
                .ToList();

            if (fileParameters.Any() || HasFormFileProperty(context))
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = GetFormProperties(context),
                                Required = new HashSet<string> { "file" }
                            }
                        }
                    }
                };
            }
        }

        private bool HasFormFileProperty(OperationFilterContext context)
        {
            var parameters = context.MethodInfo.GetParameters();
            foreach (var param in parameters)
            {
                if (param.GetCustomAttributes(typeof(FromFormAttribute), false).Any())
                {
                    var properties = param.ParameterType.GetProperties();
                    if (properties.Any(p => p.PropertyType == typeof(IFormFile)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Dictionary<string, OpenApiSchema> GetFormProperties(OperationFilterContext context)
        {
            var properties = new Dictionary<string, OpenApiSchema>();

            var parameters = context.MethodInfo.GetParameters();
            foreach (var param in parameters)
            {
                if (param.GetCustomAttributes(typeof(FromFormAttribute), false).Any())
                {
                    var paramProperties = param.ParameterType.GetProperties();
                    foreach (var prop in paramProperties)
                    {
                        if (prop.PropertyType == typeof(IFormFile))
                        {
                            properties[prop.Name.ToLower()] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            };
                        }
                        else if (prop.PropertyType == typeof(string))
                        {
                            properties[prop.Name.ToLower()] = new OpenApiSchema
                            {
                                Type = "string"
                            };
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            properties[prop.Name.ToLower()] = new OpenApiSchema
                            {
                                Type = "integer"
                            };
                        }
                    }
                }
            }

            return properties;
        }
    }
}