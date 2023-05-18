using AuthenticationService.Models.DBModel;
using AuthenticationService.Repository.Base;
using AuthenticationService.Repository.Interfaces;
using AuthenticationService.Repository.Services;
using AuthenticationService.Security;
using AuthenticationService.Validator.Interfaces;
using AuthenticationService.Validator.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Logging
var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
#endregion
#region DB Connection
builder.Services.AddDbContext<DBContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion
#region Service Registration
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
#endregion

#region Validation Service Registration
builder.Services.AddTransient<IApplicationUserValidation, ApplicationUserValidation>();

#endregion




// For Entity Framework  
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


#region Register Identity

//inject the identity services which were in  IdentityHostingStartup
builder.Services.AddIdentity<ApplicationUser, ApplicationUserRole>()
    .AddRoles<ApplicationUserRole>()
    .AddRoleManager<RoleManager<ApplicationUserRole>>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<DBContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = true;
    //options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
#endregion



#region Authonticate with identity server

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.Authority = "https://testidentityserver4.test.com";
    o.Audience = "test_resource";
    o.RequireHttpsMetadata = false;
})
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
 {
     options.Events.OnRedirectToLogin = (context) =>
     {
         context.Response.StatusCode = 401;
         return Task.CompletedTask;
     };
 }).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
 {
     IConfigurationSection googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
     options.ClientId = googleAuthSection["ClientId"];
     options.ClientSecret = googleAuthSection["ClientSecret"];
     options.SignInScheme = IdentityConstants.ExternalScheme;
 })
    .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
    {
        IConfigurationSection microsoftAuthSection = builder.Configuration.GetSection("Authentication:Microsoft");
        options.ClientId = microsoftAuthSection["ClientId"];
        options.ClientSecret = microsoftAuthSection["ClientSecret"];
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.CallbackPath = microsoftAuthSection["CallbackPath"];


    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PublicSecure", policy => policy.RequireClaim("client_id", "Iqzcj12KR7ZaEvDjdVQloZUVydIuNXrLaTYFBnzL8N4="));
});
#endregion



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region CORS

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:7010"); // Safe
    });
    options.AddPolicy(name: "AllowAllOrigins", builder =>
    {
        builder.WithOrigins("https://localhost:7010"); // Safe
    });
});
#endregion
builder.Services.AddControllers();

#region Swagger Configuration
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication Services", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();
app.MapControllers();

app.Run();
