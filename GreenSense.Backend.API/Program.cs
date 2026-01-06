using System.Text;
using GreenSense.Backend.API.Services;
using GreenSense.Backend.Data.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GreenSense.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Docker/Render: приложение слушает на всех интерфейсах контейнера
            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            // Add services to the container.
            builder.Services.AddControllers();

            // OpenAPI JSON
            builder.Services.AddOpenApi();

            // Swagger UI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // DbContext (PostgreSQL)
            builder.Services.AddDbContext<GreenSenseDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("GreenSenseDb")));

            // JWT services
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ✅ 3.2.1 — сервис автоматического создания Notification по Threshold
            builder.Services.AddScoped<IReadingNotificationService, ReadingNotificationService>();

            // Authentication (JWT Bearer)
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];
            var jwtKey = builder.Configuration["Jwt:Key"];

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey!)
                        ),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // ✅ Swagger в Production по флагу (Render обычно запускает Production)
            var enableSwagger =
                app.Environment.IsDevelopment() ||
                string.Equals(app.Configuration["Swagger:Enabled"], "true", StringComparison.OrdinalIgnoreCase);

            if (enableSwagger)
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ✅ Чтобы корень сайта не был "Not Found"
            // Если Swagger включен — редиректим на него, иначе просто показываем, что сервис жив.
            app.MapGet("/", () =>
            {
                if (enableSwagger)
                    return Results.Redirect("/swagger");

                return Results.Ok("GreenSense Backend is running. Use /api/* endpoints.");
            });

            // ✅ В Render HTTPS терминируется на прокси, внутри контейнера HTTPS порта нет.
            // Поэтому редирект делаем только локально (в Development).
            if (app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            // ВАЖНО: сначала authentication, потом authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
