using System.Text;
using Data;
using Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using Options;
using Processors;
using Profiles;
using Repositories;
using Scalar.AspNetCore;
using Service;

var builder = WebApplication.CreateBuilder(args);

//Đọc thông số JWT từ cấu hình
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.JwtOptionsKey)
);
var jwtOptions = builder.Configuration.GetSection(JwtOptions.JwtOptionsKey).Get<JwtOptions>() ?? throw new ArgumentException(nameof(JwtOptions));

//Cấu hình Authentication
builder.Services.AddAuthentication(opt =>
{
   opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false; //Môi trường dev thì tắt, production thì nên bật
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
    };

    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["ACCESS_TOKEN"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//Thêm Identity
builder.Services.AddIdentityCore<User>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequiredLength = 8;
    opt.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddHttpContextAccessor();

//Khởi tạo kết nối tới DB
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));

//Khởi tạo automapper
// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Khởi tạo các dependcy injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthProcessor, AuthProcessor>();
builder.Services.AddScoped<IAuthService, AuthService>();

//Đăng ký handler xử lý các exception
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

//Đăng ký Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithTitle("Login API with JWT + Refresh Token");
    });
}

//Đăng ký sử dụng handler
app.UseExceptionHandler(_ => {});

app.Run();
