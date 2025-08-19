
using Microsoft.Extensions.Options;
using Workflow___Document_Management_System.SERVICE;

namespace Workflow___Document_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

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
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Workflow Document Management System API V1");                 
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseSession(); // Important: Add session middleware

            app.MapControllers();

            app.Run();
        }
    }
}
