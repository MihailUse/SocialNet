using API.Configs;
using API.Mapper;
using API.Middlewares;
using API.Services;
using Common.Constants;
using DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // get configs
            IConfigurationSection authSection = builder.Configuration.GetSection(AuthConfig.Position);
            AuthConfig? authConfig = authSection.Get<AuthConfig>();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(SetupSwaggerAction);

            builder.Services.Configure<AuthConfig>(authSection);
            builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            builder.Services.AddScoped<TagService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<PostService>();
            builder.Services.AddScoped<CommentService>();
            builder.Services.AddScoped<ProjectionGeneratorService>();
            builder.Services.AddSingleton<AttachService>();

            builder.Services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authConfig.Issuer,
                        ValidAudience = authConfig.Audience,
                        IssuerSigningKey = authConfig.SymmetricSecurityKey(),
                    };
                });

            builder.Services.AddAuthorization(o =>
            {
                o.AddPolicy("ValidAccessToken", p =>
                {
                    p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    p.RequireAuthenticatedUser();
                });
            });

            WebApplication app = builder.Build();

            // update database
            using (var scope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                if (scope != null)
                {
                    DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    db.Database.Migrate();
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI(SetupSwaggerUiAction);
            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // add middlewares
            app.UseExceptionHandlerMiddleware();
            app.UseTokenValidatorMiddleware();

            app.MapControllers();

            app.Run();
        }

        private static void SetupSwaggerUiAction(SwaggerUIOptions options)
        {
            options.SwaggerEndpoint($"{SwaggerDefinitionNames.Api}/swagger.json", SwaggerDefinitionNames.Api);
            options.SwaggerEndpoint($"{SwaggerDefinitionNames.Auth}/swagger.json", SwaggerDefinitionNames.Auth);
        }

        private static void SetupSwaggerAction(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            options.SwaggerDoc(SwaggerDefinitionNames.Auth, new OpenApiInfo() { Title = SwaggerDefinitionNames.Auth });
            options.SwaggerDoc(SwaggerDefinitionNames.Api, new OpenApiInfo() { Title = SwaggerDefinitionNames.Api });
        }
    }
}