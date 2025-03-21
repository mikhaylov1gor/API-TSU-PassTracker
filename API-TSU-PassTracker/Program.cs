using API_TSU_PassTracker.Models.DB;
using Microsoft.EntityFrameworkCore;
using API_TSU_PassTracker.Infrastructure;
using API_TSU_PassTracker.Models.DB;
using API_TSU_PassTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using API_TSU_PassTracker.Filters;
using API_TSU_PassTracker.Models.DTO;
using API_TSU_PassTracker.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;
using API_TSU_PassTracker.Filter;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<CustomExceptionFilter>();
    });

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
    c.SchemaFilter<FormFileSchemaFilter>();
});


//database
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TsuPassTrackerDBContext>(options => 
    options.UseNpgsql(connection));

// services
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<ITokenBlackListService, TokenBlackListService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Services.AddScoped<ISeedDataService, SeedDataService>();

//jwt
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

//jwt configure
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtOptions:Issuer"], 
            ValidAudience = builder.Configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"])),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var error = new ErrorResponse(
                    status: 401,
                    message: "Токен не валиден",
                    details: null
                );
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(error));
            },
            OnChallenge = async context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var error = new ErrorResponse(
                        status: 401,
                        message: "Требуется авторизация",
                        details: null
                    );
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(error));
                }
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var error = new ErrorResponse(
                    status: 403,
                    message: "Недостаточно прав",
                    details: null
                );
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(error));
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

//seedData
using (var scope = app.Services.CreateScope())
{
    var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
    await seedDataService.SeedUsers();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCustomErrorHandling(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
