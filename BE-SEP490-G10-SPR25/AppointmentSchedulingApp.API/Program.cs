using System.Text;
using AppointmentSchedulingApp.Domain.Repositories;
using AppointmentSchedulingApp.Domain.UnitOfWork;
using AppointmentSchedulingApp.Domain.Entities;
using AppointmentSchedulingApp.Infrastructure;
using AppointmentSchedulingApp.Infrastructure.Database;
using AppointmentSchedulingApp.Infrastructure.Repositories;
using AppointmentSchedulingApp.Infrastructure.UnitOfWork;
using AppointmentSchedulingApp.Services.DTOs;
using AppointmentSchedulingApp.Services.Helper;
using AppointmentSchedulingApp.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<CategoryDTO>("Category");
modelBuilder.EntitySet<ReservationDTO>("Reservations");
modelBuilder.EntitySet<MedicalRecordDTO>("MedicalRecords");
modelBuilder.EntitySet<DoctorDTO>("Doctors");
modelBuilder.EntitySet<SpecialtyDTO>("Specialties");
modelBuilder.EntitySet<ServiceDTO>("Services");
var provider = builder.Services.BuildServiceProvider();
var config = provider.GetService<IConfiguration>();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(config.GetValue<string>("Frontend_url"))
               .AllowAnyMethod()
               .AllowAnyHeader();

    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Appsettings"));
var secretKey = builder.Configuration["Appsettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        // t? c?p token
        ValidateIssuer = false,
        ValidateAudience = false,

        // k� v�o token
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddControllers().AddOData(opt => opt.Select().Filter().SetMaxTop(100).Expand().OrderBy().Count().AddRouteComponents("odata", modelBuilder.GetEdmModel()));

builder.Services.AddDbContext<AppointmentSchedulingDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));



builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();

builder.Services.AddScoped<IDoctorService, DoctorService>();

builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();

builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
