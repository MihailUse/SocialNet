using API.Configs;
using API.Middlewares;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // get configs
            var authSection = builder.Configuration.GetSection(AuthConfig.Position);
            var authConfig = authSection.Get<AuthConfig>();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(SetupSwaggerAction);

            builder.Services.Configure<AuthConfig>(authSection);
            builder.Services.AddDbContext<DAL.DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<PostService>();
            builder.Services.AddScoped<CommentService>();
            builder.Services.AddSingleton<AttachService>();

            builder.Services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = builder.Environment.IsProduction();
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
                    var db = scope.ServiceProvider.GetRequiredService<DAL.DataContext>();
                    db.Database.Migrate();
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // add middlewares
            app.UseTokenValidatorMiddleware();

            app.MapControllers();

            app.Run();
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
        }
    }
}