
using BookInformationService;
using BookInformationService.BusinessLayer;
using BookInformationService.DataAccessLayer;
using BookInformationService.DatabaseContext;
using BookInformationService.Services;
using BookInformationService.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var configuration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");


AppSettings? appSettings = configuration.GetRequiredSection("AppSettings").Get<AppSettings>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Serilog\\log_.txt"), rollOnFileSizeLimit: true, fileSizeLimitBytes: 1000000, rollingInterval: RollingInterval.Month, retainedFileCountLimit: 24, flushToDiskInterval: TimeSpan.FromSeconds(1))
    //.WriteTo.Email(emailInfo)                           
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// EF
builder.Services.AddDbContext<SystemDbContext>(options =>
            options.UseSqlite(defaultConnectionString));

// Models
builder.Services.AddScoped<IBookInformationDL, BookInformationDL>();
builder.Services.AddScoped<IBookInformationBL, BookInformationBL>();

// Register FluentValidation validators
builder.Services.AddScoped<IValidator<GetBookInformationRequest>, GetBookInformationRequestValidator>();
builder.Services.AddScoped<IValidator<CreateBookInformationRequest>, CreateBookInformationRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateBookInformationRequest>, UpdateBookInformationRequestValidator>();
builder.Services.AddScoped<IValidator<DeleteBookInformationRequest>, DeleteBookInformationRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<BookInformationGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
