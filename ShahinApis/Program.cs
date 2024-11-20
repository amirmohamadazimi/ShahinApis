using Microsoft.EntityFrameworkCore;
using ShahinApis.Data;
using ShahinApis.Data.Model;
using ShahinApis.Data.Repository;
using ShahinApis.Infrastucture;
using ShahinApis.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient("")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, certChain, policyErrors) => true
        });
builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
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
