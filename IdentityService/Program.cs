using System.Text;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Profiles;

var builder = WebApplication.CreateBuilder(args);

//Đọc thông số JWT từ cấu hình
var jwtSetting = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSetting["Key"]);

//Cấu hình Authentication
builder.Services.AddAuthentication(opt =>
{
   opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
        ValidIssuer = jwtSetting["Issuer"],
        ValidAudience = jwtSetting["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("UserInMemoryDB"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//Đăng ký Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();


app.Run();
