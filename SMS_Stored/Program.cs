using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Rotativa.AspNetCore;
using ServiceStack.Text;
using SMS.BL.Log;
using SMS.BL.Log.Interface;
using SMS.BL.Student;
using SMS.BL.Student.Interface;
using SMS.Data;
using System.Configuration;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);
//var dateformat = new DateTimeFormatInfo { ShortDatePattern = "MM/dd/yyyy", LongDatePattern = "MM/dd/yyyy hh:mm:ss tt" };
//var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
builder.Logging.ClearProviders();
//builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();


builder.Services.AddDbContext<SMS_StoredContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SMS_StoredContext")));
    // Add services to the container.
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddControllersWithViews();

  

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
   
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
//RotativaConfiguration.Setup(env);
var env = app.Environment;
RotativaConfiguration.Setup(env.WebRootPath,"Rotativa");
app.Run();
