using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShahinApis.Data;
using ShahinApis.Data.Model;
using ShahinApis.Data.Repository;
using ShahinApis.Infrastucture;
using ShahinApis.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ShahinOptions>(builder.Configuration.GetSection(ShahinOptions.SectionName));

builder.Services.AddScoped<IShahinRepository, ShahinRepository>();
builder.Services.AddScoped<IShahinService, ShahinService>();
builder.Services.AddScoped<BaseLog>();

builder.Services.AddDbContext<ShahinDbContext>(opt => opt.UseOracle
    (builder.Configuration["ConnectionStrings:ConnectionString"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
