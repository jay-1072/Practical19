using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practical19_API.Data;
using Practical19_API.Mappings;
using Practical19_API.Models;

var builder = WebApplication.CreateBuilder(args);

// Register your DBContext.
builder.Services.AddDbContext<MyDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBContext") ?? throw new InvalidOperationException("Connection string 'MyDBContext' not found.")));

// Add services to the container.

// 1. Register MappingProfile to use AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 2. Register Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MyDBContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    RoleInitializer.InitializeAsync(scope.ServiceProvider).Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();