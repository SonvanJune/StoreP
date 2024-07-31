using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StoreSp.Commonds;
using StoreSp.Services;
using StoreSp.Services.Impl;

namespace StoreSp.Configs;

public static class BuiderConfig
{
    public static WebApplicationBuilder RunConfig(this WebApplicationBuilder builder)
    {
        ConfigVariables();
        builder.Services.AddTransient<AuthServiceImpl>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthCore API", Version = "v1" });
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            c.AddSecurityDefinition("Bearer", securityScheme);


            var securityRequirement = new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
            };

            c.AddSecurityRequirement(securityRequirement);
        });
        builder.Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthConfig.PrivateKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("quan-tri-vien", policy => policy.RequireRole("quan-tri-vien"));
            options.AddPolicy("nguoi-mua", policy => policy.RequireRole("nguoi-mua"));
            options.AddPolicy("nguoi-ban", policy => policy.RequireRole("nguoi-ban"));
        });
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            });
        });
        builder.Services.AddTransient<IEmailService , EmailServiceImpl>();
        return builder;
    }

    public static void ConfigVariables(){
        //example
        VariableConfig<double>.Application["price-of-kilometer"] = 500;
    }
}

